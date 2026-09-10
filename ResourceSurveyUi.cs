using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private bool _surveying;
    private SourceKey? _selectedSource;
    private SourceKey? _lastSurveySource;
    private ResourceSurvey? _sourceReport;
    private Button _surveyToggle=null!;
    private Button _surveyBack=null!;
    private VBoxContainer _surveyDetails=null!, _sourceWorkplaces=null!;
    private Label _sourceInfo=null!, _sourceWorkplaceHeading=null!;
    private OptionButton _sourceChoice=null!;
    private Control _sourceMarkers=null!;
    private readonly Dictionary<SourceKey,Button> _sourceButtons=new();
    private readonly List<ResourceSource> _sourceList=new();
    private readonly Dictionary<int,Button> _sourceWorkplaceLinks=new();
    private float _nextSourceRefresh;

    private void MakeResourceSurvey(VBoxContainer inspection)
    {
        _surveyBack=Button("Back to source",()=> { if(_lastSurveySource is SourceKey key) SelectResourceSource(key,true); });
        inspection.AddChild(_surveyBack); inspection.MoveChild(_surveyBack,1); _surveyBack.Hide();
        _sourceMarkers=new Control { MouseFilter=Control.MouseFilterEnum.Ignore }; _hud.AddChild(_sourceMarkers);
        _sourceMarkers.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect); _sourceMarkers.Hide();
        _surveyDetails=new VBoxContainer(); _surveyDetails.AddThemeConstantOverride("separation",10); inspection.AddChild(_surveyDetails);
        _surveyDetails.AddChild(Text("RESOURCE SURVEY",16));
        _sourceChoice=DirectoryFilter(_surveyDetails,"Choose a source here, or click its marker on the map.");
        _sourceChoice.ItemSelected+=i=> { if(i>0 && i<=_sourceList.Count) SelectResourceSource(_sourceList[(int)i-1].Key,true); };
        _sourceInfo=Text("",14,true); _surveyDetails.AddChild(_sourceInfo);
        _sourceWorkplaceHeading=Text("",12,true); _surveyDetails.AddChild(_sourceWorkplaceHeading);
        _sourceWorkplaces=new VBoxContainer(); _surveyDetails.AddChild(_sourceWorkplaces);
        _surveyDetails.AddChild(Button("Finish survey [U / Esc]",StopResourceSurvey));
        _surveyDetails.Hide();
    }
    private void ToggleResourceSurvey()
    {
        if(_surveying) { StopResourceSurvey(); return; }
        ExitWatch(); _placing=false; _pathStroke=false; _woodlandStroke=false; _showSupplyRoutes=false; RefreshGhost(); ClearSelection();
        _surveying=true; _sourceMarkers.Show();
        foreach(var button in _sourceButtons.Values) { _sourceMarkers.RemoveChild(button); button.QueueFree(); } _sourceButtons.Clear();
        _sourceList.Clear(); _sourceList.AddRange(_world.ResourceSources());
        _sourceChoice.Clear(); _sourceChoice.AddItem("Choose a map source");
        foreach(var source in _sourceList)
        {
            _sourceChoice.AddItem(source.Name); var key=source.Key;
            var button=Button(source.Name,()=>SelectResourceSource(key,false));
            button.AddThemeFontSizeOverride("font_size",13); button.CustomMinimumSize=new(100,30);
            button.TooltipText="Inspect source stock, access and related workplaces.";
            _sourceMarkers.AddChild(button); _sourceButtons[key]=button;
        }
        ShowInspector(); _nextSourceRefresh=0; UpdateResourceSurvey();
    }
    private void StopResourceSurvey()
    {
        if(!_surveying) return;
        bool selected=_selectedSource!=null || (_selectedSite<0 && _selectedPerson<0);
        _surveying=false; _selectedSource=null; _lastSurveySource=null; _sourceReport=null; _surveyDetails.Hide(); _surveyBack.Hide(); _sourceMarkers.Hide();
        if(selected) { _inspector.Hide(); RefreshSelection(); }
    }
    private void SelectResourceSource(SourceKey key,bool moveCamera)
    {
        var report=_world.ReadResourceSurvey(key); if(report==null) return;
        ClearSelection(); _selectedSource=key; _lastSurveySource=key; _sourceReport=report;
        _sourceChoice.Select(_sourceList.FindIndex(s=>s.Key==key)+1);
        if(moveCamera) { _focus=OnGround(report.Source.Cell.X,report.Source.Cell.Z); UpdateCamera(); }
        _nextSourceRefresh=0; ShowInspector(); RefreshSelection(); UpdateResourceSurvey();
    }
    private bool PickResourceSource(Vector2 point)
    {
        if(!_surveying || Ground(point) is not Vector3 ground) return false;
        var cell=new Cell(Mathf.RoundToInt(ground.X),Mathf.RoundToInt(ground.Z));
        var source=_sourceList.Where(s=>(s.Cell.Point-cell.Point).LengthSquared()<=(s.Key.Kind==SourceKind.Woodland?25:2))
            .OrderBy(s=>(s.Cell.Point-cell.Point).LengthSquared()).FirstOrDefault();
        if(source==null) return false;
        SelectResourceSource(source.Key,false); return true;
    }
    private void UpdateResourceSurvey()
    {
        _surveyToggle.Text=_surveying?"Finish resource survey [U]":"Survey map resources [U]";
        _surveyDetails.Visible=_surveying && _selectedSite<0 && _selectedPerson<0;
        _surveyBack.Visible=_surveying && !_surveyDetails.Visible && _lastSurveySource!=null;
        foreach(var view in _depositViews.Values)
            foreach(var label in view.Body.GetChildren().OfType<Label3D>()) label.Visible=_showWorldLabels && !_surveying;
        foreach(var region in _wildlifeViews)
            foreach(var node in region.GetChildren().OfType<Node3D>())
                foreach(var label in node.GetChildren().OfType<Label3D>()) label.Visible=_showWorldLabels && !_surveying;
        if(!_surveying) return;
        foreach(var source in _sourceList)
        {
            var button=_sourceButtons[source.Key];
            var point=_camera.UnprojectPosition(OnGround(source.Cell.X,source.Cell.Z,1.1f));
            button.Position=point-button.Size/2;
            var rect=button.GetGlobalRect();
            button.Visible=rect.Position.X>=16 && rect.End.X<_hud.Size.X-16 && rect.Position.Y>84 && rect.End.Y<_hud.Size.Y-84 &&
                !(_drawer.Visible && rect.Intersects(_drawer.GetGlobalRect())) && !(_inspector.Visible && rect.Intersects(_inspector.GetGlobalRect()));
            button.Modulate=_selectedSource==source.Key?_cream:Colors.White;
        }
        if(_uiTime<_nextSourceRefresh) return;
        _nextSourceRefresh=_uiTime+.5f;
        _sourceReport=_selectedSource is SourceKey key?_world.ReadResourceSurvey(key):null;
        _sourceInfo.Text=_sourceReport==null ? _sourceList.Count==0?"No fish grounds, stone outcrops or wildlife habitats on this map. These sources are authored per map.":"Click a source marker, or choose one above. Stock belongs to the source; delivery to storage takes worker time.":$"{_sourceReport.Source.Name.ToUpperInvariant()}\n{_sourceReport.Detail}";
        var ids=_sourceReport?.Workplaces ?? System.Array.Empty<int>();
        _sourceWorkplaceHeading.Text=_sourceReport==null?"":_sourceReport.WorkplaceHeading+(ids.Length==0?"\nNone qualify for this source.":"");
        foreach(int id in _sourceWorkplaceLinks.Keys.Where(id=>!ids.Contains(id)).ToArray())
        { var button=_sourceWorkplaceLinks[id]; _sourceWorkplaces.RemoveChild(button); button.QueueFree(); _sourceWorkplaceLinks.Remove(id); }
        foreach(int id in ids)
        {
            if(!_sourceWorkplaceLinks.TryGetValue(id,out var button))
            {
                int target=id; button=Button("",()=>ShowServicePlace(target)); button.AutowrapMode=TextServer.AutowrapMode.WordSmart;
                _sourceWorkplaces.AddChild(button); _sourceWorkplaceLinks[id]=button;
            }
            var site=_world.Cottages.Single(c=>c.Id==id);
            button.Text=$"{BuildingName(site.Kind)} {id} · {(!site.Complete?"planned":site.WorkPaused?"paused":"inspect")}";
        }
    }
}
