using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckPlacementPreview()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Move(Vector2 point)
        {
            Input.ParseInputEvent(new InputEventMouseMotion { Position = point, GlobalPosition = point });
            for (int i = 0; i < 3; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        string saved = _world.SaveJson();
        _rotated = false;
        foreach (var kind in Enum.GetValues<BuildingKind>())
        {
            if (kind is BuildingKind.Bridge or BuildingKind.FishingDock or BuildingKind.Quarry) continue; // Environmental prerequisites have dedicated rendered checks.
            BeginPlacement(kind);
            await Move(_camera.UnprojectPosition(new(3, 0, 0)));
            Check(_ghost.Visible && _ghostValid && _previewMaterials.Count > 0, $"Missing legal {kind} preview: visible={_ghost.Visible}, hover={_hover}, reason={_placementProblem}, pointer={GetViewport().GetMousePosition()}");
            Check(_buildDescription.Text.Contains(BuildingDescription(kind)), "Building description missing");
            await Press(Key.R);
            Check(_rotated && _ghostModel.RotationDegrees.Y == 90 && _ghostModel.Position.X == 2.5f, "Rotation did not match final building");
            await Press(Key.R);
        }
        Check(_buildDescription.Text.Contains("builders wait"), "Material shortage did not explain planning");
        await Capture("artifacts/f21b-preview.png");
        var size = GetWindow().Size;
        GetWindow().Size = new(960, 640);
        for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await Move(_camera.UnprojectPosition(new(3, 0, 0)));
        Check(_ghost.Visible && _ghostValid && _hintPanel.GetGlobalRect().End.Y <= 640, "Narrow placement feedback failed");
        await Capture("artifacts/f21b-preview-960.png");
        GetWindow().Size = size;
        for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await Move(_camera.UnprojectPosition(new(-3, 0, -4)));
        Check(!_ghostValid && _hint.Text.Contains("tree"), "Obstruction not explained");
        Check(_previewMaterials[0].AlbedoColor.R > _previewMaterials[0].AlbedoColor.G, "Blocked preview not red");
        await Capture("artifacts/f21b-blocked.png");
        await Move(_topBar.GetGlobalRect().GetCenter());
        Check(!_ghost.Visible, "Preview visible over UI");
        Check(_world.SaveJson() == saved, "Preview changed simulation");
        await Press(Key.T);
        await Move(_camera.UnprojectPosition(new(3, 0, 0)));
        Check(_ghostValid && _ghostModelKey == "tree", "Tree preview missing");
        await Click(_camera.UnprojectPosition(new(3, 0, 0)));
        Check(_placing && _plantingTrees && !_ghostValid, "Tree planting did not stay active or reject duplicate");
        await Move(_camera.UnprojectPosition(new(5, 0, 0)));
        Check(_ghostValid, "Repeat planting preview failed");
        await Press(Key.Escape);
        _world = World.LoadJson(saved); CreateActors();
    }
}
