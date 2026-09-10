using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckServiceCoverageUi()
    {
        var previous=_world; var size=GetWindow().Size;
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        async Task PeoplePage() { CloseDrawer(); ToggleDrawer(0); await Frames(); }
        async Task ClickVisible(Button c) { _drawerPages[0].EnsureControlVisible(c); await Frames(); await UiClick(c); await Frames(); }
        try
        {
            foreach(var kind in new[]{BuildingKind.Square,BuildingKind.GatheringHall})
            {
                var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
                foreach(var at in new[]{new Cell(0,0),new(3,0),new(6,0),new(0,6)}) Check(w.Place(at)!=null,"Service homes rejected");
                var venue=w.Place(new(3,6),false,kind) ?? throw new Exception("Service venue rejected");
                AdoptWorld(w); _paused=true; await Frames(); await PeoplePage(); await ClickVisible(_serviceToggle);
                Check(_serviceResidents.Count==8 && _serviceResidents.Values.All(r=>r.Row.Visible),"First visits wrongly credited");
                Check(_serviceResidents[0].Reason.Text.Contains("Next visit due"),"New resident schedule missing");
                for(int i=0;i<3000 && !w.People.Any(p=>p.LeisureSiteId==venue.Id);i++) w.Tick(.1f);
                await Frames(); var visitor=w.People.First(p=>p.LeisureSiteId==venue.Id);
                Check(_serviceResidents[visitor.Id].Venue.Text.Contains("Current outing"),"Active destination mislabeled");
                for(int i=0;i<3000 && !(w.RecentlyRested(visitor) && HasRecreation(visitor));i++) w.Tick(.1f);
                await Frames(); Check(w.RecentlyRested(visitor) && HasRecreation(visitor),"Service fixture never served resident");
                Check(!_serviceResidents[visitor.Id].Row.Visible,"Served resident remains in missing filter");
                _serviceFilter.Select(3); await Frames();
                foreach(int width in new[]{1440,960})
                {
                    GetWindow().Size=new(width,width==960?640:900); await Frames(); await PeoplePage();
                    var row=_serviceResidents[visitor.Id]; string saved=w.SaveJson();
                    await ClickVisible(row.Person); Check(_selectedPerson==visitor.Id,"Service resident link wrong");
                    await PeoplePage(); await ClickVisible(row.Home); Check(_selectedSite==visitor.HomeId,"Service home link wrong");
                    await PeoplePage(); await ClickVisible(row.Venue); Check(_selectedSite==venue.Id,"Service venue link wrong");
                    Check(w.SaveJson()==saved,"Service navigation changed village");
                    await PeoplePage(); _drawerPages[0].EnsureControlVisible(row.Reason); await Frames();
                    Check(row.Reason.Size.X<=_drawer.Size.X && row.Person.Size.X<=_drawer.Size.X,"Coverage row overflow");
                    await Capture($"artifacts/f21i-{kind}-{width}.png");
                }
                _serviceFilter.Select(1); await Frames();
                Check(_serviceResidents.All(r=>r.Value.Row.Visible==!w.RecentlyRested(w.People[r.Key])),"Rest filter incorrect");
                _serviceFilter.Select(2); await Frames();
                Check(_serviceResidents.All(r=>r.Value.Row.Visible==!HasRecreation(w.People[r.Key])),"Recreation filter incorrect");
                if(_serviceResidents.Values.All(r=>!r.Row.Visible)) Check(_serviceCount.Text.Contains("No residents match"),"Empty service filter unexplained");
                SelectPerson(visitor.Id); await Frames();
                _inspectionScroll.EnsureControlVisible(_recreationLink); await Frames(); await UiClick(_recreationLink); await Frames();
                Check(_selectedSite==venue.Id,"Inspector recreation link wrong");
                Check(w.RemoveBuilding(venue.Id),"Service venue removal failed");
                _serviceFilter.Select(3); await PeoplePage(); await Frames();
                Check(_serviceResidents.Values.All(r=>!r.Venue.Visible),"Removed venue leaves stale links");
                var savedWorld=World.LoadJson(w.SaveJson()); AdoptWorld(savedWorld); _paused=true; await Frames();
                Check(!_servicePanel.Visible && _serviceFilter.Selected==0,"World switch retained coverage filter");
                await PeoplePage(); await ClickVisible(_serviceToggle);
                Check(_serviceResidents.Count==8,"Reload coverage rows missing");
            }
            AdoptWorld(new World()); _paused=true; await Frames(); await PeoplePage(); await ClickVisible(_serviceToggle);
            Check(_serviceResidents.Values.All(r=>!r.Home.Visible && !r.Venue.Visible),"Missing services have stale destination links");
            Check(_serviceResidents[0].Reason.Text.Contains("No assigned home") && _serviceResidents[0].Reason.Text.Contains("No open square"),"Missing service guidance absent");
            GD.Print("PASS: actual rest/recreation coverage, filters, square/hall destinations, resident/home/venue navigation, missing services, read-only saves and 960/1440 layout.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
