using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeRiverFarmstead()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await CaptureReviewBundle("farmstead-menu");
        await UiClick(_mainButtons["New · A place of our own"]);await Frames();
        Check(_world.Founding?.RiverFarmstead==true && CurrentSavePath==RiverFarmsteadPath && _paused,"Farmstead entry/slot failed");
        Check(_foundingFinish.Visible && _foundingFinish.Disabled,"Opening finish feedback missing");
        await CaptureReviewBundle("farmstead-brief");
        foreach(var cell in new[]{new Cell(-3,6),new(1,0),new(1,6)})Check(_world.Place(cell,0,BuildingKind.Cottage)!=null,"Home order failed");
        Check(_world.Place(new(3,3),3,BuildingKind.VegetableGarden)!=null,"Garden order failed");
        Check(_world.Place(new(8,3),1,BuildingKind.FishingDock)!=null,"Dock order failed");
        for(int i=0;i<2400;i++)_world.Tick(.1f);
        UpdateHud();await Frames();Check(_world.FinishFoundingProblem()==null && _world.Population==8,"No-growth finish unavailable");
        CloseDrawer();_inspector.Hide();await Frames();
        var person=_world.People[0];
        await Click(_camera.UnprojectPosition(_people[person.Id].Body.Position+new Vector3(0,.5f,0)));await Frames();
        Check(_dailyCard.Visible && _dailyPerson>=0 && !_dailyRestore.Visible,"Normal resident journey missing/false restore");
        await UiClick(_dailyFollow);await Frames();Check(_followPerson,"Resident follow failed");
        _dailyExpanded=true;await Frames();await CaptureReviewBundle("farmstead-daily-life");
        await UiClick(_dailyDetails);await Frames();Check(_inspector.Visible && _selectedPerson>=0,"Resident details unavailable");
        Check(_inspect.Text.Contains("Shared village work") && _assignButton.Text.Contains("Shared work"),"Misleading shared role");
        await Press(Key.G);await Frames();
        _drawerPages[2].EnsureControlVisible(_foundingFinish);await Frames();await UiClick(_foundingFinish,6);await Frames();
        Check(_world.Founding!.Finished && _paused && !_hallBegin.Visible,"Finish or old hall recipe wrong");
        await CaptureReviewBundle("farmstead-finished");string saved=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.SaveJson()==saved && CurrentSavePath==RiverFarmsteadPath,"Farmstead Continue differs");
        await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Farmstead F9 differs");
        Reset();await Frames();
        Check(_world.Founding?.RiverFarmstead==true && CurrentSavePath==RiverFarmsteadPath && !_world.Founding.Finished,"Restart changed scenario/slot");
        await Press(Key.O);await Frames();UpdateRecoveryUi();
        _drawerPages[3].EnsureControlVisible(_restoreRestart);await Frames();
        Check(!_restoreRestart.Disabled,"Restart backup unavailable");await UiClick(_restoreRestart);await Frames();
        Check(_world.SaveJson()==saved,"Before-restart recovery differs");
        await Press(Key.F5);await Frames();await Press(Key.F9);await Frames();
        Check(_world.SaveJson()==saved,"Recovered manual save differs");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.SaveJson()==saved && CurrentSavePath==RiverFarmsteadPath,"Recovered Continue differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"farmstead-controls.txt"),"PASS: actual menu entry/held finish/restart/restore/F5/F9/Continue, eight residents. Construction commands and accelerated ticks; not human play.");
    }
}
