using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckWoodsArt()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        bool before=OS.GetCmdlineUserArgs().Contains("--woods-art-before");
        string prefix=before?"woods-art-before":"woods-art";
        await Frames();
        var thumb=_kindButtons[BuildingKind.HuntingLodge].GetChild<HBoxContainer>(0).GetChild<TextureRect>(0).Texture;
        Check(thumb.GetImage().SavePng($"artifacts/{prefix}-thumbnail.png")==Error.Ok,"Hunting thumbnail capture failed");
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewCampaign(9);
            foreach(var tree in w.Trees)w.SetTreePreserved(tree.Cell,true);
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,rotation,BuildingKind.HuntingLodge)==null)
                .OrderBy(c=>(c.Point-new Cell(0,-3).Point).LengthSquared()).First();
            var lodge=w.Place(cell,rotation,BuildingKind.HuntingLodge)!;w.Assign(6,Role.Hunter);
            for(int i=0;i<7000 && w.People[6].Task!=Work.Hunting;i++)w.Tick(.1f);
            Check(lodge.Complete && w.People[6].Task==Work.Hunting,"Rotated lodge cannot build and hunt");w.Validate();
            AdoptWorld(w);_paused=true;CloseManagementUi();_placing=false;_noticeUntil=0;
            _focus=OnGround(cell.X,cell.Z);_camera.Size=10;_angle=.72f;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                Check(_people[6].Bow.Visible,"Active hunting bow missing");
                await Capture($"artifacts/{prefix}-{rotation}-{width}.png");
            }
            string saved=w.SaveJson();await Frames();Check(w.SaveJson()==saved,"Paused art check changed world");
            AdoptWorld(World.LoadJson(saved));await Frames();Check(_world.SaveJson()==saved,"Active hunt reload differs");w=_world;
            for(int i=0;i<300 && w.People[6].Carried==0;i++)w.Tick(.1f);
            await Frames();Check(_people[6].Carry.Visible && _people[6].Cargo==Inlanders.Simulation.Resource.Game,"Actual catch missing");
            for(int i=0;i<3000 && w.DeliveredGame==0;i++)w.Tick(.1f);
            Check(w.DeliveredGame>0,"Rotated lodge catch not delivered");w.Validate();
        }
        foreach(string state in new[]{"prepared","depleted","cleared","restoring","complete"})
        {
            AdoptWorld(World.LoadFile($"artifacts/woods-{state}.json"));_paused=true;CloseManagementUi();_noticeUntil=0;
            _focus=new(2,0,-2);_camera.Size=27;_angle=.72f;UpdateCamera();await Frames();
            await Capture($"artifacts/{prefix}-{state}-1440.png");
            if(!before)
            {
                foreach(var h in _world.Map.Wildlife)
                {
                    var region=_wildlifeViews[_world.Map.Wildlife.IndexOf(h)];
                    int deer=region.GetChildren().Count(n=>n.Name.ToString().StartsWith("Deer"));
                    Check(deer<=4 && (_world.AvailableGame(h)>0 || deer==0),"Deer imply unclaimed game where none remains");
                    var floor=region.GetNode<Node3D>("ForestFloor");
                    Check(floor.GetChildCount()<=2 && (_world.HabitatTrees(h)>0?floor.GetChildCount()>0:floor.GetChildCount()==0),"Forest cover is unbounded or implies missing mature trees");
                }
                string saved=_world.SaveJson();int nodes=GetTree().GetNodeCount();await Frames();
                Check(saved==_world.SaveJson() && nodes==GetTree().GetNodeCount(),"Paused woodland rebuilds or mutates");
            }
        }
        // Clear the pending visitor stand before isolating the construction sheet.
        AdoptWorld(World.NewCampaign(9));await Frames();
        _dynamic.Hide();_landscape.Hide();if(_pathView!=null)_pathView.Hide();_hud.Hide();_watchRoot.Hide();_showWorldLabels=false;ApplyWorldLabels();
        var sheet=new Node3D();AddChild(sheet);
        for(int stage=0;stage<4;stage++)
        {
            var model=new Node3D{Position=new(stage*4-6,0,0)};sheet.AddChild(model);
            MakeBuilding(model,new Cottage{Kind=BuildingKind.HuntingLodge},stage);
        }
        Box(sheet,new(0,-.12f,0),new(18,.15f,6),new("777f62"));
        _focus=Vector3.Zero;_camera.Size=21;UpdateCamera();await Frames();await Capture($"artifacts/{prefix}-stages.png");
        GD.Print("PASS: woodland art, four-orientation actual construction/hunting/catch delivery, active reload, paused stability and habitat-state captures.");
    }
}
