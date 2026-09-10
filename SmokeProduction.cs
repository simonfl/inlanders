using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunProductionSmoke()
    {
        try { await CheckProductionUi(); await CheckHappinessUi(); await CheckDemolitionUi(); for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); GetTree().Quit(); }
        catch (Exception e) { GD.PrintErr("PRODUCTION SMOKE FAIL: " + e); GetTree().Quit(1); }
    }
    private async Task CheckProductionUi()
    {
        void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); }
        async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
        var previous = _world; var window = GetWindow().Size;
        try
        {
            var w = World.NewCreative(); foreach (var p in w.People) w.Assign(p.Id, Role.Unassigned);
            var bakery = w.Place(new(3,-3), false, BuildingKind.Bakery)!;
            w.Assign(0, Role.Baker);
            AdoptWorld(w); _paused = true;
            foreach (var size in new[] { new Vector2I(1440,900), new(960,640) })
            {
                GetWindow().Size = size; SelectBuilding(bakery.Id); await Frames();
                Check(_productionState.Text.Contains("MISSING GRAIN") && _productionSource.Visible, "Missing-input diagnostic/source omitted");
                string before = w.SaveJson(); await UiClick(_productionSource); await Frames();
                Check(w.SaveJson() == before && _focus.X == w.YardAccess.X && _focus.Z == w.YardAccess.Z, "Source navigation changed simulation or missed pantry");
                await UiClick(_productionPause); await Frames();
                Check(bakery.WorkPaused && _productionState.Text.Contains("PAUSED") && w.People[0].Role == Role.Baker, "Pause changed staff or failed");
                await UiClick(_productionPause); await Frames();
                if (!_productionTargetControls.Visible) await UiClick(_productionTargetToggle);
                await UiClick(_outputTargetMore); await Frames();
                Check(bakery.OutputTarget == 4, "Target button did not set four");
                await UiClick(_outputTargetLess); await Frames();
                Check(bakery.OutputTarget == 0 && _productionState.Text.Contains("TARGET MET"), "Zero target not explained");
                await Capture($"artifacts/f21h-target-{size.X}.png");
                await UiClick(_outputTargetUnlimited); await Frames();
                Check(bakery.OutputTarget == -1 && _productionState.Text.Contains("MISSING GRAIN"), "No-limit control failed");
                await UiClick(_productionTargetToggle); await Frames();
                _inspectionScroll.EnsureControlVisible(_staffPlus); await Frames();
                Check(_staffPlus.TooltipText.Contains("village-wide") && _staffPlus.TooltipText.Contains("Ivo"), "Role transfer candidate not explained");
            }
            w.Food.Grain = w.Food.GrownGrain = 8;
            for (int i = 0; i < 3000 && !w.People.Any(p => p.Task == Work.ToPantry && p.Cargo == Inlanders.Simulation.Resource.Bread); i++) w.Tick(.1f);
            SelectBuilding(bakery.Id); await Frames();
            Check(_productionState.Text.Contains("DELIVERING OUTPUT") && _workerLinks[0].Visible, "Delivery/worker link not shown");
            _inspectionScroll.ScrollVertical = 0; await Capture("artifacts/f21h-delivery.png");
            await UiClick(_productionPause); await Frames();
            string saved = w.SaveJson(); AdoptWorld(World.LoadJson(saved)); _paused = true; SelectBuilding(bakery.Id); await Frames();
            Check(_world.Cottages.Single().WorkPaused && _world.SaveJson() == saved, "World reload lost workplace controls");
            for (int i = 0; i < 800; i++) _world.Tick(.1f);
            OpenEconomy(); await Frames();
            _drawerPages[4].EnsureControlVisible(_foodFlow); await Frames();
            Check(_foodFlow.Text.Contains("Pantry deliveries: 4") && _foodFlow.Text.Contains("Portions eaten: 0 · closed/skipped demand: 0"), "Recent arrivals or Creative meal demand wrong");
            await Capture("artifacts/f21h-food-flow.png");
            var normal = World.NewCampaign(2);
            foreach (var p in normal.People) normal.Assign(p.Id, Role.Unassigned);
            var hut = normal.Cottages.First(c => c.Kind == BuildingKind.ForagerHut);
            normal.SetWorkplacePaused(hut.Id, true);
            for (int i = 0; i < 12000 && normal.EdibleStored >= normal.Population * 2; i++) normal.Tick(.1f);
            AdoptWorld(normal); _paused = true; OpenEconomy(); await Frames();
            int issue = Array.FindIndex(_economyReport!.Issues, i => i.Id == "food-paused");
            Check(issue >= 0, "Paused food production did not explain shortage");
            await UiClick(_economyIssues[issue]); await Frames();
            Check(_selectedSite == hut.Id && _productionPause.Text.Contains("Resume"), "Paused-food action did not open resume controls");
            await UiClick(_productionPause); await Frames();
            Check(!hut.WorkPaused, "Shortage recovery did not resume workplace");
            GD.Print("SMOKE PASS: workplace status/source/worker links, pause/resume, expandable targets, exact saves, named global role transfer and recent food flow at 1440/960.");
        }
        finally { AdoptWorld(previous); GetWindow().Size = window; await Frames(); }
    }
}
