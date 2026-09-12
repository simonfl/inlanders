using Godot;
using Inlanders.Simulation;
using System;
using System.IO;

public partial class Game
{
    private double _autosaveElapsed;
    private string? _lastAutosaved;
    private Label _recoveryStatus = null!;
    private Button _restoreAutosave = null!, _restoreOlderAutosave = null!, _restoreRestart = null!, _undoRecovery = null!;
    private string RecoveryPath => _world.Campaign is { } campaign ? _campaignPath + $".level-{campaign.Level}" : CurrentSavePath;
    private string AutosavePath => RecoveryPath + ".autosave";

    private void MakeRecoveryUi(VBoxContainer column)
    {
        _recoveryStatus = Text("", 14, true); column.AddChild(_recoveryStatus);
        _restoreAutosave = Button("Restore latest autosave", () => RestoreRecovery(AutosavePath)); column.AddChild(_restoreAutosave);
        _restoreOlderAutosave = Button("Restore previous autosave", () => RestoreRecovery(AutosavePath + ".bak")); column.AddChild(_restoreOlderAutosave);
        _restoreRestart = Button("Restore village before restart", () => {
            if (_world.Campaign != null) RestoreBeforeReplay(); else RestoreRecovery(CurrentSavePath + ".before-new");
        }); column.AddChild(_restoreRestart);
        _undoRecovery = Button("Undo last recovery", () => RestoreRecovery(RecoveryPath + ".before-recovery")); column.AddChild(_undoRecovery);
    }
    private void UpdateRecoveryUi()
    {
        // File metadata is read only while Options is open.
        if (!_drawer.Visible || _tabs.CurrentTab != 3) return;
        string path = AutosavePath;
        _restoreAutosave.Disabled = !File.Exists(path);
        _restoreOlderAutosave.Disabled = !File.Exists(path + ".bak");
        _undoRecovery.Disabled = !File.Exists(RecoveryPath + ".before-recovery");
        _restoreRestart.Disabled = _world.Campaign is { } c ? _campaignBook?.BeforeReplay.ContainsKey(c.Level) != true : !File.Exists(CurrentSavePath + ".before-new");
        _recoveryStatus.Text = "Autosave every 2 real minutes while the village is open, including paused edits. F9 restores the manual/session save, not autosaves.\n" +
            (File.Exists(path) ? $"Latest autosave: {File.GetLastWriteTime(path):MMM d, HH:mm:ss}" : "No autosave for this settlement yet.");
    }
    private void AdvanceAutosave(double seconds)
    {
        if (!_menuEnabled || _atMainMenu) return;
        _autosaveElapsed += seconds;
        if (_autosaveElapsed < 120) return;
        _autosaveElapsed = 0;
        try
        {
            string json = _world.SaveJson();
            if (json == _lastAutosaved) return;
            _world.SaveFile(AutosavePath); RememberSettlement(); _lastAutosaved = json;
        }
        catch (Exception e) { Notice("Autosave failed; village kept open. Try F5. " + e.Message); }
    }
    private void RestoreRecovery(string path)
    {
        try
        {
            var restored = World.LoadFile(path);
            if (restored.Creative != _world.Creative || restored.Map.Name != _world.Map.Name || restored.Campaign?.Level != _world.Campaign?.Level)
                throw new IOException("Recovery belongs to another settlement.");
            _world.SaveFile(RecoveryPath + ".before-recovery");
            if (restored.Campaign != null)
            {
                var book = ReadCampaignBook(); book.Capture(restored); _campaignBook = book;
            }
            AdoptWorld(restored);
            Notice("Recovery restored and paused. F5 keeps it as your manual save.");
        }
        catch (Exception e) { Notice("Could not restore; current village kept. " + e.Message); }
    }
    private bool SaveSession()
    {
        try
        {
            if (_world.Campaign != null) SaveCampaign(); else _world.SaveFile(CurrentSavePath);
            RememberSettlement(); return true;
        }
        catch (Exception e) { Notice("Could not save; village kept open. " + e.Message); return false; }
    }
    private void RequestQuit()
    {
        if (_atMainMenu || !_menuEnabled || SaveSession()) GetTree().Quit();
        else { _paused = true; ExitWatch(); }
    }
    public override void _Notification(int what)
    {
        if (what == NotificationWMWindowFocusOut) { CancelCameraDrag();CancelDecorationStroke();CancelAreaRemoval(); }
        if (what == NotificationWMCloseRequest) RequestQuit();
    }
}
