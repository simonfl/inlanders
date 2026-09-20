using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCultivatedBank()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();await CaptureReviewBundle("cultivated-bank-choice");
        await UiClick(_mainButtons["New hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Wrong bank profile");
        var plot=_world.Cottages.Single(c=>c.Kind==BuildingKind.VegetableGarden && c.Cell.Z==7);
        SelectBuilding(plot.Id);await Frames();await UiClick(_productionPause);await Frames();await UiClick(_moveButton);await Frames();
        await Click(_camera.UnprojectPosition(OnGround(4,-8)));await Frames();Check(plot.Cell==new Cell(4,-8) && plot.WorkPaused,"Cultivated strip move failed");
        await CaptureReviewBundle("cultivated-bank-opened");await UiClick(_productionPause);await Frames();CloseDrawer();ClearSelection();
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Bank save differs");
        Reset();await Frames();Check(_world.PublicPlace==new HamletProfile(false,true),"Bank restart changed place");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();
        await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();Check(_world.PublicPlace==new HamletProfile(true,true) && CurrentSavePath.EndsWith("cultivated-bank-relaxed.json"),"Relaxed bank slot differs");
        GD.Print("PASS: public bank comparison, actual garden move/replant, exact save, retained restart and relaxed slot (scripted UI).");
    }
}
