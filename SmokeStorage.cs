using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckStorageUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        try
        {
            AdoptWorld(new World(40)); _paused=true;
            BeginPlacement(BuildingKind.Stockpile); await Frames();
            Check(_ghostModelKey=="Stockpile" && _buildDescription.Text.Contains("12 logs"),"Stockpile preview/description missing");
            _placing=false; RefreshGhost();
            var pile=_world.Place(new(3,0),true,BuildingKind.Stockpile) ?? throw new Exception("Storage fixture rejected");
            for(int i=0;i<6000 && !pile.Complete;i++) _world.Tick(.1f);
            Check(pile.Complete,"Stockpile did not complete");
            foreach(var p in _world.People) _world.Assign(p.Id,Role.Unassigned);
            for(int i=0;i<1500 && _world.People.Any(p=>p.Carried>0);i++) _world.Tick(.1f);
            await Frames(); SelectBuilding(pile.Id); await Frames();
            Check(_storageControls.Visible && _siteInfo.Text.Contains("Log storage"),"Storage inspector missing");
            await UiClick(_targetMore); Check(pile.LogTarget==8,"Target increase failed");
            await UiClick(_targetLess); Check(pile.LogTarget==6,"Target decrease failed");
            await UiClick(_staffPlus); await Frames();
            Check(_world.People.Any(p=>p.Role==Role.Hauler),"Stockpile staffing did not assign a hauler");
            _world.SetLogTarget(pile.Id,12);
            for(int i=0;i<5000 && pile.StoredLogs<12;i++)
            {
                _world.Tick(.1f); _world.Validate();
                if(i%100==0) await Frames();
            }
            await Frames();
            Check(pile.StoredLogs==12 && _cottages[pile.Id].Stage==312,"Stockpile log meshes did not refresh");
            Check(_cottages[pile.Id].Body.RotationDegrees.Y==90,"Stockpile rotation failed");
            Check(_lastStored==_world.YardLogs,"Main yard visual duplicated local inventory");
            string saved=_world.SaveJson();
            AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
            pile=_world.Cottages.Single(c=>c.Kind==BuildingKind.Stockpile);
            Check(_cottages[pile.Id].Stage==312,"Loaded stockpile did not show saved logs");
            _focus=new(2,0,0); _camera.Size=15; UpdateCamera(); _noticeUntil=0;
            foreach(var windowSize in new[]{new Vector2I(1440,900),new Vector2I(960,640)})
            {
                GetWindow().Size=windowSize; await Frames(); SelectBuilding(pile.Id); await Frames();
                _inspectionScroll.EnsureControlVisible(_targetMore); await Frames();
                Check(_targetMore.GetGlobalRect().End.Y<=_inspector.GetGlobalRect().End.Y,"Storage target overflowed inspector");
                await Capture($"artifacts/f07-stockpile-{windowSize.X}.png");
                OpenEconomy(); await Frames(); _drawerPages[4].EnsureControlVisible(_logLocations); await Frames();
                Check(_logLocations.Text.Contains($"Stockpile {pile.Id}: 12/12"),"Per-location economy missing");
                await Capture($"artifacts/f07-economy-{windowSize.X}.png");
            }
            SelectBuilding(pile.Id); await Frames();
            for(int i=0;i<6;i++) await UiClick(_targetLess);
            Check(pile.LogTarget==0,"Zero target control failed");
            for(int i=0;i<5000 && (pile.StoredLogs>0 || _world.People.Any(p=>p.Task is Work.ToHaulPickup or Work.ToHaulDrop));i++)
                _world.Tick(.1f);
            await Frames(); Check(pile.StoredLogs==0 && _cottages[pile.Id].Stage==300,"Drained pile still showed logs");
            GD.Print("SMOKE PASS: stockpile preview/build/rotation, targets and staffing, visible fill/drain, local and yard totals, save/load and 1440/960 stockpile/economy controls.");
        }
        finally { AdoptWorld(previous); GetWindow().Size=size; }
    }
}
