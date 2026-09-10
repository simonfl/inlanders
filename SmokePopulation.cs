using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckPopulationUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        try
        {
            AdoptWorld(World.NewCampaign(2)); _paused=true;
            await Frames(); Check(_inviteButton.Disabled,"Invite enabled without spare housing");
            _world.Place(new(3,-3)); _world.Place(new(6,-3));
            for(int i=0;i<6000 && _world.Beds<12;i++) _world.Tick(.1f);
            Check(_world.Beds==12,"Population fixture did not build");
            foreach(var p in _world.People) _world.Assign(p.Id,Role.Unassigned);
            await Frames(); await OpenMenu(0); await UiClick(_inviteButton); await Frames();
            Check(_world.Population==10 && _people.Count==10 && _roster.Count==10,"Invitation did not create actors and roster");
            _paused=false; UpdateAudio(.1f); _paused=true;
            Check(_soundTraces.ContainsKey(9),"Newcomer audio was not registered");
            await UiClick(_inviteButton); await Frames();
            Check(_world.Population==12 && _inviteButton.Disabled,"Repeated invitation ignored beds");
            string saved=_world.SaveJson();
            AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
            Check(_people.Count==12 && _roster[11].Text.StartsWith(_world.People[11].Name),"Loaded newcomers not visible");
            foreach(var windowSize in new[]{new Vector2I(1440,900),new Vector2I(960,640)})
            {
                GetWindow().Size=windowSize; await Frames();
                await OpenMenu(0); await UiClick(_roster[11]); await Frames();
                Check(_selectedPerson==11 && _personDetails.Visible,"Newcomer inspection failed");
                _jobChoice.Select((int)Role.Farmer); await Frames(); await UiClick(_assignButton); await Frames();
                Check(_world.People[11].Role==Role.Farmer,"Newcomer assignment failed");
                await Capture($"artifacts/f06-newcomer-{windowSize.X}.png");
                await OpenMenu(0); _drawerPages[0].EnsureControlVisible(_inviteButton); await Frames();
                Check(_inviteButton.GetGlobalRect().End.Y<=_drawer.GetGlobalRect().End.Y,"Invitation overflowed drawer");
                await Capture($"artifacts/f06-people-{windowSize.X}.png");
                OpenEconomy(); await Frames();
                Check(_economyFood.Text.Contains("12 portions requested per minute") && _idleLinks.Count==12,"Economy ignored new population");
                _world.Assign(11,Role.Unassigned);
            }
            AdoptWorld(World.NewScenario()); _paused=true; await Frames();
            Check(_roster.Count==8 && _people.Count==8 && _idleLinks.Count==8 && _workerLinks.Count==8,"Switching to smaller village left stale people");
            GD.Print("SMOKE PASS: invitation controls, 12 rendered villagers, saved population, newcomer inspection/assignment, dynamic economy and 1440/960 layout, smaller-world switching.");
        }
        finally { AdoptWorld(previous); GetWindow().Size=size; }
    }
}
