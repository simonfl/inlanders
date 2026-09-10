using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private readonly List<Node3D> _wildlifeViews=new();
    private string _wildlifeKey="";
    private void GameParcel(Node3D parent,Vector3 at)
    {
        Box(parent,at,new(.26f,.19f,.27f),new("99745d"));
        Box(parent,at+new Vector3(0,.105f,0),new(.035f,.025f,.29f),new("dfcda7"));
        Box(parent,at+new Vector3(0,.107f,0),new(.28f,.025f,.035f),new("dfcda7"));
    }
    private void MakeHuntingLodge(Node3D parent,int stage)
    {
        foreach(float x in new[]{-1f,1f}) foreach(float z in new[]{-.5f,.5f}) StoneFoot(parent,new(x,.1f,z),new(.28f,.2f,.28f));
        if(stage==0) return;
        foreach(float x in new[]{-1f,1f}) Box(parent,new(x,.8f,-.35f),new(.17f,1.6f,.17f),_frameTimber);
        if(stage<2) return;
        for(int i=0;i<7;i++) Box(parent,new(0,.3f+i*.17f,-.62f),new(2.1f,.14f,.18f),new("715a40"));
        Box(parent,new(-.95f,.65f,0),new(.15f,1.1f,1.15f),new("887651"));
        if(stage<3) return;
        VillageRoof(parent,new(-.18f,1.65f,-.1f),2.65f,1.9f,.75f,new("65754c"),false);
        foreach(float x in new[]{.52f,1.08f}) TimberBeam(parent,new(x,.1f,.7f),new(x,1.4f,.7f),.065f,_wood);
        TimberBeam(parent,new(.48f,1.3f,.7f),new(1.12f,1.3f,.7f),.065f,_wood);
        Box(parent,new(.80f,1.02f,.7f),new(.38f,.4f,.04f),new("ae9b70"));
        Cylinder(parent,new(-.55f,.24f,.45f),.29f,.46f,_wood);
        FoodSign(parent,"HUNTING LODGE",2.9f);
    }
    private void RenderWildlife()
    {
        bool survey=_surveying || _placing && (_buildKind==BuildingKind.HuntingLodge || _clearingTrees || _woodlandTool>0);
        string key=string.Join(";",_world.Map.Wildlife.Select(h=>$"{h.Id}:{(int)h.Stock}:{_world.AvailableGame(h)}:{_world.HabitatTrees(h)}"))+$"/{survey}/{_world.Cottages.Count}/{_world.Trees.Count}/{_world.Decorations.Count}";
        if(key!=_wildlifeKey)
        {
            _wildlifeKey=key; foreach(var node in _wildlifeViews) node.QueueFree(); _wildlifeViews.Clear();
            foreach(var h in _world.Map.Wildlife)
            {
                var region=new Node3D(); _dynamic.AddChild(region); _wildlifeViews.Add(region);
                var marker=new Node3D { Position=OnGround(h.Cell.X,h.Cell.Z) }; region.AddChild(marker);
                FoodSign(marker,$"WOODLAND {h.Id} · {_world.AvailableGame(h)}/{_world.HabitatCapacity(h)} GAME",1.2f);
                if(survey)
                    for(int i=0;i<24;i++) { float a=i*Mathf.Tau/24; var at=new Cell(h.Cell.X+(int)MathF.Round(MathF.Cos(a)*5),h.Cell.Z+(int)MathF.Round(MathF.Sin(a)*5)); if(_world.Map.Contains(at)) Cylinder(region,OnGround(at.X,at.Z,.04f),.13f,.035f,new("c8b578")); }
                int count=Math.Min(4,(int)MathF.Ceiling(h.Stock/3));
                var spots=_world.WildlifeSpots(h).Where(c=>(c.Point-h.Cell.Point).LengthSquared()>=4).Take(count*4).Where((_,i)=>i%4==0).ToArray();
                for(int i=0;i<Math.Min(count,spots.Length);i++)
                {
                    var deer=new Node3D { Position=OnGround(spots[i].X,spots[i].Z),Name="Deer"+i }; region.AddChild(deer);
                    var torso=Mesh(deer,new SphereMesh { Radius=.29f,Height=.5f,RadialSegments=7,Rings=3 },new(0,.48f,0),new("9e7d58")); torso.Scale=new(.8f,1,1.45f);
                    foreach(float x in new[]{-.15f,.15f}) foreach(float z in new[]{-.22f,.22f}) Box(deer,new(x,.23f,z),new(.065f,.46f,.07f),new("68553f"));
                    var head=new Node3D { Name="Head",Position=new(0,.65f,-.33f) }; deer.AddChild(head);
                    Mesh(head,new SphereMesh { Radius=.15f,Height=.35f,RadialSegments=6,Rings=3 },new(0,.12f,0),new("b5966e"));
                    foreach(float x in new[]{-.11f,.11f}) { var ear=Box(head,new(x,.30f,0),new(.07f,.22f,.05f),new("c6ac84")); ear.Rotation=new(0,0,x*3); }
                    Box(head,new(0,.08f,-.15f),new(.13f,.12f,.15f),new("5f5140"));
                    BatchStaticGeometry(head); BatchStaticGeometry(deer);
                }
            }
        }
        foreach(var region in _wildlifeViews)
            foreach(var deer in region.GetChildren().OfType<Node3D>().Where(n=>n.Name.ToString().StartsWith("Deer")))
            {
                float phase=_world.Food.Time*.6f+deer.Position.X;
                deer.Rotation=new(0,MathF.Sin(phase*.18f)*.5f,0);
                deer.GetNode<Node3D>("Head").Rotation=new(.25f+MathF.Sin(phase)*.35f,MathF.Sin(phase*.5f)*.15f,0);
            }
    }
}
