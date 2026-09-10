using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunArtSmoke()
    {
        try
        {
            void Check(bool value, string message) { if (!value) throw new Exception(message); }
            void HideLabels(Node node)
            {
                if (node is Label3D label) label.Hide();
                foreach (var child in node.GetChildren()) HideLabels(child);
            }
            async Task Frames()
            {
                RenderActors(.1f); RenderFoodViews(); HideLabels(this);
                for (int i = 0; i < 4; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            }
            _paused = true; _world = World.NewCreative();
            Cottage Place(Cell at, BuildingKind kind) => _world.Place(at, false, kind) ?? throw new Exception($"Art scene placement: {kind} {at}: {_world.PlacementProblem(at, false, kind)}");
            Place(new(0, 0), BuildingKind.Cottage);
            var bakery = Place(new(4, 0), BuildingKind.Bakery);
            var mill = Place(new(6, -3), BuildingKind.Sawmill);
            Place(new(3, -3), BuildingKind.Farm);
            Place(new(0, 6), BuildingKind.Cottage);
            Place(new(4, 6), BuildingKind.Cottage);
            Place(new(6, 3), BuildingKind.VegetableGarden);
            for (int x = -2; x <= 6; x++) _world.SetPath(new(x, 1), true);
            for (int z = -2; z <= 7; z++) _world.SetPath(new(2, z), true);
            for (int x = 0; x <= 5; x++) _world.SetPath(new(x, 7), true);
            _world.Assign(4, Role.Farmer); _world.Assign(5, Role.Baker); _world.Assign(6, Role.Sawyer);
            CreateActors(); CloseManagementUi(); _hud.Hide(); _watchRoot.Hide();
            _focus = new(1.5f, 0, 1); _camera.Size = 23; _angle = .72f; UpdateCamera();
            GetWindow().Size = new(1440, 900); await Frames();
            Check(!_cottages[bakery.Id].Body.GetNode<Node3D>("OvenGlow").Visible, "Empty bakery glows");
            await Capture("artifacts/f23a-empty-1440.png");
            bool baking = false, sawing = false, bread = false, planks = false;
            for (int i = 0; i < 4000 && !(baking && sawing && bread && planks); i++)
            {
                _world.Tick(.1f);
                if (i % 50 == 0) _world.Validate();
                if (!sawing && mill.SawProgress > .1f)
                {
                    await Frames(); var blade = _cottages[mill.Id].Body.GetNode<Node3D>("SawBlade");
                    var pose = blade.Position; await Frames(); Check(blade.Position == pose, "Paused saw moved");
                    _world.Tick(.1f); await Frames(); Check(blade.Position != pose, "Working saw did not move");
                    await Capture("artifacts/f23a-sawing.png"); sawing = true;
                    for (int frame = 0; frame < 20; frame++)
                    {
                        _world.Tick(.1f); await Frames();
                        await Capture($"artifacts/f23a-motion-{frame:D2}.png");
                    }
                }
                if (!baking && bakery.BakeProgress > .1f)
                {
                    await Frames(); Check(_cottages[bakery.Id].Body.GetNode<Node3D>("OvenGlow").Visible, "Working bakery has no glow");
                    await Capture("artifacts/f23a-baking.png"); baking = true;
                }
                if (!bread && bakery.OutputBread == 4)
                {
                    await Frames(); Check(_cottages[bakery.Id].Body.GetChildren().Count(n => n.Name.ToString().StartsWith("BreadLoaf")) == 4, "Bread display disagrees with buffer");
                    Check(!_cottages[bakery.Id].Body.GetNode<Node3D>("OvenGlow").Visible, "Idle bakery still glows");
                    await Capture("artifacts/f23a-bread.png"); bread = true;
                }
                if (!planks && mill.OutputPlanks == 4)
                {
                    await Frames(); Check(_cottages[mill.Id].Body.GetChildren().Count(n => n.Name.ToString().StartsWith("SawPlank")) == 4, "Plank display disagrees with buffer");
                    await Capture("artifacts/f23a-planks.png"); planks = true;
                }
            }
            Check(baking && sawing && bread && planks, "Art scene did not exercise all workshop states");
            _world.Validate();
            System.IO.Directory.CreateDirectory("artifacts");
            System.IO.File.WriteAllText("artifacts/f23a-village.json", _world.SaveJson());
            foreach (var size in new[] { new Vector2I(1440, 900), new(960, 640) })
            {
                GetWindow().Size = size;
                for (int angle = 0; angle < 4; angle++)
                {
                    _angle = .72f + angle * Mathf.Pi / 2; UpdateCamera(); await Frames();
                    await Capture($"artifacts/f23a-village-{size.X}-{angle}.png");
                }
            }
            GetWindow().Size = new(1440, 900); _angle = .72f; UpdateCamera(); await Frames();
            await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
            var gray = GetViewport().GetTexture().GetImage();
            for (int y = 0; y < gray.GetHeight(); y++) for (int x = 0; x < gray.GetWidth(); x++)
            {
                var color = gray.GetPixel(x, y); float value = color.R * .2126f + color.G * .7152f + color.B * .0722f;
                gray.SetPixel(x, y, new(value, value, value, color.A));
            }
            Check(gray.SavePng("artifacts/f23a-grayscale.png") == Error.Ok, "Grayscale capture failed");
            // A separate presentation sheet checks all construction stages and rotation.
            _dynamic.Hide(); _landscape.Hide(); _pathView.Hide(); var sheet = new Node3D(); AddChild(sheet);
            foreach (var (kind, row) in new[] { (BuildingKind.Cottage, 0), (BuildingKind.Bakery, 1), (BuildingKind.Sawmill, 2) })
                for (int stage = 0; stage < 4; stage++)
                {
                    var body = new Node3D { Position = new(stage * 4 - 6, 0, row * 4 - 4), RotationDegrees = new(0, stage == 3 ? 90 : 0, 0) }; sheet.AddChild(body);
                    MakeBuilding(body, new Cottage { Kind = kind }, stage);
                }
            Box(sheet, new(0, -.12f, 0), new(18, .15f, 14), new("777f62"));
            _focus = Vector3.Zero; _camera.Size = 23; UpdateCamera(); HideLabels(sheet); await Frames();
            await Capture("artifacts/f23a-construction.png");
            GD.Print("PASS: art scene, real bakery/sawmill buffers, working/paused saw, oven state, four camera directions at 960/1440 and construction sheet. Visual appeal requires human review.");
            GetTree().Quit();
        }
        catch (Exception e) { GD.PushError(e.ToString()); GetTree().Quit(1); }
    }
}
