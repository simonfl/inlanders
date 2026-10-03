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
