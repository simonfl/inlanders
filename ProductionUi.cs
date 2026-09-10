using Godot;
using Inlanders.Simulation;
using System.Linq;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private VBoxContainer _productionControls = null!;
    private VBoxContainer _productionTargetControls = null!;
    private Button _productionTargetToggle = null!;
    private int _productionTargetSite = -1;
    private Label _productionState = null!, _productionTargetInfo = null!;
    private Button _productionPause = null!, _productionSource = null!, _outputTargetLess = null!, _outputTargetMore = null!, _outputTargetUnlimited = null!;
    private void MakeProductionControls()
    {
        _productionControls = new(); _productionControls.AddThemeConstantOverride("separation", 8);
        _buildingDetails.AddChild(_productionControls);
        _productionState = Text("", 14, true); _productionControls.AddChild(_productionState);
        _productionSource = Button("Show supply point", () =>
        {
            var site = _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite);
            if (site == null) return;
            var report = _world.ReadWorkplace(site);
            if (report.SourceBuilding is int id) { SelectBuilding(id); }
            else if (report.Source is Cell cell) { _focus = OnGround(cell.X, cell.Z, 0); _followPerson = false; UpdateCamera(); }
        });
        _productionControls.AddChild(_productionSource);
        _productionPause = Button("Pause new work", () =>
        {
            var site = _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite);
            if (site != null) _world.SetWorkplacePaused(site.Id, !site.WorkPaused);
        });
        _productionControls.AddChild(_productionPause);
        _productionTargetToggle = Button("Stock target", () => _productionTargetControls.Visible = !_productionTargetControls.Visible);
        _productionControls.AddChild(_productionTargetToggle);
        _productionTargetControls = new() { Visible = false }; _productionControls.AddChild(_productionTargetControls);
        _productionTargetInfo = Text("", 14, true); _productionTargetControls.AddChild(_productionTargetInfo);
        var row = new HBoxContainer(); _productionTargetControls.AddChild(row);
        void Change(int delta)
        {
            var site = _world.Cottages.FirstOrDefault(c => c.Id == _selectedSite);
            if (site != null) _world.SetOutputTarget(site.Id, System.Math.Clamp((site.OutputTarget < 0 ? 0 : site.OutputTarget) + delta, 0, 200));
        }
        _outputTargetLess = Button("− 4 target", () => Change(-4)); _outputTargetMore = Button("+ 4 target", () => Change(4));
        _outputTargetLess.SizeFlagsHorizontal = _outputTargetMore.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(_outputTargetLess); row.AddChild(_outputTargetMore);
        _outputTargetUnlimited = Button("No limit", () => _world.SetOutputTarget(_selectedSite, -1)); _productionTargetControls.AddChild(_outputTargetUnlimited);
        _productionTargetControls.AddChild(Text("Targets use village-wide stock + committed production. New crops/batches may exceed the target by one batch. Existing crops and output are still collected. Other workplaces keep their own targets.", 13, true));
    }
    private void UpdateProductionControls(Cottage? site)
    {
        _productionControls.Visible = site?.Complete == true && (World.ProductionOutput(site.Kind) != null || site.Kind==BuildingKind.Carpenter);
        if (!_productionControls.Visible || site == null) return;
        if (_productionTargetSite != site.Id) { _productionTargetControls.Hide(); _productionTargetSite = site.Id; }
        var report = _world.ReadWorkplace(site);
        _productionState.Text = report.State.ToUpperInvariant() + "\n" + report.Detail;
        _productionSource.Visible = report.Source != null;
        _productionSource.Text = report.SourceBuilding is int id ? $"Inspect stockpile {id}" : report.Source == _world.YardAccess ?
            site.Kind == BuildingKind.Sawmill ? "Show timber yard" : "Show pantry" : "Show berry patch";
        _productionPause.Text = site.WorkPaused ? "Resume workplace" : "Pause new work";
        _productionPause.Disabled=_world.Food.Celebrating || site.DemolitionRequested;
        _productionTargetToggle.Visible=site.Kind!=BuildingKind.Carpenter;
        if(site.Kind==BuildingKind.Carpenter)
        {
            _productionTargetControls.Hide();
            _productionSource.Text="Inspect home being improved";
            return;
        }
        _productionPause.TooltipText = "Current work and deliveries finish. Crops keep growing. Worker roles do not change.";
        Resource output = World.ProductionOutput(site.Kind)!.Value;
        _productionTargetToggle.Text = $"{output} target · {(site.OutputTarget < 0 ? "No limit" : site.OutputTarget.ToString())} {(_productionTargetControls.Visible ? "▴" : "▾")}";
        _productionTargetInfo.Text = $"{output} target: {(site.OutputTarget < 0 ? "No limit" : site.OutputTarget.ToString())}\n{_world.ProductionCommitted(output)} stored or committed village-wide";
        _productionPause.Disabled = _outputTargetMore.Disabled = _outputTargetUnlimited.Disabled = _world.Food.Celebrating || site.DemolitionRequested;
        _outputTargetLess.Disabled = _world.Food.Celebrating || site.DemolitionRequested || site.OutputTarget <= 0;
        _outputTargetMore.Disabled |= site.OutputTarget >= 200;
        _outputTargetUnlimited.Disabled |= site.OutputTarget < 0;
    }
}
