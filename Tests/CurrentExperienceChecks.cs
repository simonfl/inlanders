using System.Diagnostics;
using System.Text.Json;
static class CurrentExperienceChecks
{
    public static void Run(string? from=null)
    {
        Directory.CreateDirectory("artifacts/current-experience");
        var cases=new (string Name,Action Execute)[]{
            ("AutomaticWork",AutomaticWorkChecks.Run),
            ("TimberReserve",TimberReserveChecks.Run),
            ("MaterialReserve",MaterialReserveChecks.Run),
            ("FoodFinish",FoodFinishChecks.Run),
            ("GrainReserve",GrainReserveChecks.Run),
            ("CropOrders",CropOrderChecks.Run),
            ("WorkFocus",WorkFocusChecks.Run),
            ("ConstructionBudget",ConstructionBudgetChecks.Run),
            ("DisconnectedDemolition",DisconnectedDemolitionChecks.Run),
            ("ReachableFood",ReachableFoodChecks.Run),
            ("UrgentHarvest",UrgentHarvestChecks.Run),
            ("NewcomerChallenge",NewcomerChallengeChecks.Run),
            ("ChallengeContinuation",ChallengeContinuationChecks.Run),
            ("ChallengeObservation",ChallengeObservationChecks.Run),
            ("ChallengeStatus",ChallengeStatusChecks.Run),
            ("FloodIsland",FloodIslandChecks.Run),
            ("SupplyProducer",SupplyProducerChecks.Run),
            ("TimberJourney",TimberJourneyChecks.Run),
            ("CultivationJourney",CultivationJourneyChecks.Run),
            ("StaffingContinuity",StaffingContinuityChecks.Run),
            ("GatheringEstablishment",GatheringEstablishmentChecks.Run),
            ("ArrivalRecovery",ArrivalRecoveryChecks.Run),
            ("ForagingSiting",ForagingSitingChecks.Run),
            ("GroveRenewal",GroveRenewalChecks.Run),
            ("WoodlandCare",WoodlandCareChecks.Run),
            ("NewGrove",NewGroveChecks.Run),
            ("WoodlandReconsideration",WoodlandReconsiderationChecks.Run),
            ("GroundPreparation",GroundPreparationChecks.Run),
            ("PreparedAddition",PreparedAdditionChecks.Run),
            ("MaterialJourney",MaterialJourneyChecks.Run),
            ("LeisureRoute",LeisureRouteChecks.Run),
            ("TimberRecovery",TimberRecoveryChecks.Run),
            ("FieldCollection",FieldCollectionChecks.Run),
            ("RiverLivelihood",RiverLivelihoodChecks.Run),
            ("RiverLandscape",RiverLandscapeChecks.Run),
            ("GroupRecovery",GroupRecoveryChecks.Run),
            ("FoundedClearing",FoundedClearingChecks.Run),
            ("GroupArrangement",GroupArrangementChecks.Run),
            ("PathStroke",PathStrokeChecks.Run),
            ("QuietSharedPlace",QuietSharedPlaceChecks.Run),
            ("SharedPlaceLayout",SharedPlaceLayoutChecks.Run),
            ("MultipleSharedPlaces",MultipleSharedPlacesChecks.Run),
            ("SharedPlaceScale",SharedPlaceScaleChecks.Run),
            ("FieldExtension",FieldExtensionChecks.Run),
            ("RelocationLivelihood",RelocationLivelihoodChecks.Run),
            ("PublicMood",PublicMoodChecks.Run),
            ("WorkingClearing",WorkingClearingChecks.Run),
            ("HouseholdMove",HouseholdMoveChecks.Run),
            ("PlotRevision",PlotRevisionChecks.Run),
            ("Cultivation",CultivationChecks.Run),
            ("HomePlot",HomePlotChecks.Run),
            ("GroundUse",GroundUseChecks.Run),
            ("Establishment",EstablishmentChecks.Run),
            ("HomeInvitation",HomeInvitationChecks.Run),
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
        if(from!=null)
        {
            int start=Array.FindIndex(cases,c=>c.Name==from);
            if(start<0)throw new ArgumentException("Unknown current-experience starting suite: "+from);
            cases=cases[start..];
        }
        var rows=new List<object>();string status="running",runId=Guid.NewGuid().ToString("N");var startedUtc=DateTime.UtcNow;
        void Report()=>File.WriteAllText("artifacts/current-experience/report.json",JsonSerializer.Serialize(new {
            runId,startedUtc,status,selectedFrom=from,expectedSuites=cases.Length,finishedSuites=rows.Count,
            assembly=typeof(CurrentExperienceChecks).Assembly.ManifestModule.ModuleVersionId,
            prerequisite="ChallengeObservation generates220-arrived for ChallengeStatus; RiverFarmstead generates snapshots used by HomeWaiting and ProductionRecovery",suites=rows
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
