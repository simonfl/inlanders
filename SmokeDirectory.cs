using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckDirectoryUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        try
        {
            AdoptWorld(World.NewCampaign(2)); _paused=true;
            var pile=_world.Place(new(3,-3),false,BuildingKind.Stockpile)!;
            var farm=_world.Place(new(6,-3),false,BuildingKind.Farm)!;
            for(int i=0;i<6000 && (!pile.Complete || !farm.Complete);i++) _world.Tick(.1f);
            Check(pile.Complete && farm.Complete,"Directory fixture did not build");
            var plan=_world.Place(new(0,6)) ?? throw new Exception("Directory construction fixture rejected");
            _world.Assign(7,Role.Unassigned); await Frames();
            string saved=_world.SaveJson();
            foreach(var windowSize in new[]{new Vector2I(1440,900),new Vector2I(960,640)})
            {
                GetWindow().Size=windowSize; await Frames(); await OpenMenu(0);
                _rosterFilter.Select(_rosterFilter.GetItemIndex((int)Role.Unassigned)); await Frames();
                Check(_roster.Where(b=>b.Visible).Count()==_world.People.Count(p=>p.Role==Role.Unassigned),"Unassigned filter wrong");
                Check(_roster[7].Text.Contains("Unassigned"),"Roster omitted roles");
                await UiClick(_roster[7]); Check(_selectedPerson==7,"Filtered selection used wrong identity");
                await OpenMenu(0); _rosterFilter.Select(1); await Frames();
                Check(_roster.Where(b=>b.Visible).Count()==_world.People.Count(p=>p.Task==Work.Waiting),"Idle filter wrong");
                _rosterFilter.Select(_rosterFilter.GetItemIndex((int)Role.Hauler)); await Frames();
                Check(_roster.All(b=>!b.Visible) && _rosterResults.Text.Contains("No matching"),"Empty roster filter unexplained");
                _rosterFilter.Select(0); await Frames();
                _drawerPages[0].EnsureControlVisible(_roster[7]); await Frames();
                await Capture($"artifacts/f21f-people-{windowSize.X}.png");

                await OpenMenu(1); _buildingFilter.Select(2); await Frames();
                Check(_kindButtons[BuildingKind.Farm].Visible && !_kindButtons[BuildingKind.Stockpile].Visible,"Building category failed");
                Check(_queueButtons[farm.Id].Visible && !_queueButtons[pile.Id].Visible,"Directory ignored category");
                _constructionFilter.Select(1); await Frames();
                Check(_queueButtons.Values.All(b=>!b.Visible) && _buildingResults.Text.Contains("No matching"),"Combined empty filter failed");
                _buildingFilter.Select(0); await Frames();
                Check(_queueButtons.Values.Count(b=>b.Visible)==1 && _queueButtons[plan.Id].Visible,"Construction filter wrong");
                await UiClick(_queueButtons[plan.Id]); Check(_selectedSite==plan.Id,"Filtered building identity wrong");
                await OpenMenu(1); _constructionFilter.Select(2); _buildingFilter.Select(4); await Frames();
                Check(_queueButtons[pile.Id].Visible && !_queueButtons[plan.Id].Visible,"Completed storage filter wrong");
                _drawerPages[1].EnsureControlVisible(_queueButtons[pile.Id]); await Frames();
                await Capture($"artifacts/f21f-buildings-{windowSize.X}.png");
                OpenEconomy(); await Frames(); await UiClick(_storageLinks[pile.Id]); await Frames();
                Check(_selectedSite==pile.Id && _inspector.Visible && Math.Abs(_focus.X-pile.Cell.X)<.01f && Math.Abs(_focus.Z-pile.Cell.Z)<.01f,"Stockpile jump failed");
                Check(windowSize.X>=1100 || !_drawer.Visible,"Narrow jump crowded inspector");
                OpenEconomy(); await Frames(); await UiClick(_yardLink); await Frames();
                Check(_selectedSite==-1 && _selectedPerson==-1 && Math.Abs(_focus.X-_world.Stockpile.X)<.01f,"Yard jump failed");
                _drawerPages[4].EnsureControlVisible(_storageLinks[pile.Id]); await Frames();
                await Capture($"artifacts/f21f-storage-{windowSize.X}.png");
                Check(_world.SaveJson()==saved,"Directory navigation changed simulation");
            }
            AdoptWorld(World.NewScenario()); _paused=true; await Frames();
            Check(_rosterFilter.Selected==0 && _buildingFilter.Selected==0 && _constructionFilter.Selected==0,"World switch retained restrictive filters");
            Check(_storageLinks.Count==0 && _roster.All(b=>b.Visible),"World switch retained stale directory entries");
            GD.Print("SMOKE PASS: role/idle filters, empty results, category/construction filters, stable selection IDs, storage/camera jumps, unchanged simulation and 1440/960 navigation.");
        }
        finally { AdoptWorld(previous); GetWindow().Size=size; }
    }
}
