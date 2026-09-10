using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunCatalogSmoke()
    {
        try { await CheckBuildCatalogUi(); for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); GetTree().Quit(); }
        catch (Exception e) { GD.PushError(e.ToString()); GetTree().Quit(1); }
    }
    private async Task CheckBuildCatalogUi()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
        var previous = _world; var size = GetWindow().Size; string preferences = _atmospherePath;
        bool labels = _showWorldLabels;
        _atmospherePath = "artifacts/f21g-preferences.cfg";
        try
        {
            AdoptWorld(World.NewCampaign(2)); _paused = true; await Frames();
            string state = _world.SaveJson();
            foreach (var window in new[] { new Vector2I(1440, 900), new(960, 640) })
            {
                GetWindow().Size = window; await Frames(); await OpenMenu(1); SelectBuildSection(0); await Frames();
                Check(_buildSections[0].IsVisibleInTree() && !_buildSections[1].IsVisibleInTree() && !_buildSections[2].IsVisibleInTree(), "Build sections overlap");
                Check(_cardCosts.Count == 10 && _cardCosts[BuildingKind.Bakery].Text.Contains("1 baker"), "Card catalog incomplete");
                foreach (var button in _kindButtons.Values)
                {
                    Check(button.GetRect().End.X <= _drawer.Size.X, "Card exceeds drawer width");
                    var texture = button.GetChild<HBoxContainer>(0).GetChild<TextureRect>(0).Texture;
                    Check(texture != null && texture.GetWidth() > 0, "Missing building thumbnail");
                }
                await Capture($"artifacts/f21g-place-{window.X}.png");
                await UiClick(_kindButtons[BuildingKind.Bakery]); await Frames();
                Check(_placing && _buildKind == BuildingKind.Bakery && _buildFooter.IsVisibleInTree(), "Card did not begin guided placement");
                _drawerPages[1].ScrollVertical = 10000; await Frames();
                Check(_buildFooter.GetGlobalRect().End.Y <= _drawer.GetGlobalRect().End.Y, "Tool guidance leaves drawer");
                Check(!_hintPanel.GetGlobalRect().Intersects(_buildButton.GetGlobalRect()), "Map hint covers cancel control");
                await Capture($"artifacts/f21g-tool-{window.X}.png");
                await UiClick(_buildButton); Check(!_placing, "Pinned cancel did not work");
                await UiClick(_buildSectionButtons[1]); await Frames();
                Check(_plantTreeButton.IsVisibleInTree() && !_kindButtons[BuildingKind.Cottage].IsVisibleInTree(), "Landscape retains building cards");
                await Capture($"artifacts/f21g-landscape-{window.X}.png");
                await UiClick(_buildSectionButtons[2]); await Frames();
                Check(_queueButtons.Values.Any(b => b.IsVisibleInTree()) && !_decorateButton.IsVisibleInTree(), "Existing list is not separate");
                await Capture($"artifacts/f21g-existing-{window.X}.png");
                await OpenMenu(3); _drawerPages[3].EnsureControlVisible(_viewName); await Frames();
                _viewName.GrabFocus(); var focus = _focus; float angle = _angle;
                Input.ParseInputEvent(new InputEventKey { Keycode = Key.W, PhysicalKeycode = Key.W, Unicode = 'w', Pressed = true });
                await Frames();
                Input.ParseInputEvent(new InputEventKey { Keycode = Key.W, PhysicalKeycode = Key.W, Pressed = false });
                Check(_focus == focus && _angle == angle && _viewName.Text.Contains('w'), "Typing moved camera or failed to edit name");
                _viewName.ReleaseFocus(); _viewName.Text = "";
                Input.ParseInputEvent(new InputEventKey { Keycode = Key.W, PhysicalKeycode = Key.W, Pressed = true }); await Frames();
                Input.ParseInputEvent(new InputEventKey { Keycode = Key.W, PhysicalKeycode = Key.W, Pressed = false });
                Check(_focus != focus, "Camera did not recover after text entry");
                if (!_showWorldLabels) ToggleWorldLabels();
                await UiClick(_worldLabelsButton); await Frames();
                Check(GetTree().GetNodesInGroup("world_labels").Cast<Node3D>().All(n => !n.Visible), "World labels remain visible");
                _showWorldLabels = true; LoadAtmosphere();
                Check(!_showWorldLabels, "Label preference did not reload from disk"); UpdateLabelButtons();
                CreateActors(); await Frames();
                Check(GetTree().GetNodesInGroup("world_labels").Cast<Node3D>().All(n => !n.Visible), "Rebuilt world forgot label preference");
                ToggleWatch(); await Frames();
                Check(_watchLabelsButton.IsVisibleInTree() && _watchBar.GetGlobalRect().End.X <= window.X, "Watch label control does not fit");
                await Capture($"artifacts/f21g-watch-{window.X}.png");
                await UiClick(_watchLabelsButton); Check(_showWorldLabels, "Watch cannot restore labels"); ExitWatch();
                Check(_world.SaveJson() == state, "Catalog/navigation changed settlement state");
            }
            Check(TaskName(Work.Leisure) != "Idle" && TaskName(Work.ToLeisure) != "Idle", "Breaks labelled idle");
            GD.Print("PASS: visual catalog, separate Place/Landscape/Existing, pinned guidance, typing vs camera, persistent world labels and Watch controls at 1440/960.");
        }
        finally
        {
            Input.ParseInputEvent(new InputEventKey { Keycode = Key.W, PhysicalKeycode = Key.W, Pressed = false });
            if (_showWorldLabels != labels) ToggleWorldLabels();
            _atmospherePath = preferences; AdoptWorld(previous); GetWindow().Size = size;
        }
    }
}
