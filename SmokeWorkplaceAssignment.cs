using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckWorkplaceAssignmentUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(Control target)
        {for(int i=0;i<25 && GetViewport().GuiGetFocusOwner()!=target;i++){await Press(Key.Tab);await Frames();}Check(GetViewport().GuiGetFocusOwner()==target,"Cannot focus workplace control");}
        System.IO.Directory.CreateDirectory("artifacts/workplace-assignment");
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var first=w.Place(new(3,0),0,BuildingKind.VegetableGarden)!;var second=w.Place(new(6,0),0,BuildingKind.VegetableGarden)!;
            w.Assign(0,Role.Farmer);w.Assign(1,Role.Farmer);
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);_peopleLastPerson=0;_rosterFilter.Select(0);UpdateVillageDirectory();await Frames();
            await Press(Key.V);await Focus(_roster[0]);await Press(Key.Enter);await Frames();await Focus(_assignmentChoice);
            string before=w.SaveJson();await Press(Key.Right);await Press(Key.Right);await Frames();
            Check(_assignmentChoice.GetSelectedId()==second.Id && w.SaveJson()==before,"Workplace draft mutated world or chose wrong duplicate site");
            w.SetWorkplaceAssignment(1,second.Id);await Frames();Check(_assignmentApply.Disabled && _assignmentHelp.Text.Contains("Fully assigned"),"Full-site reason absent");
            w.SetWorkplaceAssignment(1,null);await Frames();await Focus(_assignmentApply);await Press(Key.Enter);await Frames();
            Check(w.People[0].AssignedWorkplaceId==second.Id && GetViewport().GuiGetFocusOwner()==_assignmentChoice,"Keyboard assignment failed");
            w.SetWorkplacePaused(second.Id,true);for(int i=0;i<200;i++){w.Tick(.1f);w.Validate();}await Frames();
            Check(!second.Planted && w.People[0].WorkplaceId==null && _assignmentState.Text.Contains("Paused"),"Paused binding fell back or lacks explanation");
            _inspectionScroll.EnsureControlVisible(_assignmentApply);await Frames();await Capture($"artifacts/workplace-assignment/resident-{width}.png");
            await Focus(_assignmentChoice);await Press(Key.Left);await Press(Key.Escape);await Frames();await Press(Key.Enter);await Frames();
            Check(_assignmentChoice.GetSelectedId()==second.Id,"Esc retained unconfirmed assignment draft");
            await Focus(_assignmentVisit);await Press(Key.Enter);await Frames();Check(_selectedSite==second.Id && _workerLinks[0].Visible && _workplaceStaff.Text.Contains("1/1 assigned"),"Assigned worker vanished while waiting");
            _inspectionScroll.EnsureControlVisible(_workerLinks[0]);await Frames();await Capture($"artifacts/workplace-assignment/workplace-{width}.png");
            await UiClick(_workerLinks[0]);await Frames();Check(_selectedPerson==0,"Assigned resident link failed");
            w.RemoveBuilding(second.Id);await Frames();Check(w.People[0].AssignedWorkplaceId==null && _assignmentChoice.GetSelectedId()==0 && _assignmentApply.Disabled,"Removed site left stale assignment/draft");
            _assignmentChoice.Select(_assignmentChoice.GetItemIndex(first.Id));_assignmentChoice.EmitSignal(OptionButton.SignalName.ItemSelected,(long)_assignmentChoice.Selected);await Frames();await UiClick(_assignmentApply);await Frames();
            Check(w.People[0].AssignedWorkplaceId==first.Id,"Mouse Apply failed");string saved=w.SaveJson();AdoptWorld(World.LoadJson(saved));_paused=true;SelectPerson(0);await Frames();
            Check(_world.SaveJson()==saved && _assignmentChoice.GetSelectedId()==first.Id,"Reload did not show actual assignment");
            _world.Assign(0,Role.Logger);await Frames();Check(!_assignmentChoice.Visible && _assignmentState.Text.Contains("across the village"),"Roaming role has misleading workplace picker");
        }
        GD.Print("PASS: workplace keyboard/mouse drafts, explicit Apply, capacity conflict, paused reason, assigned worker links, Esc, removal, role change and current saves at 960/1440.");
    }
}
