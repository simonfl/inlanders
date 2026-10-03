using Godot;
using Inlanders.Simulation;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private Button _surveyBuildHere=null!;
    private VBoxContainer _landSurveyViews=null!;
    private readonly List<Button> _landViewButtons=new();
    private World? _surveyOriginWorld;
    private Vector3 _surveyOriginFocus;
    private float _surveyOriginZoom,_surveyOriginAngle;
    private void MakeLandSurveyViews(VBoxContainer parent)
    {
        _landSurveyViews=new();parent.AddChild(_landSurveyViews);
        _landSurveyViews.AddChild(Text("Look at the shore, open ground or existing trees. These are views, not suggested building sites or soil ratings.",14,true));
        _surveyBuildHere=Button("Build in this view",()=>
        {
            _surveyOriginWorld=null;StopResourceSurvey();CloseManagementUi();
            _fullBuild=false;ToggleDrawer(1);SelectBuildSection(0);UpdateVillageDirectory();
        });_landSurveyViews.AddChild(_surveyBuildHere);
        _surveyBuildHere.TooltipText="Keep this camera view and choose a home, growing ground, landing or timber work. Nothing is placed until you choose ground.";
        var row=new HBoxContainer();_landSurveyViews.AddChild(row);
        for(int i=0;i<3;i++){int choice=i;var b=Button(new[]{"Shore","Open land","Woodlot"}[i],()=>LookAtLand(choice));b.AddThemeFontSizeOverride("font_size",13);row.AddChild(b);_landViewButtons.Add(b);}
    }
    private void RememberSurveyView()
    {
        if(_world.PublicPlace==null)return;
        _surveyOriginWorld=_world;_surveyOriginFocus=_focus;_surveyOriginZoom=_camera.Size;_surveyOriginAngle=_angle;
    }
    private void RestoreSurveyView()
    {
        if(_surveyOriginWorld==_world){_focus=_surveyOriginFocus;_camera.Size=_surveyOriginZoom;_angle=_surveyOriginAngle;UpdateCamera();}
        _surveyOriginWorld=null;
    }
    private void LookAtLand(int kind)
    {
        var steps=new[]{new Cell(1,0),new Cell(-1,0),new Cell(0,1),new Cell(0,-1)};
        var cells=(kind==2?_world.Trees.Where(t=>!t.Felled && !t.NeedsPlanting).Select(t=>t.Cell):kind==0?
            _world.Map.Land.Where(c=>steps.Any(d=>_world.Map.Water.Contains(new(c.X+d.X,c.Z+d.Z)))):
            _world.Map.Land.Where(c=>_world.PathProblem(c)==null && _world.Map.SurfaceHeight(c.X,c.Z)<.1f)).ToArray();
        if(cells.Length==0){Notice("No "+new[]{"shore","open ground","standing trees"}[kind]+" in this place.");return;}
        _followPerson=false;_watchOrbit=false;
        _focus=OnGround((float)cells.Average(c=>c.X),(float)cells.Average(c=>c.Z));_camera.Size=23;UpdateCamera();
    }
}
