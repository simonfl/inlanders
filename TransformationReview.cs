using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
using System.Linq;
public partial class Game
{
    private async Task ProbeTransformation()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();
        await UiClick(_mainButtons["Between wood and water · prototype"]);await Frames();await CaptureReviewBundle("hamlet-brief");
        await UiClick(_mainButtons["New hamlet"]);await Frames();Check(_world.Founding?.TransformationHamlet==true && _world.Population==12 && CurrentSavePath==TransformationPath,"Hamlet entry failed");
        await Press(Key.G);await Frames();Check(_foodAccessEntry.GetIndex()<_foundingInvite.GetIndex() && !_foundingFood.Text.Contains("can rest"),"Village hierarchy or starting-provision status regressed");await CaptureReviewBundle("hamlet-intent");CloseDrawer();
        await Press(Key.G);await Frames();await UiClick(_foodAccessEntry);await Frames();string observation=_world.SaveJson();
        await Click(_camera.UnprojectPosition(OnGround(2,-1)));await Frames();Check(_foodAccessPlanning && _foodAccessRoutes.Length>0 && _foodAccessLine.Points.Length>1,"Ground food-access preview absent");
        await UiClick(_foodAccessPin);await Frames();Check(_foodAccessA==new Cell(2,-1),"Food spot A not kept");
        await Click(_camera.UnprojectPosition(OnGround(-1,1)));await Frames();
        Check(_foodAccessA!=_foodAccessAt && _foodAccessALine.Points.Length>1 && _foodAccessLine.Points.Length>1,"Paired routes absent");
        await CaptureReviewBundle("hamlet-food-comparison");Check(observation==_world.SaveJson(),"Food planning mutated world");await UiClick(_foodAccessClear);await Frames();Check(_foodAccessA==null && !_foodAccessALine.Visible,"Clear A failed");await Press(Key.Escape);await Frames();Check(!_foodAccessPanel.Visible,"Food preview did not close");
        var planned=_world.Place(new(4,-9),0,BuildingKind.VegetableGarden);Check(planned!=null,"Staged garden rejected");SelectBuilding(planned!.Id);await Frames();
        await UiClick(_constructionPause,6);await Frames();Check(planned.ConstructionPaused,"Held pause failed");await CaptureReviewBundle("hamlet-staged-project");
        await UiClick(_constructionPause,6);await Frames();Check(!planned.ConstructionPaused,"Held resume failed");CloseDrawer();ClearSelection();
        var garden=_world.Cottages.Single(c=>c.Cell==new Cell(1,3));SelectBuilding(garden.Id);await Frames();
        await UiClick(_productionPause);await Frames();Check(garden.WorkPaused,"Garden pause did not apply");
        await UiClick(_moveButton);await Frames();Check(_movingSite==garden.Id,"Garden move did not begin");
        await Click(_camera.UnprojectPosition(OnGround(0,-9)));await Frames();
        Check(garden.Cell==new Cell(0,-9) && !garden.Planted && garden.WorkPaused,"Garden move/replant failed");
        await CaptureReviewBundle("garden-replant");await UiClick(_productionPause);await Frames();Check(!garden.WorkPaused,"Garden resume failed");CloseDrawer();ClearSelection();
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Hamlet save failed");
        ReturnToMainMenu();await Frames();Check(_atMainMenu && _mainButtons.ContainsKey("Continue"),"Hamlet return to menu/save failed: "+_notice);await UiClick(_mainButtons["Continue"]);await Frames();Check(saved==_world.SaveJson(),"Hamlet Continue differs");
        Reset();await Frames();Check(_world.Founding?.TransformationHamlet==true,"Hamlet restart lost map");
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();await UiClick(_mainButtons["Between wood and water · prototype"]);await Frames();
        await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();Check(_world.Creative && _world.SimulatesMeals && _world.BreadPerGrain==4 && CurrentSavePath==TransformationSavePath(true),"Relaxed entry/rules/slot failed");
        await Press(Key.F5);string relaxedSave=_world.SaveJson();Reset();await Frames();Check(_world.Creative && _world.Founding?.TransformationHamlet==true,"Relaxed restart lost mode");
        await Press(Key.O);await Frames();await UiClick(_restoreRestart);await Frames();Check(_world.SaveJson()==relaxedSave,"Relaxed restore differs");
        GD.Print("PASS: hamlet menu/brief/intent, isolated save, Continue and restart through native controls.");
    }
}
