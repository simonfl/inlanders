using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private VBoxContainer _welcomeControls=null!;
    private Label _welcomeInfo=null!;
    private Button _welcomeHere=null!;
    private Node3D? _welcomeDisplay;
    private string _welcomeDisplayKey="";
    private void MakeWelcomeControls()
    {
        _welcomeControls=new();_buildingDetails.AddChild(_welcomeControls);
        _buildingDetails.MoveChild(_welcomeControls,1);
        _welcomeInfo=Text("",14,true);_welcomeControls.AddChild(_welcomeInfo);
        _welcomeHere=Button("Prepare welcome meal here",()=>{if(_world.ChooseWelcomeVenue(_selectedSite))UpdateHud();});_welcomeControls.AddChild(_welcomeHere);
    }
    private void UpdateWelcomeControls()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
        _welcomeControls.Visible=_world.Neighborhood!=null && site!=null && Buildings.Get(site.Kind).RecreationSlots>0;
        if(!_welcomeControls.Visible)return;
        string? problem=_world.WelcomeVenueProblem(site!.Id);
        bool selected=_world.Neighborhood!.VenueId==site.Id;
        _welcomeInfo.Text=selected?_world.WelcomeStatus:problem??"Helpers can bring food here for a welcome meal. Any edible food works; normal village routines continue.";
        _welcomeHere.Disabled=problem!=null || selected;
        _welcomeHere.Text=selected?"Welcome venue selected":"Prepare welcome meal here";
        _welcomeHere.TooltipText=problem??"Choose this venue. Preparations persist through shortages; visits count after residents actually eat here.";
    }
    private void RenderWelcomeDisplay()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_world.Neighborhood?.VenueId);
        string key=site==null?"":$"{site.Id}:{site.Cell}:{string.Join(',',site.PantryFood)}";
        if(key==_welcomeDisplayKey)return;
        _welcomeDisplayKey=key;_welcomeDisplay?.QueueFree();_welcomeDisplay=null;
        if(site==null)return;
        _welcomeDisplay=new(){Position=OnGround(site.Entrance.X,site.Entrance.Z)+new Vector3(.48f,0,.4f)};_dynamic.AddChild(_welcomeDisplay);
        Box(_welcomeDisplay,new(0,.35f,0),new(.85f,.12f,.58f),new("a7794f"));
        foreach(float x in new[]{-.3f,.3f})Box(_welcomeDisplay,new(x,.16f,0),new(.08f,.32f,.4f),_wood);
        var colors=new Color[]{new("ba7183"),new("87a05e"),new("d8b575"),new("8badae"),new("b08067"),new("bd5544")};
        int item=0;
        for(int k=0;k<site.PantryFood.Length;k++)for(int i=0;i<site.PantryFood[k];i++,item++)
            Box(_welcomeDisplay,new(-.28f+(item%4)*.18f,.45f+(item/12)*.1f,-.16f+(item%12/4)*.17f),new(.13f,.08f,.12f),colors[k]);
        FoodSign(_welcomeDisplay,"WELCOME",1.05f);
    }
}
