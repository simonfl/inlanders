using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private bool _decorating, _removeDecoration;
    private DecorationKind _decorationKind;
    private OptionButton _decorationChoice = null!;
    private Button _decorateButton = null!, _eraseDecorationButton = null!;
    private Node3D _decorationView = null!;
    private World? _decorationWorld;
    private int _decorationRevision = -1;
    private static string DecorationName(DecorationKind kind) => kind == DecorationKind.OrnamentalTree ? "Ornamental tree" : kind.ToString();
    private string DecorationDescription => !_removeDecoration && _decorationKind == DecorationKind.Sunflowers && !_world.SunflowersUnlocked ? "SUNFLOWERS\n" + _world.SunflowerLockReason + "\nAll ordinary decorations remain free." : _removeDecoration ? "REMOVE DECORATIONS\nClick a decoration to remove it instantly. No resources are spent or recovered." :
        $"{DecorationName(_decorationKind).ToUpperInvariant()}\nFree, instant landscaping. Click to place repeatedly; R rotates. " +
        (_decorationKind == DecorationKind.Pebbles ? "Walkable ground cover with no speed bonus; paths can cross it." :
        "Occupies one tile. Villagers walk around it; entrances and existing routes stay accessible. Ornamental trees provide no timber.") +
        "\nUse Remove decorations before building on decorated ground.";

    private void MakeDecorationMenu(VBoxContainer column)
    {
        column.AddChild(Text("DECORATE · FREE",12));
        _decorationChoice=DirectoryFilter(column,"Choose a decorative object or walkable ground cover.");
        foreach(var kind in Enum.GetValues<DecorationKind>()) _decorationChoice.AddItem(DecorationName(kind),(int)kind);
        _decorationChoice.ItemSelected += _ => { if(_decorating && !_removeDecoration) BeginDecorating(false); };
        _decorateButton=Button("Place decoration",()=>BeginDecorating(false)); column.AddChild(_decorateButton);
        _eraseDecorationButton=Button("Remove decorations",()=>BeginDecorating(true)); column.AddChild(_eraseDecorationButton);
    }
    private void BeginDecorating(bool remove)
    {
        ClearSelection(); _woodlandTool=0; _pathTool=0; _pathStroke=false; _lastPathCell=null; _clearingTrees=_plantingTrees=false;
        _decorating=_placing=true; _removeDecoration=remove; _decorationKind=(DecorationKind)_decorationChoice.GetSelectedId();
        RefreshGhost();
    }
    private void EditDecoration(Cell cell)
    {
        bool changed=_removeDecoration?_world.RemoveDecoration(cell):_world.PlaceDecoration(cell,_decorationKind,(_rotation%2!=0));
        UiCue(changed?Cue.Place:Cue.Reject); RefreshGhost();
    }
    private void RefreshDecorationGhost()
    {
        string key="decoration:"+(_removeDecoration?"remove":_decorationKind.ToString());
        if(_ghostModelKey!=key)
        {
            Clear(_ghostModel); _previewMaterials.Clear(); _ghostModelKey=key;
            if(!_removeDecoration) MakeDecoration(_ghostModel,_decorationKind);
            PreparePreview(_ghostModel);
        }
        var color=_ghostValid?new Color("aed2a0"):new Color("e38673");
        foreach(var material in _previewMaterials) material.AlbedoColor=new(color.R,color.G,color.B,.48f);
        _ghostModel.Position=OnGround(_hover.X,_hover.Z,.05f); _ghostModel.Basis = _decorationKind == DecorationKind.OrnamentalTree ? new Basis(Vector3.Up,(_rotation%2!=0) ? Mathf.Pi/2 : 0) : GroundBasis(_hover.X,_hover.Z,(_rotation%2!=0));
        Clear(_ghostCells);
        if(_removeDecoration) ClearingCross(_ghostCells,OnGround(_hover.X,_hover.Z,.1f),color,.9f);
        else GroundPatch(_ghostCells,_hover.X,_hover.Z,.96f,.96f,color.Darkened(.15f));
    }
    private void RenderDecorations()
    {
        if(_decorationView==null) { _decorationView=new(); AddChild(_decorationView); }
        if(_decorationWorld==_world && _decorationRevision==_world.DecorationRevision) return;
        Clear(_decorationView); _decorationWorld=_world; _decorationRevision=_world.DecorationRevision;
        foreach(var decoration in _world.Decorations)
        {
            var body=new Node3D { Position=OnGround(decoration.Cell.X,decoration.Cell.Z), Basis = decoration.Kind == DecorationKind.OrnamentalTree ? new Basis(Vector3.Up,decoration.Rotated ? Mathf.Pi/2 : 0) : GroundBasis(decoration.Cell.X,decoration.Cell.Z,decoration.Rotated) };
            _decorationView.AddChild(body);
            if (decoration.Kind == DecorationKind.Pebbles)
            {
                body.Transform = Transform3D.Identity;
                GroundPatch(body,decoration.Cell.X,decoration.Cell.Z,.96f,.96f,new("c0b9a1"),.018f);
                for(int i=0;i<9;i++)
                {
                    float dx=(i%3-1)*.29f,dz=(i/3-1)*.29f;
                    float x=decoration.Cell.X+(decoration.Rotated ? -dz : dx),z=decoration.Cell.Z+(decoration.Rotated ? dx : dz);
                    var stone=Box(body,OnGround(x,z,.035f),new(.17f,.025f,.12f),new("dad2b9")); stone.Basis=GroundBasis(x,z,decoration.Rotated);
                }
                BatchStaticGeometry(body);
            }
            else MakeDecoration(body,decoration.Kind);
        }
    }
    private void MakeDecoration(Node3D root,DecorationKind kind)
    {
        MakeDecorationPieces(root,kind);
        BatchStaticGeometry(root);
    }
    private void MakeDecorationPieces(Node3D root,DecorationKind kind)
    {
        if(kind==DecorationKind.Sunflowers) { MakeSunflowers(root); return; }
        if(kind==DecorationKind.Pebbles)
        {
            Box(root,new(0,.005f,0),new(.96f,.012f,.96f),new("c0b9a1"));
            for(int i=0;i<9;i++) Box(root,new((i%3-1)*.29f,.016f,(i/3-1)*.29f),new(.17f,.015f,.12f),new(i%2==0?"d6cfb8":"a8a48f"));
        }
        else if(kind==DecorationKind.Fence)
        {
            foreach(float x in new[]{-.42f,0,.42f}) Box(root,new(x,.31f,0),new(.09f,.62f,.1f),new("bfa37a"));
            foreach(float y in new[]{.22f,.48f}) Box(root,new(0,y,0),new(.96f,.085f,.07f),new("d1bb91"));
        }
        else if(kind==DecorationKind.OrnamentalTree)
        {
            Cylinder(root,new(0,.55f,0),.085f,1.1f,_wood,.06f);
            Mesh(root,new SphereMesh { Radius=.45f,Height=.9f,RadialSegments=7,Rings=4 },new(0,1.13f,0),new("bf9ba3"));
            Mesh(root,new SphereMesh { Radius=.3f,Height=.6f,RadialSegments=7,Rings=3 },new(.2f,.94f,.13f),new("d9b3b4"));
        }
        else if(kind==DecorationKind.Shrub)
        {
            for(int i=0;i<3;i++) Mesh(root,new SphereMesh { Radius=.25f,Height=.48f,RadialSegments=7,Rings=3 },new((i-1)*.21f,.25f,i%2*.12f),new(i==1?"88a56c":"68895e"));
        }
        else
        {
            Box(root,new(0,.025f,0),new(.82f,.05f,.7f),new("746447"));
            for(int i=0;i<6;i++)
            {
                var at=new Vector3((i%3-1)*.25f,.15f,(i/3-.5f)*.3f);
                Cylinder(root,at,.015f,.26f,new("628154"));
                for(int petal=0;petal<4;petal++)
                {
                    float a=petal*Mathf.Pi/2;
                    Mesh(root,new SphereMesh { Radius=.055f,Height=.08f,RadialSegments=5,Rings=3 },at+new Vector3(MathF.Cos(a)*.055f,.14f,MathF.Sin(a)*.055f),new(i%2==0?"e6c66d":"cc8c9c"));
                }
                Cylinder(root,at+new Vector3(0,.18f,0),.03f,.035f,new("f2dda0"));
            }
        }
    }
}
