using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record CampaignCondition(string Key,string Label,int Current,int Required,string Explanation)
{
    public bool Met => Current>=Required;
}
public sealed partial class World
{
    public bool ResidentMeetsCampaignCondition(Villager p,string key) => key switch
    {
        "housing"=>p.HomeId!=null,
        "rest"=>RecentlyRested(p),
        "recreation"=>p.LastLeisureTime is float t && Food.Time-t<120,
        "east-recreation"=>p.LastLeisureTime is float last && Food.Time-last<120 && Cottages.Any(c=>c.Id==p.LastLeisureSiteId && c.Kind==BuildingKind.Square && c.Cell.X>5 && c.Complete && !c.DemolitionRequested),
        _=>false
    };
    public string CampaignResidentReason(Villager p,string key)
    {
        if(key=="housing")return p.HomeId!=null?"Counted: assigned to a home.":"Not counted: no assigned home.";
        if(key=="rest")return (RecentlyRested(p)?"Counted. ":"Not counted. ")+RestSummary(p);
        if(ResidentMeetsCampaignCondition(p,key))return $"Counted: latest completed break was {(int)(Food.Time-p.LastLeisureTime!.Value)}s ago; expires after 2m.";
        return (p.LastLeisureTime==null?"No completed break yet.":Food.Time-p.LastLeisureTime>=120?"Latest completed break is older than 2m.":"Latest break was not at an available east-bank Square.")+" "+RecreationSummary(p);
    }
    public IReadOnlyList<CampaignCondition> ReadCampaignConditions()
    {
        var rows=new List<CampaignCondition>();
        if(Campaign==null || Campaign.Complete || !(IsRiverCampaign || IsLakeCampaign)) return rows;
        if(IsLakeCampaign && Campaign.Lake!.Phase==0)
        {
            rows.Add(new("fish","Fish delivered",DeliveredFish,4,"Build a dock on accessible shore and assign a fisher. Fish count after delivery, not while aboard a boat."));return rows;
        }
        bool final=IsRiverCampaign && Campaign.River!.Phase>=2;
        rows.Add(new("population","Residents",Population,final?16:12,"Invite newcomers from People after preparing spare beds and food. Extra residents increase housing and service requirements."));
        rows.Add(new("housing","Residents housed",Housed,Population,"Every resident needs a completed home. Select a resident to inspect or change their assigned home."));
        if(IsRiverCampaign)
        {
            rows.Add(new("east-beds","East-bank beds",EastBankBeds,final?8:4,"Finish homes across the river on the east bank. This counts usable beds, not where residents are standing."));
            if(final) rows.Add(new("east-recreation","East-bank Square visits · 2m",EastBankRecreation,(Population+1)/2,"Half the population must have their latest completed break at an east-bank Square within two in-game minutes. Gardens and halls do not count. Residents visit automatically; keep crossings and nearby seating accessible. Maintain this during assessment."));
        }
        else
        {
            rows.Add(new("rest","Residents recently rested",LakeRested,(Population*3+3)/4,"Three quarters need a completed home rest: the benefit lasts four minutes, or five after an improved-home visit. Travel and unfinished visits do not count."));
            rows.Add(new("recreation","Recreation visits · 2m",LakeRecreation,(Population+1)/2,"Half the population must have completed a recreation visit within two in-game minutes. Squares, seating gardens and halls count here. Keep time and accessible space for breaks."));
        }
        return rows;
    }
    public string CampaignPhaseTitle => IsRiverCampaign ? Campaign!.River!.Phase switch {0=>"Prepare the first neighborhood",1=>"Prove the first neighborhood",2=>"Prepare the final expansion",3=>"Prove the expanded village",_=>"Settlement complete"} :
        IsLakeCampaign ? Campaign!.Lake!.Phase switch {0=>"Open a fishing route",1=>"Prepare the lakeside village",2=>"Prove the lakeside village",_=>"Settlement complete"} : "Village goals";
}
