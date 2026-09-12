using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckForagerArt()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        string prefix=OS.GetCmdlineUserArgs().Contains("--before")?"before":"after";
        System.IO.Directory.CreateDirectory("artifacts/forager-art");
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewCampaign(1);
            var site=w.Place(new(0,0),rotation,BuildingKind.ForagerHut)!;
            foreach(var cell in new[]{new Cell(3,0),new(6,0),new(3,6),new(-5,6)})
                Check(w.Place(cell,0,BuildingKind.Cottage)!=null,"Home fixture rejected");
            for(int i=0;i<6000 && (!site.Complete || w.DeliveredBerries<24);i++)w.Tick(.1f);
            Check(site.Complete && w.DeliveredBerries>=24,"Rotated hut failed construction/delivery");w.Validate();
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;_showWorldLabels=false;ApplyWorldLabels();
            _focus=OnGround(0,1);_angle=.72f;_camera.Size=19;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();string saved=w.SaveJson();
                await Capture($"artifacts/forager-art/{prefix}-{rotation}-{width}.png");
                Check(w.SaveJson()==saved && World.LoadJson(saved).SaveJson()==saved,"Art inspection changed settlement");
            }
            BeginPlacement(BuildingKind.ForagerHut);_rotation=rotation;
            var target=w.Map.Land.First(c=>w.PlacementProblem(c,rotation,BuildingKind.ForagerHut)==null);
            _focus=OnGround(target.X,target.Z);_camera.Size=12;UpdateCamera();
            _pointerPosition=_camera.UnprojectPosition(OnGround(target.X,target.Z));await Frames();
            Check(_ghost.Visible && _ghostValid,"Forager preview is not valid/visible");
            await Capture($"artifacts/forager-art/{prefix}-preview-{rotation}.png");await Press(Key.Escape);
        }
        _dynamic.Hide();_landscape.Hide();if(_pathView!=null)_pathView.Hide();_hud.Hide();_watchRoot.Hide();
        var sheet=new Node3D();AddChild(sheet);
        for(int stage=0;stage<4;stage++)
        {
            var model=new Node3D{Position=new(stage*4-6,0,0)};sheet.AddChild(model);
            MakeBuilding(model,new Cottage{Kind=BuildingKind.ForagerHut},stage);
        }
        Box(sheet,new(0,-.12f,0),new(18,.15f,6),new("777f62"));
        _focus=Vector3.Zero;_camera.Size=21;UpdateCamera();await Frames();
        await Capture($"artifacts/forager-art/{prefix}-stages.png");
        GD.Print("PASS: four-way forager construction/delivery, populated 960/1440 captures, valid previews, stages and exact saves.");
    }
}
