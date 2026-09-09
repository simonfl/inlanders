using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private Label _economyFood = null!, _economySummary = null!;
    private readonly Dictionary<Resource,Label> _economyStocks = new();
    private readonly List<Button> _economyIssues = new(), _idleLinks = new();
    private EconomyReport? _economyReport;
    private void OpenEconomy()
    {
        if(!_drawer.Visible || _tabs.CurrentTab!=4) ToggleDrawer(4);
        _drawerPages[4].ScrollVertical=0;
    }
    private void MakeEconomyMenu(VBoxContainer column)
    {
        column.AddChild(Text("FOOD RESERVE",12));
        _economyFood=Text("",15,true); column.AddChild(_economyFood);
        column.AddChild(Text("NEEDS ATTENTION",12)); _economySummary=Text("",14,true); column.AddChild(_economySummary);
        for(int i=0;i<12;i++) {
            int index=i; var button=Button("",()=>ActOnEconomyIssue(index));
            button.AutowrapMode=TextServer.AutowrapMode.WordSmart; button.Alignment=HorizontalAlignment.Left;
            button.AddThemeFontSizeOverride("font_size",14); column.AddChild(button); _economyIssues.Add(button);
        }
        column.AddChild(Text("INVENTORY",12));
        column.AddChild(Text("Available = stored minus reserved. Carried goods and workplace buffers are not in storage yet. Unshipped construction demand excludes deliveries already on the way.",14,true));
        foreach(var resource in new[]{Resource.Logs,Resource.Planks,Resource.Berries,Resource.Grain,Resource.Bread,Resource.Vegetables})
        {
            var label=Text("",14,true); column.AddChild(label); _economyStocks[resource]=label;
        }
        _logLocations = Text("", 14, true); column.AddChild(_logLocations);
        MakeStorageDirectory(column);
        column.AddChild(Text("IDLE WORKERS · SELECT TO INSPECT",12));
        _idleContainer = new VBoxContainer(); column.AddChild(_idleContainer);
        column.AddChild(Text("Growing crops, regrowing berries, and a stocked sawmill can leave workers idle normally. Inspect their current task before changing jobs.",14,true));
    }
    private void ActOnEconomyIssue(int index)
    {
        // Re-read so an old button cannot act on a stale recommendation after a world switch.
        if(_economyReport==null || index>=_economyReport.Issues.Length) return;
        var issue=_world.ReadEconomy().Issues.FirstOrDefault(i=>i.Id==_economyReport.Issues[index].Id);
        if(issue==null) return;
        if(issue.Build is BuildingKind kind) { ToggleDrawer(1); BeginPlacement(kind); }
        else if(issue.Staff is Role role) {
            ToggleDrawer(0); _drawerPages[0].EnsureControlVisible(_allocationButtons[(role,1)]);
            Notice("Use + beside " + role + " to assign a worker.");
        }
        else if(issue.Plant) { ToggleDrawer(1); ClearSelection(); if(!_placing || !_plantingTrees) ToggleTreePlanting(); }
    }
    private void UpdateEconomyUi()
    {
        _economyReport=_world.ReadEconomy();
        _economyFood.Text=$"{_economyReport.Meals} full meals in storage\nNext meal in {_economyReport.NextMealSeconds:0}s of village time\n{_world.Population} food per meal · berries → vegetables → bread\nAssumes no new deliveries; grain is not edible.";
        int count=_economyReport.Issues.Length;
        _menuButtons[4].Text=count==0?"Economy":$"Economy · {count}";
        _economySummary.Text=count==0?"No immediate shortages detected.":"Select a message to open the relevant controls.";
        for(int i=0;i<_economyIssues.Count;i++) {
            _economyIssues[i].Visible=i<count;
            if(i<count) _economyIssues[i].Text=_economyReport.Issues[i].Text;
        }
        foreach(var stock in _economyReport.Stocks) {
            _economyStocks[stock.Resource].Text=$"{stock.Resource.ToString().ToUpperInvariant()} · {stock.Available} available\n{stock.Stored} stored · {stock.Reserved} reserved\n{stock.Carried} carried · {stock.AtWorkplaces} at workplaces"+
                (stock.ConstructionNeed>0?$"\n{stock.ConstructionNeed} needed for unshipped construction":"");
            _resourceValues[stock.Resource].GetParent<Control>().TooltipText=$"{stock.Available} available · {stock.Reserved} reserved · {stock.Carried} carried. Click for Economy [I].";
        }
        UpdateStorageDirectory();
        foreach(var p in _world.People) {
            _idleLinks[p.Id].Visible=p.Task==Work.Waiting;
            _idleLinks[p.Id].Text=$"{p.Name} · {p.Role}\n{p.Status}";
        }
    }
}
