using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeShortageRecovery()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Click(Button b,int page){_drawerPages[page].EnsureControlVisible(b);await Frames();await UiClick(b);await Frames();}
        Check(_world.Population==20 && _world.People.Any(p=>!p.Fed),"Recovery fixture is not hungry");
        OpenEconomy();UpdateHud();await Frames();await CaptureReviewBundle("actual-shortage");
        await Click(_mealAttention,4);Check(_serviceFilter.Selected==4 && !_serviceGuide.Visible,"Meal investigation shows unrelated guide");
        int hungry=_world.People.First(p=>_world.NeedsMealAttention(p) && _mealSourceLinks[p.Id].Disabled).Id;
        _drawerPages[0].EnsureControlVisible(_mealSourceLinks[hungry]);await Frames();
        Check(_mealSourceLinks[hungry].Text=="No meal pickup assigned","Missing source was invented");await CaptureReviewBundle("shortage-no-pickup");
        OpenEconomy();await Frames();await Click(_foodWorkplaceLink,4);
        var hut=_world.Cottages.First(c=>c.Kind==BuildingKind.ForagerHut);await Click(_queueButtons[hut.Id],1);
        Check(_selectedSite==hut.Id && _productionControls.Visible,"Cannot investigate producer");await CaptureReviewBundle("shortage-producer");
        // Script chooses the measured intervention. UI navigation is real; site orders and waiting use test commands, not human play.
        foreach(var choice in new[]{(BuildingKind.VegetableGarden,new Cell(-7,2)),(BuildingKind.FishingDock,new Cell(3,3))})
        {
            ClearSelection();OpenEconomy();await Frames();await Click(_foodExpansionLink,4);await Click(_kindButtons[choice.Item1],1);
            Check(_placing && _buildKind==choice.Item1,"Food choice did not start planning");
            var plan=_world.Map.Land.SelectMany(c=>Enumerable.Range(0,4).Select(r=>new{c,r})).Where(p=>p.c.X<=2).OrderBy(p=>(p.c.Point-choice.Item2.Point).LengthSquared()).ThenBy(p=>p.c.Z).ThenBy(p=>p.c.X).ThenBy(p=>p.r).First(p=>_world.PlacementProblem(p.c,p.r,choice.Item1)==null);
            Check(_world.Place(plan.c,plan.r,choice.Item1)!=null,"Recovery construction rejected");await Press(Key.Escape);
        }
        double lateHungry=0;
        for(int i=0;i<6000;i++){_world.Tick(.1f);if(i>=3000)lateHungry+=_world.People.Count(p=>!p.Fed)*.1;}
        _world.Validate();Check(lateHungry==0,"Recovery still misses meals in final window");
        OpenEconomy();UpdateHud();await Frames();await CaptureReviewBundle("shortage-recovered");
        SaveWorld();string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved,"Recovery restore differs");
        File.WriteAllText(Path.Combine(_reviewDirectory,"recovery-controls.txt"),"PASS: actual hunger, unassigned pickup, workplace inspection, catalogue intervention choices, command-driven construction/recovery and exact F9. Not uncoached diagnosis or human play.");
    }
}
