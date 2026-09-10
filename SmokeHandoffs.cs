using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async void RunHandoffSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=new World(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Assign(0,Role.Logger); w.Assign(2,Role.Builder); var plan=w.Place(new(3,0))!;
            AdoptWorld(w); _paused=true; CloseDrawer();
            Villager? carrier=null;
            for(int i=0;i<3000 && carrier==null;i++)
            {
                w.Tick(.1f); _clock+=.1f;
                carrier=w.People.FirstOrDefault(p=>p.Carried>0 && p.Task==Work.ToCottage && ArrivalReach(p)>.4f);
            }
            if(carrier==null) throw new Exception("Delivery approach fixture stalled");
            await Frames(); var rig=_people[carrier.Id];
            if(!rig.Carry.Visible || rig.Carry.Position.Y>=.05f || rig.Count!=carrier.Carried) throw new Exception("Actual delivery load did not lower");
            string approach=w.SaveJson();
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=OnGround(carrier.Position.X,carrier.Position.Y); _camera.Size=10; UpdateCamera(); await Frames();
                await Capture($"artifacts/f03b2-delivery-{width}.png");
            }
            _camera.Size=20; UpdateCamera(); await Frames(); await Capture("artifacts/f03b2-delivery-village-960.png");
            var cargoPose=rig.Carry.Transform; var arm=rig.Arm.Transform; await Frames();
            if(cargoPose!=rig.Carry.Transform || arm!=rig.Arm.Transform || approach!=w.SaveJson()) throw new Exception("Paused delivery changed");
            for(int i=0;i<100 && carrier.Carried>0;i++) { w.Tick(.1f); _clock+=.1f; }
            await Frames(); if(rig.Carry.Visible || rig.Carry.GetChildCount()!=0) throw new Exception("Delivered cargo remained visible");
            AdoptWorld(World.LoadJson(approach)); _paused=true; await Frames();
            if(_people[carrier.Id].Carry.Transform!=cargoPose || _people[carrier.Id].Count!=_world.People[carrier.Id].Carried) throw new Exception("Reload changed delivery pose/cargo");
            _world.Cancel(plan.Id); await Frames();
            if(!_people[carrier.Id].Carry.Visible || _world.People[carrier.Id].Carried==0) throw new Exception("Cancellation hid cargo still being returned");
            w=_world;
            foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            for(int i=0;i<3000 && w.People.Any(p=>p.Carried>0);i++) { w.Tick(.1f); _clock+=.1f; }
            w.Assign(2,Role.Builder); var next=w.Place(new(6,0))!;
            for(int i=0;i<3000 && !(w.People[2].Task==Work.ToMaterials && ArrivalReach(w.People[2])>.5f);i++) { w.Tick(.1f); _clock+=.1f; }
            await Frames();
            if(_people[2].Carry.Visible || _people[2].Torso.Rotation.X>=-.1f) throw new Exception("Pickup reach invented goods or did not reach");
            for(int i=0;i<100 && w.People[2].Carried==0;i++) { w.Tick(.1f); _clock+=.1f; await Frames(); }
            await Frames();
            if(!_people[2].Carry.Visible || _people[2].PickupStarted==null || _people[2].Carry.Position.Y>=.05f) throw new Exception("New cargo did not lift from pickup");
            w.Assign(0,Role.Logger);
            for(int i=0;i<3000 && !(w.People[2].Task==Work.Building && w.People[2].Timer%1>.72f);i++) { w.Tick(.1f); _clock+=.1f; }
            await Frames(); var builder=_people[2];
            if(!builder.WorkBoard.Visible || !builder.Hammer.Visible) throw new Exception("Builder work contact missing");
            var hammer=builder.Hammer.ToGlobal(new(0,.1f,-.55f)); var board=builder.WorkBoard.ToGlobal(new(.28f,.55f,-.6f));
            if(hammer.DistanceTo(board)>.12f) throw new Exception("Hammer misses its work board");
            _focus=OnGround(w.People[2].Position.X,w.People[2].Position.Y); _camera.Size=10; UpdateCamera(); await Frames();
            await Capture("artifacts/f03b2-hammer-960.png");
            var hammerPose=builder.Hammer.GlobalTransform; await Frames(); if(hammerPose!=builder.Hammer.GlobalTransform) throw new Exception("Paused hammer moved");
            _soundMuted=false; _effectsVolume=65; ApplyAudioSettings(); ResetWorldAudio();
            void AudioTick() { _paused=false; UpdateAudio(.1f); _paused=true; }
            AudioTick(); float lastHammer=_cueCooldown.GetValueOrDefault(Cue.Hammer);
            while(w.People[2].Timer<1.5f) { w.Tick(.1f); AudioTick(); }
            if(_cueCooldown.GetValueOrDefault(Cue.Hammer)!=lastHammer) throw new Exception("Hammer cue played during windup");
            while(w.People[2].Timer<1.8f) { w.Tick(.1f); AudioTick(); }
            if(_cueCooldown.GetValueOrDefault(Cue.Hammer)<=lastHammer) throw new Exception("Hammer impact cue missing");
            for(int i=0;i<3000 && !next.Complete;i++) { w.Tick(.1f); _clock+=.1f; }
            await Frames(); if(builder.WorkBoard.Visible) throw new Exception("Completed work retained a board");
            if(!w.RequestDemolition(next.Id)) throw new Exception("Demolition pose fixture rejected");
            for(int i=0;i<1000 && !(w.People[2].Task==Work.Demolishing && w.People[2].Timer>.8f);i++) { w.Tick(.1f); _clock+=.1f; }
            await Frames(); if(!builder.WorkBoard.Visible) throw new Exception("Active dismantling lacks work pose");
            w.Assign(2,Role.Unassigned); await Frames(); if(builder.WorkBoard.Visible || builder.Hammer.Visible) throw new Exception("Reassignment retained hammer work");
            w.Validate();
            var village=World.NewCreative();
            foreach(var p in village.People) village.Assign(p.Id,Role.Unassigned);
            village.Place(new(0,0),false,BuildingKind.ForagerHut); village.Place(new(3,0),false,BuildingKind.Farm);
            village.Place(new(6,0),false,BuildingKind.Bakery); village.Place(new(3,6),false,BuildingKind.VegetableGarden);
            village.Place(new(6,6),false,BuildingKind.Sawmill);
            village.Assign(0,Role.Logger); village.Assign(2,Role.Forager); village.Assign(3,Role.Farmer); village.Assign(4,Role.Farmer);
            village.Assign(5,Role.Baker); village.Assign(6,Role.Sawyer);
            AdoptWorld(village); _paused=true; await Frames();
            var resources=new System.Collections.Generic.HashSet<Resource>();
            for(int i=0;i<12000 && resources.Count<6;i++)
            {
                village.Tick(.1f); _clock+=.1f;
                foreach(var p in village.People.Where(p=>p.Carried>0 && DeliveryTask(p.Task) && ArrivalReach(p)>.4f && !resources.Contains(p.Cargo)))
                {
                    resources.Add(p.Cargo); await Frames(); var view=_people[p.Id];
                    if(!view.Carry.Visible || view.Count!=p.Carried || view.Cargo!=p.Cargo || view.Carry.Position.Y>=.05f) throw new Exception("Food/material handoff mismatch");
                }
            }
            if(resources.Count!=6) throw new Exception("Handoff fixture missed a food or timber load");
            village.Validate();
            var lake=World.NewCampaign(7); var dock=lake.Place(new(14,0),true,BuildingKind.FishingDock)!;
            for(int i=0;i<6000 && !dock.Complete;i++) lake.Tick(.1f);
            foreach(var p in lake.People) lake.Assign(p.Id,Role.Unassigned); lake.Assign(0,Role.Fisher);
            AdoptWorld(lake); _paused=true; await Frames();
            for(int i=0;i<6000 && !(lake.People[0].Carried>0 && lake.People[0].Task==Work.ToPantry && ArrivalReach(lake.People[0])>.4f);i++) lake.Tick(.1f);
            await Frames();
            if(lake.People[0].Cargo!=Resource.Fish || lake.People[0].Carried==0 || _people[0].Carry.Position.Y>=.05f || !_people[0].Carry.Visible) throw new Exception("Landed fish handoff failed");
            lake.Validate();
            GD.Print("PASS: real cargo approach/lift, pause/reload/cancel, no ghost goods, hammer contact, completion/demolition/reassignment and 960/1440 captures."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
