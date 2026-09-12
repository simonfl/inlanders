using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private World? _fishViewWorld;
    private Node3D? _fishGroundView;
    private readonly Dictionary<int,(Label3D Label,Node3D[] Fish)> _fishGrounds=new();
    private readonly Dictionary<int,(Node3D Root,Node3D Left,Node3D Right,Node3D Cargo,int Count)> _boatViews=new();
    private void RenderFishingGrounds()
    {
        if(_fishViewWorld!=_world)
        {
            _fishViewWorld=_world; _fishGroundView?.QueueFree(); _fishGroundView=new(); AddChild(_fishGroundView); _fishGrounds.Clear(); _boatViews.Clear();
            foreach(var habitat in _world.Map.FishingGrounds)
            {
                var root=new Node3D { Position=new(habitat.Cell.X,-.055f,habitat.Cell.Z) }; _fishGroundView.AddChild(root);
                var school=new Node3D[3];
                for(int i=0;i<school.Length;i++) { school[i]=new(); root.AddChild(school[i]); MakeFish(school[i],Vector3.Zero); }
                var label=new Label3D { Position=new(0,2f,0),FontSize=23,PixelSize=.009f,Billboard=BaseMaterial3D.BillboardModeEnum.Enabled,Modulate=new("dfede0"),OutlineSize=5 };
                root.AddChild(label); _fishGrounds[habitat.Id]=(label,school);
            }
        }
        foreach(var habitat in _world.Map.FishingGrounds)
        {
            var view=_fishGrounds[habitat.Id];
            view.Label.Text=$"{habitat.Name}\n{habitat.Available}/{habitat.Capacity} fish · +{habitat.RegrowthPerSecond*60:0.#}/min";
            view.Label.Visible=WorldLabelsVisible && !_surveying;
            for(int i=0;i<view.Fish.Length;i++)
            {
                float angle=_clock*.25f+i*Mathf.Tau/3;
                view.Fish[i].Position=new(Mathf.Cos(angle)*.55f,0,Mathf.Sin(angle)*.55f);
                view.Fish[i].Rotation=new(0,-angle,0);
                view.Fish[i].Visible=habitat.Stock>i*habitat.Capacity/3f;
            }
        }
        RenderBoats();
    }
    private void RenderBoats()
    {
        foreach(int id in _boatViews.Keys.Where(id=>!_world.Cottages.Any(c=>c.Id==id && !c.DemolitionRequested)).ToArray())
        { _boatViews[id].Root.QueueFree(); _boatViews.Remove(id); }
        foreach(var dock in _world.Cottages.Where(c=>c.Kind==BuildingKind.FishingDock && c.Complete && !c.DemolitionRequested))
        {
            if(!_boatViews.TryGetValue(dock.Id,out var view))
            {
                var root=new Node3D(); _fishGroundView!.AddChild(root);
                Box(root,new(0,.035f,0),new(.55f,.08f,1.25f),new("85674b"));
                foreach(float side in new[]{-1f,1f})
                {
                    Box(root,new(side*.31f,.16f,0),new(.09f,.25f,1.05f),new("a78661"));
                    foreach(float end in new[]{-1f,1f})
                    {
                        var bow=Box(root,new(side*.17f,.16f,end*.64f),new(.075f,.23f,.46f),new("a78661"));
                        bow.RotationDegrees=new(0,side*end*48,0);
                    }
                }
                foreach(float z in new[]{-.32f,.2f}) Box(root,new(0,.22f,z),new(.6f,.065f,.18f),new("c4aa83"));
                var left=new Node3D { Name="LeftOar",Position=new(-.3f,.3f,0) }; var right=new Node3D { Name="RightOar",Position=new(.3f,.3f,0) };
                root.AddChild(left); root.AddChild(right);
                foreach(var (oar,side) in new[]{(left,-1f),(right,1f)})
                {
                    Box(oar,new(side*.34f,0,0),new(.85f,.045f,.045f),_wood);
                    Box(oar,new(side*.75f,-.025f,0),new(.32f,.04f,.15f),new("b99d72"));
                }
                var cargo=new Node3D { Name="Catch",Position=new(0,.16f,-.42f) }; root.AddChild(cargo);
                var net=new Node3D { Name="Net",Position=new(.45f,.04f,.15f) }; root.AddChild(net);
                for(int i=0;i<5;i++)
                {
                    Box(net,new(i*.11f,0,0),new(.012f,.018f,.55f),new("bcb38e"));
                    Box(net,new(.22f,0,-.275f+i*.1375f),new(.46f,.018f,.012f),new("bcb38e"));
                }
                BatchStaticGeometry(net);
                BatchStaticGeometry(root);
                view=(root,left,right,cargo,-1); _boatViews[dock.Id]=view;
            }
            var boat=dock.Boat;
            var at=boat?.Position??dock.Launch.Point;
            view.Root.Position=new(at.X,0,at.Y);
            view.Root.Rotation=new(0,boat?.Heading??((dock.Rotation*Mathf.Pi/2)+(dock.DockFromFar?Mathf.Pi:0)),0);
            bool rowing=boat?.Route.Count>0 && boat.Phase!=BoatPhase.Moored;
            float stroke=rowing?Mathf.Sin(_clock*4)*.45f:0;
            view.Left.Rotation=new(0,stroke,-.12f); view.Right.Rotation=new(0,-stroke,.12f);
            var fishingNet=view.Root.GetNode<Node3D>("Net");
            fishingNet.Visible=boat?.Phase==BoatPhase.Fishing;
            fishingNet.Position=new(.45f,.04f+Mathf.Sin(_clock*2)*.035f,.15f);
            int count=boat?.Fish??0;
            if(view.Count!=count)
            {
                Clear(view.Cargo);
                for(int i=0;i<count;i++) MakeFish(view.Cargo,new((i%2-.5f)*.17f,i/2*.06f,0));
                view.Count=count; _boatViews[dock.Id]=view;
            }
        }
    }
    private void MakeFish(Node3D parent,Vector3 at)
    {
        var body=Mesh(parent,new SphereMesh { Radius=.13f,Height=.26f,RadialSegments=7,Rings=3 },at,new("8caeb2"));
        body.Scale=new(.65f,.42f,1.35f);
        var tail=Cylinder(parent,at+new Vector3(0,0,.19f),.10f,.15f,new("6d949f"),0);
        tail.RotationDegrees=new(90,0,0); tail.Scale=new(1,.7f,.35f);
        foreach(float side in new[]{-1f,1f})
            Mesh(parent,new SphereMesh { Radius=.013f,Height=.026f,RadialSegments=5,Rings=3 },at+new Vector3(side*.066f,.022f,-.085f),new("344b4d"));
    }
}
