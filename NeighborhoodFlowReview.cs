using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeNeighborhoodFlow()
    {
        bool landscape=_world.Map.Name=="Landing and meadow — experiment";
        bool workplaceFood=_world.HasWorkplaceFood;
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Neighborhood experiment"]);await Frames();
        await UiClick(_mainButtons[workplaceFood?"Try workplace food":landscape?"Try landing and meadow":"New neighborhood"]);await Frames();
        Check(_world.Neighborhood!=null && _paused && _drawer.Visible && _tabs.CurrentTab==2,"Menu did not open paused neighborhood goals");
        Check(_goalTitle.Text=="A new neighborhood" && !_supperButton.Visible && !_campaignSelection.Visible && _neighborhoodGoals.Visible,"Old goals leaked into experiment");
        Check(_neighborhoodCommit.Disabled && _neighborhoodCommit.TooltipText.Contains("crossing"),"Opening commitment lacks crossing guidance");
        await CaptureReviewBundle();
        _world.Tick(.1f);SaveWorld();string saved=_world.SaveJson();
        Check(CurrentSavePath==_neighborhoodPath,"Experiment uses sandbox save slot");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Continue"]);await Frames();
        Check(_world.SaveJson()==saved,"Continue changed neighborhood state");
        string mapName=_world.Map.Name;
        Reset();await Frames();Check(_world.Neighborhood!=null && _world.Food.Time==0 && _world.Map.Name==mapName && _world.HasWorkplaceFood==workplaceFood,"Restart changed game mode, food workflow or landscape");
        LoadWorld();await Frames();Check(_world.SaveJson()==saved,"Manual restore changed neighborhood state");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Neighborhood experiment"]);await Frames();
        await UiClick(_mainButtons["Play original river level"]);await Frames();
        Check(_world.IsRiverCampaign && _world.Neighborhood==null && !_neighborhoodGoals.Visible,"Original comparison entry changed mode");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Neighborhood experiment"]);await Frames();
        await UiClick(_mainButtons["Resume neighborhood"]);await Frames();
        Check(_world.SaveJson()==saved && _neighborhoodGoals.Visible,"Baseline switch overwrote experiment");
        _noticeUntil=0;await CaptureReviewBundle();
        GD.Print("PASS: neighborhood menu, objective isolation, dedicated save, Continue, restart, manual load and baseline roundtrip");
    }
}
