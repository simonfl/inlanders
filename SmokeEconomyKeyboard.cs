using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckEconomyKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(string key)
        {
            for(int i=0;i<80 && _economyFocusKey!=key;i++){await Press(Key.Tab);await Frames();}
            Check(_economyFocusKey==key,"Economy target not reachable: "+key);
        }
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative(true);
            for(int i=0;i<12;i++)
            {
                var cell=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.Stockpile)==null);
                Check(w.Place(cell,0,BuildingKind.Stockpile)!=null,"Storage fixture failed");
            }
            foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);await Frames();
            string saved=w.SaveJson();await Press(Key.I);await Frames();
            Check(_economyKeyboard && !_peopleKeyboard && !_catalogKeyboard,"Economy entry failed");
            int last=w.Cottages.Last(c=>c.Kind==BuildingKind.Stockpile).Id;string target=$"store:{last}";
            await Focus(target);await Frames();
            var rect=_storageLinks[last].GetGlobalRect();var page=_drawerPages[4].GetGlobalRect();
            Check(rect.Position.Y>=page.Position.Y && rect.End.Y<=page.End.Y,"Storage focus not scrolled into view");
            await Capture($"artifacts/economy-keyboard-storage-{width}.png");
            await Press(Key.Enter);await Frames();
            Check(_economyInspecting && _selectedSite==last && _economyBack.IsVisibleInTree(),"Wrong storage inspected");
            Check(saved==w.SaveJson(),"Read-only storage navigation changed world");
            var camera=_focus;float angle=_angle;await Press(Key.Q);await Press(Key.R);await Press(Key.Pagedown);await Frames();
            Check(_paused && _focus==camera && _angle==angle,"Inspector leaked world controls");
            await Capture($"artifacts/economy-keyboard-inspector-{width}.png");
            await Press(Key.Escape);await Frames();Check(_economyFocusKey==target && GetViewport().GuiGetFocusOwner()==_storageLinks[last],"Return lost storage identity");
            await Focus("idle:7");await Press(Key.Space);await Frames();
            Check(_economyInspecting && _selectedPerson==7 && _paused,"Idle resident selection failed");
            await Press(Key.Escape);await Frames();Check(_economyFocusKey=="idle:7","Return lost resident");
            w.Assign(7,Role.Logger);for(int i=0;i<20 && w.People[7].Task==Work.Waiting;i++)w.Tick(.1f);
            Check(w.People[7].Task!=Work.Waiting,"Idle-removal fixture did not start work");await Frames();Check(_economyFocusKey!="idle:7","Vanished idle row retained focus");
            await Focus(target);
            // Remove a focused storage location and ensure no replacement row is activated.
            w.Cottages.RemoveAll(c=>c.Id==last);await Press(Key.Enter);await Frames();
            Check(!_economyInspecting && _selectedSite<0,"Removed storage opened a replacement");
            w.Validate();saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Economy fixture save failed");
            await Press(Key.Escape);Check(!_economyKeyboard && !_drawer.Visible,"Economy close failed");
            await Press(Key.I);await Press(Key.V);await Frames();Check(_peopleKeyboard && !_economyKeyboard,"Resident shortcut handoff failed");
            await Press(Key.I);await Frames();Check(_economyKeyboard && !_peopleKeyboard,"Economy shortcut handoff failed");
            await Press(Key.B);await Frames();Check(_catalogKeyboard && !_economyKeyboard,"Build shortcut handoff failed");
            await Press(Key.I);await Frames();
            int remaining=w.Cottages.Last(c=>c.Kind==BuildingKind.Stockpile).Id;
            await Focus($"store:{remaining}");await Press(Key.Enter);await Frames();
            await UiClick(_economyBack);await Frames();Check(!_economyKeyboard && _drawer.Visible && _tabs.CurrentTab==4,"Mouse Back lost Economy context");
            await Press(Key.I);await Focus($"store:{remaining}");await Press(Key.Enter);await Frames();
            SelectPerson(0);await Frames();Check(!_economyKeyboard,"External inspection retained stale Economy context");
            await Press(Key.O);await Frames();_viewName.GrabFocus();await Press(Key.I);Check(!_economyKeyboard,"Typing opened Economy");_viewName.ReleaseFocus();
            await Press(Key.I);await Frames();
            AdoptWorld(World.LoadJson(saved));await Frames();Check(!_economyKeyboard && !_economyBack.Visible,"Reload retained Economy focus");
        }
        // Real, changing routes reuse row controls. Focus follows the worker ID.
        var live=World.LoadFile("artifacts/large-village/dense.json");AdoptWorld(live);_paused=true;await Frames();await Press(Key.I);
        await Focus("routes");await Press(Key.Enter);await Frames();
        var route=EconomyControls().FirstOrDefault(r=>r.Key.StartsWith("route:"));Check(route.Button!=null,"No real route in fixture");
        await Focus(route.Key);int worker=int.Parse(route.Key.Split(':')[1]);
        for(int i=0;i<10;i++)live.Tick(.1f);_nextSupplyRefresh=0;RenderSupplyRoutes();await Frames();
        if(EconomyControls().Any(r=>r.Key==route.Key))
        {Check(_economyFocusKey==route.Key,"Live refresh changed focused worker");await Press(Key.Enter);await Frames();Check(_selectedPerson==worker,"Reused route row selected another worker");await Press(Key.Escape);}
        else Check(_economyFocusKey!=route.Key,"Completed route retained stale focus");
        await Press(Key.Escape);live.Validate();
        GD.Print("PASS: Economy keyboard long-list storage/resident inspection, identity restoration/removal, live routes, shortcuts, 960/1440 captures and saves.");
    }
}
