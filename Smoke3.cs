using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;

public partial class Game
{
    private async void RunSmoke()
    {
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            _savePath = "artifacts/m3-rendered-save.json";
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await Press(Key.Space); Check(_paused, "Pause failed");
            await Capture("artifacts/m3-start.png");
            await UiClick(_allocationButtons[(Role.Logger, -1)]);
            Check(_world.People.Count(v => v.Role == Role.Logger) == 1, "Logger minus failed");
            await UiClick(_allocationButtons[(Role.Logger, 1)]);
            Check(_world.People.Count(v => v.Role == Role.Logger) == 2, "Logger plus failed");
            await UiClick(_roster[6]); Check(_selectedPerson == 6, "Roster failed");
            _jobChoice.Select((int)Role.Baker); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); await UiClick(_assignButton); Check(_world.People[6].Role == Role.Baker, "Individual food assignment failed");
            _jobChoice.Select((int)Role.Farmer); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); await UiClick(_assignButton);
            Check(_world.People[6].Role == Role.Farmer, "Restore farmer failed");
            await OpenMenu(1);
            Check(_tabs.CurrentTab == 1, "Build tab failed");
            await UiClick(_kindButtons[BuildingKind.ForagerHut]); Check(_placing && _buildKind == BuildingKind.ForagerHut, "Building selection failed");
            await Click(_camera.UnprojectPosition(new(-3,0,-1))); Check(_world.Cottages.Count == 0, "Invalid plan accepted");
            await Click(_camera.UnprojectPosition(new(0,0,0))); Check(_world.Cottages.Count == 1, "Hut placement failed");
            await UiClick(_cancelButton); Check(_world.Cottages.Count == 0, "Cancel failed");
            foreach (var (cell, kind, rotated) in new[] {
                (new Cell(0,0), BuildingKind.ForagerHut, false), (new Cell(3,-3), BuildingKind.Farm, false), (new Cell(6,-3), BuildingKind.Bakery, false),
                (new Cell(3,0), BuildingKind.Cottage, false), (new Cell(6,0), BuildingKind.Cottage, true),
                (new Cell(3,6), BuildingKind.Cottage, false), (new Cell(-5,6), BuildingKind.Cottage, false) })
            {
                await UiClick(_kindButtons[kind]); if (_rotated != rotated) await Press(Key.R);
                int count = _world.Cottages.Count; await Click(_camera.UnprojectPosition(new(cell.X, 0, cell.Z)));
                Check(_world.Cottages.Count == count + 1, $"Placement failed at {cell}/{kind}");
            }
            await UiClick(_queueButtons[_world.Cottages[0].Id]);
            await UiClick(_priorityButtons[2]); Check(_world.Cottages[0].Priority == 2, "Construction priority failed");
            await Capture("artifacts/m3-plans.png");
            await UiClick(_saveButton);
            string plannedSave = File.ReadAllText(_savePath); Check(plannedSave == _world.SaveJson(), "Save button failed");
            await UiClick(_resetButton);
            await UiClick(_loadButton); Check(_paused && _world.SaveJson() == plannedSave, "Load button did not restore plans/roles exactly");
            var previousWorld = _world; File.WriteAllText(_savePath, "{broken"); await Press(Key.F9);
            Check(ReferenceEquals(previousWorld, _world), "Broken save replaced the live world"); File.WriteAllText(_savePath, plannedSave);
            var oldAngle = _angle; await Press(Key.E); Check(_angle != oldAngle, "Orbit failed"); await Press(Key.Q);
            float size = _camera.Size;
            Input.ParseInputEvent(new InputEventMouseButton { Position = new(800,400), ButtonIndex = MouseButton.WheelUp, Pressed = true });
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); Check(_camera.Size < size, "Zoom failed"); _camera.Size = size;
            await UiClick(_speedButton); await UiClick(_speedButton); Check(_speed == 6, "Speed failed");
            await Press(Key.Space);
            bool harvested = false, reloaded = false;
            for (int frame = 0; frame < 22000 && !_world.CanCelebrate; frame++)
            {
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); _world.Validate();
                if (!harvested && _world.People.Any(v => v.Task == Work.Harvesting)) { await Capture("artifacts/m3-harvest.png"); harvested = true; }
                if (!reloaded && _world.People.Any(v => v.Task == Work.Baking))
                {
                    await Press(Key.Space); await Press(Key.F5); string inFlight = File.ReadAllText(_savePath);
                    await Press(Key.Space); for (int i = 0; i < 8; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    await Press(Key.F9); Check(_paused && _world.SaveJson() == inFlight, "F5/F9 lost active batch, cargo, routes, or reservations");
                    await Capture("artifacts/m3-baking.png");
                    var baker = _world.People.First(v => v.Task == Work.Baking);
                    var bakerView = _people[baker.Id];
                    Check(bakerView.Peel.Visible && !bakerView.Axe.Visible && !bakerView.Hammer.Visible, "Baker tool did not match job");
                    var pose = bakerView.Arm.Rotation;
                    for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    Check(bakerView.Arm.Rotation == pose && _world.SaveJson() == inFlight, "Paused pose or simulation changed");
                    var previousFocus = _focus; float previousSize = _camera.Size;
                    _focus = bakerView.Body.Position; _camera.Size = 12; UpdateCamera();
                    await Capture("artifacts/f03-baker.png");
                    _focus = previousFocus; _camera.Size = previousSize; UpdateCamera();
                    await Press(Key.Space); reloaded = true;
                }
            }
            Check(_world.CanCelebrate && harvested && reloaded && _world.Food.EatenBerries > 24, "Rendered food economy did not qualify for supper");
            await Press(Key.Space); _noticeUntil = 0; await Capture("artifacts/m3-ready.png");
            await UiClick(_supperButton); Check(_world.Food.Celebrating && _world.Food.SupperBread == 16, "Supper button failed");
            await Press(Key.Space); bool gathering = false;
            for (int frame = 0; frame < 6000 && !_world.Food.SupperComplete; frame++)
            {
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); _world.Validate();
                if (!gathering && _world.People.All(v => v.Task == Work.Supper)) { await Capture("artifacts/m3-gathering.png"); gathering = true; }
            }
            Check(_world.Food.SupperComplete && gathering, "Villagers did not gather and finish supper");
            await Press(Key.Space); await Press(Key.F5); await UiClick(_resetButton); await Press(Key.F9);
            Check(_paused && _world.Food.SupperComplete && _world.Housed == 8, "Completed scenario not restored");
            _noticeUntil = 0; await Capture("artifacts/m3-complete.png");
            GD.Print("SMOKE PASS: all building types, food staffing, construction priorities, save/load buttons and F5/F9, invalid-save recovery, exact active-batch restoration, regrowth/harvest/baking/meals, supper gathering, and completed-save restoration.");
            await SmokeWoodland();
            await SmokeSawmill();
            await CheckAudio();
            GetTree().Quit();
        }
        catch (Exception e) { GD.PrintErr("SMOKE FAIL: " + e); GetTree().Quit(1); }
    }
}
