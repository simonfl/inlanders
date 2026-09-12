using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async Task CheckOrchardUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(int width in new[]{960,1440})foreach(int rotation in new[]{0,1,2,3})
        {
            GetWindow().Size=new(width,width==960?640:900);
            foreach(string phase in new[]{"immature","ripe","picking","carry","repeat"})
            {
                var w=World.LoadFile($"artifacts/orchard-playable/{phase}-{rotation}.json");AdoptWorld(w);_paused=true;
                var site=w.Cottages.Single(c=>c.Kind==BuildingKind.Orchard);
                _focus=OnGround(site.Cell.X,site.Cell.Z);_camera.Size=10;UpdateCamera();SelectBuilding(site.Id);await Frames();
                string save=w.SaveJson();Check(_siteInfo.Text.Contains("fruit") && _siteInfo.Text.Contains("3 minutes"),"Orchard inspector missing establishment guidance");
                Check(_cropViews.ContainsKey(site.Id),"Orchard trees missing");
                Check(_resourceValues[Resource.Fruit].GetParent<Control>().Visible,"Fruit count missing");
                await Capture($"artifacts/orchard-playable/{phase}-{rotation}-{width}.png");
                Check(w.SaveJson()==save,"Orchard inspection mutated world");
                if(phase=="carry")Check(w.People.Any(p=>p.Cargo==Resource.Fruit && p.Carried>0),"Fruit cargo capture has no fruit");
            }
            BeginPlacement(BuildingKind.Orchard);await Frames();
            Check(_ghostModelKey=="Orchard" && _buildDescription.Text.Contains("8 fruit"),"Orchard preview guidance missing");
            await Press(Key.Escape);Check(!_placing,"Orchard preview did not cancel");
        }
        // All conditional resources together exercise the most crowded top bar.
        var crowded=World.LoadFile("artifacts/orchard-playable/mixed.json");
        crowded.Food.CaughtFish=crowded.Food.Fish=1;crowded.Food.HuntedGame=crowded.Food.Game=1;crowded.Validate();
        AdoptWorld(crowded);_paused=true;GetWindow().Size=new(960,640);OpenEconomy();await Frames();
        Check(_speedButton.GetGlobalRect().End.X<=_hud.Size.X-16,"Fruit pushed speed outside viewport");
        _drawerPages[4].EnsureControlVisible(_economyStocks[Resource.Fruit]);await Frames();
        Check(_economyStocks[Resource.Fruit].Text.Contains("FRUIT"),"Economy lost fruit inventory");
        await Capture("artifacts/orchard-playable/economy-960.png");
        var local=World.LoadFile("artifacts/orchard-playable/pantry-fruit.json");AdoptWorld(local);_paused=true;
        var pantry=local.Cottages.Single(c=>c.Kind==BuildingKind.Pantry);SelectBuilding(pantry.Id);_focus=OnGround(pantry.Cell.X,pantry.Cell.Z);_camera.Size=10;UpdateCamera();await Frames();
        Check(_pantryInfo.Text.Contains("Fruit: 2 stored"),"Local fruit stock is missing from inspector");
        await Capture("artifacts/orchard-playable/local-fruit.png");
        GD.Print("PASS: orchard staged models, fruit cargo, four orientations, 960/1440 inspection/previews, crowded resource bar, Economy and read-only state.");
    }
}
