using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunQuarrySmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=World.NewLargeMap(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            var camp=w.Place(new(-9,2),false,BuildingKind.Quarry)!;
            w.Place(new(3,0),false,BuildingKind.Sawmill);
            var hall=w.Place(new(0,0),false,BuildingKind.GatheringHall)!;
            w.Assign(0,Role.Logger); w.Assign(1,Role.Logger); w.Assign(2,Role.Builder); w.Assign(3,Role.Builder); w.Assign(4,Role.Sawyer); w.Assign(5,Role.Quarrier);
            AdoptWorld(w); _paused=true; CloseDrawer();
            for(int i=0;i<5000 && !(w.People[5].Task==Work.Quarrying && w.People[5].Timer>.8f);i++) w.Tick(.1f);
            if(w.People[5].Task!=Work.Quarrying) throw new Exception("Quarry fixture stalled");
            await Frames();
            if(!_people[5].Hammer.Visible || _people[5].Carry.Visible || _depositViews.Count!=2) throw new Exception("Missing outcrop/work presentation");
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=OnGround(-8,4); _camera.Size=12; UpdateCamera(); await Frames();
                await Capture($"artifacts/f26b1-quarry-{width}.png");
            }
            for(int i=0;i<100 && w.People[5].Carried==0;i++) w.Tick(.1f);
            await Frames();
            if(_people[5].Cargo!=Inlanders.Simulation.Resource.Stone || _people[5].Carry.GetChildCount()!=2 || !_people[5].Carry.Visible || _depositViews[0].Remaining!=14) throw new Exception("Stone cargo/depletion presentation wrong");
            string hauling=w.SaveJson(); var pose=_people[5].Carry.GlobalTransform; await Frames();
            if(pose!=_people[5].Carry.GlobalTransform || hauling!=w.SaveJson()) throw new Exception("Paused stone cargo changed");
            AdoptWorld(World.LoadJson(hauling)); _paused=true; await Frames(); w=_world; hall=w.Cottages.Single(c=>c.Id==hall.Id);
            if(_people[5].Count!=2 || _depositViews[0].Remaining!=14) throw new Exception("Stone reload visual mismatch");
            for(int i=0;i<6000 && !hall.Complete;i++) w.Tick(.1f);
            if(!hall.Complete) throw new Exception("Rendered hall never completed");
            for(int i=0;i<500 && !w.People.Any(p=>p.Task==Work.Leisure && p.LeisureSiteId==hall.Id);i++) w.Tick(.1f);
            if(!w.People.Any(p=>p.Task==Work.Leisure && p.LeisureSiteId==hall.Id)) throw new Exception("Hall never used");
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=OnGround(hall.Cell.X,hall.Cell.Z); _camera.Size=10; UpdateCamera(); await Frames();
                await Capture($"artifacts/f26b1-hall-{width}.png");
                SelectBuilding(hall.Id); await Frames();
                if(!_siteInfo.Text.Contains("/8 visitors")) throw new Exception("Hall visit capacity missing");
                await Capture($"artifacts/f26b1-hall-ui-{width}.png"); CloseDrawer(); _inspector.Hide();
            }
            GetWindow().Size=new(960,640); _camera.Size=20; UpdateCamera(); await Frames(); await Capture("artifacts/f26b1-hall-village.png");
            SelectBuilding(camp.Id); await Frames();
            if(!_siteInfo.Text.Contains("stone") || !_workplaceControls.Visible || !_cardCosts[BuildingKind.GatheringHall].Text.Contains("12 stone")) throw new Exception("Quarry controls/mixed cost missing");
            OpenEconomy(); await Frames(); if(!_economyStocks.ContainsKey(Inlanders.Simulation.Resource.Stone)) throw new Exception("No stone inventory row");
            BeginPlacement(BuildingKind.Quarry); _hover=new(6,6); RefreshGhost(); await Frames();
            if(_ghostValid) throw new Exception("Quarry preview accepted empty terrain");
            w.Validate();
            GD.Print("PASS: quarry/outcrop/cargo/depletion, pause/reload, built and attended hall, mixed costs, controls, inventory and 960/1440 captures."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
