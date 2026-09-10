using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunMapSmoke()
    {
        string stem = Path.Combine(Path.GetTempPath(), "inlanders-map-" + Guid.NewGuid());
        _savePath = stem + "-original.json"; _largeSavePath = stem + "-large.json";
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            async Task Frames() { for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
            _paused = true; string original = _world.SaveJson();
            await OpenMenu(3);
            var open = _drawerPages[3].FindChildren("*", "Button", true, false).Cast<Button>().Single(b => b.Text == "Explore larger map");
            await UiClick(open); await Frames();
            Check(_world.Map.Width == 32 && _landscape.HasNode("TerrainSurface"), "Expanded terrain not rendered");
            Check(File.ReadAllText(_savePath) == original, "Opening large map lost original village");
            _noticeUntil = 0; await Capture("artifacts/f12-overview.png");
            foreach (var size in new[] { new Vector2I(1440, 900), new(960, 640) })
            {
                GetWindow().Size = size; await Frames(); await Press(Key.Home); await Frames();
                foreach (var cell in _world.Map.Land)
                {
                    var point = _camera.UnprojectPosition(OnGround(cell.X, cell.Z));
                    Check(point.X >= 0 && point.X <= size.X && point.Y >= 82 && point.Y <= size.Y - 76, "Overview clips the map behind bars");
                }
                await Capture($"artifacts/f12-overview-{size.X}.png");
            }
            _focus = new(100, 0, -100); UpdateCamera();
            Check(_focus.X == 15 && _focus.Z == -16, "Camera pan still uses old bounds");
            await Press(Key.Home); _camera.Size = 23; _focus = new(9, 0, 3); UpdateCamera(); await Frames();
            await UiClick(_kindButtons[BuildingKind.Cottage]); CloseDrawer(); await Frames();
            var target = _camera.UnprojectPosition(new(9, 0, 3));
            Input.ParseInputEvent(new InputEventMouseMotion { Position = target, GlobalPosition = target }); await Frames();
            Check(_ghostValid && _ghost.Visible, "Distant placement preview failed");
            await Capture("artifacts/f12-distant-preview.png"); await Click(target);
            Check(_world.Cottages.Count == 1, "Distant building click failed");
            _speed = 6; _paused = false;
            for (int i = 0; i < 120; i++) { await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); _world.Validate(); }
            _paused = true;
            Check(_world.Food.Time > 0, "Expanded map did not tick at fast speed");
            for (int i = 0; i < 5000 && !_world.Cottages[0].Complete; i++) _world.Tick(0.1f);
            Check(_world.Cottages[0].Complete, "Distant plan did not finish");
            _focus = new(7,0,3); _camera.Size=18; UpdateCamera(); await Frames();
            BeginPlacement(BuildingKind.Bridge); CloseDrawer(); _rotated=false;
            var crossing=_camera.UnprojectPosition(new(7,0,0));
            Input.ParseInputEvent(new InputEventMouseMotion { Position=crossing, GlobalPosition=crossing }); await Frames();
            Check(!_ghostValid,"Bridge preview accepted wrong banks");
            await Press(Key.R); await Frames();
            Check(_ghostValid && _ghostModel.Position.X==7,"Bridge preview did not turn across stream");
            await Capture("artifacts/f12b-bridge-preview.png"); await Click(crossing); await Press(Key.Escape);
            var bridge=_world.Cottages.Single(c=>c.Kind==BuildingKind.Bridge);
            for(int i=0;i<10000 && !bridge.Complete;i++) { _world.Tick(.1f); if(i%100==0) await Frames(); }
            Check(bridge.Complete,"Rendered bridge construction stalled"); await Frames();
            Check(_cottages[bridge.Id].Body.Position.X==7,"Bridge mesh offset from water");
            await Capture("artifacts/f12b-bridge-complete.png");
            await Press(Key.F5); string saved = _world.SaveJson();
            _world.Tick(1); await Press(Key.F9); await Frames();
            Check(_world.SaveJson() == saved, "Large-map F5/F9 failed");
            OpenOriginalMap(); await Frames();
            Check(_world.Map.OriginalOutline && _world.SaveJson() == original && !_landscape.GetChildren().OfType<MultiMeshInstance3D>().Any(), "Original map not restored");
            OpenLargeMap(); await Frames(); Check(_world.SaveJson() == saved, "Large map did not resume");
            GD.Print("SMOKE PASS: large-map entry, instanced terrain, overview at 1440/960, pan limits, distant building, water and rotated bridge preview/construction, 6x simulation, separate saves, and map switching.");
            await CheckTerrainUi();
            for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            GetTree().Quit();
        }
        catch (Exception e) { GD.PrintErr("MAP SMOKE FAIL: " + e); GetTree().Quit(1); }
        finally
        {
            foreach (string path in new[] { _savePath, _largeSavePath })
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(path + suffix)) File.Delete(path + suffix);
        }
    }
}
