using System.Diagnostics;
using System.Text.Json;
static class CurrentExperienceChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/current-experience");
        var rows=new List<object>();
        void Run(string name,Action action)
        {
            var clock=Stopwatch.StartNew();string result="passed";
            try {action();} catch {result="failed";throw;}
            finally {
                rows.Add(new {name,result,seconds=clock.Elapsed.TotalSeconds});
                File.WriteAllText("artifacts/current-experience/report.json",JsonSerializer.Serialize(new {
                    assembly=typeof(CurrentExperienceChecks).Assembly.ManifestModule.ModuleVersionId,
                    prerequisite="RiverFarmstead generates snapshots used by HomeWaiting and ProductionRecovery",suites=rows
                },new JsonSerializerOptions{WriteIndented=true}));
            }
        }
        Run("RiverFarmstead",RiverFarmsteadChecks.Run);
        Run("HomeWaiting",HomeWaitingChecks.Run);
        Run("ProductionRecovery",ProductionRecoveryChecks.Run);
        Run("WorkingVillage",WorkingVillageChecks.Run);
        Run("ProvisionedLife",ProvisionedLifeChecks.Run);
        Run("NormalCommons",NormalCommonsChecks.Run);
        Run("HomeYards",HomeYardChecks.Run);
        Run("BatchBread",BatchBreadChecks.Run);
        Run("CourtExperience",CourtExperienceChecks.Run);
        Run("CreativeCourt",CreativeCourtChecks.Run);
    }
}
