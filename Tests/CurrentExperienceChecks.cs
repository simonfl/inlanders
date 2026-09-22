using System.Diagnostics;
using System.Text.Json;
static class CurrentExperienceChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/current-experience");
        var cases=new (string Name,Action Execute)[]{
            ("LivelihoodSite",LivelihoodSiteChecks.Run),
            ("PlayerFounded",PlayerFoundedChecks.Run),
            ("RiverFarmstead",RiverFarmsteadChecks.Run),
            ("HomeWaiting",HomeWaitingChecks.Run),
            ("ProductionRecovery",ProductionRecoveryChecks.Run),
            ("WorkingVillage",WorkingVillageChecks.Run),
            ("ProvisionedLife",ProvisionedLifeChecks.Run),
            ("GrainRelocation",GrainRelocationChecks.Run),
            ("OvenWorkyard",OvenWorkyardChecks.Run),
            ("Landing",LandingChecks.Run),
            ("MealPlace",MealPlaceChecks.Run),
            ("NormalCommons",NormalCommonsChecks.Run),
            ("HomeYards",HomeYardChecks.Run),
            ("DirectDomestic",DirectDomesticChecks.Run),
            ("YardArrangement",YardArrangementChecks.Run),
            ("VegetableField",VegetableFieldChecks.Run),
            ("BatchBread",BatchBreadChecks.Run),
            ("CourtExperience",CourtExperienceChecks.Run),
            ("CreativeCourt",CreativeCourtChecks.Run),
            ("CultivatedBank",CultivatedBankChecks.Run),
            ("PlaceJourneys",PlaceJourneyChecks.Run),
            ("InletBank",InletBankChecks.Run),
            ("InletChoice",InletChoiceChecks.Run),
            ("GroupedFarmsteads",GroupedFarmsteadChecks.Run),
            ("HamletEnding",HamletEndingChecks.Run),
            ("RelaxedHamlet",RelaxedHamletChecks.Run),
            ("GardenRelocation",GardenRelocationChecks.Run),
            ("HamletLayout",HamletLayoutChecks.Run),
            ("FoodAccess",FoodAccessChecks.Run),
            ("ConstructionStaging",ConstructionStagingChecks.Run),
        };
        var rows=new List<object>();string status="running",runId=Guid.NewGuid().ToString("N");var startedUtc=DateTime.UtcNow;
        void Report()=>File.WriteAllText("artifacts/current-experience/report.json",JsonSerializer.Serialize(new {
            runId,startedUtc,status,expectedSuites=cases.Length,finishedSuites=rows.Count,
            assembly=typeof(CurrentExperienceChecks).Assembly.ManifestModule.ModuleVersionId,
            prerequisite="RiverFarmstead generates snapshots used by HomeWaiting and ProductionRecovery",suites=rows
        },new JsonSerializerOptions{WriteIndented=true}));
        Report();
        try
        {
            foreach(var item in cases)
            {
                var clock=Stopwatch.StartNew();string result="passed";
                try{item.Execute();}catch{result="failed";throw;}
                finally{rows.Add(new{name=item.Name,result,seconds=clock.Elapsed.TotalSeconds});Report();}
            }
            status="completed";
        }
        catch{status="failed";throw;}
        finally{Report();}
    }
}
