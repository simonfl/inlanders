using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Button _removeBuildingButton = null!;
    private Label _removalInfo = null!;
    private Button _cancelDemolition = null!;
    private void MakeCreativeControls()
    {
        _removeBuildingButton = Button("Remove building", () =>
        {
            if (!_world.Creative)
            {
                Notice(_world.RequestDemolition(_selectedSite) ? "Demolition ordered. Builders recover goods and timber; cancel before dismantling begins." : _world.RemovalProblem(_selectedSite) ?? "Demolition already ordered.");
                return;
            }
            if (_world.RemoveBuilding(_selectedSite))
            {
                ClearSelection(); RebuildQueue(); RenderActors(0); RenderFoodViews();
                Notice("Building removed. Stored goods returned to the yard.");
            }
            else Notice(_world.RemovalProblem(_selectedSite) ?? "Cannot remove this building.");
        });
        _buildingDetails.AddChild(_removeBuildingButton);
        _removalInfo = Text("", 14, true); _buildingDetails.AddChild(_removalInfo);
        _cancelDemolition = Button("Cancel demolition", () => Notice(_world.CancelDemolition(_selectedSite) ? "Demolition cancelled. Hauled goods remain in storage." : "Dismantling has started; let builders finish."));
        _buildingDetails.AddChild(_cancelDemolition);
    }
    private void UpdateCreativeControls(Cottage? selected)
    {
        bool visible = selected?.Complete == true;
        _cancelDemolition.Visible = visible && selected!.DemolitionRequested && selected.DemolitionProgress == 0;
        _cancelDemolition.Disabled = _world.Food.Celebrating;
        _removeBuildingButton.Visible = _removalInfo.Visible = visible;
        if (!visible) return;
        var problem = _world.RemovalProblem(selected!.Id);
        if (!_world.Creative)
        {
            if (selected.DemolitionRequested) _siteInfo.Text = $"{Buildings.Get(selected.Kind).Name.ToUpperInvariant()} {selected.Id}\n\nDemolition ordered · service and beds unavailable";
            _removeBuildingButton.Text = selected.DemolitionRequested ? "Demolition ordered" : "Order demolition";
            _removeBuildingButton.Disabled = selected.DemolitionRequested || problem != null;
            _removalInfo.Text = selected.DemolitionRequested ? $"Builders evacuate stored goods, then dismantle ({selected.DemolitionProgress:P0}) and haul {selected.Delivered} remaining {selected.Material.ToString().ToLowerInvariant()} + {selected.DeliveredStone} stone. Production and beds are unavailable.\n" + (problem ?? "Keep a builder assigned. Access stays occupied until recovery finishes.") :
                problem ?? $"Recover all {Buildings.Get(selected.Kind).CostText} and stored goods by builder trips. Dismantling takes 12 work seconds plus hauling. Unripe crops and partial processing progress are lost when removed.\nHousing after order: {System.Math.Min(_world.Population, _world.Beds - Buildings.Get(selected.Kind).Beds)}/{_world.Population}. Production stops immediately. Cancel before dismantling begins.";
            return;
        }
        _removeBuildingButton.Text = "Remove building";
        _removeBuildingButton.Disabled = problem != null;
        _removalInfo.Text = problem ?? "Creative: remove instantly. Stored goods return to the yard; villagers keep carried goods.";
        if (selected.Kind == BuildingKind.VegetableGarden)
            _siteInfo.Text = $"VEGETABLE GARDEN {selected.Id}\n\n1 farmer slot · crop {selected.Growth:P0}\n{selected.Harvest} vegetables ripe · 8 per harvest\nFood needs are disabled.";
        if (selected.Kind == BuildingKind.Square)
            _siteInfo.Text = $"VILLAGE SQUARE {selected.Id}\n\nShort breaks between jobs · no staff.\nLeave open space around the entrance. Supper is disabled in Creative.";
    }
}
