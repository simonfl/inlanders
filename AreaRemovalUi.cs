using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _areaRemoving,_areaDragging;
    private Cell _areaFirst,_areaLast;
    private World? _areaWorld;
    private CreativeRemovalSelection? _areaSelection;
    private Button _areaEntry=null!,_areaConfirm=null!;
    private Label _areaText=null!;
    private PanelContainer _areaPanel=null!;
    private Node3D _areaMarks=null!;
    private void MakeAreaRemovalUi()
    {
        _areaEntry=Button("Remove an area · Creative",BeginAreaRemoval);_buildSections[1].AddChild(_areaEntry);
        _areaPanel=HudPanel(_hud);_areaPanel.Hide();var column=new VBoxContainer();_areaPanel.AddChild(column);
        column.AddChild(Text("REMOVE SELECTED",16));_areaText=Text("",14,true);column.AddChild(_areaText);
        _areaConfirm=Button("Remove selected",ConfirmAreaRemoval);column.AddChild(_areaConfirm);
        column.AddChild(Button("Cancel [Esc]",CancelAreaRemoval));
        _areaMarks=new();AddChild(_areaMarks);
    }
    private void ClearAreaMarks()
    {
        foreach(var child in _areaMarks.GetChildren()){_areaMarks.RemoveChild(child);child.QueueFree();}
    }
    private void CancelAreaRemoval()
    {
        if(!_areaRemoving)return;
        _areaRemoving=_areaDragging=false;_areaWorld=null;_areaSelection=null;_areaPanel.Hide();ClearAreaMarks();CancelCameraDrag();
    }
    private void BeginAreaRemoval()
    {
        if(!_world.Creative)return;
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _areaRemoving=true;_areaWorld=_world;_areaPanel.Show();_areaConfirm.Disabled=true;
        _areaText.Text="Drag across buildings, decorations and paths. Whole buildings count, even if only partly inside.\n\nTrees, resources and terrain stay. Nothing is removed until you confirm.";
    }
    private void UpdateAreaRemoval()
    {
        _areaEntry.Visible=_world.Creative;
        if(!_areaRemoving)return;
        if(!ReferenceEquals(_areaWorld,_world) || !_world.Creative || _placing || _watching){CancelAreaRemoval();return;}
        _areaPanel.Position=new(_hud.Size.X-300,92);_areaPanel.Size=new(284,0);
    }
    private void DrawRemovalSelection(CreativeRemovalSelection selection)
    {
        ClearAreaMarks();
        if(_areaDragging)
        {
            int left=System.Math.Min(_areaFirst.X,_areaLast.X),right=System.Math.Max(_areaFirst.X,_areaLast.X);
            int back=System.Math.Min(_areaFirst.Z,_areaLast.Z),front=System.Math.Max(_areaFirst.Z,_areaLast.Z);
            // Camera bounds limit the rectangle; sample terrain along each edge so
            // the drag outline remains attached to raised land and slopes.
            foreach(var c in _world.Map.Land.Where(c=>c.X>=left && c.X<=right && c.Z>=back && c.Z<=front && (c.X==left || c.X==right || c.Z==back || c.Z==front)))
                Box(_areaMarks,OnGround(c.X,c.Z)+new Vector3(0,.09f,0),new(.98f,.045f,.98f),new("dbc17e"));
        }
        var cells=selection.Buildings.SelectMany(b=>World.Footprint(b.Cell,b.Rotation,b.Kind))
            .Concat(selection.Decorations.Select(d=>d.Cell)).Concat(selection.Paths).Distinct();
        foreach(var cell in cells)Box(_areaMarks,OnGround(cell.X,cell.Z)+new Vector3(0,.075f,0),new(.9f,.06f,.9f),new("d88967"));
    }
    private void ReviewAreaRemoval()
    {
        _areaSelection=_world.SelectCreativeRemoval(_areaFirst,_areaLast);DrawRemovalSelection(_areaSelection);
        var result=_world.PrepareCreativeRemoval(_areaSelection);
        _areaConfirm.Disabled=_areaSelection.Count==0;
        _areaText.Text=$"Buildings: {_areaSelection.Buildings.Count} · Decorations: {_areaSelection.Decorations.Count} · Paths: {_areaSelection.Paths.Count}\n\n"+
            (result.Problem??"Ready to remove. Recoverable goods return to storage. Unripe crops and partial work are lost.")+"\n\nTrees and resources stay. Drag again to change selection. Confirmation rechecks the live village.";
    }
    private void ConfirmAreaRemoval()
    {
        if(!_areaRemoving || _areaSelection==null || !ReferenceEquals(_areaWorld,_world)){CancelAreaRemoval();return;}
        var result=_world.PrepareCreativeRemoval(_areaSelection);
        if(result.World==null){_areaText.Text=result.Problem+"\n\nNothing removed. Resolve the issue and retry, or drag a new selection.";UiCue(Cue.Reject);return;}
        int count=_areaSelection.Count;CancelAreaRemoval();ClearSelection();
        _world=result.World;CreateActors();RenderActors(0);RenderFoodViews();RebuildQueue();RefreshSelection();
        Notice($"Removed {count} selected objects. Recoverable goods returned to storage.");UiCue(Cue.Place);
    }
    private bool HandleAreaRemovalInput(InputEvent input)
    {
        if(!_areaRemoving)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right})
        {CancelAreaRemoval();return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P)
        {CancelAreaRemoval();return false;}
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left} button)
        {
            if(button.Pressed)
            {
                if(PointerOverHud(button.Position))return false;
                if(Ground(button.Position) is Vector3 point){_areaFirst=_areaLast=new(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_areaDragging=true;_areaSelection=null;_areaConfirm.Disabled=true;ClearAreaMarks();}
                return true;
            }
            if(!_areaDragging)return false;
            _areaDragging=false;
            if(!PointerOverHud(button.Position) && Ground(button.Position) is Vector3 end)
            {_areaLast=new(Mathf.RoundToInt(end.X),Mathf.RoundToInt(end.Z));ReviewAreaRemoval();}
            else {_areaText.Text="Release on the map to select an area. Drag again; nothing has changed.";ClearAreaMarks();}
            return true;
        }
        if(_areaDragging && input is InputEventMouseMotion motion && Ground(motion.Position) is Vector3 hover)
        {
            var last=new Cell(Mathf.RoundToInt(hover.X),Mathf.RoundToInt(hover.Z));
            if(last!=_areaLast){_areaLast=last;DrawRemovalSelection(_world.SelectCreativeRemoval(_areaFirst,_areaLast));}
            return true;
        }
        return false;
    }
}
