using Godot;
using Inlanders.Simulation;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private void MakeWorkplaceFoodStock(Node3D parent,Cottage site)
    {
        if(site.Kind==BuildingKind.FishingDock)
        {
            int fish=site.PantryFood[System.Array.IndexOf(World.EdibleKinds,Resource.Fish)];
            for(int i=0;i<fish;i++)MakeFish(parent,new(.77f+i%3*.17f,.75f+i/6*.055f,-.17f+i/3%2*.30f));
            return;
        }
        int portion=0;
        for(int kind=0;kind<World.EdibleKinds.Length;kind++)for(int n=0;n<site.PantryFood[kind];n++)
        {
            int slot=portion++;
            var at=new Vector3(-1.05f+slot%4*.17f,.17f+slot/8*.10f,1.02f+slot/4%2*.16f);
            Color color=World.EdibleKinds[kind] switch {Resource.Berries=>new("ac667d"),Resource.Vegetables=>new("81975e"),Resource.Fish=>new("8badae"),Resource.Game=>new("b08067"),Resource.Fruit=>new("bd5544"),_=>new("d1ab70")};
            Box(parent,at,new(.14f,.09f,.13f),color);
        }
        if(portion>0)Box(parent,new(-.79f,.08f,1.1f),new(.79f,.09f,.46f),new("846d4c"));
    }
}
