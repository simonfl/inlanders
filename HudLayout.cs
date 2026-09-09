using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private static StyleBoxFlat HudStyle(string color, int padding = 12) => new()
    {
        BgColor = new(color), CornerRadiusTopLeft = 9, CornerRadiusTopRight = 9, CornerRadiusBottomLeft = 9, CornerRadiusBottomRight = 9,
        ContentMarginLeft = padding, ContentMarginRight = padding, ContentMarginTop = padding, ContentMarginBottom = padding
    };
    private Theme HudTheme()
    {
        var theme = new Theme { DefaultFontSize = 16 };
        theme.SetStylebox("normal", "Button", HudStyle("314740", 8)); theme.SetStylebox("hover", "Button", HudStyle("486153", 8));
        theme.SetStylebox("pressed", "Button", HudStyle("66745a", 8)); theme.SetStylebox("disabled", "Button", HudStyle("293b35", 8));
        theme.SetColor("font_color", "Button", new("e9e5d7")); theme.SetColor("font_disabled_color", "Button", new("819087"));
        theme.SetStylebox("panel", "TabContainer", new StyleBoxEmpty()); return theme;
    }
    private PanelContainer HudPanel(Control root)
    {
        var p = new PanelContainer(); p.AddThemeStyleboxOverride("panel", HudStyle("223831f5")); root.AddChild(p); return p;
    }
    private VBoxContainer DrawerPage(string name)
    {
        var scroll = new ScrollContainer { Name = name, HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        _tabs.AddChild(scroll); _drawerPages.Add(scroll);
        var column = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill }; column.AddThemeConstantOverride("separation", 10); scroll.AddChild(column); return column;
    }
    private void LayoutHud()
    {
        _hudSize = _hud.Size; float width = _hudSize.X, height = _hudSize.Y;
        _brand.Visible = width >= 1200; _shortcuts.Visible = width >= 1100;
        _topBar.Position = new(16, 12); _topBar.Size = new(width - 32, 68);
        _bottomBar.Position = new(16, height - 76); _bottomBar.Size = new(width - 32, 64);
        _drawer.Position = new(16, 92); _drawer.Size = new(316, Math.Max(260, height - 184));
        _inspector.Position = new(width - 324, 92); _inspector.Size = new(308, Math.Min(620, height - 184));
        _hintPanel.Position = new(Math.Max(16, (width - 650) / 2), height - 140); _hintPanel.Size = new(Math.Min(650, width - 32), 0);
        if (width < 1100 && _drawer.Visible && _inspector.Visible) _inspector.Hide();
    }
    private void ToggleDrawer(int index)
    {
        if (_drawer.Visible && _tabs.CurrentTab == index) { CloseDrawer(); return; }
        _tabs.CurrentTab = index; _drawerTitle.Text = MenuNames[index]; _drawer.Show();
        if (_hud.Size.X < 1100) _inspector.Hide();
    }
    private void CloseDrawer() => _drawer.Hide();
    private void ClearSelection() { _followPerson = false; _selectedPerson = -1; _selectedSite = -1; _inspector.Hide(); RefreshSelection(); }
    private void CloseManagementUi() { ExitWatch(); CloseDrawer(); ClearSelection(); }
    private void SelectPerson(int id) { _jobChoice.Select((int)_world.People[id].Role); _jobChoicePerson=id; _selectedPerson = id; _selectedSite = -1; ShowInspector(); RefreshSelection(); }
    private void SelectBuilding(int id) { _followPerson = false; _selectedSite = id; _selectedPerson = -1; ShowInspector(); RefreshSelection(); }
    private void ShowInspector() { if (_hud.Size.X < 1100) CloseDrawer(); _inspector.Show(); }
    private void BeginPlacement(BuildingKind kind) { ClearSelection(); _pathTool = 0; _buildKind = kind; _clearingTrees = false; _plantingTrees = false; _placing = true; RefreshGhost(); }
}
