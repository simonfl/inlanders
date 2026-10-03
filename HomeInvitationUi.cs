using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Button _homeInvite=null!;
    private void MakeHomeInvitation(VBoxContainer column)
    {
        _homeInvite=Button("Invite 2 here · +2 meals/min",()=>{
            int id=_workCardSite;
            if(_world.InviteToHome(id)){_nextWorkCard=0;Notice("Two neighbors have arrived and will live here. They share work and need two more meals per minute.");}
            else Notice(_world.HomeInvitationProblem(id)??"This invitation is no longer available.");
        });column.AddChild(_homeInvite);
    }
    private void RenderHomeInvitation(Cottage home)
    {
        int beds=Buildings.Get(home.Kind).Beds;
        _homeInvite.Visible=_homeOptions && _world.PublicPlace!=null && home.Complete && beds>0 && !home.DemolitionRequested && _yardPreviewSide<0 && _placeJourneySite!=home.Id && beds-_world.People.Count(p=>p.HomeId==home.Id)>=2;
        _homeInvite.Disabled=_world.HomeInvitationProblem(home.Id)!=null;
        _homeInvite.TooltipText=_world.HomeInvitationProblem(home.Id)??$"Two neighbors will use this home and share the work. Population {_world.Population} → {_world.Population+2}; food demand {_world.Population} → {_world.Population+2} portions/minute. {_world.EdibleStored} portions stored now; storage is not a promise of continued supply. Staying small is fine.";
    }
}
