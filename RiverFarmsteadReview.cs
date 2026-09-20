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
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();
        await UiClick(_mainButtons["Try · A place of our own"]);await Frames();await CaptureReviewBundle("farmstead-menu");
        await UiClick(_mainButtons["New · A place of our own"]);await Frames();
        Check(_world.Founding?.RiverFarmstead==true && CurrentSavePath==RiverFarmsteadPath && _paused,"Farmstead entry/slot failed");
        Check(_foundingFinish.Visible && _foundingFinish.Disabled,"Opening finish feedback missing");
        await CaptureReviewBundle("farmstead-brief");
        foreach(var cell in new[]{new Cell(-3,6),new(1,0),new(1,6)})Check(_world.Place(cell,0,BuildingKind.Cottage)!=null,"Home order failed");
        Check(_world.Place(new(3,3),3,BuildingKind.VegetableGarden)!=null,"Garden order failed");
        Check(_world.Place(new(8,3),1,BuildingKind.FishingDock)!=null,"Dock order failed");
        for(int i=0;i<2400;i++)_world.Tick(.1f);
        UpdateHud();await Frames();Check(_world.FinishFoundingProblem()==null && _world.Population==8,"No-growth finish unavailable");
        _drawerPages[2].EnsureControlVisible(_foundingFinish);await Frames();await UiClick(_foundingFinish,6);await Frames();
        Check(_world.Founding!.Finished && _paused && !_hallBegin.Visible,"Finish or old hall recipe wrong");
        await CaptureReviewBundle("farmstead-finished");string saved=_world.SaveJson();
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.SaveJson()==saved && CurrentSavePath==RiverFarmsteadPath,"Farmstead Continue differs");
        await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Farmstead F9 differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"farmstead-controls.txt"),"PASS: actual menu entry/held finish/Continue/F9, eight residents. Construction commands and accelerated ticks; not human play.");
    }
}
