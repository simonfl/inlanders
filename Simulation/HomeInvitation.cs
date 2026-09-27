using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public string? HomeInvitationProblem(int homeId)
    {
        if(PublicPlace==null)return "Choose a home in the current village.";
        var home=Cottages.FirstOrDefault(c=>c.Id==homeId && IsHome(c));
        if(home==null)return "Finish this home before inviting neighbors.";
        if(Buildings.Get(home.Kind).Beds-People.Count(p=>p.HomeId==homeId)<2)return "This home needs two spare beds.";
        if(Food.Celebrating)return "Welcome neighbors after supper finishes.";
        return InvitationProblem();
    }
    public bool InviteToHome(int homeId)
    {
        if(HomeInvitationProblem(homeId)!=null)return false;
        int first=Population;
        if(!InviteNewcomers())return false;
        // Ordinary arrivals still enter by the yard. Only their destination home is chosen.
        foreach(var person in People.Skip(first))AssignHome(person.Id,homeId);
        History.Add($"Two neighbors made their home at {Buildings.Get(Cottages.Single(c=>c.Id==homeId).Kind).Name} {homeId}.");
        return true;
    }
}
