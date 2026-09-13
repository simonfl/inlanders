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
    }
}
