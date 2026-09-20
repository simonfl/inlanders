using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private VBoxContainer _yardChoices=null!;
    private readonly Button[] _yardSides=new Button[4];
    private Button _yardApply=null!,_yardFurnish=null!;
    private Label _yardPreviewInfo=null!;
    private int _yardPreviewSide=-1,_yardPreviewHome=-1;
    private Node3D? _yardPreviewGround;
    private string _yardPreviewKey="";
    private Vector3 _yardPreviousFocus;
    private float _yardPreviousAngle,_yardPreviousZoom;
    private bool _yardCameraSaved;
    private void FrameYardPreview()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_yardPreviewHome);if(home==null)return;
        // Look toward the house from the selected ground, so its roof sits behind the proposal.
        var direction=World.RotateOffset(home.Cell,_yardPreviewSide==1?-2:_yardPreviewSide==3?2:0,_yardPreviewSide==0?2:_yardPreviewSide==2?-2:0,home.Rotation);
        _angle=Mathf.Atan2(direction.X-home.Cell.X,direction.Z-home.Cell.Z)+.22f;
        _followPerson=false;_watchOrbit=false;_camera.Size=12;
        _focus=OnGround((home.Cell.X+direction.X)*.5f,(home.Cell.Z+direction.Z)*.5f);UpdateCamera();
        _workCardRight=true;
        var target=new Vector2((_hud.Size.X-346)/2,_hud.Size.Y*.51f);
        _focus+=CameraDragPoint(_hud.Size*.5f)-CameraDragPoint(target);UpdateCamera();
    }
    private void FinishYardPreview()
    {
        _yardCameraSaved=false;StopYardPreview();
    }
    private void StopYardPreview()
    {
        if(_yardCameraSaved){_focus=_yardPreviousFocus;_angle=_yardPreviousAngle;_camera.Size=_yardPreviousZoom;_yardCameraSaved=false;UpdateCamera();}
        _yardPreviewSide=_yardPreviewHome=-1;_yardPreviewKey="";
        if(_yardChoices!=null)_yardChoices.Hide();
        if(_yardPreviewGround!=null && GodotObject.IsInstanceValid(_yardPreviewGround))_yardPreviewGround.Hide();
        _nextWorkCard=0;
    }
    private void BeginYardPreview()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(home==null)return;
        _yardPreviousFocus=_focus;_yardPreviousAngle=_angle;_yardPreviousZoom=_camera.Size;_yardCameraSaved=true;
        _yardPreviewHome=home.Id;_yardPreviewSide=home.YardSide;_nextWorkCard=0;FrameYardPreview();
    }
    private void MakeYardPreview(VBoxContainer column)
    {
        _yardChoices=new(){Visible=false};column.AddChild(_yardChoices);
        var grid=new GridContainer{Columns=2};_yardChoices.AddChild(grid);
        for(int i=0;i<4;i++){int side=i;_yardSides[i]=Button(World.YardSideName(i),()=>{_yardPreviewSide=side;_nextWorkCard=0;FrameYardPreview();});_yardSides[i].SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;grid.AddChild(_yardSides[i]);}
        _yardPreviewInfo=Text("",13,true);_yardChoices.AddChild(_yardPreviewInfo);
        _yardFurnish=Button("Furnish here",()=>{if(_world.FurnishHomeYard(_yardPreviewHome,_yardPreviewSide))FinishYardPreview();});_yardChoices.AddChild(_yardFurnish);
        var actions=new HBoxContainer();_yardChoices.AddChild(actions);
        _yardApply=Button("Use this ground",()=>{if(_world.SetHomeYard(_yardPreviewHome,_yardPreviewSide))FinishYardPreview();});actions.AddChild(_yardApply);
        actions.AddChild(Button("Cancel [Esc]",StopYardPreview));
    }
    private void RenderYardPreview(Cottage home)
    {
        bool show=_yardPreviewHome==home.Id && _yardPreviewSide>=0;_yardChoices.Visible=show;if(!show)return;
        string? problem=_world.HomeYardProblem(home.Id,_yardPreviewSide);
        var places=_world.YardPlaces(home,_yardPreviewSide);
        _yardPreviewInfo.Text=problem??"Blue furniture is a preview. Residents use this ground for mending and nearby meals.";
        if(problem==null && !home.Improved && !_world.Creative && _world.AvailablePlanks<World.ComfortCost(home))
            _yardPreviewInfo.Text+=$" Only {_world.AvailablePlanks} planks available; the order will wait for supplies.";
        _yardApply.Disabled=problem!=null;_yardApply.Text=home.Improved?"Move yard here":"Choose ground only";
        _yardFurnish.Visible=!home.Improved;_yardFurnish.Disabled=_world.FurnishHomeYardProblem(home.Id,_yardPreviewSide)!=null;
        _yardFurnish.Text=_world.Creative?"Furnish here · free":$"Furnish here · {World.ComfortCost(home)} planks";
        _yardFurnish.TooltipText=_world.FurnishHomeYardProblem(home.Id,_yardPreviewSide)??"Choose this ground and order shared workers to deliver and install the furnishings.";
        foreach(var side in Enumerable.Range(0,4))_yardSides[side].Modulate=side==_yardPreviewSide?_cream:Colors.White;
        if(_yardPreviewGround==null || !GodotObject.IsInstanceValid(_yardPreviewGround)){_yardPreviewGround=new();AddChild(_yardPreviewGround);_yardPreviewKey="";}
        _yardPreviewGround.Show();string key=$"{home.Id}/{_yardPreviewSide}/{problem}/"+string.Join(';',places.Select(c=>$"{c}:{Height(c.X,c.Z)}"));
        if(key==_yardPreviewKey)return;_yardPreviewKey=key;Clear(_yardPreviewGround);
        foreach(var c in places)
        {
            GroundPatch(_yardPreviewGround,c.X,c.Z,.92f,.92f,problem==null?new("367a89"):new Color("bf7860"),.075f);
            MakeHomeYardFurniture(_yardPreviewGround,c,true);
            foreach(float x in new[]{-.40f,.40f})Box(_yardPreviewGround,OnGround(c.X+x,c.Z,.11f),new(.035f,.05f,.84f),_cream);
        }
        // A proposal is a world-space overlay: nearby foliage/roofs must not hide the choice.
        foreach(var mesh in _yardPreviewGround.GetChildren().OfType<MeshInstance3D>())
        {
            if(mesh.MaterialOverride is StandardMaterial3D material){material.NoDepthTest=true;material.ShadingMode=BaseMaterial3D.ShadingModeEnum.Unshaded;material.RenderPriority=1;}
            mesh.CastShadow=GeometryInstance3D.ShadowCastingSetting.Off;
        }
    }
    private string HomeCardOutcome(Cottage home)
    {
        if(_yardPreviewHome==home.Id && _yardPreviewSide>=0)return $"Preview: {World.YardSideName(_yardPreviewSide)}\n{_world.YardPlaces(home,_yardPreviewSide).Length} usable outdoor places · not yet confirmed.";
        if(home.ImprovementRequested)
        {
            var worker=_world.People.FirstOrDefault(p=>p.ComfortHomeId==home.Id);
            return "Making room for outdoor life.\n"+(worker!=null?worker.Name+": "+worker.Status:home.ImprovementPlanks<World.ComfortCost(home) && _world.AvailablePlanks==0?"Waiting for planks; a sawmill makes them from logs.":"Shared workers will deliver and install the furnishings.");
        }
        if(!home.Improved && home.ImprovementPlanks>0)return "Cancelled furnishing. Workers are recovering the planks before another order.";
        string places=$"{World.YardSideName(home.YardSide)} · {_world.PotentialHomeYardPlaces(home).Length} usable outdoor places.";
        if(home.Improved)return "A yard for mending and nearby meals.\n"+places;
        return "An optional yard for mending and nearby meals.\n"+places;
    }
}
