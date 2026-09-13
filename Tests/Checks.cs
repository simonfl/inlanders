using Inlanders.Simulation;
if(args.Contains("--neighborhood-local-services")) {try{NeighborhoodComparison.Recovery(true,localServices:true);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--workplace-food")) {try{WorkplaceFoodChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--workplace-food-comparison")) {try{WorkplaceFoodComparison.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-district")) {try{NeighborhoodComparison.Recovery(true,localServices:true,district:true);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-no-slowdown")) {try{NeighborhoodComparison.Recovery(true,true);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-recovery-capacity")) {try{NeighborhoodComparison.Recovery(true);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-recovery")) {try{NeighborhoodComparison.Recovery();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-landscape")) {try{NeighborhoodComparison.Run(true);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood-comparison")) {try{NeighborhoodComparison.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--welcome-meal")) {try{WelcomeMealChecks.Run();NeighborhoodChecks.Run();PantryChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--neighborhood")) {try{NeighborhoodChecks.Run();SharedWorkChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--shared-work")) {try{SharedWorkChecks.Run();LocalGrainChecks.Run();WorkplaceAssignmentChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--local-grain")) {try{LocalGrainChecks.Run();EconomyChecks.Run();SupplyRouteChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--review-fixture")) {try{ReviewFixtures.Run(args);}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--finale-alternatives")) {try{FinaleAlternativesReview.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--gateways")) {try{GatewayChecks.Run();DecorationChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--terrain-shaping")) {try{TerrainShapingChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--terrain-shaping-review")) {try{TerrainShapingReview.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--bush-relocation")) {try{BushRelocationChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--workplace-assignment")) {try{WorkplaceAssignmentChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--stone-staging-playable")) {try{StoneStagingPlayable.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--stone-storage")) {try{StoneStorageChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--stone-staging")) {try{StoneStagingReview.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--creative-stock")) {try{CreativeStockChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--creative-removal")) {try{CreativeRemovalChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--campaign-review")) {try{CampaignChecks.Run();LakeChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--relocation")) {try{RelocationChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--orchard-playable")) {try{OrchardChecks.Run();PantryProducerChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--orchard")) {try{OrchardComparison.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--restoration")) {try{RestorationChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--civic-budget")) {try{CivicBudgetChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--finale-campaign")) {try{FinaleCampaignChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--finale-decision")) {try{FinaleDecisionChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--woods-campaign")) {try{WoodsCampaignChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--woods-brief")) {try{LivingWoodsBriefChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--quarry-challenge")) {try{QuarryChallengeExperiment.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--quarry-campaign")) {try{QuarryCampaignChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--quarry-brief")) {try{QuarryBriefChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--rotation")) {try{RotationChecks.Run();}catch(Exception e){Console.Error.WriteLine(e);Environment.ExitCode=1;}return;}
if(args.Contains("--comfort-comparison")) { try {ComfortComparison.Run();} catch(Exception e) {Console.Error.WriteLine(e);Environment.ExitCode=1;} return; }
if(args.Contains("--comfort")) { try {ComfortChecks.Run();} catch(Exception e) {Console.Error.WriteLine(e);Environment.ExitCode=1;} return; }

