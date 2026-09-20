using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeTransformation()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();
        await UiClick(_mainButtons["Between wood and water · prototype"]);await Frames();await CaptureReviewBundle("hamlet-brief");
        await UiClick(_mainButtons["New hamlet"]);await Frames();Check(_world.Founding?.TransformationHamlet==true && _world.Population==12 && CurrentSavePath==TransformationPath,"Hamlet entry failed");
        await Press(Key.G);await Frames();await CaptureReviewBundle("hamlet-intent");CloseDrawer();
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Hamlet save failed");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();Check(saved==_world.SaveJson(),"Hamlet Continue differs");
        Reset();await Frames();Check(_world.Founding?.TransformationHamlet==true,"Hamlet restart lost map");
        GD.Print("PASS: hamlet menu/brief/intent, isolated save, Continue and restart through native controls.");
    }
}
