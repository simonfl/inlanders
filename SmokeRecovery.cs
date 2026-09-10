using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckRecoveryUi()
    {
        void Check(bool ok, string why) { if (!ok) throw new Exception(why); }
        async Task Frames() { for (int i=0;i<4;i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
        foreach (bool creative in new[] { false, true })
        foreach (bool large in new[] { false, true })
        {
            EnterFromMenu(creative ? World.NewCreative(large) : large ? World.NewLargeMap() : World.NewScenario());
            await Frames(); SaveWorld(); string manual = _world.SaveJson(); string path = AutosavePath;
            _world.Tick(.1f); string first = _world.SaveJson(); AdvanceAutosave(120);
            Check(World.LoadFile(path).SaveJson() == first && World.LoadFile(CurrentSavePath).SaveJson() == manual, "Autosave changed manual save or wrong mode");
            _world.Tick(.1f); string second = _world.SaveJson(); AdvanceAutosave(120);
            Check(World.LoadFile(path + ".bak").SaveJson() == first && World.LoadFile(path).SaveJson() == second, "Autosave did not rotate");
            AdvanceAutosave(120); Check(World.LoadFile(path + ".bak").SaveJson() == first, "Unchanged paused village erased older autosave");
            LoadWorld(); await Frames(); Check(_world.SaveJson() == manual, "F9 loaded autosave instead of manual save");
            RestoreRecovery(path + ".bak"); await Frames(); Check(_world.SaveJson() == first && _paused, "Older recovery failed");
            RestoreRecovery(RecoveryPath + ".before-recovery"); await Frames(); Check(_world.SaveJson() == manual, "Undo recovery failed");
            _world.Tick(.3f); string beforeRestart = _world.SaveJson(); Reset(); await Frames();
            Check(_paused && World.LoadFile(CurrentSavePath + ".before-new").SaveJson() == beforeRestart, "Restart discarded live progress");
            Check(World.LoadFile(CurrentSavePath).SaveJson() == manual, "Restart overwrote F5 checkpoint");
            RestoreRecovery(CurrentSavePath + ".before-new"); await Frames(); Check(_world.SaveJson() == beforeRestart, "Restart recovery changed mode or village");
            _world.Tick(.2f); string session = _world.SaveJson(); Check(SaveSession(), "Exit save failed");
            Check(World.LoadFile(CurrentSavePath).SaveJson() == session && World.LoadFile(_continuePath).SaveJson() == session, "Exit save missed latest state");
            File.WriteAllText(path, "broken"); RestoreRecovery(path); Check(_world.SaveJson() == session, "Corrupt recovery replaced village");
        }
        OpenMenuCampaign(1, false); await Frames(); SaveWorld(); string campaignManual = File.ReadAllText(_campaignPath);
        _world.Campaign!.Guidance = !_world.Campaign.Guidance; string current = _world.SaveJson(); AdvanceAutosave(120);
        string levelOneAuto = AutosavePath;
        Check(File.ReadAllText(_campaignPath) == campaignManual, "Campaign autosave changed F9 checkpoint");
        LoadWorld(); await Frames(); RestoreRecovery(levelOneAuto); await Frames(); Check(_world.SaveJson() == current, "Campaign autosave lost guidance");
        SwitchCampaign(2, false); await Frames(); AdvanceAutosave(120);
        Check(AutosavePath != levelOneAuto && World.LoadFile(levelOneAuto).Campaign!.Level == 1, "Campaign autosaves crossed levels");
        string levelTwo = _world.SaveJson(); RestoreRecovery(levelOneAuto);
        Check(_world.SaveJson() == levelTwo, "Wrong-level recovery replaced live campaign");
        Reset(); await Frames(); Check(_campaignBook!.BeforeReplay[2] == levelTwo, "Campaign restart lost preceding village");
        RestoreBeforeReplay(); await Frames(); Check(_world.SaveJson() == levelTwo, "Campaign restart recovery failed");
        ReturnToMainMenu(); await Frames(); _campaignBook = null; ContinueFromMenu(); await Frames();
        Check(_world.SaveJson() == levelTwo && _campaignBook!.BeforeReplay.ContainsKey(2), "Continue lost campaign recovery after relaunch");
        string good = _campaignPath; _campaignPath = Path.Combine(_continuePath, "invalid.json");
        Check(!SaveSession() && !_atMainMenu && _world.SaveJson() == levelTwo, "Save failure closed or changed live village");
        ToggleWatch(); _Notification((int)NotificationWMCloseRequest); await Frames();
        Check(!_atMainMenu && !_watching && _paused && _hud.Visible && _world.SaveJson() == levelTwo, "Failed close discarded village or hid error in Watch");
        AdvanceAutosave(120); Check(_world.SaveJson() == levelTwo, "Failed autosave changed village");
        _campaignPath = good; _noticeUntil = 0;
        foreach (int width in new[] { 1440, 960 })
        {
            GetWindow().Size = new(width, width == 960 ? 640 : 900); ToggleDrawer(3); UpdateRecoveryUi(); await Frames();
            _drawerPages[3].EnsureControlVisible(_restoreAutosave); await Frames();
            Check(!_restoreAutosave.Disabled && !_restoreRestart.Disabled, "Recovery buttons unavailable");
            await Capture($"artifacts/f19b-recovery-{width}.png");
            await UiClick(_restoreAutosave); await Frames();
            Check(_world.SaveJson() == levelTwo && _paused, "Autosave button did not restore and pause");
            CloseDrawer();
        }
        GD.Print("PASS: rolling autosaves across four sandbox modes and campaign levels, unchanged pause, F9 isolation, restart/recovery/undo, corrupt and wrong-level recovery, exit persistence and save failure.");
    }
}
