using Inlanders.Simulation;
using System;
using System.Linq;

namespace Inlanders.Development;

public static class ReviewWorlds
{
    // Shared with the existing large-village profile: normal construction and arrivals.
    public static World Dense(World dense)
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        Check(!dense.Creative,"Profile requires normal simulation");
        foreach(var p in dense.People.Where(p=>p.Role==Role.Unassigned).Take(3))dense.Assign(p.Id,Role.Builder);
        while(dense.Cottages.Sum(c=>Buildings.Get(c.Kind).Beds)<32)
        {
            var cell=dense.Map.Land.Where(c=>dense.PlacementProblem(c,0,BuildingKind.Cottage)==null)
                .OrderBy(c=>(c.Point-new Cell(8,4).Point).LengthSquared()).First();
            Check(dense.Place(cell,0,BuildingKind.Cottage)!=null,"Profile housing refused");
        }
        for(int i=0;i<10000 && dense.Beds<32;i++)dense.Tick(.1f);
        Check(dense.Beds>=32,"Profile housing stalled");
        while(dense.Population<32)Check(dense.InviteNewcomers(),"Profile invitations refused: "+dense.InvitationProblem());
        for(int n=0;n<2;n++)
        {
            var cell=dense.Map.Land.Where(c=>dense.PlacementProblem(c,0,BuildingKind.VegetableGarden)==null)
                .OrderBy(c=>(c.Point-new Cell(8,4).Point).LengthSquared()).First();
            Check(dense.Place(cell,0,BuildingKind.VegetableGarden)!=null,"Profile garden refused");
        }
        dense.Assign(24,Role.Farmer);dense.Assign(25,Role.Farmer);dense.Assign(26,Role.Hauler);dense.Assign(27,Role.Hauler);
        for(int i=0;i<1200;i++)dense.Tick(.1f);
        foreach(var c in dense.People.SelectMany(p=>p.Route).Distinct().ToArray())dense.SetPath(c,true);
        foreach(var c in dense.Map.Land.OrderBy(c=>dense.Cottages.Min(b=>(b.Cell.Point-c.Point).LengthSquared())).ThenBy(c=>c.Z).ThenBy(c=>c.X))
        {
            if(dense.Decorations.Count>=120)break;
            if(!dense.Paths.Contains(c))dense.PlaceDecoration(c,(DecorationKind)(dense.Decorations.Count%4));
        }
        Check(dense.Decorations.Count>=80,"Dense fixture lacks decorations");dense.Validate();return dense;
    }
}
