using System;
using System.Linq;
namespace Inlanders.Simulation;
public sealed partial class World
{
    // Same people, reserves and rules as the inlet; geography is the comparison.
    public static World NewRiverFrontage(bool relaxed=false,bool inhabited=false)
    {
        var w=NewPlayerFounded();w.Founding!.RiverFrontage=true;
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
        if(inhabited)
        {
            w.Founding.PlayerFounded=false;w.Founding.AcrossTheInlet=false;
            foreach(var person in w.People)person.Position=w.YardAccess.Point;
            Cottage Ready(Cell at,BuildingKind kind,int turn=0)
            {
                var b=w.Place(at,turn,kind)??throw new InvalidOperationException($"Frontage {kind} {at}: {w.PlacementProblem(at,turn,kind)}");
                b.Delivered=b.Required;b.Construction=1;b.EstablishmentPending=false;return b;
            }
            foreach(var (at,turn) in new[]{(new Cell(-2,-10),1),(new Cell(3,-7),0),(new Cell(4,-2),1),(new Cell(4,4),1),(new Cell(2,9),2),(new Cell(-3,10),1)})Ready(at,BuildingKind.Cottage,turn);
            foreach(var at in new[]{new Cell(1,-6),new Cell(1,0),new Cell(0,5)})
            {var b=Ready(at,at.Z==5?BuildingKind.VegetableGarden:BuildingKind.VegetableField,at.Z==5?0:1);b.Planted=true;b.Growth=.6f;}
            if(!w.InviteNewcomers() || !w.InviteNewcomers())throw new InvalidOperationException("Frontage households refused");
            w.ReconcileHomes();foreach(var person in w.People)person.Position=w.Cottages.First(c=>c.Id==person.HomeId).Entrance.Point;
            foreach(var b in w.Cottages)if(!w.ConnectPaths(w.YardAccess,b.Entrance))throw new InvalidOperationException("Frontage approach unavailable");
            w._yardLogs=12;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(c=>c.Delivered)+w._yardLogs;
            w.Food.InitialBerries=w.Food.Berries=72;
            w.Founding.StartingBuildings=w.Cottages.Select(c=>new StartingBuilding{Id=c.Id,Cell=c.Cell,Rotation=c.Rotation,Kind=c.Kind}).ToList();
            w.History.Clear();w.History.Add("Homes follow the river; cultivated strips reach inland toward the woodlot. The same twelve neighbors, two fields, kitchen garden and reserves as the inlet. Keep the long frontage, bring a household closer to work, or open another shared clearing. Nothing must be built to finish.");
        }
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
}
