using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task SmokeWoodland()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        await UiClick(_resetButton);
        Check(_paused, "Woodland setup must be paused");
        _savePath = "artifacts/f02-rendered-save.json";
        await UiClick(_plantTreeButton);
        Check(_placing && _plantingTrees, "Tree planting button failed");
        await Click(_camera.UnprojectPosition(new(-3,0,-1)));
        Check(_world.Trees.Count == 6, "Planted over a living tree");
        await Click(_camera.UnprojectPosition(new(0,0,0)));
        var tree = _world.Trees.Last();
        Check(_world.Trees.Count == 7 && tree.NeedsPlanting && _placing, "Tree marking or repeat placement failed");
        await Press(Key.Escape); Check(!_placing, "Tree placement escape failed");
        await Press(Key.F5); await Press(Key.F9);
        Check(_world.Trees.Last().NeedsPlanting, "Pending planting not restored");
        tree = _world.Trees.Last();
        for (int i = 0; i < 1500 && tree.NeedsPlanting; i++) { _world.Tick(0.1f); _world.Validate(); }
        Check(!tree.NeedsPlanting, "Rendered planting never finished");
        foreach (var v in _world.People) _world.Assign(v.Id, Role.Unassigned);
        for (int i = 0; i < 900; i++) _world.Tick(0.1f);
        await Press(Key.F5); string saved = File.ReadAllText(_savePath); await Press(Key.F9);
        Check(_world.SaveJson() == saved, "Growing tree not restored exactly");
        tree = _world.Trees.Last();
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        Check(_trees[tree.Id].Top.Visible && _trees[tree.Id].Top.Scale.X < 0.7f, "Sapling visual missing");
        _noticeUntil = 0; await Capture("artifacts/f02-growing.png");
        for (int i = 0; i < 910; i++) _world.Tick(0.1f);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        Check(tree.Logs == 8 && _trees[tree.Id].Top.Scale.X > 0.89f, "Mature tree visual missing");
        await Capture("artifacts/f02-mature.png");
        _world.Assign(0, Role.Logger);
        for (int i = 0; i < 10000 && !(tree.Felled && tree.Logs == 0); i++) { _world.Tick(0.1f); _world.Validate(); }
        Check(tree.Felled && tree.Logs == 0, "Rendered tree not harvested");
        await Press(Key.T); Check(_placing && _plantingTrees, "Tree keyboard shortcut failed");
        await Click(_camera.UnprojectPosition(new(0,0,0)));
        Check(tree.NeedsPlanting && !tree.Felled && _world.Trees.Count == 7, "Stump replanting failed");
        await Press(Key.Escape);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        Check(!_trees[tree.Id].Top.Visible && _trees[tree.Id].Stage == 0, "Replanting marker not restored");
        _noticeUntil = 0; await Capture("artifacts/f02-replant.png");
        GD.Print("SMOKE PASS: planting button/T/Esc, illegal sites, repeat marking, planting and growth save/load, sapling/mature visuals, logging, and stump replanting.");
    }
}
