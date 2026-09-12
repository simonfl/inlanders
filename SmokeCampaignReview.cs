using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckCampaignReview()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        System.IO.Directory.CreateDirectory("artifacts/campaign-review");
        _campaignPath="artifacts/campaign-review/book.json";_campaignBook=new();
        var lake=World.NewCampaign(7);
        Check(lake.Place(new(3,4),false,BuildingKind.FishingDock)!=null,"Review dock placement failed");
        lake.Assign(6,Role.Fisher);
        for(int i=0;i<20000 && lake.DeliveredFish<4;i++)lake.Tick(.1f);
        Check(lake.AdvanceLakePhase(),"Review first catch failed");lake.Validate();
        foreach(int width in new[]{960,1440})foreach(var w in new[]{World.NewCampaign(1),lake,World.LoadFile("artifacts/campaign-review/hunger.json")})
        {
            GetWindow().Size=new(width,width==960?640:900);AdoptWorld(w);_paused=true;await Frames();
            string saved=w.SaveJson();await Press(Key.G);await Frames();
            if(w.IsLakeCampaign)
            {
                Check(_goalDashboard.Visible && !_goalArrival.Visible,"Lake phase should use current goal cards");
            }
            else Check(_objective.Text.Contains("Fresh berries delivered") && _objective.Text.Contains("Meals never erase"),"Opening lesson lost cumulative explanation");
            var reading=w.Food.Hunger>0?_tutorialText:w.IsLakeCampaign?_goalPhase:_goalArrival;
            _drawerPages[_goalsPage].EnsureControlVisible(reading);await Frames();
            Check(reading.IsVisibleInTree() && reading.Size.X<=_drawerPages[_goalsPage].Size.X,"Campaign guidance is hidden or exceeds drawer width");
            if(w.Food.Hunger>0)Check(reading.Text.Contains("available berries and access"),"Recovery hint not shown");
            await Capture($"artifacts/campaign-review/level-{w.Campaign!.Level}-{(w.Food.Hunger>0?"hungry":"opening")}-{width}.png");
            await Press(Key.Pagedown);await Press(Key.Pageup);await Press(Key.Escape);
            Check(w.SaveJson()==saved,"Reading campaign guidance changed settlement");
        }
        GD.Print("PASS: intro/lake goals, actual first catch, visible hunger recovery, 960/1440 reading and unchanged saves.");
    }
}
