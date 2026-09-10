using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Button _removeBuildingButton = null!;
    private Label _removalInfo = null!;
    private void MakeCreativeControls()
    {
        _removeBuildingButton = Button("Remove building", () =>
        {
            if (_world.RemoveBuilding(_selectedSite))
            {
                ClearSelection(); RebuildQueue(); RenderActors(0); RenderFoodViews();
                Notice("Building removed. Stored goods returned to the yard.");
            }
            else Notice(_world.RemovalProblem(_selectedSite) ?? "Cannot remove this building.");
        });
        _buildingDetails.AddChild(_removeBuildingButton);
        _removalInfo = Text("", 14, true); _buildingDetails.AddChild(_removalInfo);
    }
    private void UpdateCreativeControls(Cottage? selected)
    {
        bool visible = _world.Creative && selected?.Complete == true;
        _removeBuildingButton.Visible = _removalInfo.Visible = visible;
        if (!visible) return;
        var problem = _world.RemovalProblem(selected!.Id);
        _removeBuildingButton.Disabled = problem != null;
        _removalInfo.Text = problem ?? "Creative: remove instantly. Stored goods return to the yard; villagers keep carried goods.";
        if (selected.Kind == BuildingKind.VegetableGarden)
            _siteInfo.Text = $"VEGETABLE GARDEN {selected.Id}\n\n1 farmer slot · crop {selected.Growth:P0}\n{selected.Harvest} vegetables ripe · 8 per harvest\nFood needs are disabled.";
        if (selected.Kind == BuildingKind.Square)
            _siteInfo.Text = $"VILLAGE SQUARE {selected.Id}\n\nShort breaks between jobs · no staff.\nLeave open space around the entrance. Supper is disabled in Creative.";
    }
}
