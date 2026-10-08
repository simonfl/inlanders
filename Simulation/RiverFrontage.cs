using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    // Same people, reserves and rules as the inlet; geography is the comparison.
    public static World NewRiverFrontage(bool relaxed=false,bool inhabited=false,bool landscape=false)
    {
        var w=NewPlayerFounded();w.Founding!.RiverFrontage=true;w.Founding.RiverLandscape=landscape;
        w.Map.Name="La terre au fleuve · River frontage";w.Map.Water.Clear();
        for(int z=w.Map.MinZ;z<=w.Map.MaxZ;z++)for(int x=w.Map.MinX;x<=w.Map.MaxX;x++)
        {
            int edge=z<-8?7:z<-3?8:z<4?10:z<9?9:7;
            var c=new Cell(x,z);if(w.Map.Contains(c) && x>=edge)w.Map.Water.Add(c);
        }
        w.Map.FishingGrounds.Clear();
        w.Map.FishingGrounds.Add(new(){Id=0,Name="Sheltered bend",Cell=new(11,8),Capacity=16,Stock=16,RegrowthPerSecond=.1f});
        w.Map.FishingGrounds.Add(new(){Id=1,Name="Upper river",Cell=new(11,-8),Capacity=16,Stock=16,RegrowthPerSecond=1f/15});
        w.History.Clear();w.History.Add("A broad river bends beside open growing ground and a rising woodlot. Settle near the landing, gather around a shared clearing, or stretch homes along the bank. The same eight neighbors and starting supplies as the inlet; no required arrangement.");
        if(landscape)
        {
            w.Trees.Clear();w._nextTree=0;
            foreach(var at in new[]{new Cell(-6,-10),new(-6,-7),new(-6,-4),new(-7,-1),new(-6,3),new(-7,6),new(-6,9),new(-5,12),new(-2,13),new(2,13)})
                w.Trees.Add(new(){Id=w._nextTree++,Cell=at,Logs=8,Preserved=true});
        }
        if(inhabited)
        {
            w.Founding.PlayerFounded=false;w.Founding.AcrossTheInlet=false;
            foreach(var person in w.People)person.Position=w.YardAccess.Point;
            Cottage Ready(Cell at,BuildingKind kind,int turn=0)
            {
                var b=w.Place(at,turn,kind)??throw new InvalidOperationException($"Frontage {kind} {at}: {w.PlacementProblem(at,turn,kind)}");
                b.Delivered=b.Required;b.Construction=1;b.EstablishmentPending=false;return b;
            }
            foreach(var (at,turn) in landscape?new[]{(new Cell(3,-7),3),(new Cell(3,0),3),(new Cell(4,5),3),(new Cell(4,10),2),(new Cell(0,10),2),(new Cell(-3,7),0)}:new[]{(new Cell(-2,-10),1),(new Cell(3,-7),0),(new Cell(4,-2),1),(new Cell(4,4),1),(new Cell(2,9),2),(new Cell(-3,10),1)})Ready(at,BuildingKind.Cottage,turn);
            foreach(var at in landscape?new[]{new Cell(0,-7),new Cell(0,0)}:new[]{new Cell(1,-6),new Cell(1,0)})
            {var b=Ready(at,BuildingKind.VegetableField,1);b.Planted=true;b.Growth=.6f;}
            Ready(new(8,5),BuildingKind.FishingDock,1);
            if(!w.InviteNewcomers() || !w.InviteNewcomers())throw new InvalidOperationException("Frontage households refused");
            w.ReconcileHomes();foreach(var person in w.People)person.Position=w.Cottages.First(c=>c.Id==person.HomeId).Entrance.Point;
            foreach(var b in w.Cottages)if(!w.ConnectPaths(w.YardAccess,b.Entrance))throw new InvalidOperationException("Frontage approach unavailable");
            w._yardLogs=8;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
            w.Food.InitialBerries=w.Food.Berries=72;
            if(!w.SetCommons(new(0,5)))throw new InvalidOperationException("Frontage shared ground unavailable");
            w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
            w.History.Clear();w.History.Add("Homes follow the river; cultivated strips reach inland toward the woodlot. Twelve neighbors share two fields, a fishing landing and outdoor ground. The landing replaces the inlet kitchen garden: four more logs invested, four fewer in the yard (eight), with the same total timber investment and72 initial food. Fields yield40 vegetables per combined crop; the river supplies actual catches instead of the garden’s8 vegetables. Keep the long frontage, shorten a journey or reshape a yard. Nothing must be built to finish.");
        }
        if(landscape){w.Map.Name="Des terres au fleuve · River farmsteads";w.History.Clear();w.History.Add("Two cultivated strips run from the homes toward retained woodland. The landing and shared ground serve the southern homes. Same twelve neighbors, two full fields, fishing stock, food and total timber as the inhabited river frontage. Arrange a working landscape, keep the woodland edge, or choose another relation between home and land. No required change.");}
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}
