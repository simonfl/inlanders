using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckCottageFinishes()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        bool HasColor(Node node,Color color) => node is MeshInstance3D mesh && mesh.MaterialOverride is StandardMaterial3D material && material.AlbedoColor.IsEqualApprox(color) || node.GetChildren().Any(c=>HasColor(c,color));
        _goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewScenario();var home=w.Place(new(3,0),rotation,BuildingKind.Cottage)??throw new Exception("Rotated cottage placement failed");
            Check(w.SetCottageFinish(home.Id,CottageFinish.Slate),"Planned finish rejected");
            AdoptWorld(w);_paused=true;_noticeUntil=0;CloseManagementUi();_focus=new(3,0,0);_camera.Size=12;_angle=.72f;UpdateCamera();
            int last=-1;
            for(int i=0;i<10000;i++)
            {
                int stage=home.Complete?3:home.Construction>.4f?2:home.Delivered>0?1:0;
                if(stage!=last)
                {
                    await Frames();last=stage;
                    if(stage>=2)Check(HasColor(_cottages[home.Id].Body,CottagePalette(CottageFinish.Slate).Wall),"Construction lost chosen plaster");
                    await Capture($"artifacts/cottage-finish-construction-{rotation}-{stage}.png");
                }
                if(home.Complete)break;w.Tick(.1f);
            }
            Check(home.Complete,"Painted cottage construction stalled");w.Validate();
            foreach(var finish in Enum.GetValues<CottageFinish>())
            {
                w.SetCottageFinish(home.Id,CottageFinish.Automatic);string before=w.SaveJson();
                Check(w.SetCottageFinish(home.Id,finish),"Finish command failed");await Frames();
                var body=_cottages[home.Id].Body;
                Check(HasColor(body,CottagePalette(World.ResolvedCottageFinish(home)).Roof),"Rendered roof did not update after finish choice");
                Check(Math.Abs(body.RotationDegrees.Y-rotation*90)<.01f,"Finish changed house orientation");
                string saved=w.SaveJson();var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"Chosen finish save differs");
                copy.SetCottageFinish(home.Id,CottageFinish.Automatic);Check(copy.SaveJson()==before,"Cosmetic choice changed another part of the village");
            }
            if(rotation==0)foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);w.SetCottageFinish(home.Id,CottageFinish.Automatic);SelectBuilding(home.Id);await Frames();
                if(!_finishDetails.Visible)await UiClick(_finishToggle);await Frames();
                await UiClick(_finishOptions[CottageFinish.Rose]);await Frames();
                Check(home.Finish==CottageFinish.Rose && _finishToggle.Text.Contains("Rose"),"Inspector selection failed");
                _inspectionScroll.EnsureControlVisible(_finishOptions[CottageFinish.Rose]);await Frames();
                Check(_finishOptions.Values.All(b=>b.GetGlobalRect().End.X<=GetWindow().Size.X),"Finish control overflows narrow inspector");
                await Capture($"artifacts/cottage-finish-inspector-{width}.png");
                await UiClick(_finishOptions[CottageFinish.Automatic]);await Frames();
                Check(home.Finish==CottageFinish.Automatic,"Inspector automatic reset failed");
            }
            w.SetCottageFinish(home.Id,CottageFinish.Slate);
            Check(w.RequestDemolition(home.Id) && !w.SetCottageFinish(home.Id,CottageFinish.Rose),"Demolition allows finish edits");
            for(int i=0;i<10000 && w.Cottages.Contains(home);i++)
            {
                w.Tick(.1f);
                if(home.DemolitionProgress>0 && home.DemolitionProgress<.7f)
                {
                    await Frames();Check(HasColor(_cottages[home.Id].Body,CottagePalette(CottageFinish.Slate).Wall),"Dismantling lost chosen plaster");break;
                }
            }
            string dismantling=w.SaveJson();w=World.LoadJson(dismantling);Check(w.SaveJson()==dismantling,"Dismantling finish save differs");
            for(int i=0;i<10000 && w.Cottages.Any(c=>c.Id==home.Id);i++)w.Tick(.1f);
            Check(!w.Cottages.Any(c=>c.Id==home.Id) && !w.SetCottageFinish(home.Id,CottageFinish.Clay),"Demolished cottage retained finish command");w.Validate();
        }
        var village=World.NewCreative();foreach(var tree in village.Trees.ToArray())village.SetClearing(tree.Cell,true);
        for(int r=0;r<4;r++)Check(village.Place(new(-5+r*4,-4),r,BuildingKind.Cottage)!=null,"Gallery cottage placement failed");
        village.Tick(.1f);AdoptWorld(village);_paused=true;CloseManagementUi();_noticeUntil=0;
        GetWindow().Size=new(1440,900);_focus=new(1,0,-4);_camera.Size=20;_angle=.72f;UpdateCamera();await Frames();
        await Capture("artifacts/cottage-finishes-automatic.png");
        foreach(var h in village.Cottages)village.SetCottageFinish(h.Id,h.Id%2==0?CottageFinish.Slate:CottageFinish.Rose);
        await Frames();await Capture("artifacts/cottage-finishes-chosen.png");
        foreach(var h in village.Cottages)Check(village.RequestImprovement(h.Id),"Gallery home improvement failed");
        await Frames();
        foreach(var h in village.Cottages)Check(_cottages[h.Id].Body.HasNode("HomeComfort") && HasColor(_cottages[h.Id].Body,CottagePalette(World.ResolvedCottageFinish(h)).Roof),"Improvement lost selected finish or shutters");
        foreach(var h in village.Cottages)
        {
            var original=h.Finish;village.SetCottageFinish(h.Id,CottageFinish.Ochre);await Frames();
            Check(_cottages[h.Id].Body.HasNode("HomeComfort") && HasColor(_cottages[h.Id].Body,CottagePalette(CottageFinish.Ochre).Roof),"Repainting an improved home lost its finish or shutters");
            village.SetCottageFinish(h.Id,original);await Frames();
        }
        string improved=village.SaveJson();AdoptWorld(World.LoadJson(improved));_paused=true;await Frames();Check(_world.SaveJson()==improved,"Improved finish reload differs");
        _focus=new(1,0,-4);_camera.Size=20;UpdateCamera();await Frames();await Capture("artifacts/cottage-finishes-improved.png");
        var other=_world.Place(new(-4,2),0,BuildingKind.Bakery)??throw new Exception("Bakery fixture placement failed");Check(!_world.SetCottageFinish(other.Id,CottageFinish.Clay),"Non-cottage accepts finish");
        SelectBuilding(other!.Id);await Frames();Check(!_finishControls.Visible,"Other building shows cottage finishes");
        Check(!_world.SetCottageFinish(_world.Cottages.First().Id,(CottageFinish)999),"Invalid finish accepted");
        GD.Print("PASS: five finishes/automatic across four rotations, real construction/demolition, only-finish mutation, exact saves, inspector selection/reset at 960/1440 and improved-home colors/shutters.");
    }
}
