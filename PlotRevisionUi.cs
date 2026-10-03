using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private int _reshapingPlot=-1,_revisionRows;
    private VBoxContainer _revisionPanel=null!;
    private PanelContainer _revisionProposal=null!;
    private Vector3 _revisionPreviousFocus;
    private float _revisionPreviousZoom;
    private World? _revisionCameraWorld;
    private Button _reshapePlot=null!,_revisionApply=null!,_revisionLess=null!,_revisionMore=null!;
    private Label _revisionInfo=null!;
    private Node3D? _revisionGround;
    private void MakePlotRevision(VBoxContainer column)
    {
        _reshapePlot=Button("Reshape cultivated ground",BeginPlotRevision);column.AddChild(_reshapePlot);
        _revisionProposal=HudPanel(_hud);_revisionProposal.Hide();_revisionPanel=new(){Visible=false};_revisionProposal.AddChild(_revisionPanel);var title=Text("SHAPE CULTIVATED GROUND",15);title.CustomMinimumSize=new(306,0);_revisionPanel.AddChild(title);_revisionInfo=Text("",13,true);_revisionPanel.AddChild(_revisionInfo);
        var row=new HBoxContainer();_revisionPanel.AddChild(row);
        _revisionLess=Button("Shorter",()=>{_revisionRows=System.Math.Max(1,_revisionRows-1);_nextWorkCard=0;});row.AddChild(_revisionLess);_revisionMore=Button("Longer",()=>{_revisionRows=System.Math.Min(8,_revisionRows+1);_nextWorkCard=0;});row.AddChild(_revisionMore);
        var actions=new HBoxContainer();_revisionPanel.AddChild(actions);
        _revisionApply=Button("Use this ground",()=>{if(_revisionRows>_world.PreparedPlotRows(_world.Cottages.Single(c=>c.Id==_reshapingPlot))?_world.ExtendCultivation(_reshapingPlot,_revisionRows):_world.ReviseCultivation(_reshapingPlot,_revisionRows)){EndPlotRevision();CreateActors();RenderActors(0);RenderFoodViews();RefreshSelection();}});actions.AddChild(_revisionApply);
        actions.AddChild(Button("Cancel [Esc]",EndPlotRevision));
    }
    private void BeginPlotRevision()
    {
        var site=_world.Cottages.Single(c=>c.Id==_workCardSite);_reshapingPlot=site.Id;_revisionRows=site.Depth;_nextWorkCard=0;
        _revisionPreviousFocus=_focus;_revisionPreviousZoom=_camera.Size;_revisionCameraWorld=_world;
        _focus=(BuildingPosition(site)+RevisionEdge(site,8))*.5f;_camera.Size=22;_followPerson=false;_watchOrbit=false;UpdateCamera();
        _focus+=CameraDragPoint(_hud.Size*.5f)-CameraDragPoint(new((_hud.Size.X-346)/2,_hud.Size.Y*.48f));UpdateCamera();
    }
    private void EndPlotRevision()
    {
        if(_revisionCameraWorld==_world){_focus=_revisionPreviousFocus;_camera.Size=_revisionPreviousZoom;UpdateCamera();}_revisionCameraWorld=null;
        _dragPlotEdge=false;_reshapingPlot=-1;_revisionPanel.Hide();_revisionProposal.Hide();if(_revisionGround!=null)_revisionGround.Hide();_nextWorkCard=0;
    }
    private void RenderPlotRevision(Cottage site)
    {
        bool eligible=_world.PublicPlace!=null && site.Complete && site.Kind==BuildingKind.VegetableField;
        _reshapePlot.Visible=eligible && _reshapingPlot!=site.Id;_reshapePlot.Disabled=site.Harvest>0;
        _reshapePlot.TooltipText=site.Harvest>0?"Collect ripe crops first.":"Preview released land, restore prepared rows or extend with timber. Work pauses only when applying. Growing crops restart; timber and stored food stay.";
        bool show=eligible && _reshapingPlot==site.Id;_revisionProposal.Visible=_revisionPanel.Visible=show;
        _revisionProposal.Size=new(330,0);_revisionProposal.Position=new(Mathf.Max(16,_hud.Size.X-346),92);
        if(!show){if(_revisionGround!=null)_revisionGround.Hide();return;}
        bool extending=_revisionRows>_world.PreparedPlotRows(site);
        var problem=extending?_world.ExtendCultivationProblem(site.Id,_revisionRows):_world.ReshapePlotProblem(site.Id,_revisionRows,false);
        _revisionLess.Disabled=_revisionRows==1;_revisionMore.Disabled=_revisionRows==8;
        _revisionInfo.Text="Drag the gold end of the strip, or use the buttons. Apply when ready.\n"+$"3 × {_revisionRows} tiles · {_revisionRows*4} vegetables/crop\n"+(problem??(extending?(_world.Creative?"Free preparation.":$"{(_revisionRows-_world.PreparedPlotRows(site))*2} extra logs delivered by shared workers.")+" Stored food stays; growing crops restart after preparation.":"Timber and food stay. Growing crops restart; previous working status stays."));_revisionApply.Disabled=problem!=null;
        if(_revisionGround==null){_revisionGround=new();AddChild(_revisionGround);}Clear(_revisionGround);_revisionGround.Show();
        var edge=RevisionEdge(site,_revisionRows);
        Cylinder(_revisionGround,edge,.26f,.06f,new("edc57c"));
        foreach(var c in World.Footprint(site.Cell,site.Rotation,site.Kind,_revisionRows))GroundPatch(_revisionGround,c.X,c.Z,.88f,.88f,problem==null?new("69adb3"):new Color("bf7860"),.09f);
    }
}
