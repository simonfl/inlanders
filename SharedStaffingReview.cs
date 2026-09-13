using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeSharedStaffing()
    {
        void Check(bool value,string why){if(!value)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var site=_world.Cottages.First(c=>c.Complete && Buildings.Get(c.Kind).Worker is Role role && World.SupportsWorkplaceAssignment(role));
        SelectBuilding(site.Id);await Frames();
        _inspectionScroll.EnsureControlVisible(_staffPlus);await Frames();
        Check(_staffPlus.Text=="Dedicate worker" && !_staffPlus.Disabled,"Missing building dedication control");
        await UiClick(_staffPlus);await Frames();
        Check(_world.AssignedWorkers(site.Id)==1 && _workplaceStaff.Text.Contains("1/"),"Building dedication did not update inspector");
        await CaptureReviewBundle();
        await UiClick(_staffMinus);await Frames();
        Check(_world.AssignedWorkers(site.Id)==0 && _world.People.All(p=>p.SharedWorker),"Building release did not restore pool");
        Check(_staffMinus.Disabled && _workplaceStaff.Text.Contains("shared workers"),"Released staffing feedback incorrect");
        _world.Validate();await CaptureReviewBundle();
        GD.Print("PASS: scripted building dedication and release through rendered controls");
        if(_world.Neighborhood!=null) {
            var bridge=_world.Place(new(5,2),1,BuildingKind.Bridge)!;
            for(int i=0;i<18000 && !bridge.Complete;i++)_world.Tick(.1f);
            Check(bridge.Complete,"Arrival probe crossing failed");RenderActors(0);UpdateHud();
            await OpenMenu(0);_drawerPages[0].EnsureControlVisible(_inviteButton);await Frames();
            Check(!_inviteButton.Disabled && _inviteButton.Text=="Welcome four neighbors","Commitment control missing");
            await UiClick(_inviteButton);await Frames();
            Check(_world.Neighborhood.CommittedAt!=null && _world.Population==8 && _inviteButton.Disabled,"Commitment UI failed");
            await CaptureReviewBundle();
            for(int i=0;i<920;i++)_world.Tick(.1f);
            _noticeUntil=0;RenderActors(0);UpdateHud();await Frames();
            Check(_world.Population==12 && _people.Count==12 && _roster.Count==12 && _arrivalInfo.Text.Contains("have arrived"),"Arrived population UI stale");
            _drawerPages[0].EnsureControlVisible(_inviteButton);await Frames();await CaptureReviewBundle();
            GD.Print("PASS: scripted commitment and arrival actors/roster/status");
        }
    }
}
