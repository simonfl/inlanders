using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunMainMenuSmoke()
    {
        string stem = Path.Combine(Path.GetTempPath(), "inlanders-menu-" + Guid.NewGuid());
        _savePath = stem + "-original.json"; _largeSavePath = stem + "-large.json";
        _campaignPath = stem + "-campaign.json"; _continuePath = stem + "-continue.json";
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
            async Task MenuClick(string label)
            {
                var button = _mainButtons[label]; _mainScroll.EnsureControlVisible(button); await Frames();
                await Click(button.GetGlobalRect().GetCenter()); await Frames();
            }
            MakeMainMenu(); await Frames();
            Check(_atMainMenu && !_hud.Visible && _mainButtons["Continue"].Disabled, "Fresh title screen incorrect");
            string initial = _world.SaveJson();
            await Press(Key.Space); await Press(Key.T); await Press(Key.P); await Frames();
            Check(_world.SaveJson() == initial && !_placing && _paused, "Menu input leaked into simulation");
            foreach (var size in new[] { new Vector2I(1440, 900), new(960, 640) })
            {
                GetWindow().Size = size; await Frames();
                Check(_mainPanel.GetGlobalRect().End.Y <= size.Y && _mainPanel.GetGlobalRect().End.X <= size.X, "Title screen outside viewport");
                await Capture($"artifacts/f19-menu-{size.X}.png");
            }
            await MenuClick("Settings"); bool muted = _soundMuted;
            await MenuClick(muted ? "Unmute sound" : "Mute sound"); Check(_soundMuted != muted, "Menu mute failed");
            await MenuClick(muted ? "Mute sound" : "Unmute sound"); await MenuClick("Back");
            await MenuClick("Free play"); await MenuClick("New Original clearing");
            Check(!_atMainMenu && _hud.Visible && _world.Map.OriginalOutline && _paused, "New free play failed");
            _world.SetPath(new(3, 0), true); _world.Tick(0.1f); string original = _world.SaveJson();
            ReturnToMainMenu(); await Frames(); Check(_atMainMenu, "Return to menu failed");
            await MenuClick("Continue"); Check(_world.SaveJson() == original, "Continue lost original settlement");
            ReturnToMainMenu(); await Frames();
            await MenuClick("Free play"); await MenuClick("New Three clearings");
            Check(_world.Map.Width == 32, "Large free play failed");
            string large = _world.SaveJson(); ReturnToMainMenu(); await Frames(); await MenuClick("Continue");
            Check(_world.SaveJson() == large, "Continue ignored last played large map");
            ReturnToMainMenu(); await Frames(); await MenuClick("Campaign"); await MenuClick("Start level 1");
            _world.Campaign!.Guidance = false; string campaign = _world.SaveJson(); ReturnToMainMenu(); await Frames();
            await MenuClick("Continue"); Check(_world.SaveJson() == campaign, "Continue lost campaign tutorial state");
            ReturnToMainMenu(); await Frames(); await MenuClick("Campaign"); await MenuClick("Replay level 1");
            Check(_world.Campaign!.Guidance && _campaignBook!.BeforeReplay[1] == campaign, "Replay archive failed");
            ReturnToMainMenu(); await Frames();
            File.WriteAllText(_continuePath, "broken"); await MenuClick("Continue");
            Check(_atMainMenu && _menuMessage.Text.Contains("Could not open"), "Corrupt Continue left menu");
            await MenuClick("Free play"); await MenuClick("Resume Original clearing"); Check(_world.SaveJson() == original, "Recovery through separate save failed");
            ReturnToMainMenu(); await Frames(); await MenuClick("Free play"); await MenuClick("New Original clearing");
            ReturnToMainMenu(); await Frames(); await MenuClick("Free play"); await MenuClick("Restore previous Original clearing");
            Check(_world.SaveJson() == original, "New free play lost preceding village");
            ReturnToMainMenu(); await Frames();
            File.Delete(_continuePath); _campaignBook = null; File.SetLastWriteTimeUtc(_largeSavePath, DateTime.UtcNow.AddMinutes(1));
            await MenuClick("Continue"); Check(_world.SaveJson() == large, "Legacy latest-save fallback failed");
            // Save failure must leave the current village open and untouched.
            string validPath = _savePath; _savePath = _largeSavePath;
            string previousLarge = _largeSavePath; _largeSavePath = Path.Combine(_savePath, "invalid.json");
            ReturnToMainMenu(); Check(!_atMainMenu && _world.SaveJson() == large, "Failed save discarded live village");
            _largeSavePath = previousLarge; _savePath = validPath;
            GD.Print("SMOKE PASS: main menu at 1440/960, input isolation, sound settings, original/large/campaign Continue, replay archives, corrupt-save recovery, legacy fallback, and failed-save protection.");
            GetTree().Quit();
        }
        catch (Exception e) { GD.PrintErr("MENU SMOKE FAIL: " + e); GetTree().Quit(1); }
        finally
        {
            foreach (string kind in new[] { "original", "large", "campaign", "continue" })
                foreach (string suffix in new[] { "", ".bak", ".tmp", ".before-new", ".before-restore" }) { string path = stem + "-" + kind + ".json" + suffix; if (File.Exists(path)) File.Delete(path); }
        }
    }
}
