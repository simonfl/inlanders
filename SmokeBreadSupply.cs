using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private async Task CheckBreadSupplyUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(int width in new[]{960,1440})
        {
            AdoptWorld(World.LoadFile("artifacts/finale-poor-bakery.json"));_paused=true;_noticeUntil=0;
            GetWindow().Size=new(width,width==960?640:900);await Frames();
            string saved=_world.SaveJson();
            Check(_world.Food.BakedBread>0 && _world.Food.EatenBread>0 && !_world.CanCelebrate,"Missing actual stalled bread fixture");
            ToggleDrawer(2);await Frames();await UiClick(_supperBreadLink);await Frames();
            Check(_drawer.Visible && _tabs.CurrentTab==4 && _breadDetails.Visible,"Goals bread investigation failed");
            await Frames();
            Check(_breadSummary.Text.Contains("40 more needed") && _breadSummary.Text.Contains("eaten in meals"),"Reserve problem not explained");
            Check(_breadSummary.Text.Contains("central pantry, not directly from farms") && _breadSummary.Text.Contains("before adding ovens"),"Bread advice hides the grain pickup trip");
            Check(_breadSummary.GetGlobalRect().Position.X>=_drawer.GetGlobalRect().Position.X && _breadSummary.GetGlobalRect().End.X<=_drawer.GetGlobalRect().End.X,"Bread summary escapes drawer");
            await Capture($"artifacts/bread-supply-{width}.png");
            _drawerPages[4].ScrollVertical+=180;await Frames();await Capture($"artifacts/bread-supply-guidance-{width}.png");
            int bakery=_world.Cottages.First(c=>c.Kind==BuildingKind.Bakery).Id;
            await UiClick(_breadPlaceLinks[bakery]);await Frames();
            Check(_selectedSite==bakery && _productionControls.Visible,"Bakery investigation did not reach work controls");
            OpenBreadReserve();await Frames();
            int pantry=_world.Cottages.First(c=>c.Kind==BuildingKind.Pantry).Id;
            await UiClick(_breadPlaceLinks[pantry]);await Frames();
            Check(_selectedSite==pantry,"Pantry investigation did not reach target controls");
            Check(_world.SaveJson()==saved,"Bread investigation changed simulation");
        }
        // Observe real production reaching a local pantry, then actual surplus return.
        var w=World.LoadFile("artifacts/finale-poor-bakery.json");
        Cottage local=w.Cottages.First(c=>c.Kind==BuildingKind.Pantry);
        for(int i=0;i<12000 && w.FoodAt(local.Id,Resource.Bread)==0;i++)w.Tick(.1f);
        Check(w.FoodAt(local.Id,Resource.Bread)>0,"Did not observe actual local bread delivery");
        w.Validate();Check(w.BreadSupplySummary().Contains("Haulers return surplus food"),"Local return explanation missing");
        w.SetPantryTarget(local.Id,0);w.SetWorkplacePaused(local.Id,true);w.Assign(19,Role.Hauler);
        bool sawReturn=false;
        for(int i=0;i<6000 && !sawReturn;i++)
        {
            w.Tick(.1f);
            sawReturn=w.People.Any(p=>p.Role==Role.Hauler && p.FoodTransfer && p.Task==Work.ToPantry && p.FoodDestinationId==null && p.Carried>0);
        }
        Check(sawReturn,"Lowered target did not produce actual surplus return");
        w.Validate();string exact=w.SaveJson();Check(World.LoadJson(exact).SaveJson()==exact,"Active food return save changed");
        AdoptWorld(w);_paused=true;OpenBreadReserve();await Frames();
        await Frames();await Capture("artifacts/bread-local-return.png");
        AdoptWorld(World.LoadFile("artifacts/finale-celebrated-local-prebuildFalse-earlyFalse-poorTrue.json"));_paused=true;await Frames();
        Check(!_supperBreadLink.Visible && !_world.BreadSupplySummary().Contains("more needed"),"Completed supper still asks for bread");
        GD.Print("PASS: Goals to bread evidence to bakery/pantry controls at 960/1440; read-only navigation; actual local bread delivery and surplus return; active save and completed supper.");
    }
}
