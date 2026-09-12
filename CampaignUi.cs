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
    private Button _riverAction = null!;

    private void MakeCampaignUi(VBoxContainer column)
    {
        _campaignControls = new(); column.AddChild(_campaignControls);
        _riverAction = Button("", () => { if (_world.IsFinaleCampaign ? _world.AdvanceFinalePhase() : _world.IsWoodsCampaign ? _world.AdvanceWoodsPhase() : _world.IsQuarryCampaign ? _world.AdvanceQuarryPhase() : _world.IsLakeCampaign ? _world.AdvanceLakePhase() : _world.AdvanceRiverPhase()) SaveWorld(); else Notice((_world.IsFinaleCampaign ? _world.FinaleActionProblem() : _world.IsWoodsCampaign ? _world.WoodsActionProblem() : _world.IsQuarryCampaign ? _world.QuarryActionProblem() : _world.IsLakeCampaign ? _world.LakeActionProblem() : _world.RiverActionProblem()) ?? "Keep developing the village."); });
        _goalDashboard.AddChild(_riverAction);_goalDashboard.MoveChild(_riverAction,1);
        _tutorialText = Text("", 15, true); _campaignControls.AddChild(_tutorialText);
        _dismissHint = Button("Dismiss this hint", () => { var hint = _world.CurrentCampaignHint(); if (hint != null) _world.Campaign!.Dismissed.Add(hint.Id); }); _campaignControls.AddChild(_dismissHint);
        _guidance = Button("", () => _world.Campaign!.Guidance = !_world.Campaign.Guidance); _campaignControls.AddChild(_guidance);
        _reopenHints = Button("Show hints again", () => { _world.Campaign!.Dismissed.Clear(); _world.Campaign.Guidance = true; }); _campaignControls.AddChild(_reopenHints);
        _keepPlaying = Button("Continue playing", CloseDrawer); _campaignControls.AddChild(_keepPlaying);
        _nextLevel = Button("Next settlement", () => SwitchCampaign(_world.Campaign!.Level + 1, false)); _campaignControls.AddChild(_nextLevel);
        _replayLevel = Button("Replay this settlement", () => SwitchCampaign(_world.Campaign!.Level, true)); _campaignControls.AddChild(_replayLevel);
        _restoreReplay = Button("Restore village before replay", RestoreBeforeReplay); _campaignControls.AddChild(_restoreReplay);
        column.AddChild(Text($"CAMPAIGN · {World.CampaignLevels.Length} SETTLEMENTS", 12, true));
        _campaignRecord = Text("", 14, true); column.AddChild(_campaignRecord);
        foreach (var level in World.CampaignLevels)
        {
            int id = level.Id;
            var button = Button($"{id}. {level.Title}", () => SwitchCampaign(id, false));
            button.AddThemeFontSizeOverride("font_size", 14); column.AddChild(button); _levelButtons.Add(button);
        }
        column.AddChild(Button("Return to standalone supper", () => SwitchCampaign(0, false)));
        column.AddChild(Text("Choose a settlement to start or resume it. Switching saves the village you leave. All buildings are available.", 14, true));
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
        CancelCameraDrag();
        _trackedGoalKey=null;
        bool mapChanged = !ReferenceEquals(_world.Map, world.Map);
        _autosaveElapsed = 0; _lastAutosaved = null;
        _world = world; CloseManagementUi(); _placing = false; _accumulator = 0; _paused = true;
        _completionAnnounced = world.Campaign?.Complete == true;
        CreateActors(); RefreshGhost(); RefreshSelection(); RebuildQueue();
        if (mapChanged) FrameMap();
        if (_menuEnabled && !_atMainMenu)
        {
            try { RememberSettlement(); } catch (Exception e) { Notice("Village opened, but Continue could not be updated: " + e.Message); }
        }
    }
    private void SwitchCampaign(int level, bool replay)
    {
        try
        {
            var book = ReadCampaignBook();
            // Prepare and validate the destination before writing or replacing the live settlement.
            var next = !_world.Creative && level == (_world.Campaign?.Level ?? 0) && !replay ? _world :
                !replay && book.Settlements.TryGetValue(level, out var json) ? World.LoadJson(json) : level == 0 ? World.NewScenario() : World.NewCampaign(level);
            if (_world.Creative) _world.SaveFile(CurrentSavePath); else book.Capture(_world);
            if (replay && book.Settlements.TryGetValue(level, out var previous)) book.BeforeReplay[level] = previous;
            book.Capture(next); book.SaveFile(_campaignPath);
            _campaignBook = book; AdoptWorld(next); ToggleDrawer(2); _drawerPages[2].ScrollVertical=0;
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
        _riverAction.Visible = campaign?.Complete != true && (_world.IsFinaleCampaign && campaign!.Finale!.Phase is 0 or 2 || _world.IsWoodsCampaign && campaign!.Woods!.Phase<2 || _world.IsRiverCampaign && campaign!.River!.Phase < 3 || _world.IsLakeCampaign && campaign!.Lake!.Phase<2 || _world.IsQuarryCampaign && campaign!.Quarry!.Phase==0);
        if (_riverAction.Visible) { var problem=_world.IsFinaleCampaign?_world.FinaleActionProblem():_world.IsWoodsCampaign?_world.WoodsActionProblem():_world.IsQuarryCampaign?_world.QuarryActionProblem():_world.IsLakeCampaign?_world.LakeActionProblem():_world.RiverActionProblem(); _riverAction.Text = _world.IsFinaleCampaign?_world.FinaleActionLabel:_world.IsWoodsCampaign?_world.WoodsActionLabel:_world.IsQuarryCampaign?"Assess the gathering place":_world.IsLakeCampaign?_world.LakeActionLabel:_world.RiverActionLabel; _riverAction.Disabled = problem != null; _riverAction.TooltipText = problem ?? "Advance this settlement's next phase when you are ready."; }
        UpdateGoalDashboard();
        UpdateTrackedGoal();
        _progress.Visible = !_world.Creative;
        if (_world.Creative)
        {
            _campaignControls.Hide(); _standaloneGuide.Hide(); _supperButton.Hide(); _supperBreadLink.Hide();
            _goalTitle.Text = "Creative · arrange and watch";
            _goalArrival.Text = "Build instantly for free. Villagers keep working, growing crops and taking breaks; meals and hunger are disabled.";
            _objective.Text = "All decorations are available. Select a completed building to remove it and recover its stored goods. Keep entrances and crossings connected.";
            return;
        }
        _campaignControls.Visible = campaign != null; _standaloneGuide.Visible = campaign == null; _supperButton.Visible = campaign == null || campaign.Level == 4 || _world.IsFinaleCampaign && campaign.Finale!.Phase>=4;
        _supperBreadLink.Visible=_supperButton.Visible && !_world.Food.SupperComplete && !_world.Food.Celebrating;
        _goalTitle.Text = campaign == null ? "The first village supper" : $"{campaign.Level}. {World.CampaignLevels[campaign.Level - 1].Title}";
        _goalArrival.Text = campaign == null ? "Give your neighbors a home and enough bread to celebrate together." : World.CampaignLevels[campaign.Level - 1].Arrival;
        if (_world.IsRiverCampaign && campaign!.River!.Phase>0) _goalArrival.Text = "Grow at your own pace. Food can come from either bank; assessments count actual pantry deliveries, meals and square visits.";
        if (_world.IsLakeCampaign && campaign!.Lake!.Phase>0) _goalArrival.Text = "Grow when ready. Choose a food mix and give residents time to rest and meet. The assessment needs three consecutive full mixed meals with fresh supply.";
        _campaignRecord.Text = _campaignBook?.Completed.Count > 0 ? "Completed: " + string.Join(", ", _campaignBook.Completed.OrderBy(i => i)) : $"{World.CampaignLevels.Length} settlements to learn at your own pace.";
        if (campaign == null) return;
        _restoreReplay.Visible = _campaignBook?.BeforeReplay.ContainsKey(campaign.Level) == true;
        _objective.Text = campaign.Complete ? (campaign.Level == World.CampaignLevels.Length ? "Campaign complete! A home, a livelihood, and a table for everyone. Keep playing or replay any settlement." : "Settlement complete! Continue to the next village or keep playing here.") : _world.CampaignObjective;
        _progress.Value = campaign.Complete ? 100 : _world.CampaignProgress * 100;
        _menuButtons[2].Text = _world.Gardener == VisitorState.Pending ? "Goals · Visitor" : campaign.Complete ? "Goals · Complete" : "Goals";
        var hint = _world.CurrentCampaignHint();
        _tutorialText.Text = hint?.Text ?? (campaign.Guidance ? "No more hints right now. You can show dismissed hints again below." : "Tutorial guidance is off. Objectives still count.");
        _tutorialText.Visible = !campaign.Complete;
        _dismissHint.Visible = hint != null;
        _guidance.Text = campaign.Guidance ? "Guidance: on" : "Guidance: off";
        _keepPlaying.Visible = campaign.Complete;
        _nextLevel.Visible = campaign.Complete && campaign.Level < World.CampaignLevels.Length;
        if (campaign.Complete && !_completionAnnounced)
        {
            _completionAnnounced = true;
            try { SaveCampaign(); RememberSettlement(); Notice("Settlement complete! Progress saved. Open Goals [G] to continue or keep playing here."); }
            catch (Exception e) { Notice("Settlement complete, but progress could not be saved. Try F5. " + e.Message); }
        }
    }
}
