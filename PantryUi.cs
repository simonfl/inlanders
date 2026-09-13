using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private VBoxContainer _pantryControls=null!;
    private Label _pantryInfo=null!;
    private Button _pantryLess=null!,_pantryMore=null!;
    private void MakePantryControls()
    {
        _pantryControls=new(); _buildingDetails.AddChild(_pantryControls);
        _pantryInfo=Text("",14,true); _pantryControls.AddChild(_pantryInfo);
        var row=new HBoxContainer(); _pantryControls.AddChild(row);
        void Change(int amount)
        {
            var p=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && c.Kind==BuildingKind.Pantry);
            if(p!=null) _world.SetPantryTarget(p.Id,System.Math.Clamp(p.PantryTarget+amount,0,24));
        }
        _pantryLess=Button("Target −4",()=>Change(-4)); row.AddChild(_pantryLess);
        _pantryMore=Button("Target +4",()=>Change(4)); row.AddChild(_pantryMore);
        _pantryControls.Hide();
    }
    private void UpdatePantryControls()
    {
        var pantry=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && (c.Kind==BuildingKind.Pantry || _world.IsWorkplaceFoodStore(c)) && !c.DemolitionRequested);
        _pantryControls.Visible=pantry!=null; if(pantry==null) return;
        bool workplace=_world.IsWorkplaceFoodStore(pantry);
        _pantryLess.Visible=_pantryMore.Visible=!workplace;
        if(workplace)
        {
            _pantryInfo.Text="WORKPLACE FOOD\n"+(pantry.Complete?string.Join("\n",World.EdibleKinds.Where(k=>_world.FoodAt(pantry.Id,k)>0).Select(k=>$"{k}: {_world.FoodAt(pantry.Id,k)} stored · {_world.FoodReservedAt(pantry.Id,k)} reserved"))+$"\n{pantry.PantryFood.Sum()}/24 portions · {_world.FoodIncoming(pantry.Id)} incoming":"Storage opens after construction.")+"\nPeople can collect meals here. Haulers leave four portions for local meals and move surplus to neighborhood pantries or central storage. Pausing production leaves stored food available. A full store sends new output to another pantry.";
            return;
        }
        _pantryInfo.Text=$"Supply target {pantry.PantryTarget}/24 portions\n"+(pantry.Complete?string.Join("\n",World.EdibleKinds.Select(k=>$"{k}: {_world.FoodAt(pantry.Id,k)} stored · {_world.FoodReservedAt(pantry.Id,k)} reserved"))+$"\n{_world.FoodIncoming(pantry.Id)} incoming":"Food service begins after construction.")+"\nProducers can deliver here directly. Optional haulers replenish from central food and return stock above the target. Target 0 drains stock with haulers; meals and direct producer deposits continue.";
        _pantryLess.Disabled=pantry.PantryTarget==0; _pantryMore.Disabled=pantry.PantryTarget==24;
        if(_world.HasWorkplaceFood)_pantryInfo.Text+="\nIn this experiment, haulers also collect surplus directly from workplace stores.";
    }
    private void MakePantry(Node3D parent,Cottage site,int stage)
    {
        Box(parent,new(0,.08f,0),new(2.7f,.16f,1.7f),new("967e5a"));
        if(stage==0) return;
        foreach(float x in new[]{-1.15f,1.15f}) foreach(float z in new[]{-.65f,.65f})
            Box(parent,new(x,.9f,z),new(.12f,1.8f,.12f),_wood);
        Box(parent,new(0,.5f,-.5f),new(2.5f,.1f,.5f),_wood);
        if(stage<2) return;
        Box(parent,new(0,.6f,.55f),new(2.5f,.18f,.42f),new("bb9b67"));
        if(stage<3) return;
        VillageRoof(parent,new(0,1.85f,0),2.9f,1.95f,.35f,new("798963"),false);
        var colors=new Color[]{new("ba7183"),new("87a05e"),new("d8b575"),new("8badae"),new("b08067"),new("bd5544")};
        for(int kind=0;kind<World.EdibleKinds.Length;kind++) for(int n=0;n<site.PantryFood[kind];n++)
        {
            var at=new Vector3(-1f+kind*.4f,.62f+n/4*.12f,-.62f+n%4*.12f);
            if(World.EdibleKinds[kind]==Inlanders.Simulation.Resource.Fruit)MakeFruit(parent,at,.06f);
            else Box(parent,at,new(.22f,.10f,.10f),colors[kind]);
        }
        FoodSign(parent,"NEIGHBORHOOD PANTRY",2.6f);
    }
}
