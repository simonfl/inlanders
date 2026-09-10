using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private VBoxContainer _visitorPanel = null!;
    private Label _visitorText = null!;
    private Button _visitorAccept = null!, _visitorDecline = null!, _visitorPlant = null!;
    private Node3D _visitorStand = null!;
    private void MakeVisitorUi(VBoxContainer column)
    {
        _visitorPanel=new VBoxContainer(); column.AddChild(_visitorPanel);
        _visitorPanel.AddChild(Text("A GARDENER'S VISIT",18));
        _visitorText=Text("",14,true); _visitorPanel.AddChild(_visitorText);
        _visitorAccept=Button("Trade 8 berries",()=> { if(_world.AcceptGardener()) { UiCue(Cue.Complete); UpdateVisitorUi(); } });
        _visitorPanel.AddChild(_visitorAccept);
        _visitorDecline=Button("Decline this visit",()=> { _world.DeclineGardener(); UpdateVisitorUi(); }); _visitorPanel.AddChild(_visitorDecline);
        _visitorDecline.TooltipText="The gardener leaves for good in this settlement. No penalty.";
        _visitorPlant=Button("Plant sunflowers",()=> { ToggleDrawer(1); _decorationChoice.Select((int)DecorationKind.Sunflowers); BeginDecorating(false); });
        _visitorPanel.AddChild(_visitorPlant);
    }
    private void UpdateVisitorUi()
    {
        if(_visitorPanel==null) return;
        var state=_world.Gardener;
        _visitorPanel.Visible=state is VisitorState.Pending or VisitorState.Accepted;
        _visitorAccept.Visible=_visitorDecline.Visible=state==VisitorState.Pending;
        _visitorPlant.Visible=state==VisitorState.Accepted;
        _visitorAccept.Disabled=_world.GardenerTradeProblem()!=null; _visitorDecline.Disabled=_world.Food.Celebrating;
        _visitorAccept.TooltipText=_world.GardenerTradeProblem() ?? "Pay once to add freely placeable sunflowers to your decoration palette.";
        _visitorText.Text=state==VisitorState.Accepted ?
            "The gardener shared sunflower seeds. Your decorative palette now includes tall golden sunflower patches, free to place and remove." :
            "A gardener offers sunflower seeds for 8 stored berries. Unlock a decorative sunflower patch you can place as often as you like. It produces no food.\n\n" +
            $"Berries: {_world.Food.Berries}/8 · food after trade: {Math.Max(0,_world.FoodAfterGardenerTrade)} ({Math.Max(0,_world.FoodAfterGardenerTrade)/(float)_world.Population:F1} village meals).\n" +
            (_world.FoodAfterGardenerTrade < _world.Population*2 ? "This would leave less than two meals in storage.\n" : "") +
            "No deadline. Keep this offer open or decline; your other buildings and decorations are unaffected.";
    }
    private void RenderVisitor()
    {
        if(_visitorStand==null)
        {
            _visitorStand=new Node3D { Position=new(-3.62f,0,3.58f) }; AddChild(_visitorStand);
            Box(_visitorStand,new(0,.34f,0),new(.06f,.68f,.06f),_wood);
            Box(_visitorStand,new(0,.65f,0),new(.36f,.27f,.05f),new("b9a36d"));
            var flower=new Node3D { Scale=Vector3.One*.28f, Position=new(0,.65f,.07f) }; _visitorStand.AddChild(flower); MakeSunflowers(flower);
            _visitorStand.AddChild(new Label3D { Text="GARDENER · GOALS [G]",Position=new(0,1.45f,0),FontSize=26,PixelSize=.008f,
                Billboard=BaseMaterial3D.BillboardModeEnum.Enabled,Modulate=_cream,OutlineSize=4 });
        }
        _visitorStand.Visible=_world.Gardener==VisitorState.Pending;
    }
    private void MakeSunflowers(Node3D root)
    {
        Box(root,new(0,.025f,0),new(.78f,.05f,.72f),new("796447"));
        for(int i=0;i<3;i++)
        {
            float x=(i-1)*.24f,z=i%2*.23f-.1f,height=.68f+i%2*.19f;
            Cylinder(root,new(x,height/2,z),.025f,height,new("5e8050"));
            var leaf=Mesh(root,new SphereMesh { Radius=.13f,Height=.12f,RadialSegments=6,Rings=3 },new(x+.1f,height*.55f,z),new("799654"));
            leaf.Scale=new(1,.5f,1);
            var head=new Node3D { Position=new(x,height,z),RotationDegrees=new(25,0,0) }; root.AddChild(head);
            for(int p=0;p<8;p++)
            {
                float a=p*Mathf.Tau/8;
                Mesh(head,new SphereMesh { Radius=.075f,Height=.14f,RadialSegments=5,Rings=3 },new(MathF.Cos(a)*.13f,MathF.Sin(a)*.13f,0),new("e6b849"));
            }
            var center=Cylinder(head,Vector3.Zero,.095f,.07f,new("725239")); center.RotationDegrees=new(90,0,0);
        }
    }
}
