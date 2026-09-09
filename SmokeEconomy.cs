using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckEconomyUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        AdoptWorld(World.NewScenario()); _paused=true; _noticeUntil=0;
        GetWindow().Size=new(960,640); await Frames();
        string saved=_world.SaveJson();
        await Click(_resourceValues[Inlanders.Simulation.Resource.Logs].GetParent<Control>().GetGlobalRect().GetCenter()); await Frames();
        Check(_tabs.CurrentTab==4 && _drawer.Visible,"Resource bar did not open Economy");
        Check(_economyFood.Text.Contains("3 full meals") && _menuButtons[4].Text.Contains("·"),"Food or issue badge missing");
        Check(_world.SaveJson()==saved,"Opening Economy changed simulation");
        await Capture("artifacts/f21d-economy.png");
        int issue=Array.FindIndex(_economyReport!.Issues,i=>i.Build==BuildingKind.ForagerHut);
        await UiClick(_economyIssues[issue]); await Frames();
        Check(_placing && _buildKind==BuildingKind.ForagerHut && _tabs.CurrentTab==1,"Build recommendation did not open placement");
        await Press(Key.Escape); await Press(Key.I); await Frames();
        Check(_tabs.CurrentTab==4,"Economy keyboard shortcut failed");
        var inventory=_economyStocks[Inlanders.Simulation.Resource.Logs];
        _drawerPages[4].EnsureControlVisible(inventory); await Frames();
        Check(inventory.Text.Contains("reserved") && inventory.GetGlobalRect().End.Y<=_drawer.GetGlobalRect().End.Y,"Inventory is not accessible at 960px");
        await Capture("artifacts/f21d-inventory.png");
        var plan=_world.Place(new(3,0))!;
        foreach(var p in _world.People) _world.Assign(p.Id,Role.Unassigned);
        await Frames();
        issue=Array.FindIndex(_economyReport!.Issues,i=>i.Id=="builders");
        await UiClick(_economyIssues[issue]); await Frames();
        Check(_tabs.CurrentTab==0,"Staff recommendation did not open People");
        await UiClick(_allocationButtons[(Role.Builder,1)]); await Frames();
        Check(!_world.ReadEconomy().Issues.Any(i=>i.Id=="builders"),"Resolved warning remained");
        _world.Cancel(plan.Id);
        AdoptWorld(previous); GetWindow().Size=size; await Frames();
        GD.Print("SMOKE PASS: Economy resource-bar/keyboard entry, issue badge, build/staff actions, live resolution, inventory scrolling and read-only navigation.");
    }
}
