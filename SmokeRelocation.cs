using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckRelocationUi()
    {
        System.IO.Directory.CreateDirectory("artifacts/relocation");
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(int width in new[]{960,1440})foreach(var kind in new[]{BuildingKind.Cottage,BuildingKind.Orchard,BuildingKind.Pantry})
        {
            var w=World.NewCreative(true);foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var site=w.Place(w.Map.Land.First(c=>w.PlacementProblem(c,0,kind)==null),0,kind)!;
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);
            SelectBuilding(site.Id);await Frames();
            Check(_moveButton.Visible && !_moveButton.Disabled,"Move button unavailable");
            string before=w.SaveJson();await UiClick(_moveButton);await Frames();
            Check(_movingSite==site.Id && _placing,"Move button did not begin preview");
            await Press(Key.R);await Press(Key.Escape);await Frames();
            Check(_movingSite<0 && !_placing && w.SaveJson()==before,"Cancel changed simulation");
            for(int rotation=0;rotation<4;rotation++)
            {
                SelectBuilding(site.Id);await Frames();await UiClick(_moveButton);
                _rotation=rotation;_hover=new(999,999);RefreshGhost();
                Check(!_ghostValid,"Invalid destination preview accepted");PlaceCottage(_hover);
                Check(_movingSite==site.Id && w.SaveJson()==before,"Rejected move changed world");
                var target=w.Map.Land.First(c=>c!=site.Cell && w.RelocationProblem(site.Id,c,rotation)==null && w.PlacementProblem(c,rotation,kind)==null);
                _hover=target;RefreshGhost();Check(_ghostValid,"Valid destination preview rejected");
                if(rotation==0)
                {
                    var obstacle=w.Place(target,rotation,kind)!;string occupied=w.SaveJson();
                    PlaceCottage(target);Check(_movingSite==site.Id && w.SaveJson()==occupied,"Confirmation trusted stale preview");
                    w.Cottages.Remove(obstacle);_moveCheck=null;RefreshGhost();
                }
                Check(_ghostModelKey.StartsWith("move:"),"Preview lost existing building state");
                _focus=OnGround(target.X,target.Z);_camera.Size=14;UpdateCamera();
                _pointerPosition=_camera.UnprojectPosition(OnGround(target.X,target.Z));await Frames();
                Check(_hover==target && _ghostValid && _ghost.Visible,"Visible preview lost target");
                await Capture($"artifacts/relocation/preview-{kind}-{rotation}-{width}.png");
                await Click(_pointerPosition);await Frames();w.Validate();
                Check(site.Cell==target && site.Rotation==rotation && _movingSite<0 && !_placing && _paused,"Move failed or reset pause");
                Check(_selectedSite==site.Id,"Moved building not selected");
                before=w.SaveJson();Check(World.LoadJson(before).SaveJson()==before,"Moved save changed on load");
            }
            BeginRelocation();BeginPlacement(BuildingKind.Bakery);Check(_movingSite<0,"New placement retained move");
            await Press(Key.Escape);
        }
        GD.Print("PASS: relocation inspector, cancellation, rejected destinations, stateful previews, four rotations, 960/1440 and save roundtrips.");
    }
}
