using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task SmokeSawmill()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        await UiClick(_resetButton);
        Check(_paused, "Sawmill fixture must be paused"); _savePath = "artifacts/f08-rendered-save.json";
        await UiClick(_kindButtons[BuildingKind.Sawmill]);
        await Click(_camera.UnprojectPosition(new(3,0,0)));
        Check(_world.Cottages.Count == 1 && _world.Cottages[0].Kind == BuildingKind.Sawmill, "Sawmill selector failed");
        await UiClick(_kindButtons[BuildingKind.Lodge]); await Press(Key.R);
        await Click(_camera.UnprojectPosition(new(6,0,0)));
        Check(_world.Cottages.Count == 2 && _world.Cottages[1].Required == Buildings.Get(BuildingKind.Lodge).Cost && _world.Cottages[1].Material == Inlanders.Simulation.Resource.Planks, "Lodge selector/cost failed");
        await OpenMenu(0);
        await UiClick(_allocationButtons[(Role.Sawyer, 1)]);
        Check(_world.People.Count(v => v.Role == Role.Sawyer) == 1, "Sawyer allocation failed");
        await Capture("artifacts/f08-workforce.png");
        await OpenMenu(1);
        for (int i = 0; i < 15000 && !_world.People.Any(v => v.Task == Work.Sawing); i++) { _world.Tick(0.1f); _world.Validate(); }
        Check(_world.People.Any(v => v.Task == Work.Sawing), "Sawmill never started a batch");
        SelectBuilding(_world.Cottages[0].Id);
        await Press(Key.F5); string batch = File.ReadAllText(_savePath);
        for (int i = 0; i < 30; i++) _world.Tick(0.1f);
        await Press(Key.F9); Check(_world.SaveJson() == batch && _paused, "Sawmill batch restore failed");
        SelectBuilding(_world.Cottages[0].Id); _selectedPerson = _world.People.First(v => v.Role == Role.Sawyer).Id;
        _focus = new(3,0,0); _camera.Size = 17; UpdateCamera();
        for (int i = 0; i < 8; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        Check(_people[_selectedPerson].Saw.Visible, "Sawyer tool missing");
        _noticeUntil = 0; await Capture("artifacts/f08-sawmill.png");
        for (int i = 0; i < 3000 && !_world.People.Any(v => v.Carried > 0 && v.Cargo == Inlanders.Simulation.Resource.Planks); i++) _world.Tick(0.1f);
        var carrier = _world.People.First(v => v.Carried > 0 && v.Cargo == Inlanders.Simulation.Resource.Planks);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        Check(_people[carrier.Id].Carry.Visible && _people[carrier.Id].Count == carrier.Carried, "Plank cargo missing");
        await Capture("artifacts/f08-planks.png");
        for (int i = 0; i < 15000 && _world.Housed != 4; i++) { _world.Tick(0.1f); _world.Validate(); }
        Check(_world.Housed == 4, "Lodge never completed");
        await Press(Key.F5); await Press(Key.F9);
        SelectBuilding(_world.Cottages[1].Id);
        await Capture("artifacts/f08-lodge.png");
        Check(_siteInfo.Text.Contains("4 beds ready"), "Lodge inspector not restored");
        GD.Print("SMOKE PASS: sawmill/lodge placement, sawyer staffing, saved sawing batch, saw tool, plank cargo, and four-bed lodge completion/load.");
    }
}
