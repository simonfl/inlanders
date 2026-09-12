using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckPeopleKeyboard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Focus(Control target)
        {
            for(int i=0;i<35 && GetViewport().GuiGetFocusOwner()!=target;i++){await Press(Key.Tab);await Frames();}
            Check(GetViewport().GuiGetFocusOwner()==target,"Resident control not reachable: "+target.Name);
        }
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();
            while(w.Beds<24)
            {var cell=w.Map.Land.First(c=>w.PlacementProblem(c,0,BuildingKind.Lodge)==null);Check(w.Place(cell,0,BuildingKind.Lodge)!=null,"Housing fixture failed");}
            while(w.Population<24)Check(w.InviteNewcomers(),"Invitation fixture failed");
            foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);await Frames();
            string saved=w.SaveJson();var camera=_focus;float angle=_angle;
            await Press(Key.V);await Frames();Check(_peopleKeyboard && !_catalogKeyboard,"V did not open resident focus");
            await Focus(_roster[23]);await Frames();
            var rect=_roster[23].GetGlobalRect();var viewport=_drawerPages[0].GetGlobalRect();
            Check(rect.Position.Y>=viewport.Position.Y && rect.End.Y<=viewport.End.Y,"Last resident clipped");
            Check(_hint.Text.Contains(w.People[23].Name) && _hint.Text.Contains(w.People[23].Status),"Focused resident lacks actual activity");
            await Capture($"artifacts/people-keyboard-roster-{width}.png");
            await Press(Key.Q);await Press(Key.R);await Press(Key.Space); // Space inspects, never toggles pause.
            await Frames();Check(_paused && _angle==angle && _focus==camera && _selectedPerson==23,"Inspection leaked world shortcut or selected wrong resident");
            Check(_inspect.Text.Contains(w.People[23].Name.ToUpperInvariant()) && _inspect.Text.Contains(w.People[23].Status),"Inspector lacks current activity");
            Check(saved==w.SaveJson(),"Browsing changed simulation");
            await Press(Key.Right);await Frames();var role=(Role)_jobChoice.GetSelectedId();
            Check(role!=Role.Unassigned && w.People[23].Role==Role.Unassigned,"Role choice assigned without confirmation");
            await Focus(_assignButton);Check(_assignButton.Text.Contains(w.People[23].Name),"Assignment omits resident name");await Capture($"artifacts/people-keyboard-assign-{width}.png");
            await Press(Key.Enter);await Frames();Check(w.People[23].Role==role && w.People.Take(23).All(p=>p.Role==Role.Unassigned),"Assignment affected wrong resident");
            Check(_assignButton.Disabled && GetViewport().GuiGetFocusOwner()==_jobChoice,"Assignment did not return to role choice");
            saved=w.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Assigned role save failed");
            await Press(Key.Escape);await Frames();Check(_peopleKeyboard && _peopleKeyboardPerson<0 && GetViewport().GuiGetFocusOwner()==_roster[23],"Esc did not restore resident");
            await Press(Key.Escape);Check(!_peopleKeyboard && !_drawer.Visible,"Second Esc did not close");
            await Press(Key.V);await Press(Key.Enter);await Frames();
            w.Food.Celebrating=true;await Frames();Check(_jobChoice.Disabled && _assignButton.Disabled,"Supper did not disable assignment");
            Check(GetViewport().GuiGetFocusOwner()==_peopleBackButton,"Disabled assignment left focus stranded");
            await Press(Key.Right);Check(w.People[23].Role==role,"Disabled choice assigned");
            w.Food.Celebrating=false;await Frames();await Press(Key.Escape);
            await Focus(_rosterFilter);
            for(int i=0;i<_rosterFilter.ItemCount && _rosterFilter.GetSelectedId()!=(int)Role.Unassigned;i++)await Press(Key.Right);
            await Frames();await Focus(_roster[22]);w.Assign(22,Role.Logger);await Frames();
            Check(GetViewport().GuiGetFocusOwner()==_rosterFilter,"Disappearing filtered row retained focus");
            saved=w.SaveJson();await Press(Key.Enter);Check(saved==w.SaveJson(),"Stale row activation changed resident");
            await Press(Key.B);await Frames();Check(_catalogKeyboard && !_peopleKeyboard,"Build handoff failed");
            await Press(Key.V);await Frames();Check(!_catalogKeyboard && _peopleKeyboard,"People handoff failed");
            await UiClick(_roster[0]);await Frames();Check(!_peopleKeyboard && _selectedPerson==0,"Mouse takeover failed");
            await Press(Key.V);await Focus(_roster[0]);await Press(Key.Enter);await Frames();
            await UiClick(_peopleBackButton);await Frames();Check(!_peopleKeyboard && _drawer.Visible && _tabs.CurrentTab==0,"Mouse Back lost its action");
            await Press(Key.V);SelectPerson(1);await Frames();Check(!_peopleKeyboard,"External selection retained stale keyboard target");
            await Press(Key.O);await Frames();_viewName.GrabFocus();await Press(Key.V);Check(!_peopleKeyboard,"Typing opened People");_viewName.ReleaseFocus();
            await Press(Key.V);AdoptWorld(World.NewCreative());await Frames();Check(!_peopleKeyboard && _roster.Count==8,"Smaller world retained focus");
        }
        GD.Print("PASS: People keyboard inspect/explicit role assignment, 24-person scrolling, disabled/filtered controls, return paths, typing, mouse/Build handoff, current saves and world reset at 960/1440.");
    }
}
