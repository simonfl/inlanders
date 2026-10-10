using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Cell? _pathDraftEnd;
    private PanelContainer _pathProposalPanel=null!;
    private Label _pathProposalText=null!;
    private Button _pathProposalApply=null!,_pathProposalUndo=null!,_pathProposalFrame=null!;
    private void MakePathProposalUi()
    {
        _pathProposalPanel=HudPanel(_hud);var column=new VBoxContainer();_pathProposalPanel.AddChild(column);
        _pathProposalText=Text("",14,true);_pathProposalText.CustomMinimumSize=new(266,0);column.AddChild(_pathProposalText);
        _pathProposalApply=Button("Apply path [Enter]",ConfirmPathProposal);column.AddChild(_pathProposalApply);
        var row=new HBoxContainer();column.AddChild(row);
        _pathProposalUndo=Button("Undo bend",UndoPathBend);row.AddChild(_pathProposalUndo);
        _pathProposalFrame=Button("Frame path",FramePathProposal);row.AddChild(_pathProposalFrame);
        column.AddChild(Button("Cancel [Esc]",()=>{if(HasPlacePathOrigin)EndPlacePath();else{_placing=false;_pathAnchor=null;_pathDraftEnd=null;_pathWaypoints.Clear();RefreshGhost();}}));_pathProposalPanel.Hide();
    }
    private void BeginPathFromTrip()
    {
        if(_placeJourney is not {} trip || _world.Cottages.FirstOrDefault(c=>c.Id==_placeJourneySite) is not {} site)return;
        var destination=PathEndpoint(trip.Steps[^1]);BeginPlacePath(site.Entrance,site.Id);_pathDraftEnd=destination;_connectionWorld=null;RefreshGhost();FramePathProposal();
    }
    private void ConfirmPathProposal()
    {
        if(_pathAnchor==null || _pathDraftEnd is not Cell end)return;
        if(!_world.ConnectPaths(ConnectionStops(end))){UiCue(Cue.Reject);_connectionWorld=null;return;}
        _pathAnchor=null;_pathDraftEnd=null;_pathWaypoints.Clear();_connectionWorld=null;UiCue(Cue.Click);
        if(HasPlacePathOrigin)EndPlacePath();else RefreshGhost();
    }
    private void UndoPathBend()
    {
        if(_pathWaypoints.Count==0)return;_pathWaypoints.RemoveAt(_pathWaypoints.Count-1);_connectionWorld=null;RefreshGhost();
    }
    private void FramePathProposal()
    {
        ConnectionProblem(_hover);if(_connectionRoute.Count==0)return;
        var cells=_connectionRoute;_focus=OnGround((cells.Min(c=>c.X)+cells.Max(c=>c.X))*.5f,(cells.Min(c=>c.Z)+cells.Max(c=>c.Z))*.5f);
        _camera.Size=22;UpdateCamera();var points=cells.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z))).ToArray();
        float scale=Mathf.Max((points.Max(p=>p.X)-points.Min(p=>p.X)+340)/_hud.Size.X,(points.Max(p=>p.Y)-points.Min(p=>p.Y)+200)/_hud.Size.Y);
        _camera.Size=Mathf.Clamp(22*scale,14,MaximumZoom);UpdateCamera();
    }
    private bool HandlePathProposalInput(InputEvent input)
    {
        if(!_placing || _pathTool!=3)return false;
        if(input is InputEventKey{Pressed:true,Echo:false} key){if(key.Keycode==Key.Backspace){UndoPathBend();return true;}if(key.Keycode==Key.Enter){ConfirmPathProposal();return true;}}
        return false;
    }
    private void RenderPathProposalUi()
    {
        bool show=_placing && _pathTool==3 && !_atMainMenu && !_drawer.Visible;_pathProposalPanel.Visible=show;if(!show)return;
        _pathProposalPanel.Position=new(_hud.Size.X-306,92);_pathProposalPanel.Size=new(290,0);
        string? problem=_pathAnchor==null?null:ConnectionProblem(_hover);
        _pathProposalText.Text="CONNECT PLACES\n"+(_pathAnchor==null?"Click a place, resource or clear ground to begin.":_pathDraftEnd==null?"Click a destination. Shift-click intermediate ground to add bends.":"Destination chosen. Click to revise; Shift-click to add a bend.\n"+(problem??"Apply when the approach looks right."));
        _pathProposalApply.Disabled=_pathDraftEnd==null || problem!=null;_pathProposalUndo.Disabled=_pathWaypoints.Count==0;_pathProposalFrame.Disabled=_pathAnchor==null;
    }
}
