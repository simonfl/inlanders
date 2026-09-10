using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunHomeSmoke()
    {
        try { await CheckHomeUi(); GetTree().Quit(); }
        catch(Exception e) { GD.PrintErr("HOME SMOKE FAIL: "+e); GetTree().Quit(1); }
    }
    private async Task CheckHomeUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            foreach(var at in new[]{new Cell(0,0),new(3,0),new(6,0),new(0,6),new(3,6)})
                if(w.Place(at)==null) throw new Exception("Home UI fixture placement failed");
            AdoptWorld(w); _paused=true; await Frames();
            var person=w.People[0];
            for(int i=0;i<1500 && person.Task!=Work.Resting;i++) w.Tick(.1f);
            if(person.Task!=Work.Resting) throw new Exception("No visible home visit");
            await Frames(); var pose=_people[0].Head.Rotation; string saved=w.SaveJson(); await Frames();
            if(w.SaveJson()!=saved || pose!=_people[0].Head.Rotation) throw new Exception("Paused rest changed");
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); SelectPerson(0); await Frames();
                if(!_homeNeeds.Text.Contains("Resting at home") || !_homeNeeds.Text.Contains("No open square")) throw new Exception("Home/recreation reasons missing");
                _inspectionScroll.EnsureControlVisible(_homeNeeds); await Frames();
                await Capture($"artifacts/f25a-rest-{width}.png");
                int home=person.HomeId!.Value; await UiClick(_homeLink); await Frames();
                if(_selectedSite!=home || !_siteInfo.Text.Contains(person.Name)) throw new Exception("Home link or resident list missing");
                SelectPerson(0); await Frames();
            }
            int target=w.Cottages.First(h=>w.People.All(p=>p.HomeId!=h.Id)).Id;
            _homeChoice.Select(_homeChoice.GetItemIndex(target)); await Frames();
            await UiClick(_previewHome); await Frames();
            var targetHouse=w.Cottages.Single(h=>h.Id==target);
            if(_selectedPerson!=0 || _focus.X!=targetHouse.Cell.X || _focus.Z!=targetHouse.Cell.Z) throw new Exception("Home preview lost resident context or showed wrong home");
            if(_moveHome.Disabled) throw new Exception("Spare home unavailable");
            int visits=person.RestVisits; await UiClick(_moveHome); await Frames();
            if(person.HomeId!=target || person.Task==Work.Resting || person.RestVisits!=visits) throw new Exception("Home move did not interrupt without credit");
            for(int i=0;i<2000 && person.RestVisits==visits;i++) w.Tick(.1f);
            if(person.RestVisits==visits) throw new Exception("Resident never visited the chosen home");
            await Frames(); SelectPerson(0); _inspectionScroll.EnsureControlVisible(_homeNeeds); await Frames();
            if(!_homeNeeds.Text.Contains("Rested") || w.ReadHappiness(person).Rest!=10) throw new Exception("Completed home visit feedback missing");
            await Capture("artifacts/f25a-moved-home.png");
            var restored=World.LoadJson(w.SaveJson());
            if(restored.People[0].HomeId!=target || restored.People[0].RestVisits!=person.RestVisits) throw new Exception("Home move save lost");
            GD.Print("PASS: visible/paused home rest, resident and home links, spare-bed reassignment, honest recreation feedback and current saves at 1440/960.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
