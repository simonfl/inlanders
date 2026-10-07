using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private World? _localWatchWorld;
    private int _localWatchSite=-1;
    private Cell? _localWatchCommons;
    private void WatchSharedPlace()
    {
        if(SelectedSharedPlace is not {} place)return;
        _localWatchWorld=_world;_localWatchSite=-1;_localWatchCommons=place.Center;
        _localWatchFocus=_focus;_localWatchAngle=_angle;_localWatchZoom=_camera.Size;
        _focus=OnGround(place.Center.X,place.Center.Z);_camera.Size=14;_followPerson=false;_watchOrbit=false;UpdateCamera();ToggleWatch();
    }
    private Vector3 _localWatchFocus;
    private float _localWatchAngle,_localWatchZoom;
    private void WatchLocalPlace()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site==null)return;
        _localWatchWorld=_world;_localWatchSite=site.Id;_localWatchCommons=null;
        _localWatchFocus=_focus;_localWatchAngle=_angle;_localWatchZoom=_camera.Size;
        var nearby=_world.Cottages.Where(c=>c.Complete && (c.Cell.Point-site.Cell.Point).LengthSquared()<=64).ToList();
        // Include the nearest actual food workplace when homes stand apart from their fields.
        if(!nearby.Any(c=>_world.IsWorkplaceFoodStore(c)))
        {
            var food=_world.Cottages.Where(c=>c.Complete && _world.IsWorkplaceFoodStore(c)).OrderBy(c=>(c.Cell.Point-site.Cell.Point).LengthSquared()).FirstOrDefault();
            if(food!=null)nearby.Add(food);
        }
        var cells=nearby.SelectMany(c=>World.Footprint(c).Append(c.Entrance).Concat(_world.HomeYardPlaces(c))).ToList();
        foreach(var commons in _world.SharedPlaces.Where(c=>(c.Center.Point-site.Cell.Point).LengthSquared()<=144))cells.AddRange(commons.Places);
        if(cells.Count==0)cells.Add(site.Cell);
        _focus=OnGround((cells.Min(c=>c.X)+cells.Max(c=>c.X))*.5f,(cells.Min(c=>c.Z)+cells.Max(c=>c.Z))*.5f);
        _camera.Size=22;_followPerson=false;_watchOrbit=false;UpdateCamera();
        var projected=cells.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z))).ToArray();
        float scale=Mathf.Max((projected.Max(p=>p.X)-projected.Min(p=>p.X)+150)/_hud.Size.X,(projected.Max(p=>p.Y)-projected.Min(p=>p.Y)+180)/_hud.Size.Y);
        _camera.Size=Mathf.Clamp(22*scale,14,MaximumZoom);UpdateCamera();ToggleWatch();
    }
    private void ReturnFromLocalWatch()
    {
        if(_localWatchWorld==_world && _world.Cottages.Any(c=>c.Id==_localWatchSite))
        {
            _focus=_localWatchFocus;_angle=_localWatchAngle;_camera.Size=_localWatchZoom;_followPerson=false;UpdateCamera();
            ShowWorkplaceCard(_localWatchSite);
        }
        if(_localWatchWorld==_world && _localWatchCommons is Cell center && _world.SharedPlaces.Any(c=>c.Center==center))
        {
            ShowCommonsCard(center);_focus=_localWatchFocus;_angle=_localWatchAngle;_camera.Size=_localWatchZoom;UpdateCamera();
        }
        _localWatchWorld=null;_localWatchSite=-1;_localWatchCommons=null;
    }
}
