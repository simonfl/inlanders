using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private int _woodlandTool;
    private bool _woodlandStroke;
    private Cell? _lastWoodlandCell;
    private Node3D? _woodlandView;
    private World? _woodlandViewWorld;
    private string _woodlandViewKey="";
    private string WoodlandToolName => _woodlandTool switch { 1=>"Preserve trees",2=>"Allow harvesting",3=>"Manage grove",_=>"Remove grove spots" };
    private void MakeWoodlandControls(VBoxContainer parent)
    {
        parent.AddChild(Text("WOODLAND",12));
        foreach(var pair in new[]{("Preserve trees",1),("Allow harvesting",2),("Manage grove · replant",3),("Remove grove spots",4)})
        {
            int tool=pair.Item2; var button=Button(pair.Item1,()=>BeginWoodlandTool(tool)); parent.AddChild(button);
            button.TooltipText=tool switch {
                1=>"Click or drag over living trees. Loggers leave them standing, including stopping a cut in progress. Explicit clearing overrides preservation.",
                2=>"Click or drag over preserved trees to let loggers harvest them again.",
                3=>"Click or drag to mark up to 32 tree spots. Loggers replant after timber is collected; unsafe planting waits. Clearing, buildings, paths and decorations replace grove spots.",
                _=>"Click or drag to stop future replanting. Existing trees and current planting work remain."
            };
        }
    }
    private void BeginWoodlandTool(int tool)
    {
        ClearSelection(); _placing=true; _woodlandTool=tool;
        _pathTool=0; _pathStroke=false; _decorating=_clearingTrees=_plantingTrees=false;
        _woodlandStroke=false; _lastWoodlandCell=null; RefreshGhost();
    }
    private string? WoodlandProblem(Cell cell) => _woodlandTool<=2 ? _world.PreserveTreeProblem(cell) : _world.ManagedWoodlandProblem(cell,_woodlandTool==4);
    private void PaintWoodland(Cell cell)
    {
        if(_lastWoodlandCell==cell) return;
        bool Apply(Cell c) => _woodlandTool<=2 ? _world.SetTreePreserved(c,_woodlandTool==1) : _world.SetManagedWoodland(c,_woodlandTool==3);
        bool changed=false;
        if(_lastWoodlandCell is Cell previous)
        {
            int steps=System.Math.Max(System.Math.Abs(cell.X-previous.X),System.Math.Abs(cell.Z-previous.Z));
            for(int i=1;i<=steps;i++) changed|=Apply(new(previous.X+(int)System.Math.Round((cell.X-previous.X)*i/(double)steps),previous.Z+(int)System.Math.Round((cell.Z-previous.Z)*i/(double)steps)));
        }
        else changed=Apply(cell);
        _lastWoodlandCell=cell; UiCue(changed?Cue.Click:Cue.Reject); RefreshGhost();
    }
    private void RefreshWoodlandGhost()
    {
        Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey="woodland"; Clear(_ghostCells);
        GroundPatch(_ghostCells,_hover.X,_hover.Z,.9f,.9f,_ghostValid?new("a4caa0"):new("e38673"),.08f);
    }
    private void RenderManagedWoodland()
    {
        _woodlandView??=new Node3D(); if(_woodlandView.GetParent()==null) AddChild(_woodlandView);
        _woodlandView.Visible=_placing && _woodlandTool>0;
        if(!_woodlandView.Visible) return;
        string key=string.Join(";",_world.ManagedWoodland.OrderBy(c=>c.X).ThenBy(c=>c.Z));
        if(_woodlandViewWorld==_world && key==_woodlandViewKey) return;
        _woodlandViewWorld=_world; _woodlandViewKey=key; Clear(_woodlandView);
        foreach(var cell in _world.ManagedWoodland)
            GroundPatch(_woodlandView,cell.X,cell.Z,.75f,.75f,new("81996d"),.045f);
    }
}
