using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunHudSmoke()
    {
        try { await CheckHud();
            // Let deferred frees from the final world restore finish before shutting down Godot.
            for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            GD.Print(OS.GetCmdlineUserArgs().Contains("--comfort-only") ? "SMOKE PASS: focused home comfort UI and visuals." : OS.GetCmdlineUserArgs().Contains("--watch-only") ? "SMOKE PASS: focused Watch and clean-view checks." : OS.GetCmdlineUserArgs().Contains("--hud-services-only") ? "SMOKE PASS: focused service/household HUD checks." : "SMOKE PASS: responsive HUD, menu/context controls, all building previews, rotation, rejection feedback, repeat planting, and preview state isolation."); GetTree().Quit(); }
        catch (Exception e) { GD.PrintErr("HUD SMOKE FAIL: " + e); GetTree().Quit(1); }
    }
    private async Task CheckHud()
    {
        if(OS.GetCmdlineUserArgs().Contains("--comfort-only")) { _paused=true;await CheckComfortUi();return; }
        if(OS.GetCmdlineUserArgs().Contains("--watch-only")) { _paused=true;await CheckWatchUi();return; }
        if(OS.GetCmdlineUserArgs().Contains("--hud-services-only")) { _paused=true;await CheckServiceHud();return; }
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Settle() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw); }
        if (!_paused) await Press(Key.Space);
        var originalSize = GetWindow().Size;
        foreach (var size in new[] { new Vector2I(1440,900), new(1280,720), new(960,640) })
        {
            GetWindow().Size = size; CloseManagementUi(); _placing = false; _noticeUntil = 0; await Settle();
            string state = _world.SaveJson();
            Check(_hud.Size == new Vector2(size.X, size.Y), "HUD still scales a fixed virtual canvas");
            Check(!_drawer.Visible && !_inspector.Visible, "Default village view obstructed");
            void Within(Control control)
            {
                var r = control.GetGlobalRect();
                Check(r.Position.X >= 0 && r.Position.Y >= 0 && r.End.X <= size.X && r.End.Y <= size.Y, $"Control outside {size}: {control.Name} {r}");
            }
            Within(_topBar); Within(_bottomBar); Within(_pauseButton); Within(_speedButton);
            Check(!_topBar.GetGlobalRect().Intersects(_bottomBar.GetGlobalRect()), "Bars overlap");
            await Capture($"artifacts/f21a-clear-{size.X}.png");
            await Press(Key.B); await Settle(); Check(_drawer.Visible && _tabs.CurrentTab == 1, "B did not open Build"); Within(_drawer);
            int sites = _world.Cottages.Count;
            await UiClick(_kindButtons[BuildingKind.Cottage]); Check(_placing && _world.Cottages.Count == sites, "UI click leaked into world placement");
            await Press(Key.Escape); Check(!_placing && _drawer.Visible, "Esc did not cancel preview first");
            await Capture($"artifacts/f21a-build-{size.X}.png");
            await Press(Key.B); Check(!_drawer.Visible, "Build menu did not collapse");
            await Press(Key.V); await UiClick(_roster[7]); await Settle();
            Check(_selectedPerson == 7 && _selectedSite == -1 && _inspector.Visible && _personDetails.Visible && !_buildingDetails.Visible, "Villager context did not open");
            Within(_inspector); Within(_assignButton);
            Check(size.X >= 1100 || !_drawer.Visible, "Narrow layout crowded by two panels");
            await Capture($"artifacts/f21a-person-{size.X}.png");
            await Press(Key.O); await Settle(); Check(_drawer.Visible && _tabs.CurrentTab == 3, "O did not open Options");
            Check(size.X >= 1100 || !_inspector.Visible, "Options did not replace narrow inspector");
            _drawerPages[3].EnsureControlVisible(_ambienceSlider); await Settle(); Within(_ambienceSlider);
            await Press(Key.G); await Settle(); Check(_tabs.CurrentTab == 2, "G did not open Goals");
            _drawerPages[2].EnsureControlVisible(_supperButton); await Settle(); Within(_supperButton);
            Check(_world.SaveJson() == state, "HUD navigation mutated the village");
            CloseManagementUi();
        }
        GetWindow().Size = originalSize; await Settle();
        await CheckPlacementPreview();
        await CheckBuildCatalogUi();
        // Exercise actual ground selection and its replacement by villager selection.
        var plan = _world.Place(new(3,0)) ?? throw new Exception("HUD building fixture rejected");
        await Settle(); await Click(_camera.UnprojectPosition(new(3,0,0))); await Settle();
        Check(_selectedSite == plan.Id && _selectedPerson == -1 && _buildingDetails.Visible && !_personDetails.Visible, "Ground selection did not open building context");
        await UiClick(_priorityButtons[2]); Check(plan.Priority == 2, "Context priority failed");
        await Capture("artifacts/f21a-building.png");
        await UiClick(_cancelButton); Check(!_inspector.Visible && _world.Cottages.Count == 0, "Cancellation did not clear inspector");
        await Press(Key.Escape); Check(!_drawer.Visible && !_inspector.Visible, "Esc did not clear panels");
        await CheckPathsUi();
        await CheckManagementUi();
        await CheckProductionUi();
        await CheckEconomyUi();
        await CheckResourceSurveyUi();
        await CheckWatchUi();
        await CheckAtmosphereUi();
        await CheckGrowthUi();
        await CheckPopulationUi();
        await CheckStorageUi();
        await CheckDirectoryUi();
        await CheckServiceHud();
    }
    private async Task CheckServiceHud()
    {
        await CheckVegetableUi();
            await CheckLeisureUi();
            await CheckDecorationUi();
            await CheckHappinessUi(); await CheckHomeUi(); await CheckServiceCoverageUi(); await CheckDemolitionUi();
            await CheckCameraViewsUi();
            await CheckVisitorUi();
    }
}
