using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunLoggingSmoke()
    {
        try
        {
            if(OS.GetCmdlineUserArgs().Contains("--loose-stock")){await CheckLooseStock();GetTree().Quit();return;}
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<5;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            w.Assign(0,Role.Logger); AdoptWorld(w); _paused=true; CloseDrawer();
            for(int i=0;i<2000 && w.People[0].Task!=Work.Chopping;i++) { w.Tick(.1f); _clock+=.1f; }
            if(w.People[0].Task!=Work.Chopping) throw new Exception("Logging fixture never started");
            var tree=w.Trees.Single(t=>t.Id==w.People[0].TreeId);
            while(w.People[0].Timer<2.7f) { w.Tick(.1f); _clock+=.1f; }
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=OnGround(tree.Cell.X,tree.Cell.Z); _camera.Size=10; UpdateCamera(); await Frames();
                await Capture($"artifacts/f03b1-after-{width}.png");
            }
            while(w.People[0].Timer<2.8f) { w.Tick(.1f); _clock+=.1f; } await Frames();
            var edge=_people[0].AxeEdge.GlobalPosition; var center=OnGround(tree.Cell.X,tree.Cell.Z);
            float contact=new Vector2(edge.X-center.X,edge.Z-center.Z).Length();
            GD.Print($"AXE CONTACT: edge {contact:0.000} tiles from trunk center, height {edge.Y-center.Y:0.000}.");
            if(contact>.28f || edge.Y-center.Y is <.25f or >1.1f) throw new Exception("Axe misses trunk at impact");
            string standing=w.SaveJson();
            var pose=_people[0].Arm.Transform; await Frames(); if(pose!=_people[0].Arm.Transform) throw new Exception("Paused axe moved");
            while(w.People[0].Timer<3.5f) { w.Tick(.1f); _clock+=.1f; } await Frames();
            await Capture("artifacts/f03b1-windup-960.png");
            while(!tree.Felled) { w.Tick(.1f); _clock+=.1f; } await Frames();
            if(_trees[tree.Id].FallStarted==null) throw new Exception("Tree disappeared without a fall");
            for(int i=0;i<4;i++) { w.Tick(.1f); _clock+=.1f; } await Frames();
            var falling=_trees[tree.Id].Top.Transform; string midfall=w.SaveJson();
            if(!_trees[tree.Id].Top.Visible || _trees[tree.Id].Top.Rotation.Z<.1f || _trees[tree.Id].Top.Scale.X<.89f) throw new Exception("Tree did not tilt at full size");
            await Capture("artifacts/f03b1-falling-960.png"); await Frames();
            _camera.Size=20; UpdateCamera(); await Frames(); await Capture("artifacts/f03b1-falling-village-960.png");
            _camera.Size=10; UpdateCamera(); await Frames();
            if(falling!=_trees[tree.Id].Top.Transform || midfall!=w.SaveJson()) throw new Exception("Paused fall changed");
            for(int i=0;i<6;i++) { w.Tick(.1f); _clock+=.1f; } await Frames();
            if(_trees[tree.Id].Top.Visible || !_trees[tree.Id].Pile.Visible) throw new Exception("Fall did not resolve into timber pile");
            _camera.Size=20; UpdateCamera(); await Frames(); await Capture("artifacts/f03b1-village-960.png");
            AdoptWorld(World.LoadJson(midfall)); _paused=true; await Frames();
            if(_trees[tree.Id].Top.Visible) throw new Exception("Reload replayed a completed felling event");
            AdoptWorld(World.LoadJson(standing)); _paused=true; await Frames();
            _soundMuted=false; _effectsVolume=65; ApplyAudioSettings();
            void AudioTick() { _paused=false; UpdateAudio(.1f); _paused=true; }
            AudioTick(); int sounds=_worldSoundCount;
            while(_world.People[0].Timer<3.5f) { _world.Tick(.1f); AudioTick(); }
            if(_worldSoundCount!=sounds) throw new Exception("Axe sound played during windup");
            while(_world.People[0].Timer<3.8f) { _world.Tick(.1f); AudioTick(); }
            if(_worldSoundCount!=sounds+1 || _soundTraces[0].ChopBeat!=3) throw new Exception("Axe strike did not produce one timed cue");
            UpdateAudio(.1f); if(_worldSoundCount!=sounds+1) throw new Exception("Paused strike emitted sound");
            if(!_world.SetTreePreserved(tree.Cell,true)) throw new Exception("Could not interrupt cut with preservation");
            await Frames(); if(_people[0].Axe.Visible || !_trees[tree.Id].Top.Visible || _trees[tree.Id].Top.Rotation!=Vector3.Zero) throw new Exception("Interrupted cut retained work/fall pose");
            _world.Validate();
            GD.Print("PASS: logging contact and windup, strike-timed sound, paused axe/fall, fall-to-pile transition, load without replay, preservation interruption and 960/1440 captures."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
