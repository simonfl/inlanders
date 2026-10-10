using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private PanelContainer _additionCard=null!;
    private Label _additionText=null!;
    private Node3D _additionMarks=null!;
    private PreparedAddition? _shownAddition;
    private World? _additionWorld;
    private bool _additionSelected;
    private Button _additionCancel=null!,_additionResume=null!;
    private void MakePreparedAdditionUi()
    {
        _additionMarks=new();AddChild(_additionMarks);_additionCard=HudPanel(_hud);var box=new VBoxContainer();_additionCard.AddChild(box);
        _additionText=Text("",14,true);_additionText.CustomMinimumSize=new(270,0);box.AddChild(_additionText);
        box.AddChild(Button("Watch preparation",()=>{_additionSelected=false;_paused=false;ToggleWatch();}));
        _additionResume=Button("Resume clearance & build",()=>{if(_world.ResumePreparedAddition())UiCue(Cue.Click);});box.AddChild(_additionResume);
        _additionCancel=Button("Cancel plan",()=>{if(_world.CancelPreparedAddition()){ClearSelection();UiCue(Cue.Click);}});box.AddChild(_additionCancel);
        box.AddChild(Button("Close",ClearSelection));_additionCard.Hide();
    }
    private void ShowPreparedAddition()
    {CloseManagementUi();_additionSelected=true;_additionWorld=_world;}
    private void RenderPreparedAddition()
    {
        if(_additionMarks==null)return;
        if(_additionWorld!=_world)_additionSelected=false;
        var plan=_world.PendingAddition;
        if(_additionSelected && plan==null && _shownAddition is {} done)
        {
            _additionSelected=false;var site=_world.Cottages.FirstOrDefault(c=>c.Cell==done.Cell && c.Kind==done.Kind);
            if(site!=null)ShowWorkplaceCard(site.Id);
        }
        if(_shownAddition!=plan || _additionWorld!=_world)
        {
            Clear(_additionMarks);_shownAddition=plan;_additionWorld=_world;
            if(plan!=null)foreach(var cell in World.Footprint(plan.Cell,plan.Rotation,plan.Kind,plan.Rows))
                GroundPatch(_additionMarks,cell.X,cell.Z,.92f,.92f,new("d7bb7f"),.055f);
        }
        _additionMarks.Visible=!_atMainMenu;
        _additionCard.Visible=_additionSelected && plan!=null && !_atMainMenu && !_placing && !_watching && !_drawer.Visible;
        if(!_additionCard.Visible || plan==null)return;
        _additionCard.Position=new(_hud.Size.X-306,92);_additionCard.Size=new(290,0);
        _additionResume.Visible=_world.PreparedAdditionConflict();
        _additionResume.Disabled=_world.PreviewGroundPreparation(plan.Cell,plan.Rotation,plan.Kind,plan.Rows)==null;
        var definition=Buildings.Get(plan.Kind);string cost=_world.Creative?"Free":$"{(plan.Rows>0?plan.Rows*2:definition.Cost)} {definition.Material.ToString().ToLowerInvariant()}"+(definition.StoneCost>0?$" + {definition.StoneCost} stone":"");
        _additionText.Text=BuildingName(plan.Kind).ToUpperInvariant()+" · PLANNED\n"+_world.PreparedAdditionStatus()+
            "\nThen build: "+cost+". Ground must remain clear and legal.\nCancel keeps later woodland choices and completed work.";
        _additionCancel.Disabled=_world.Food.Celebrating;
    }
}
