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
        try { if (_world.Campaign != null) SaveCampaign(); else _world.SaveFile(CurrentSavePath); RememberSettlement(); Notice("Settlement saved. F9 restores this save."); }
        catch (Exception e) { Notice("Could not save: " + e.Message); }
    }
    private void LoadWorld()
    {
        try
        {
            World restored;
            if (_world.Campaign != null)
            {
                var book = CampaignBook.LoadFile(_campaignPath);
                restored = World.LoadJson(book.Settlements[book.ActiveLevel]);
                if (_campaignBook != null) book.Completed.UnionWith(_campaignBook.Completed);
                _campaignBook = book;
            }
            else restored = World.LoadFile(CurrentSavePath); // Validate fully before replacing the current game.
            _autosaveElapsed = 0; _lastAutosaved = null;
            _world = restored; CloseManagementUi(); _placing = false; _accumulator = 0;
            _paused = true; _pauseButton.Text = "Resume  [Space]";
            CreateActors(); RefreshGhost(); RefreshSelection(); RebuildQueue();
            Notice("Settlement restored and paused. Press Space to continue.");
            _completionAnnounced = _world.Campaign?.Complete == true;
            try { RememberSettlement(); }
            catch (Exception e) { Notice("Settlement restored, but Continue could not be updated: " + e.Message); }
        }
        catch (Exception e) { Notice("Could not load; current game kept. " + e.Message); }
    }
}
