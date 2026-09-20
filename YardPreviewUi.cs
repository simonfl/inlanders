using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private VBoxContainer _yardChoices=null!;
    private readonly Button[] _yardSides=new Button[4];
    private Button _yardApply=null!;
    private Label _yardPreviewInfo=null!;
    private int _yardPreviewSide=-1,_yardPreviewHome=-1;
    private Node3D? _yardPreviewGround;
    private string _yardPreviewKey="";
    private void StopYardPreview()
    {
        _yardPreviewSide=_yardPreviewHome=-1;_yardPreviewKey="";
        if(_yardChoices!=null)_yardChoices.Hide();
        if(_yardPreviewGround!=null && GodotObject.IsInstanceValid(_yardPreviewGround))_yardPreviewGround.Hide();
        _nextWorkCard=0;
    }
    private void BeginYardPreview()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(home==null)return;
        _yardPreviewHome=home.Id;_yardPreviewSide=home.YardSide;_nextWorkCard=0;
    }
    private void MakeYardPreview(VBoxContainer column)
    {
        _yardChoices=new(){Visible=false};column.AddChild(_yardChoices);
        var grid=new GridContainer{Columns=2};_yardChoices.AddChild(grid);
        for(int i=0;i<4;i++){int side=i;_yardSides[i]=Button(World.YardSideName(i),()=>{_yardPreviewSide=side;_nextWorkCard=0;});_yardSides[i].SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;grid.AddChild(_yardSides[i]);}
        _yardPreviewInfo=Text("",13,true);_yardChoices.AddChild(_yardPreviewInfo);
        var actions=new HBoxContainer();_yardChoices.AddChild(actions);
        _yardApply=Button("Use this ground",()=>{if(_world.SetHomeYard(_yardPreviewHome,_yardPreviewSide))StopYardPreview();});actions.AddChild(_yardApply);
        actions.AddChild(Button("Cancel [Esc]",StopYardPreview));
    }
    private void RenderYardPreview(Cottage home)
    {
        bool show=_yardPreviewHome==home.Id && _yardPreviewSide>=0;_yardChoices.Visible=show;if(!show)return;
        string? problem=_world.HomeYardProblem(home.Id,_yardPreviewSide);
        var places=_world.YardPlaces(home,_yardPreviewSide);
        _yardPreviewInfo.Text=problem??$"Gold ground · {places.Length} usable places for quiet work and nearby meals. "+(home.Improved?"Move existing furniture free.":$"Furnishing afterward costs {(_world.Creative?"nothing":World.ComfortCost(home)+" planks")}.");
        _yardApply.Disabled=problem!=null;_yardApply.Text=home.Improved?"Move yard here":"Use this ground";
        foreach(var side in Enumerable.Range(0,4))_yardSides[side].Modulate=side==_yardPreviewSide?_cream:Colors.White;
        if(_yardPreviewGround==null || !GodotObject.IsInstanceValid(_yardPreviewGround)){_yardPreviewGround=new();AddChild(_yardPreviewGround);_yardPreviewKey="";}
        _yardPreviewGround.Show();string key=$"{home.Id}/{_yardPreviewSide}/{problem}/"+string.Join(';',places.Select(c=>$"{c}:{Height(c.X,c.Z)}"));
        if(key==_yardPreviewKey)return;_yardPreviewKey=key;Clear(_yardPreviewGround);
        foreach(var c in places)
        {
            GroundPatch(_yardPreviewGround,c.X,c.Z,.92f,.92f,problem==null?new("e4c779"):new Color("bf7860"),.075f);
            // The preview shows the footprint and scale of the usable space, not a simulated resident.
            foreach(float x in new[]{-.40f,.40f})Box(_yardPreviewGround,OnGround(c.X+x,c.Z,.11f),new(.035f,.05f,.84f),_cream);
        }
    }
    private string HomeCardOutcome(Cottage home)
    {
        if(home.ImprovementRequested)
        {
            var worker=_world.People.FirstOrDefault(p=>p.ComfortHomeId==home.Id);
            return "Making room for outdoor life.\n"+(worker!=null?worker.Name+": "+worker.Status:home.ImprovementPlanks<World.ComfortCost(home) && _world.AvailablePlanks==0?"Waiting for planks; a sawmill makes them from logs.":"Shared workers will deliver and install the furnishings.");
        }
        if(!home.Improved && home.ImprovementPlanks>0)return "Cancelled furnishing. Workers are recovering the planks before another order.";
        string places=$"{World.YardSideName(home.YardSide)} · {_world.PotentialHomeYardPlaces(home).Length} usable outdoor places.";
        if(home.Improved)return "A yard for mending and nearby meals.\n"+places;
        return "Give these neighbors a place to mend and eat outside.\n"+places;
    }
}
