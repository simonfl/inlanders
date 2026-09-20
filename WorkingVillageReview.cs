using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeWorkingVillage()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Another village · The long way home"]);await Frames();await CaptureReviewBundle("working-village-context");
        await UiClick(_mainButtons["New · The long way home"]);await Frames();
        Check(_world.Founding?.WorkingVillage==true && _world.Housed==8 && CurrentSavePath==WorkingVillagePath,"Wrong inhabited entry/slot");
        Check(_world.Place(new(4,2),0,BuildingKind.Bridge)!=null,"Crossing rejected");
        for(int i=0;i<2400;i++)_world.Tick(.1f);UpdateHud();RenderActors(0);await Frames();
        CloseDrawer();await Frames();await Click(_camera.UnprojectPosition(_people[0].Body.Position+new Vector3(0,.5f,0)));await Frames();
        Check(_dailyCard.Visible,"Inhabited resident reading absent");_dailyExpanded=true;await Frames();await CaptureReviewBundle("working-village-crossing");
        await Press(Key.G);await Frames();await UiClick(_foundingFinish,6);await Frames();Check(_world.Founding!.Finished,"Optional ending failed");
        string save=_world.SaveJson();ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(_world.SaveJson()==save && CurrentSavePath==WorkingVillagePath,"Continue lost situation");
        Reset();await Frames();Check(_world.Founding?.WorkingVillage==true && !_world.Founding.Finished,"Restart lost situation");
        await Press(Key.O);await Frames();await UiClick(_restoreRestart);await Frames();Check(_world.SaveJson()==save,"Situation restart restore differs");
        await Press(Key.F5);await Frames();await Press(Key.F9);await Frames();Check(_world.SaveJson()==save,"Situation save/load differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"working-village-controls.txt"),"PASS: menu/context, inhabited entry, resident click, optional finish, Continue/restart/restore/F5/F9. Scripted construction and accelerated ticks, not human play.");
    }
}
