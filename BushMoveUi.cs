using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _bushMoving;
    private int _bushMoveId=-1;
    private Cell? _bushMoveAt;
    private World? _bushMoveWorld;
    private Button _bushMoveEntry=null!,_bushMoveConfirm=null!;
    private Label _bushMoveText=null!;
    private PanelContainer _bushMovePanel=null!;
    private Node3D _bushMoveMarks=null!;
    private string _bushMoveDrawKey="";
    private float _bushMoveRefresh;
    private void MakeBushMoveUi()
    {
        _bushMoveEntry=Button("Move berry bush · Creative",BeginBushMove);_buildSections[1].AddChild(_bushMoveEntry);
        _bushMovePanel=HudPanel(_hud);_bushMovePanel.Hide();var column=new VBoxContainer();_bushMovePanel.AddChild(column);
        column.AddChild(Text("MOVE BERRY BUSH",16));_bushMoveText=Text("",14,true);column.AddChild(_bushMoveText);
        _bushMoveConfirm=Button("Move here",ConfirmBushMove);column.AddChild(_bushMoveConfirm);
        column.AddChild(Button("Choose another bush",()=>{_bushMoveId=-1;_bushMoveAt=null;_bushMoveRefresh=0;ClearBushMoveMarks();}));
        column.AddChild(Button("Cancel [Esc]",CancelBushMove));_bushMoveMarks=new();AddChild(_bushMoveMarks);
    }
    private void ClearBushMoveMarks()
    {foreach(var node in _bushMoveMarks.GetChildren()){_bushMoveMarks.RemoveChild(node);node.QueueFree();}_bushMoveDrawKey="";}
    private void CancelBushMove()
    {
        if(!_bushMoving)return;
        _bushMoving=false;_bushMoveId=-1;_bushMoveAt=null;_bushMoveWorld=null;_bushMovePanel.Hide();ClearBushMoveMarks();CancelCameraDrag();
    }
    private void BeginBushMove()
    {
        if(!_world.Creative)return;
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _bushMoving=true;_bushMoveWorld=_world;_bushMovePanel.Show();_bushMoveRefresh=0;UpdateBushMove();
    }
    private void UpdateBushMove()
    {
        _bushMoveEntry.Visible=_world.Creative;
        if(!_bushMoving)return;
        if(_bushMoveWorld!=_world || !_world.Creative || _placing || _watching){CancelBushMove();return;}
        _bushMovePanel.Position=new(_hud.Size.X-300,92);_bushMovePanel.Size=new(284,0);
        if(_uiTime<_bushMoveRefresh)return;_bushMoveRefresh=_uiTime+.25f;
        var bush=_world.Bushes.FirstOrDefault(b=>b.Id==_bushMoveId);
        if(bush==null){_bushMoveId=-1;_bushMoveAt=null;_bushMoveConfirm.Disabled=true;_bushMoveText.Text="Click an existing berry bush, then click a destination. Confirm with Move here.\n\nEsc or right-click cancels. Middle-drag or WASD moves the camera.";ClearBushMoveMarks();return;}
        string? problem=_bushMoveAt is Cell at?_world.BushMoveProblem(bush.Id,at):"Click bare ground for a destination.";
        _bushMoveConfirm.Disabled=_bushMoveAt==null || problem!=null;
        _bushMoveText.Text=$"Bush {bush.Id} · {bush.Ripe}/8 ripe berries\n"+(problem??"Ready to move. The small marker is the east-side picking spot.")+"\n\nRipe berries and regrowth stay. Moving releases the current picker; gathered berries continue to storage.";
        string key=$"{bush.Id}:{bush.Cell}:{_bushMoveAt}:{problem}";if(key==_bushMoveDrawKey)return;
        ClearBushMoveMarks();_bushMoveDrawKey=key;
        Box(_bushMoveMarks,OnGround(bush.Cell.X,bush.Cell.Z,.04f),new(1,.05f,1),new("dbc17e"));
        if(_bushMoveAt is Cell target)
        {
            var tint=new Color(problem==null?"a4caa0":"e38673");
            Box(_bushMoveMarks,OnGround(target.X,target.Z,.06f),new(.94f,.06f,.94f),tint);
            Box(_bushMoveMarks,OnGround(target.X+1,target.Z,.06f),new(.4f,.06f,.4f),tint);
            Mesh(_bushMoveMarks,new SphereMesh{Radius=.42f,Height=.72f,RadialSegments=7,Rings=4},OnGround(target.X,target.Z,.34f),tint);
        }
    }
    private void ConfirmBushMove()
    {
        if(!_bushMoving || _bushMoveWorld!=_world || _bushMoveAt is not Cell at){CancelBushMove();return;}
        if(_world.BushMoveProblem(_bushMoveId,at) is string problem){_bushMoveRefresh=0;UpdateBushMove();Notice(problem);UiCue(Cue.Reject);return;}
        if(!_world.MoveBush(_bushMoveId,at))return;
        CancelBushMove();RenderFoodViews();RenderActors(0);Notice("Berry bush moved; berries and regrowth retained.");UiCue(Cue.Place);
    }
    private bool HandleBushMoveInput(InputEvent input)
    {
        if(!_bushMoving)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelBushMove();return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){CancelBushMove();return false;}
        if(input is not InputEventMouseButton{ButtonIndex:MouseButton.Left} button || PointerOverHud(button.Position))return false;
        if(button.Pressed && Ground(button.Position) is Vector3 point)
        {
            var cell=new Cell(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));
            if(_bushMoveId<0){var bush=_world.Bushes.FirstOrDefault(b=>b.Cell==cell);if(bush!=null)_bushMoveId=bush.Id;else Notice("Choose a berry bush first.");}
            else _bushMoveAt=cell;
            _bushMoveRefresh=0;UpdateBushMove();
        }
        return true;
    }
}
