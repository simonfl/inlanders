using Inlanders.Simulation;
using System.Text.Json.Nodes;
static class GroundUseChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/ground-use");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewPlayerFounded(relaxed);Check(w.ReadGroundUse().Length==0,"Unplayed village has false wear");
            foreach(var cell in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})w.Place(cell,0,BuildingKind.Cottage);
            w.Place(new(4,3),0,BuildingKind.VegetableGarden);w.Place(new(4,7),0,BuildingKind.VegetableGarden);
            for(int i=0;i<6000;i++)w.Tick(.1f);
            var marks=w.ReadGroundUse();Check(marks.Length>10 && marks.Any(m=>m.Visits>=8) && marks.All(m=>m.Visits<=32 && !w.Map.Water.Contains(m.Cell)),"Ordinary life did not leave bounded dry-ground wear");
            string before=w.SaveJson();marks[0]=new(marks[0].Cell,999);Check(w.SaveJson()==before,"Inspection exposes mutable ground use");
            var copy=World.LoadJson(before);for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Ground-use continuation differs");
            w.SaveFile($"artifacts/ground-use/{relaxed}.json");
            var bad=JsonNode.Parse(before)!;bad["GroundUse"]![0]!["Visits"]=33;
            bool rejected=false;try{World.LoadJson(bad.ToJsonString());}catch(InvalidOperationException){rejected=true;}Check(rejected,"Invalid wear accepted");
            Console.WriteLine($"PASS ground use {relaxed}: {w.ReadGroundUse().Length} actual visited cells, bounded density, detached query and exact save continuation.");
        }
    }
}
