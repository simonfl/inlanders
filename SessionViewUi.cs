using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private void RememberSessionView()
    {
        if(_camera==null || _atMainMenu)return;
        int person=_dailyPerson>=0?_dailyPerson:_selectedPerson;
        int site=_workCardSite>=0?_workCardSite:_selectedSite;
        _world.SessionView=new(_focus.X,_focus.Y,_focus.Z,_camera.Size,_angle,person,site,_additionSelected,_selectedCommons?_selectedCommonsCenter:null);
    }
    private void RestoreSessionView()
    {
        if(_world.SessionView is not {} v)return;
        CloseManagementUi();CancelCameraDrag();_followPerson=false;_watchOrbit=false;_paused=true;
        _focus=new(v.X,v.Y,v.Z);_angle=v.Angle;_camera.Size=v.Zoom;UpdateCamera();
        _dailyWorld=_world;
        if(v.Person>=0 && v.Person<_world.People.Count)ShowDailyLife(v.Person);
        else if(v.Site>=0 && _world.Cottages.Any(c=>c.Id==v.Site))ShowWorkplaceCard(v.Site);
        else if(v.Addition && _world.PendingAddition!=null)ShowPreparedAddition();
        else if(v.Commons is Cell center && _world.SharedPlaces.Any(c=>c.Center==center))ShowCommonsCard(center);
        _focus=new(v.X,v.Y,v.Z);_angle=v.Angle;_camera.Size=v.Zoom;UpdateCamera();
    }
}
