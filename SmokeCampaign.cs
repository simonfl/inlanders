using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunCampaignSmoke()
    {
        _campaignPath = Path.Combine(Path.GetTempPath(), "inlanders-rendered-campaign-" + Guid.NewGuid() + ".json");
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
            async Task FinishLevel()
            {
                for (int i = 0; i < 10000 && !_world.Campaign!.Complete; i++)
                {
                    _world.Tick(0.1f); _world.Validate();
                    if (i % 100 == 0) await Frames();
                }
                await Frames(); Check(_world.Campaign!.Complete, "Campaign level did not finish");
            }
            _paused = true; string standalone = _world.SaveJson();
            await OpenMenu(2); await UiClick(_levelButtons[0]); await Frames();
            Check(_world.Campaign?.Level == 1 && _paused && _tutorialText.Text.Contains("WASD"), "Campaign entry failed");
            Check(_kindButtons.Values.All(b => !b.Disabled), "Campaign restricted buildings");
            await Capture("artifacts/f11-level1.png");
            await UiClick(_dismissHint); Check(_world.Campaign!.Dismissed.Contains("welcome"), "Dismiss failed");
            await UiClick(_reopenHints); await UiClick(_guidance); await Press(Key.F5);
            string saved = _world.SaveJson(); await UiClick(_guidance); await Press(Key.F9); await Frames();
            Check(_world.SaveJson() == saved && !_world.Campaign!.Guidance, "Tutorial save/load failed");
            foreach (var cell in new[] { new Cell(3, 0), new(6, 0), new(3, 6), new(-5, 6) })
            {
                await UiClick(_kindButtons[BuildingKind.Cottage]);
                await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z)));
            }
            Check(_world.Cottages.Count == 5, "Campaign building clicks failed");
            await FinishLevel();
            Check(_campaignBook!.Completed.Contains(1), "Completion not persisted");
            await OpenMenu(2); await UiClick(_nextLevel); await Frames();
            Check(_world.Campaign?.Level == 2 && _world.DeliveredBerries == 0, "Next level did not reset economy");
            await UiClick(_kindButtons[BuildingKind.ForagerHut]); await Click(_camera.UnprojectPosition(Vector3.Zero));
            await UiClick(_allocationButtons[(Role.Forager, 1)]); await UiClick(_allocationButtons[(Role.Forager, 1)]);
            for (int i = 0; i < 400; i++) _world.Tick(0.1f);
            await Press(Key.F5); string midway = _world.SaveJson();
            _world.Tick(1); await Press(Key.F9); Check(_world.SaveJson() == midway, "Mid-goal save/load failed");
            await OpenMenu(2); GetWindow().Size = new(960, 640); await Frames();
            _drawerPages[2].ScrollVertical = 0; await Frames(); await Capture("artifacts/f11-level2-960.png");
            Check(_drawer.GetGlobalRect().End.Y <= 640, "Campaign drawer outside small window");
            await FinishLevel(); await OpenMenu(2);
            Check(_campaignBook!.Completed.SetEquals(new[] { 1, 2 }), "Opening campaign progress wrong");
            string complete = _world.SaveJson();
            _world = World.NewScenario(); _campaignBook = null; ResumeCampaignOnLaunch(); await Frames();
            Check(_world.SaveJson() == complete && _paused, "Launch resume failed");
            await UiClick(_replayLevel); await Frames();
            Check(!_world.Campaign!.Complete && _campaignBook!.Completed.Contains(2), "Replay erased completion");
            await UiClick(_restoreReplay); await Frames();
            Check(_world.SaveJson() == complete, "Replay lost previous village");
            // A corrupt campaign file must not replace the live world.
            File.WriteAllText(_campaignPath, "broken"); await Press(Key.F9);
            Check(_world.SaveJson() == complete, "Bad campaign replaced live settlement");
            SaveCampaign(); SwitchCampaign(0, false); await Frames();
            Check(_world.Campaign == null && _world.SaveJson() == standalone, "Standalone snapshot was not preserved");
            GD.Print("SMOKE PASS: campaign entry, both authored levels, tutorial controls/save/load, completion, next/replay/restore, invalid-save recovery, standalone return, and 960px layout.");
            GetTree().Quit();
        }
        catch (Exception e) { GD.PrintErr("CAMPAIGN SMOKE FAIL: " + e); GetTree().Quit(1); }
        finally { foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(_campaignPath + suffix)) File.Delete(_campaignPath + suffix); }
    }
}
