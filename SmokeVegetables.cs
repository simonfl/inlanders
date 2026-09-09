using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async Task CheckVegetableUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        try
        {
            AdoptWorld(World.NewScenario()); _paused=true;
            BeginPlacement(BuildingKind.VegetableGarden); await Frames();
            Check(_ghostModelKey=="VegetableGarden" && _buildDescription.Text.Contains("8 vegetables"),"Garden preview/description missing");
            _placing=false; RefreshGhost();
            var garden=_world.Place(new(3,-3),true,BuildingKind.VegetableGarden)!;
            var seen=new System.Collections.Generic.HashSet<int>(); bool cargo=false;
            for(int i=0;i<6000;i++)
            {
                _world.Tick(.1f); _world.Validate();
                if(!garden.Complete) continue;
                int key=garden.Harvest>0?40+garden.Harvest:garden.Planted?10*(1+(int)(garden.Growth*2.9f)):0;
                if(seen.Add(key))
                {
                    await Frames(); Check(_cropViews[garden.Id].Stage==key,"Garden growth mesh stale");
                    _focus=new(2,0,-2); _camera.Size=12; UpdateCamera();
                    if(garden.Harvest>0)
                        Check(_cropViews[garden.Id].Body.GetChildren().Cast<Node3D>().Count(n=>n.GetMeta("standing").AsBool())==garden.Harvest,"Harvested vegetable beds did not clear");
                    await Capture($"artifacts/f05-garden-{key}.png");
                    if(key==46)
                    {
                        string saved=_world.SaveJson(); await Frames(); Check(_world.SaveJson()==saved,"Paused garden advanced");
                        AdoptWorld(World.LoadJson(saved)); _paused=true; garden=_world.Cottages.Single();
                        await Frames(); Check(_cropViews[garden.Id].Stage==46,"Partial vegetable harvest not restored");
                    }
                }
                var carrier=_world.People.FirstOrDefault(p=>p.Cargo==Resource.Vegetables && p.Carried>0);
                if(carrier!=null && !cargo)
                {
                    await Frames(); cargo=true;
                    Check(_people[carrier.Id].Cargo==Resource.Vegetables && _people[carrier.Id].Count==2 && _people[carrier.Id].Carry.Visible,"Vegetable basket missing");
                }
                if(seen.Contains(42) && garden.Harvest==0) break;
            }
            Check(new[]{10,20,30,48,46,44,42}.All(seen.Contains) && cargo,"Missing garden growth/harvest/cargo phase");
            await Frames(); Check(_world.Food.Vegetables>0,"Vegetables not delivered");
            foreach(var windowSize in new[]{new Vector2I(1440,900),new Vector2I(960,640)})
            {
                GetWindow().Size=windowSize; await Frames(); SelectBuilding(garden.Id); await Frames();
                Check(_siteInfo.Text.Contains("vegetables ripe") && WorkplaceRole(garden.Kind)==Role.Farmer,"Garden inspector/staffing missing");
                var veg=_resourceValues[Resource.Vegetables];
                Check(veg.Text==_world.Food.Vegetables.ToString() && veg.GetGlobalRect().End.X<=_topBar.GetGlobalRect().End.X,"Vegetable count missing or overflowing");
                _noticeUntil=0; await Capture($"artifacts/f05-inspector-{windowSize.X}.png");
                OpenEconomy(); await Frames(); _drawerPages[4].EnsureControlVisible(_economyStocks[Resource.Vegetables]); await Frames();
                Check(_economyStocks[Resource.Vegetables].Text.Contains("VEGETABLES"),"Vegetable economy row missing");
            }
            GD.Print("SMOKE PASS: garden preview, rotated growth and progressive harvest, vegetable baskets, partial save/load, pause, six-resource HUD and garden/economy at 1440/960.");
        }
        finally { AdoptWorld(previous); GetWindow().Size=size; }
    }
}
