using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeFounding()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Click(Button b){_drawerPages[2].EnsureControlVisible(b);await Frames();await UiClick(b,6);await Frames();}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Found a village · A home by the water"]);await Frames();
        await CaptureReviewBundle("founding-start-menu");
        await UiClick(_mainButtons["New · A home by the water"]);await Frames();
        Check(_world.Founding!=null && _paused && CurrentSavePath==FoundingPath && _world.Housed==2,"Founding entry/slot failed");
        Check(_foundingGoals.IsVisibleInTree() && !_goalDashboard.IsVisibleInTree() && !_supperButton.IsVisibleInTree(),"Legacy goals leaked");
        Check(_foundingInvite.Disabled && _foundingFinish.Disabled,"Opening enabled growth/finish");
        await CaptureReviewBundle("founding-brief");
        await Click(_foundingGoals.GetChildren().OfType<Button>().First(b=>b.Text=="Build homes and workplaces"));
        Check(_tabs.CurrentTab==1 && _buildSection==0 && _buildingFilter.Selected==1,"Founding build shortcut opens wrong page");
        await ProbeBuildingGroups();CloseDrawer();
        // Real construction commands; bounded simulation advancement, not a human playthrough.
        foreach(var at in new[]{new Cell(-3,0),new(0,0),new(0,6),new(-3,5),new(-6,6)})
        {
            var cell=_world.Map.Land.OrderBy(c=>(c.Point-at.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X).First(c=>_world.PlacementProblem(c,0,BuildingKind.Cottage)==null);
            Check(_world.Place(cell,0,BuildingKind.Cottage)!=null,"Home order rejected");
        }
        var garden=_world.Map.Land.OrderBy(c=>(c.Point-new Cell(-7,2).Point).LengthSquared()).First(c=>_world.PlacementProblem(c,0,BuildingKind.VegetableGarden)==null);
        Check(_world.Place(garden,0,BuildingKind.VegetableGarden)!=null,"Food order rejected");
        for(int i=0;i<12000 && _world.FinishFoundingProblem()!=null;i++)
        {
            _world.Tick(.1f);
            if(_world.Population<12 && _world.InvitationProblem()==null){UpdateHud();await Click(_foundingInvite);}
        }
        _world.Validate();UpdateHud();await Frames();Check(_world.FinishFoundingProblem()==null,"Founding route did not settle");
        await Click(_foundingFinish);Check(_world.Founding!.Finished && _paused,"Ending action failed");
        _drawerPages[2].ScrollVertical=0;await Frames();
        var viewport=_drawerPages[2].GetGlobalRect();
        foreach(var action in new[]{_foundingContinue,_hallBegin,_foundingLeave})
            Check(viewport.Encloses(action.GetGlobalRect()),"Finished village primary action below fold");
        Check(!_objective.Text.Contains("Finish when satisfied"),"Finished status still asks to finish");
        await CaptureReviewBundle("founding-finished");SaveWorld();string saved=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.SaveJson()==saved && CurrentSavePath==FoundingPath,"Continue changed finished village");
        await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Restore lost founding state");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Found a village · A home by the water"]);await Frames();
        await UiClick(_mainButtons["New · A home by the water"]);await Frames();await UiClick(_mainButtons["Cancel"]);await Frames();
        Check(File.ReadAllText(FoundingPath)==saved,"Cancelled replacement changed save");
        await UiClick(_mainButtons["Resume · A home by the water"]);await Frames();
        Check(_world.SaveJson()==saved,"Dedicated Resume differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"founding-controls.txt"),"PASS: actual menu/new/brief/invite/finish/Continue/F9/cancel/Resume; ordinary construction via commands and accelerated test ticks, not human play");
    }
}
