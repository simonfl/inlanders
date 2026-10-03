using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    public int PreparedPlotRows(Cottage site)=>Math.Max(site.Depth,site.PreparedRows);
    public string? ReshapePlotProblem(int id,int rows,bool requirePaused=true)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(PublicPlace==null || site==null || site.Kind!=BuildingKind.VegetableField || !site.Complete || site.DemolitionRequested)return "Choose a finished cultivated strip.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        if(requirePaused && !site.WorkPaused)return "Pause the field before changing its ground.";
        if(rows<1 || rows>PreparedPlotRows(site))return "Use the prepared extent; make a new strip for additional cultivation.";
        if(rows==site.Depth)return null;
        if(site.Harvest>0)return "Finish collecting the ripe crop before reshaping. Resume the field first.";
        int index=Cottages.IndexOf(site);Cottages.RemoveAt(index);
        try{return PlacementProblem(site.Cell,site.Rotation,site.Kind,rows);}
        finally{Cottages.Insert(index,site);}
    }
    public string? ExtendCultivationProblem(int id,int rows)
    {
        var site=Cottages.FirstOrDefault(c=>c.Id==id);
        if(PublicPlace==null || site==null || site.Kind!=BuildingKind.VegetableField || !site.Complete || site.DemolitionRequested)return "Choose a finished cultivated strip.";
        if(rows<=PreparedPlotRows(site) || rows>8)return "Extend beyond the prepared rows, up to eight rows.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        if(site.Harvest>0)return "Collect the ripe crop before preparing more ground.";
        int index=Cottages.IndexOf(site);Cottages.RemoveAt(index);
        try{return PlacementProblem(site.Cell,site.Rotation,site.Kind,rows);}
        finally{Cottages.Insert(index,site);}
    }
    public bool ExtendCultivation(int id,int rows)
    {
        if(ExtendCultivationProblem(id,rows)!=null)return false;
        var site=Cottages.Single(c=>c.Id==id);int oldRequired=site.Required;
        foreach(var person in People.Where(p=>MoveAffects(p,site)).ToArray())Interrupt(person);
        if(!Creative){site.ExtensionFromRows=site.Depth;site.ExtensionFromPrepared=PreparedPlotRows(site);}
        site.PlotRows=site.PreparedRows=rows;site.Planted=false;site.Growth=0;
        if(!Creative)site.Construction=(float)oldRequired/site.Required;
        RemovePaths(Footprint(site));ManagedWoodland.ExceptWith(Footprint(site));
        foreach(var person in People.Where(p=>p.Route.Count>0))SetRoute(person,person.Destination);
        History.Add($"Cultivated strip {id} extended to {rows} rows. Prepare the extra ground before sowing; stored food stays.");_retry=0;return true;
    }
    private bool CancelCultivationExtension(Cottage site)
    {
        int oldRows=site.ExtensionFromRows,oldPrepared=site.ExtensionFromPrepared;
        int extra=site.Delivered-oldPrepared*2;
        Cell? salvage=null;
        if(extra>0)
        {
            int rows=site.PlotRows,prepared=site.PreparedRows;
            site.PlotRows=oldRows;site.PreparedRows=oldPrepared;
            try{salvage=Map.Land.OrderBy(c=>(c.Point-site.Entrance.Point).LengthSquared()).Cast<Cell?>().FirstOrDefault(c=>PlantingProblem(c!.Value)==null);}
            finally{site.PlotRows=rows;site.PreparedRows=prepared;}
            if(salvage==null)return false;
        }
        foreach(var person in People.Where(p=>MoveAffects(p,site)).ToArray())Interrupt(person);
        site.PlotRows=oldRows;site.PreparedRows=oldPrepared;site.Delivered=oldPrepared*2;
        site.Construction=1;site.ConstructionPaused=false;site.ExtensionFromRows=site.ExtensionFromPrepared=0;
        if(salvage is Cell at){RemovePaths(new[]{at});Trees.Add(new(){Id=_nextTree++,Cell=at,Logs=extra,Material=Resource.Logs,Felled=true,Salvage=true});}
        foreach(var person in People.Where(p=>p.Route.Count>0))SetRoute(person,person.Destination);
        History.Add($"Cancelled extension of cultivated strip {site.Id}; original ground and stored food retained. Extra delivered timber is salvage.");_retry=0;return true;
    }
    public bool ReviseCultivation(int id,int rows)
    {
        if(ReshapePlotProblem(id,rows,false)!=null)return false;
        var site=Cottages.Single(c=>c.Id==id);if(rows==site.Depth)return true;
        bool paused=site.WorkPaused;
        if(!paused && !SetWorkplacePaused(id,true))return false;
        try{return ReshapePlot(id,rows);}
        finally{if(!paused)SetWorkplacePaused(id,false);}
    }
    public bool ReshapePlot(int id,int rows)
    {
        if(ReshapePlotProblem(id,rows)!=null)return false;
        var site=Cottages.Single(c=>c.Id==id);if(rows==site.Depth)return true;
        foreach(var person in People.Where(p=>MoveAffects(p,site)).ToArray())Interrupt(person);
        site.PreparedRows=PreparedPlotRows(site);site.PlotRows=rows;site.Planted=false;site.Growth=0;
        RemovePaths(Footprint(site));ManagedWoodland.ExceptWith(Footprint(site));
        foreach(var person in People.Where(p=>p.Route.Count>0))SetRoute(person,person.Destination);
        History.Add($"Cultivated strip {id} now uses {rows} rows. Timber and stored food retained; sow a fresh crop.");_retry=0;return true;
    }
}
