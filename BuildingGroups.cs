using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;

public partial class Game
{
    private static readonly string[] BuildingCategoryNames = { "All buildings", "Homes", "Food", "Materials", "Storage & bridges", "Gathering" };
    private GridContainer _buildingCategories = null!;
    private readonly List<Button> _categoryButtons = new();
    private readonly List<(VBoxContainer Panel, int Category)> _buildingGroups = new();

    private void MakeBuildingCategories(VBoxContainer column)
    {
        _buildingCategories = new GridContainer { Columns = 2 }; column.AddChild(_buildingCategories);
        // Start with the two housing choices; every other category stays one click away.
        foreach (int category in new[] { 1, 2, 3, 4, 5, 0 })
        {
            int choice = category;
            var button = Button(BuildingCategoryNames[category], () =>
            {
                _buildingFilter.Select(choice); UpdateVillageDirectory(); _drawerPages[1].ScrollVertical = 0;
            });
            button.CustomMinimumSize = new(0, 34); button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            button.AddThemeFontSizeOverride("font_size", 13);
            _buildingCategories.AddChild(button); _categoryButtons.Add(button);
        }
        _buildingFilter.Select(1);
    }
    private void UpdateBuildingCategoryNavigation()
    {
        _buildingCategories.Visible = _buildSection == 0;
        _buildingFilter.Visible = _buildSection == 2 || (_buildSection == 0 && _catalogKeyboard);
    }
    private void MakeGroupedBuildingCards(VBoxContainer parent)
    {
        void Group(int category, string title, string explanation, params BuildingKind[] kinds)
        {
            var panel = new VBoxContainer(); panel.AddThemeConstantOverride("separation", 8); parent.AddChild(panel);
            panel.AddChild(Text(title, 16, true)); panel.AddChild(Text(explanation, 13, true));
            foreach (var kind in kinds) MakeBuildingCard(panel, kind);
            _buildingGroups.Add((panel, category));
        }
        Group(1, "Homes", "Give residents a place to live and sleep.", BuildingKind.Cottage, BuildingKind.Lodge);
        Group(2, "Gather food", "Use what grows or lives nearby.", BuildingKind.ForagerHut, BuildingKind.FishingDock, BuildingKind.HuntingLodge);
        Group(2, "Grow food", "Harvest food villagers can eat directly.", BuildingKind.VegetableGarden, BuildingKind.Orchard);
        Group(2, "Make bread", "Farm grain, then turn it into food at a bakery. You need both.", BuildingKind.Farm, BuildingKind.Bakery);
        Group(2, "Bring meals closer", "A pantry moves existing food near residents; it does not produce food.", BuildingKind.Pantry);
        Group(3, "Materials & home improvements", "Make planks, extract stone, or improve occupied homes.", BuildingKind.Sawmill, BuildingKind.Quarry, BuildingKind.Carpenter);
        Group(4, "Storage & bridges", "Shorten material deliveries and cross water.", BuildingKind.Stockpile, BuildingKind.Bridge);
        Group(5, "Places to gather", "Give neighbors somewhere to take a break together.", BuildingKind.SeatingGarden, BuildingKind.Square, BuildingKind.GatheringHall);
    }
    private void UpdateBuildingGroups(int category)
    {
        foreach (var group in _buildingGroups) group.Panel.Visible = category == 0 || group.Category == category;
        foreach (var button in _categoryButtons)
        {
            bool selected = button.Text.TrimStart('›', ' ') == BuildingCategoryNames[category];
            button.Text = (selected ? "› " : "") + button.Text.TrimStart('›', ' ');
            button.Modulate = selected ? _cream : Colors.White;
        }
    }
}
