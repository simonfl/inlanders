using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _careActive,_careDragging,_careSelected,_careWasPaused;
    private World? _careWorld;
    private Cell _careFirst,_careLast;
    private WoodlandIntent _careIntent=WoodlandIntent.Renew;
    private PanelContainer _carePanel=null!;
    private Label _careText=null!;
    private Button _careApply=null!;
    private readonly System.Collections.Generic.Dictionary<WoodlandIntent,Button> _careChoices=new();
    private Node3D _careMarks=null!;
    private void MakeWoodlandCare(VBoxContainer parent)
    {
        parent.AddChild(Button("Care for woodland",BeginWoodlandCare));
        _carePanel=HudPanel(_hud);var column=new VBoxContainer();_carePanel.AddChild(column);
        column.AddChild(Text("THIS WOODLAND",16));
        foreach(var choice in new[]{(WoodlandIntent.Keep,"Keep standing"),(WoodlandIntent.Renew,"Harvest & renew"),(WoodlandIntent.Clear,"Clear for another use")})
        {var intent=choice.Item1;var button=Button(choice.Item2,()=>{_careIntent=intent;RefreshWoodlandCare();});button.ToggleMode=true;_careChoices[intent]=button;column.AddChild(button);}
        _careText=Text("",13,true);_careText.CustomMinimumSize=new(250,0);column.AddChild(_careText);
        _careApply=Button("Apply & watch",ApplyWoodlandCare);column.AddChild(_careApply);
        column.AddChild(Button("Cancel [Esc]",CancelWoodlandCare));_carePanel.Hide();_careMarks=new();AddChild(_careMarks);
    }
    private void BeginWoodlandCare()
    {
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _careActive=true;_careWorld=_world;_careWasPaused=_paused;_paused=true;_pauseButton.Disabled=true;
        _careSelected=_careDragging=false;_carePanel.Show();RefreshWoodlandCare();
    }
    private void CancelWoodlandCare()
    {
        if(!_careActive)return;_careActive=_careDragging=false;_carePanel.Hide();Clear(_careMarks);
        _paused=_careWasPaused;_pauseButton.Disabled=false;
    }
    private void RefreshWoodlandCare()
    {
        if(!_careActive)return;
        _carePanel.Position=new(_hud.Size.X-292,92);_carePanel.Size=new(276,0);Clear(_careMarks);
        foreach(var pair in _careChoices)pair.Value.SetPressedNoSignal(pair.Key==_careIntent);
        string consequence=_careIntent switch{WoodlandIntent.Keep=>"Cancel clearing; keep living trees standing. Fallen trees need renewal.",WoodlandIntent.Renew=>"Cancel clearing, take needed timber and replant. Collected timber stays collected.",_=>_world.Creative?"Recover timber and remove trees and roots immediately.":"Collect timber and remove roots for new uses. No regrowth."};
        _careText.Text=consequence+(_careIntent==WoodlandIntent.Renew?"\nDrag to renew trees and plant spaced bare ground.":"\nDrag trees or click a spot; Apply starts work.");
        _careApply.Disabled=!_careSelected;
        if(!_careSelected)return;
        var plan=_world.PreviewWoodlandCare(_careFirst,_careLast,_careIntent);_careApply.Disabled=plan.Problem!=null;
        _careText.Text+=$"\n{plan.Cells.Length} tree spots · {plan.Timber} existing logs\n"+(plan.Problem??"Ready; drag again to revise.");
        if(_careIntent!=WoodlandIntent.Keep && plan.Cells.Any(c=>_world.HabitatLoss(c)!=""))_careText.Text+="\nFelling reduces hunting habitat.";
        foreach(var c in plan.Cells)GroundPatch(_careMarks,c.X,c.Z,.92f,.92f,plan.Problem==null?new("d6bc7a"):new("cc7967"),.07f);
    }
    private void ApplyWoodlandCare()
    {
        if(!_careActive || !_careSelected || _careWorld!=_world)return;
        if(!_world.ApplyWoodlandCare(_careFirst,_careLast,_careIntent)){RefreshWoodlandCare();return;}
        CancelWoodlandCare();_paused=false;ToggleWatch();UiCue(Cue.Place);
    }
    private bool HandleWoodlandCare(InputEvent input)
    {
        if(!_careActive)return false;
        if(_careWorld!=_world){CancelWoodlandCare();return false;}
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelWoodlandCare();return true;}
        if(input is InputEventKey{Pressed:true} key)
        {
            if(key.Keycode is Key.Space or Key.F5 or Key.F9)return true;
            if(key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){CancelWoodlandCare();return false;}
        }
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left} click)
        {
            if(PointerOverHud(click.Position)){if(!click.Pressed)_careDragging=false;return false;}
            if(Ground(click.Position) is not Vector3 point)return true;
            var cell=new Cell(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));
            if(click.Pressed){_careFirst=_careLast=cell;_careDragging=_careSelected=true;}
            else if(_careDragging){_careLast=cell;_careDragging=false;}
            RefreshWoodlandCare();return true;
        }
        if(_careDragging && input is InputEventMouseMotion motion && !PointerOverHud(motion.Position) && Ground(motion.Position) is Vector3 at)
        {var cell=new Cell(Mathf.RoundToInt(at.X),Mathf.RoundToInt(at.Z));if(cell!=_careLast){_careLast=cell;RefreshWoodlandCare();}return true;}
        return false;
    }
}
