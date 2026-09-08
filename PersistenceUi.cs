using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private string _savePath = "saves/settlement.json";
    private string _notice = "";
    private float _uiTime, _noticeUntil;
    private void Notice(string message) { _notice = message; _noticeUntil = _uiTime + 8; }
    private void SaveWorld()
    {
        try { _world.SaveFile(_savePath); Notice("Settlement saved. F9 restores this save."); }
        catch (Exception e) { Notice("Could not save: " + e.Message); }
    }
    private void LoadWorld()
    {
        try
        {
            var restored = World.LoadFile(_savePath); // Validate fully before replacing the current game.
            _world = restored; CloseManagementUi(); _placing = false; _accumulator = 0;
            _paused = true; _pauseButton.Text = "Resume  [Space]";
            CreateActors(); RefreshGhost(); RefreshSelection(); RebuildQueue();
            Notice("Settlement restored and paused. Press Space to continue.");
        }
        catch (Exception e) { Notice("Could not load; current game kept. " + e.Message); }
    }
}
