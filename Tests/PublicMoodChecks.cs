using Inlanders.Simulation;
static class PublicMoodChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewRiverFrontage(relaxed,true);var p=w.People[0];
            w.Food.LastMealRequired=12;w.Food.LastMealBerries=12;w.Food.LastMealChoices=1;
            int single=w.ReadHappiness(p).Score;
            w.Food.LastMealBerries=4;w.Food.LastMealVegetables=4;w.Food.LastMealFish=4;w.Food.LastMealChoices=3;
            if(single!=w.ReadHappiness(p).Score || w.ReadHappiness(p).Choice!=0 || w.LastMealSummary.Contains("credit"))throw new Exception("Public mood still grades diet");
            string saved=w.SaveJson();_ = w.ReadHappiness(p).Reasons;if(saved!=w.SaveJson())throw new Exception("Mood query mutated world");
            for(int i=0;i<1800;i++)w.Tick(.1f);w.Validate();
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}if(w.SaveJson()!=copy.SaveJson())throw new Exception("Mood continuation differs");
            Console.WriteLine($"PASS public mood {relaxed}: optional food variety, actual daily-life feedback and exact continuation");
        }
        HappinessChecks.Run();
    }
}
