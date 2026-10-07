using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Button _placePath=null!;
    private HBoxContainer _placePathRow=null!;
    private int _pathOrigin=-1;
    private World? _pathOriginWorld;
    private Cell? _pathSharedOrigin;
    private Vector3 _pathReturnFocus;
    private float _pathReturnAngle,_pathReturnZoom;
    private bool HasPlacePathOrigin=>_pathOrigin>=0 || _pathSharedOrigin!=null;
    private void BeginPlacePath(Cell start,int id=-1,Cell? shared=null)
    {
        var focus=_focus;float angle=_angle,zoom=_camera.Size;
        CloseManagementUi();TogglePaths(3);_placing=true;
        _pathWaypoints.Clear();_pathDraftEnd=null;_pathOrigin=id;_pathSharedOrigin=shared;_pathOriginWorld=_world;_pathAnchor=start;_connectionWorld=null;
        _pathReturnFocus=focus;_pathReturnAngle=angle;_pathReturnZoom=zoom;RefreshGhost();
    }
    private void BeginSharedPlacePath()
    {
        if(SelectedSharedPlace is {} place)BeginPlacePath(place.Center,shared:place.Center);
    }
    private void MakePlacePath(VBoxContainer column)
    {
        _placePathRow=new();column.AddChild(_placePathRow);_turnHomeButton.Reparent(_placePathRow);
        _placePath=Button("Path from here",()=>{
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site==null)return;
            BeginPlacePath(site.Entrance,site.Id);
        });_placePath.TooltipText="Preview a free walking path from this entrance to another place or clear ground. Choose the destination and Apply; Esc returns without changing paths.";_placePathRow.AddChild(_placePath);
    }
    private void EndPlacePath()
    {
        int id=_pathOrigin;var shared=_pathSharedOrigin;bool same=_pathOriginWorld==_world;
        _pathOrigin=-1;_pathSharedOrigin=null;_pathOriginWorld=null;_pathAnchor=null;_pathWaypoints.Clear();_pathDraftEnd=null;_connectionWorld=null;
        _placing=false;_pathTool=0;RefreshGhost();
        if(same)
        {
            if(shared is Cell center && _world.SharedPlaces.Any(c=>c.Center==center))ShowCommonsCard(center);
            else if(_world.Cottages.Any(c=>c.Id==id))ShowWorkplaceCard(id);
            _focus=_pathReturnFocus;_angle=_pathReturnAngle;_camera.Size=_pathReturnZoom;UpdateCamera();
        }
    }
    private void RenderPlacePath(Cottage site)
    {
        _placePathRow.Visible=_world.PublicPlace!=null && site.Complete && _yardPreviewSide<0 && _reshapingPlot<0 && (Buildings.Get(site.Kind).Beds==0 || _homeOptions);
        _placePath.Disabled=_world.PathProblem(site.Entrance)!=null || _world.Food.Celebrating;
    }
}
