using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;

public partial class Game
{
    private bool _decorationStroke;
    private Cell? _lastDecorationCell;
    private World? _decorationStrokeWorld;
    private readonly HashSet<Cell> _decorationStrokeCells = new();
    private string? _decorationStrokeProblem;
    private DecorationKind _strokeDecorationKind;
    private bool _strokeDecorationRemove;
    private int _strokeDecorationRotation;

    private void CancelDecorationStroke()
    {
        _decorationStroke=false;_lastDecorationCell=null;_decorationStrokeWorld=null;
        _decorationStrokeCells.Clear();_decorationStrokeProblem=null;
    }
    private void BeginDecorationStroke(Cell cell)
    {
        CancelDecorationStroke();_decorationStroke=true;_decorationStrokeWorld=_world;
        _strokeDecorationKind=_decorationKind;_strokeDecorationRemove=_removeDecoration;_strokeDecorationRotation=_rotation;
        PaintDecoration(cell);
    }
    private void HandleDecorationStroke(InputEvent input)
    {
        if(!_decorationStroke)return;
        if(!_placing || !_decorating || _decorationStrokeWorld!=_world || _strokeDecorationKind!=_decorationKind ||
            _strokeDecorationRemove!=_removeDecoration || _strokeDecorationRotation!=_rotation ||
            input is InputEventKey { Pressed:true } || input is InputEventMouseButton)
        { CancelDecorationStroke();return; }
        if(input is not InputEventMouseMotion motion)return;
        if((motion.ButtonMask & MouseButtonMask.Left)==0 || PointerOverHud(motion.Position) || Ground(motion.Position) is not Vector3 point)
        { CancelDecorationStroke();return; }
        PaintDecoration(new(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z)));
    }
    // Canonical low-X (then low-Z) direction, inserting X before Z at diagonals.
    // Reverse gestures traverse the same tiles, useful when erasing a boundary.
    private static IEnumerable<Cell> DecorationStrokeLine(Cell from,Cell to)
    {
        bool reverse=from.X>to.X || from.X==to.X && from.Z>to.Z;
        if(reverse)(from,to)=(to,from);
        var cells=new List<Cell>{from};
        int steps=Math.Max(Math.Abs(to.X-from.X),Math.Abs(to.Z-from.Z));var cursor=from;
        for(int i=1;i<=steps;i++)
        {
            var next=new Cell(from.X+(int)Math.Round((to.X-from.X)*i/(double)steps),from.Z+(int)Math.Round((to.Z-from.Z)*i/(double)steps));
            if(cursor.X!=next.X && cursor.Z!=next.Z)cells.Add(new(next.X,cursor.Z));
            cells.Add(next);cursor=next;
        }
        if(reverse)cells.Reverse();
        for(int i=1;i<cells.Count;i++)yield return cells[i];
    }
    private void PaintDecoration(Cell cell)
    {
        bool changed=false,firstRejection=false;
        void Apply(Cell at)
        {
            if(!_decorationStrokeCells.Add(at))return;
            string? problem=_world.DecorationProblem(at,_decorationKind,_removeDecoration);
            if(problem!=null)
            {
                if(_decorationStrokeProblem==null) { _decorationStrokeProblem=problem;firstRejection=true;Notice("Decoration skipped: "+problem); }
                return;
            }
            changed|=_removeDecoration?_world.RemoveDecoration(at):_world.PlaceDecoration(at,_decorationKind,_rotation%2!=0,_rotation);
        }
        if(_lastDecorationCell is Cell previous)foreach(var at in DecorationStrokeLine(previous,cell))Apply(at);
        else Apply(cell);
        _lastDecorationCell=cell;_hover=cell;
        if(firstRejection)UiCue(Cue.Reject);else if(changed)UiCue(Cue.Place);
        RefreshGhost();
    }
}
