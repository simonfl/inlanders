using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;

public partial class Game
{
    private CampaignBook? _campaignBook;
    private string _campaignPath = "saves/campaign.json";
    private Label _goalTitle = null!, _goalArrival = null!, _tutorialText = null!, _campaignRecord = null!;
    private VBoxContainer _standaloneGuide = null!, _campaignControls = null!;
    private Button _nextLevel = null!, _replayLevel = null!, _keepPlaying = null!, _guidance = null!, _dismissHint = null!, _reopenHints = null!;
    private readonly System.Collections.Generic.List<Button> _levelButtons = new();
    private bool _completionAnnounced;
    private Button _restoreReplay = null!;

    private void MakeCampaignUi(VBoxContainer column)
    {
        _campaignControls = new(); column.AddChild(_campaignControls);
        _tutorialText = Text("", 15, true); _campaignControls.AddChild(_tutorialText);
        _dismissHint = Button("Dismiss this hint", () => { var hint = _world.CurrentCampaignHint(); if (hint != null) _world.Campaign!.Dismissed.Add(hint.Id); }); _campaignControls.AddChild(_dismissHint);
        _guidance = Button("", () => _world.Campaign!.Guidance = !_world.Campaign.Guidance); _campaignControls.AddChild(_guidance);
        _reopenHints = Button("Show hints again", () => { _world.Campaign!.Dismissed.Clear(); _world.Campaign.Guidance = true; }); _campaignControls.AddChild(_reopenHints);
        _keepPlaying = Button("Continue playing", CloseDrawer); _campaignControls.AddChild(_keepPlaying);
        _nextLevel = Button("Next settlement", () => SwitchCampaign(2, false)); _campaignControls.AddChild(_nextLevel);
        _replayLevel = Button("Replay this settlement", () => SwitchCampaign(_world.Campaign!.Level, true)); _campaignControls.AddChild(_replayLevel);
        _restoreReplay = Button("Restore village before replay", RestoreBeforeReplay); _campaignControls.AddChild(_restoreReplay);
        column.AddChild(Text("CAMPAIGN · FIRST TWO SETTLEMENTS", 12, true));
        _campaignRecord = Text("", 14, true); column.AddChild(_campaignRecord);
        foreach (var level in World.CampaignLevels)
        {
            int id = level.Id;
            var button = Button($"{id}. {level.Title}", () => SwitchCampaign(id, false));
            button.AddThemeFontSizeOverride("font_size", 14); column.AddChild(button); _levelButtons.Add(button);
        }
        column.AddChild(Button("Return to standalone supper", () => SwitchCampaign(0, false)));
        column.AddChild(Text("Choose a settlement to start or resume it. Switching saves the village you leave. Levels 3–5 are planned. All buildings are available.", 14, true));
    }
    private CampaignBook ReadCampaignBook()
    {
        var saved = _campaignBook ?? (File.Exists(_campaignPath) ? CampaignBook.LoadFile(_campaignPath) : new());
        return new() { ActiveLevel = saved.ActiveLevel, Settlements = new(saved.Settlements), BeforeReplay = new(saved.BeforeReplay), Completed = new(saved.Completed) };
    }
    private void ResumeCampaignOnLaunch()
    {
        if (!File.Exists(_campaignPath)) return;
        try
        {
            var book = CampaignBook.LoadFile(_campaignPath); _campaignBook = book;
            if (book.ActiveLevel == 0) return;
            AdoptWorld(World.LoadJson(book.Settlements[book.ActiveLevel])); ToggleDrawer(2);
            Notice("Campaign restored and paused. Press Space to continue.");
        }
        catch (Exception e) { Notice("Could not resume campaign: " + e.Message); }
    }
    private void AdoptWorld(World world)
    {
        _world = world; CloseManagementUi(); _placing = false; _accumulator = 0; _paused = true;
        _completionAnnounced = world.Campaign?.Complete == true;
        CreateActors(); RefreshGhost(); RefreshSelection(); RebuildQueue();
    }
    private void SwitchCampaign(int level, bool replay)
    {
        try
        {
            var book = ReadCampaignBook();
            // Prepare and validate the destination before writing or replacing the live settlement.
            var next = level == (_world.Campaign?.Level ?? 0) && !replay ? _world :
                !replay && book.Settlements.TryGetValue(level, out var json) ? World.LoadJson(json) : level == 0 ? World.NewScenario() : World.NewCampaign(level);
            book.Capture(_world);
            if (replay && book.Settlements.TryGetValue(level, out var previous)) book.BeforeReplay[level] = previous;
            book.Capture(next); book.SaveFile(_campaignPath);
            _campaignBook = book; AdoptWorld(next); ToggleDrawer(2);
            Notice("Settlement ready and paused. Press Space to begin; Goals [G] has your objectives and hints.");
        }
        catch (Exception e) { Notice("Could not switch settlements; current village kept. " + e.Message); }
    }
    private void SaveCampaign()
    {
        var book = ReadCampaignBook(); book.Capture(_world); book.SaveFile(_campaignPath); _campaignBook = book;
    }
    private void RestoreBeforeReplay()
    {
        try
        {
            var book = ReadCampaignBook(); int level = _world.Campaign!.Level;
            var restored = World.LoadJson(book.BeforeReplay[level]);
            book.BeforeReplay[level] = _world.SaveJson(); book.Capture(restored); book.SaveFile(_campaignPath);
            _campaignBook = book; AdoptWorld(restored); ToggleDrawer(2);
            Notice("Previous village restored and paused. Your replay is now the other saved version.");
        }
        catch (Exception e) { Notice("Could not restore previous village: " + e.Message); }
    }
    private void UpdateCampaignUi()
    {
        var campaign = _world.Campaign;
        _campaignControls.Visible = campaign != null; _standaloneGuide.Visible = campaign == null; _supperButton.Visible = campaign == null;
        _goalTitle.Text = campaign == null ? "The first village supper" : $"{campaign.Level}. {World.CampaignLevels[campaign.Level - 1].Title}";
        _goalArrival.Text = campaign == null ? "Give eight neighbors a home and enough bread to celebrate together." : World.CampaignLevels[campaign.Level - 1].Arrival;
        _campaignRecord.Text = _campaignBook?.Completed.Count > 0 ? "Completed: " + string.Join(", ", _campaignBook.Completed.OrderBy(i => i)) : "Two small settlements to learn at your own pace.";
        if (campaign == null) return;
        _restoreReplay.Visible = _campaignBook?.BeforeReplay.ContainsKey(campaign.Level) == true;
        _objective.Text = campaign.Complete ? (campaign.Level == 1 ? "Everyone has a home. Welcome to the village!" : "The berry camp is supplying the hamlet. This settlement is complete; more settlements are planned.") : _world.CampaignObjective;
        _progress.Value = campaign.Complete ? 100 : _world.CampaignProgress * 100;
        _menuButtons[2].Text = campaign.Complete ? "Goals · Complete" : "Goals";
        var hint = _world.CurrentCampaignHint();
        _tutorialText.Text = hint?.Text ?? (campaign.Guidance ? "No more hints right now. You can show dismissed hints again below." : "Tutorial guidance is off. Objectives still count.");
        _tutorialText.Visible = !campaign.Complete;
        _dismissHint.Visible = hint != null;
        _guidance.Text = campaign.Guidance ? "Guidance: on" : "Guidance: off";
        _keepPlaying.Visible = campaign.Complete;
        _nextLevel.Visible = campaign.Complete && campaign.Level == 1;
        if (campaign.Complete && !_completionAnnounced)
        {
            _completionAnnounced = true;
            try { SaveCampaign(); Notice("Settlement complete! Progress saved. Open Goals [G] to continue or keep playing here."); }
            catch (Exception e) { Notice("Settlement complete, but progress could not be saved. Try F5. " + e.Message); }
        }
    }
}