if (args.Contains("--pantry-layouts")) { try { PantryLayoutComparison.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--pantry-producers")) { try { PantryProducerChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--garden-lesson")) { try { GardenLessonChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--happiness")) { try { HappinessChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--meal-variety")) { try { MealVarietyChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }


if (args.Contains("--food-service-balance")) { try { FoodServiceBalance.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--pantry")) { try { PantryChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--meal-service")) { try { MealServiceChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--meals")) { try { MealExperimentChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--seating")) { try { SeatingGardenChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--survey")) { try { ResourceSurveyChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--wildlife")) { try { WildlifeChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--water")) { try { WaterChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--quarry")) { try { QuarryChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }

if (args.Contains("--river")) { try { RiverChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--homes")) { HomeChecks.Run(); return; }
if (args.Contains("--fish")) { FishChecks.Run(); return; }
if (args.Contains("--fish-balance")) { FishingBalance.Run(); return; }
if (args.Contains("--lake")) { try { LakeChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--lake-pressure")) { LakePressureExperiments.Run(); return; }
if (args.Contains("--woodland")) { try { ManagedWoodlandChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--routes")) { try { SupplyRouteChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--local-storage")) { try { LocalStorageExperiments.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--plank-storage")) { try { PlankStorageChecks.Run(); } catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; } return; }
if (args.Contains("--balance")) { BalanceExperiments.Run(); return; }

try
{
static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
static void Steps(World w, int count) { for (int i = 0; i < count; i++) { w.Tick(0.1f); w.Validate(); } }
static void Until(World w, Func<bool> done, string message, int max = 12000)
{
    for (int i = 0; i < max && !done(); i++) Steps(w, 1);
    Check(done(), message);
}
static Cottage Plan(World w, Cell c, bool r = false) => w.Place(c, r) ?? throw new Exception($"Rejected fixture {c}");
static void Roles(World w, Role role) { foreach (var p in w.People) w.Assign(p.Id, role); w.Validate(); }

var village = new World();
var sites = new[] { Plan(village, new(3, 0)), Plan(village, new(6, 0), true), Plan(village, new(3, 6)), Plan(village, new(-5, 6)) };
bool concurrent = false;
Until(village, () => { concurrent |= village.People.Count(v => v.Task != Work.Waiting) >= 6; return village.Housed == 8; }, "Village did not finish");
Check(concurrent, "Did not observe competing concurrent jobs");
Check(sites.All(c => c.Delivered == 6 && c.Incoming == 0 && c.Builder == null), "Final claims leaked");
Console.WriteLine("PASS: eight workers complete four cottages, preserving timber and exclusive ownership on every tick.");

var scarce = new World(1); var low = Plan(scarce, new(3, 0)); var high = Plan(scarce, new(6, 0), true);
scarce.SetPriority(low.Id, 0); scarce.SetPriority(high.Id, 2);
Until(scarce, () => high.Complete, "High-priority site stalled");
Steps(scarce, 100); Check(low.Delivered == 0 && scarce.ReservedStorage == 0, "Scarce supply went to low priority or leaked");
Check(scarce.People.Any(v => v.Status.Contains("Waiting for timber")), "Missing shortage explanation");
Console.WriteLine("PASS: scarce timber is reserved once and routed to the high-priority site; shortage is explained.");

var reprioritized = new World(); Roles(reprioritized, Role.Logger);
Until(reprioritized, () => reprioritized.Stored == reprioritized.InitialLogs, "Could not stock priority fixture");
var older = Plan(reprioritized, new(3, 0)); var newer = Plan(reprioritized, new(6, 0), true);
reprioritized.Assign(0, Role.Builder);
Until(reprioritized, () => reprioritized.People[0].Task == Work.ToMaterials, "No initial shipment reserved");
Check(reprioritized.People[0].SiteId == older.Id, "Equal priority is not oldest first");
reprioritized.SetPriority(newer.Id, 2);
Check(reprioritized.People[0].SiteId == older.Id, "Priority change abandoned committed shipment");
Until(reprioritized, () => reprioritized.People[0].SiteId == newer.Id, "New priority did not affect next claim");
Check(older.Delivered == 2, "Committed delivery lost or extra low priority work claimed");
Until(reprioritized, () => newer.Complete, "Reprioritized site did not finish");
Check(!older.Complete, "Old site ignored new priority");
Console.WriteLine("PASS: live priority changes preserve committed shipments and redirect the next job.");

foreach (var phase in new[] { Work.ToTree, Work.Chopping, Work.ToStockpile, Work.ToMaterials, Work.ToCottage, Work.ToBuild, Work.Building })
{
    var w = new World(); var site = Plan(w, new(3, 0));
    Until(w, () => w.People.Any(v => v.Task == phase), $"Never reached {phase}");
    var p = w.People.First(v => v.Task == phase); int carried = p.Carried;
    w.Assign(p.Id, Role.Unassigned); w.Validate();
    Check(p.Reserved == 0 && p.SiteId == null && p.TreeId == null, $"Reassignment leaked {phase}");
    if (carried > 0) Check(p.Carried == carried && p.Task == Work.ToStockpile, "Reassignment teleported/lost cargo");
    Until(w, () => site.Complete && p.Carried == 0, $"Reassignment deadlocked {phase}");
}
Console.WriteLine("PASS: reassignment in all seven active phases releases claims and physically returns carried logs.");

foreach (var phase in new[] { Work.ToMaterials, Work.ToCottage, Work.ToBuild, Work.Building })
{
    var w = new World(); var site = Plan(w, new(3, 0));
    Until(w, () => w.People.Any(v => v.Task == phase), $"Never reached cancellation phase {phase}");
    Check(w.Cancel(site.Id), "Cancel rejected"); w.Validate();
    Check(w.People.All(v => v.SiteId != site.Id) && site.Incoming == 0 && site.Builder == null, "Cancellation leaked claims");
    var replacement = Plan(w, new(6, 0), true);
    Until(w, () => replacement.Complete, "Cancellation blocked remaining work");
    Roles(w, Role.Logger);
    Until(w, () => w.Stored == w.InitialLogs - Buildings.Get(BuildingKind.Cottage).Cost, "Salvage/cargo not recoverable");
    Check(w.Trees.All(t => !t.Salvage), "Empty salvage still blocks construction");
    Check(w.CanPlace(site.Cell, site.Rotation), "Cancelled footprint not reusable after salvage");
}
Console.WriteLine("PASS: cancellation releases claims, returns cargo, and recovers delivered logs as salvage.");

var staffing = new World(); var target = Plan(staffing, new(3, 0)); Roles(staffing, Role.Builder); Steps(staffing, 100);
bool MealRoutine(Villager v) => v.Task is Work.ToMealSupply or Work.ToMealSeat or Work.EatingMeal or Work.ReturnMeal;
Check(target.Delivered == 0 && staffing.People.All(v => v.Status.Contains("Waiting for timber") || MealRoutine(v)), "Builders fabricated logs or lack explanation outside meal routines");
Roles(staffing, Role.Unassigned); Steps(staffing, 10); Check(staffing.People.All(v => v.Status.Contains("Unassigned") || MealRoutine(v)), "Missing unassigned reason outside meal routines");
for (int i = 0; i < 4; i++) staffing.AdjustWorkers(Role.Logger, 1);
for (int i = 0; i < 4; i++) staffing.AdjustWorkers(Role.Builder, 1);
Until(staffing, () => target.Complete, "Staffing controls did not resume work");
Check(!staffing.Cancel(target.Id), "Completed cottage was destructively cancelled");
Console.WriteLine("PASS: zero-worker conditions, allocation controls, and unassigned recovery.");

int layouts = 0;
foreach (bool rotated in new[] { false, true })
for (int x = -9; x <= 9; x++) for (int z = -8; z <= 8; z++)
{
    var w = new World(); var cell = new Cell(x, z); bool legal = w.CanPlace(cell, rotated);
    var site = w.Place(cell, rotated); Check((site != null) == legal, "Placement disagreement");
    if (site == null) continue;
    Until(w, () => site.Complete, $"Layout failed {cell}/{rotated}"); layouts++;
}
Console.WriteLine($"PASS: {layouts} legal initial layouts finish; invalid layouts rejected.");

for (int seed = 0; seed < 12; seed++)
{
    var w = new World(); var random = new Random(seed);
    for (int tick = 0; tick < 2400; tick++)
    {
        if (tick % 31 == 0) w.Assign(random.Next(8), (Role)random.Next(3));
        if (tick % 73 == 0) w.Place(new(random.Next(-7, 8), random.Next(-6, 7)), random.Next(2) == 0);
        if (tick % 47 == 0 && w.Cottages.Count > 0) w.SetPriority(w.Cottages[random.Next(w.Cottages.Count)].Id, random.Next(3));
        if (tick % 137 == 0 && w.Cottages.Count > 0) w.Cancel(w.Cottages[random.Next(w.Cottages.Count)].Id);
        Steps(w, 1);
    }
}
Console.WriteLine("PASS: 12 seeded stress runs with live placement, reassignment, cancellation, and reprioritization.");
FoodChecks.Run();
EconomyChecks.Run();
ProductionChecks.Run();
WoodlandChecks.Run();
SawmillChecks.Run();
PlacementChecks.Run();
CampaignChecks.Run();
MapChecks.Run();
WaterChecks.Run();
RotationChecks.Run();
ClearingChecks.Run();
PathChecks.Run();
PopulationChecks.Run();
StorageChecks.Run();
VegetableChecks.Run();
OrchardChecks.Run();
RelocationChecks.Run();
CreativeRemovalChecks.Run();
CreativeStockChecks.Run();
LeisureChecks.Run();
GatewayChecks.Run();
DecorationChecks.Run();
HappinessChecks.Run();
MealVarietyChecks.Run();
CameraViewChecks.Run();
VisitorChecks.Run();
GardenLessonChecks.Run();
CreativeChecks.Run();
TerrainChecks.Run();
DemolitionChecks.Run();

RiverChecks.Run();
HomeChecks.Run();
ComfortChecks.Run();
FishChecks.Run();
LakeChecks.Run();
ManagedWoodlandChecks.Run();
SupplyRouteChecks.Run();
PlankStorageChecks.Run();
StoneStorageChecks.Run();
WorkplaceAssignmentChecks.Run();
TerrainShapingChecks.Run();
BushRelocationChecks.Run();
QuarryChecks.Run();
QuarryCampaignChecks.Run();
WoodsCampaignChecks.Run();
FinaleCampaignChecks.Run();
WildlifeChecks.Run();
ResourceSurveyChecks.Run();
SeatingGardenChecks.Run();
MealServiceChecks.Run();
PantryChecks.Run();
PantryProducerChecks.Run();
}
catch(Exception e) { Console.Error.WriteLine(e); Environment.ExitCode=1; }
