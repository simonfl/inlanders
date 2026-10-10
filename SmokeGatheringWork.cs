using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunGatheringWorkSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            var w=World.NewRiverLivelihood();
            var hut=w.Place(new(-4,10),2,BuildingKind.ForagerHut)??throw new Exception("Hut placement");
            Villager? picker=null;
            for(int i=0;i<3000;i++){w.Tick(.05f);picker=w.People.FirstOrDefault(p=>p.Task==Work.Foraging && p.Timer>=.45f && p.Timer<=.55f);if(picker!=null)break;}
            if(picker==null)throw new Exception("No real berry contact");
            AdoptWorld(w);_paused=true;CloseDrawer();await Frames();
            var view=_people[picker.Id];
            if(view.Arm.ToGlobal(new(0,-.36f,0)).DistanceTo(BerryWorkTarget(picker))>.02f || view.Carry.Visible)throw new Exception("Picking misses ripe berry or invents cargo");
            var pose=view.Arm.GlobalTransform;string saved=w.SaveJson();await Frames();
            if(pose!=view.Arm.GlobalTransform || saved!=w.SaveJson())throw new Exception("Paused picking changes");
            _focus=PresentedPerson(picker.Id);_camera.Size=13;UpdateCamera();await Frames();await Capture("artifacts/181-berry-contact.png");
            AdoptWorld(World.LoadJson(saved));_paused=true;await Frames();
            if(pose!=_people[picker.Id].Arm.GlobalTransform)throw new Exception("Reload changes picking");
            picker=_world.People[picker.Id];
            for(int i=0;i<60 && picker.Carried==0;i++)_world.Tick(.05f);
            await Frames();
            if(picker.Carried!=2 || !_people[picker.Id].Carry.Visible || _people[picker.Id].Rig.Position.Length()>.1f)throw new Exception("Real picking cargo/stance transition");
            for(int i=0;i<2500 && !_world.FoundingHasNewFood;i++)_world.Tick(.05f);
            if(!_world.FoundingHasNewFood)throw new Exception("Picking never delivered");
            _world.Validate();GD.Print("PASS berry contact, real cargo/delivery, pause/reload and stance cleanup.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}
