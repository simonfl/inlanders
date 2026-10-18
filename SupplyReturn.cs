using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private World? _supplyReturnWorld;
    private int _supplyOrigin=-1;
    private Vector3 _supplyFocus;
    private float _supplyAngle,_supplyZoom;
    private Button _supplyReturn=null!;
    private Cottage? SupplyOrigin=>_supplyReturnWorld==_world?_world.Cottages.FirstOrDefault(c=>c.Id==_supplyOrigin && !c.DemolitionRequested):null;
    private void RememberSupplyOrigin()
    {
        if(SupplyOrigin!=null)return;
        _supplyReturnWorld=_world;_supplyOrigin=_workCardSite;
        _supplyFocus=_focus;_supplyAngle=_angle;_supplyZoom=_camera.Size;
    }
    private void ForgetSupplyOrigin(){_supplyReturnWorld=null;_supplyOrigin=-1;}
    private bool ReturnToSupplyOrigin()
    {
        var origin=SupplyOrigin;if(origin==null){ForgetSupplyOrigin();return false;}
        _placing=false;RefreshGhost();ShowWorkplaceCard(origin.Id);
        _focus=_supplyFocus;_angle=_supplyAngle;_camera.Size=_supplyZoom;_followPerson=false;UpdateCamera();
        ForgetSupplyOrigin();return true;
    }
    private void MakeSupplyReturn(VBoxContainer column)
    {
        _supplyReturn=Button("",()=>ReturnToSupplyOrigin());column.AddChild(_supplyReturn);
        _supplyReturn.TooltipText="Return to the project and view you were supplying. Work and deliveries continue.";
    }
    private void RenderSupplyReturn(Cottage site)
    {
        var origin=SupplyOrigin;_supplyReturn.Visible=origin!=null && site.Id!=origin.Id;
        if(origin!=null)_supplyReturn.Text="Back to "+BuildingName(origin.Kind).ToLowerInvariant();
    }
}

