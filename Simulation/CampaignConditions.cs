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
        "hall-visits"=>p.LastLeisureTime is float ht && Food.Time-ht<240 && Cottages.Any(c=>c.Id==p.LastLeisureSiteId && c.Kind==BuildingKind.GatheringHall && c.Complete && !c.DemolitionRequested),
        "recreation"=>p.LastLeisureTime is float t && Food.Time-t<120,
        "east-recreation"=>p.LastLeisureTime is float last && Food.Time-last<120 && Cottages.Any(c=>c.Id==p.LastLeisureSiteId && c.Kind==BuildingKind.Square && c.Cell.X>5 && c.Complete && !c.DemolitionRequested),
        _=>false
    };
    public string CampaignResidentReason(Villager p,string key)
    {
        if(key=="housing")return p.HomeId!=null?"Counted: assigned to a home.":"Not counted: no assigned home.";
        if(key=="rest")return (RecentlyRested(p)?"Counted. ":"Not counted. ")+RestSummary(p);
        if(key=="hall-visits")return (ResidentMeetsCampaignCondition(p,key)?"Counted: latest completed break at an available hall, within 4m. ":"Not counted: latest completed break must be at an available hall within 4m. ")+RecreationSummary(p);
        if(ResidentMeetsCampaignCondition(p,key))return $"Counted: latest completed break was {(int)(Food.Time-p.LastLeisureTime!.Value)}s ago; expires after 2m.";
        return (p.LastLeisureTime==null?"No completed break yet.":Food.Time-p.LastLeisureTime>=120?"Latest completed break is older than 2m.":"Latest break was not at an available east-bank Square.")+" "+RecreationSummary(p);
    }
    public IReadOnlyList<CampaignCondition> ReadCampaignConditions()
    {
        var rows=new List<CampaignCondition>();
        if(Campaign==null || Campaign.Complete || !(IsRiverCampaign || IsLakeCampaign || IsQuarryCampaign)) return rows;
        if(IsQuarryCampaign)
        {
            var hall=CampaignHall;
            rows.Add(new("hall","Gathering hall",hall?.Complete==true?1:0,1,"Finish a hall near homes or work. Residents use it automatically between jobs. Leave clear space within two tiles of its entrance."));
            if(Campaign.Quarry!.Phase==0)
            {
                rows.Add(new("hall-planks","Planks delivered to hall",hall?.Delivered??0,8,"A sawyer turns four logs into eight planks. Builders deliver them to the hall; incorporated materials still count."));
                rows.Add(new("hall-stone","Stone delivered to hall",hall?.DeliveredStone??0,12,"The nearby outcrop holds eight stone; the distant one holds thirty-six. Two camps shorten hauling; one distant camp saves six logs. Stone goes to the central yard before builders collect it. Use survey links below."));
            }
            else
            {
                rows.Add(new("housing","Residents housed",Housed,Population,"Keep a completed home for every resident. Extra newcomers increase service requirements."));
                rows.Add(new("hall-visits","Gathering hall visits · 4m",RecentHallVisitors,(Population+1)/2,"Half the population must have their latest completed break at an available hall within four in-game minutes. Squares and gardens do not count here and may attract visits if closer. Keep the hall near everyday trips."));
            }
            return rows;
        }
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
    public string CampaignPhaseTitle => IsQuarryCampaign ? Campaign!.Quarry!.Phase switch {0=>"Build a shared place",1=>"Keep the gathering place working",_=>"Settlement complete"} : IsRiverCampaign ? Campaign!.River!.Phase switch {0=>"Prepare the first neighborhood",1=>"Prove the first neighborhood",2=>"Prepare the final expansion",3=>"Prove the expanded village",_=>"Settlement complete"} :
        IsLakeCampaign ? Campaign!.Lake!.Phase switch {0=>"Open a fishing route",1=>"Prepare the lakeside village",2=>"Prove the lakeside village",_=>"Settlement complete"} : "Village goals";
}
