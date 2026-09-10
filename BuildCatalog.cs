using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;

public partial class Game
{
    private readonly VBoxContainer[] _buildSections = new VBoxContainer[3];
    private readonly Button[] _buildSectionButtons = new Button[3];
    private readonly Dictionary<BuildingKind, Label> _cardCosts = new();
    private HBoxContainer _buildNavigation = null!;
    private VBoxContainer _buildFooter = null!;
    private int _buildSection;

    private static string BuildingPurpose(BuildingKind kind) => kind switch
    {
        BuildingKind.Cottage or BuildingKind.Lodge => $"A home for {Buildings.Get(kind).Beds}",
        BuildingKind.ForagerHut => "Gather berries",
        BuildingKind.Farm => "Grow grain for bread",
        BuildingKind.VegetableGarden => "Grow ready-to-eat food",
        BuildingKind.Bakery => "Turn grain into bread",
        BuildingKind.Sawmill => "Turn logs into planks",
        BuildingKind.Stockpile => "Keep logs close to work",
        BuildingKind.Bridge => "Cross a water tile",
        _ => "A place to gather"
    };
    private static string BuildingStaff(BuildingKind kind)
    {
        var building = Buildings.Get(kind);
        return building.Worker == null ? "No staff" : building.Slots == 0 ? "Shared haulers" :
            $"{building.Slots} {building.Worker.ToString()!.ToLowerInvariant()} slot{(building.Slots == 1 ? "" : "s")}";
    }
    private void MakeBuildNavigation(VBoxContainer parent)
    {
        _buildNavigation = new(); parent.AddChild(_buildNavigation);
        for (int i = 0; i < 3; i++)
        {
            int section = i;
            var button = Button(new[] { "Place", "Landscape", "Existing" }[i], () => SelectBuildSection(section));
            button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; button.AddThemeFontSizeOverride("font_size", 14);
            _buildNavigation.AddChild(button); _buildSectionButtons[i] = button;
        }
    }
    private void SelectBuildSection(int section)
    {
        _buildSection = section;
        for (int i = 0; i < 3; i++)
        {
            _buildSections[i].Visible = i == section;
            _buildSectionButtons[i].Modulate = i == section ? _cream : Colors.White;
        }
        _buildingFilter.Visible = section != 1;
        _drawerPages[1].ScrollVertical = 0;
    }
    private void MakeBuildFooter(VBoxContainer parent)
    {
        _buildFooter = new(); parent.AddChild(_buildFooter);
        _buildDescription = Text("", 13, true); _buildDescription.MaxLinesVisible = 3;
        _buildDescription.MouseFilter = Control.MouseFilterEnum.Pass;
        _buildFooter.AddChild(_buildDescription);
        _buildButton = Button("Cancel preview [Esc]", () => { _placing = false; RefreshGhost(); });
        _buildFooter.AddChild(_buildButton);
    }
    private void UpdateBuildCatalog()
    {
        bool buildOpen = _drawer.Visible && _tabs.CurrentTab == 1;
        _buildNavigation.Visible = buildOpen;
        _buildFooter.Visible = buildOpen && _placing;
        _buildDescription.TooltipText = _buildDescription.Text;
        foreach (var (kind, cost) in _cardCosts)
            cost.Text = (_world.Creative ? "Free · instant" : Buildings.Get(kind).CostText) + " · " + BuildingStaff(kind);
    }
    private void MakeBuildingCard(VBoxContainer column, BuildingKind kind)
    {
        var button = Button("", () => BeginPlacement(kind));
        button.CustomMinimumSize = new(0, 112); button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        button.TooltipText = BuildingDescription(kind); column.AddChild(button); _kindButtons[kind] = button;
        var content = new HBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        button.AddChild(content); content.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        content.OffsetLeft = 6; content.OffsetRight = -6; content.OffsetTop = 8; content.OffsetBottom = -8;
        var thumbnail = new TextureRect { Texture = BuildingThumbnail(kind), CustomMinimumSize = new(76, 76),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore };
        content.AddChild(thumbnail);
        var words = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, MouseFilter = Control.MouseFilterEnum.Ignore };
        content.AddChild(words);
        words.AddChild(Text(BuildingName(kind), 16, true));
        words.AddChild(Text(BuildingPurpose(kind), 13, true));
        var cost = Text("", 12, true); cost.Modulate = new("b8c9b6"); words.AddChild(cost); _cardCosts[kind] = cost;
    }
    private Texture2D BuildingThumbnail(BuildingKind kind)
    {
        // Render each current model once; thumbnails do not run a second simulation.
        var viewport = new SubViewport { Size = new(152, 128), OwnWorld3D = true, TransparentBg = true,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Once };
        AddChild(viewport);
        var root = new Node3D(); viewport.AddChild(root);
        var env = new Godot.Environment { AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new("dce5df"), AmbientLightEnergy = .65f };
        root.AddChild(new WorldEnvironment { Environment = env });
        root.AddChild(new DirectionalLight3D { RotationDegrees = new(-50, -30, 0), LightEnergy = .8f, ShadowEnabled = true });
        var model = new Node3D(); root.AddChild(model); MakeBuilding(model, new Cottage { Kind = kind }, 3);
        HideModelLabels(model);
        var camera = new Camera3D { Projection = Camera3D.ProjectionType.Orthogonal, Size = 4.7f, Position = new(5, 5, 7), Current = true };
        root.AddChild(camera); camera.LookAt(new(0, 1.0f, 0));
        return viewport.GetTexture();
    }
    private static void HideModelLabels(Node node)
    {
        if (node is Label3D label) label.Hide();
        foreach (var child in node.GetChildren()) HideModelLabels(child);
    }
}
