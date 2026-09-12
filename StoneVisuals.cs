using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private readonly Dictionary<int,(Node3D Body,int Remaining)> _depositViews=new();
    private Node3D? _stoneStoreView;
    private int _shownStone=-1;
    private void StonePiece(Node3D parent,Vector3 position,float size=.22f)
    {
        var rock=Mesh(parent,new SphereMesh { Radius=size,Height=size*1.45f,RadialSegments=5,Rings=2 },position,new("909589"));
        rock.Scale=new(1.25f,1,.85f); rock.Rotation=new(.1f,.3f,.12f);
    }
    private void RenderStone()
    {
        foreach(var deposit in _world.Map.StoneDeposits)
        {
            if(_depositViews.TryGetValue(deposit.Id,out var old) && old.Remaining==deposit.Remaining) continue;
            old.Body?.QueueFree();
            var body=new Node3D { Position=OnGround(deposit.Cell.X,deposit.Cell.Z) }; _dynamic.AddChild(body);
            float fullness=(float)deposit.Remaining/deposit.Capacity;
            for(int i=0;i<3;i++)
            {
                var rock=Box(body,new((i-1)*.22f,.15f+fullness*.35f,(i%2-.5f)*.16f),new(.45f,.25f+fullness*.8f,.65f),new Color("868d82").Lightened(i*.05f));
                rock.Rotation=new(0,(i-1)*.18f,(i-1)*.12f);
            }
            FoodSign(body,deposit.Remaining>0?$"STONE {deposit.Remaining}":"EXHAUSTED",1.5f);
            _depositViews[deposit.Id]=(body,deposit.Remaining);
        }
        if(_shownStone==_world.YardStone) return;
        _shownStone=_world.YardStone; _stoneStoreView?.QueueFree(); _stoneStoreView=new(); _dynamic.AddChild(_stoneStoreView);
        _stoneStoreView.Position=OnGround(_world.Stockpile.X-1.1f,_world.Stockpile.Z+1.1f);
        for(int i=0;i<Math.Min(12,_world.YardStone);i++) StonePiece(_stoneStoreView,new(i%3*.3f,.12f+i/6*.23f,i%6/3*.3f),.17f);
    }
    private void MakeQuarryCamp(Node3D parent,int stage)
    {
        foreach(float x in new[]{-.95f,.95f}) Box(parent,new(x,.10f,-.55f),new(.3f,.2f,.3f),_wood);
        if(stage==0) return;
        foreach(float x in new[]{-.95f,.95f})
        {
            Box(parent,new(x,.77f,-.55f),new(.14f,1.5f,.14f),_frameTimber);
            TimberBeam(parent,new(x,.3f,-.55f),new(x,.9f,.15f),.10f,_wood);
        }
        if(stage<2) return;
        Box(parent,new(0,1.48f,-.55f),new(2.2f,.15f,.16f),_wood);
        var canopy=Box(parent,new(0,1.4f,-.2f),new(2.35f,.07f,1.15f),new("aa9775")); canopy.Rotation=new(.16f,0,0);
        Box(parent,new(-.7f,.3f,.35f),new(.85f,.6f,.6f),_wood);
        if(stage<3) return;
        Cylinder(parent,new(.72f,.32f,.15f),.3f,.25f,new("605d50"));
        foreach(float x in new[]{-.8f,-.5f}) TimberBeam(parent,new(x,.25f,.7f),new(x+.1f,1.1f,.5f),.05f,_wood);
        FoodSign(parent,"QUARRY CAMP",2f);
    }
}
