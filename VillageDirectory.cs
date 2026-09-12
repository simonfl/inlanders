using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private OptionButton _rosterFilter = null!, _buildingFilter = null!, _constructionFilter = null!;
    private Label _rosterResults = null!, _buildingResults = null!;
    private VBoxContainer _storageDirectory = null!;
    private Button _yardLink = null!;
    private readonly Dictionary<int,Button> _storageLinks = new();

    private OptionButton DirectoryFilter(VBoxContainer column, string tooltip)
    {
        var filter=new OptionButton { CustomMinimumSize=new(0,34), SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,
            FocusMode=Control.FocusModeEnum.None, TooltipText=tooltip };
        column.AddChild(filter);
        return filter;
    }
    private void MakeRosterFilter(VBoxContainer column)
    {
        _rosterFilter=DirectoryFilter(column,"Filter villagers by current role or idle state.");
        _rosterFilter.AddItem("All villagers",100); _rosterFilter.AddItem("Idle villagers",101);
        foreach(var role in Enum.GetValues<Role>()) _rosterFilter.AddItem(RoleName(role),(int)role);
        _rosterResults=Text("",13,true); column.AddChild(_rosterResults);
    }
    private void MakeBuildingFilter(VBoxContainer column)
    {
        _buildingFilter=DirectoryFilter(column,"Filter building cards and the existing-building list by category.");
        foreach(string category in new[]{"All buildings","Homes","Food","Industry","Storage & crossings","Community"})
            _buildingFilter.AddItem(category);
    }
    private void MakeConstructionFilter(VBoxContainer column)
    {
        _constructionFilter=DirectoryFilter(column,"Show all sites, unfinished construction, or completed buildings.");
        foreach(string state in new[]{"All sites","Under construction","Completed"})
            _constructionFilter.AddItem(state);
        _buildingResults=Text("",13,true); column.AddChild(_buildingResults);
    }
    private static int BuildingCategory(BuildingKind kind) => kind switch
    {
        BuildingKind.Cottage or BuildingKind.Lodge=>1,
        BuildingKind.Orchard or BuildingKind.ForagerHut or BuildingKind.Farm or BuildingKind.VegetableGarden or BuildingKind.Bakery or BuildingKind.FishingDock or BuildingKind.HuntingLodge=>2,
        BuildingKind.Carpenter or BuildingKind.Sawmill or BuildingKind.Quarry=>3,
        BuildingKind.Stockpile or BuildingKind.Bridge=>4,
        _=>5
    };
    private void ResetDirectoryFilters()
    {
        _rosterFilter?.Select(0); _buildingFilter?.Select(0); _constructionFilter?.Select(0);
    }
    private string BuildingStatus(Cottage site)
    {
        if(!site.Complete)
            return $"{site.Construction:P0} built · {site.Delivered}/{site.Required} {site.Material.ToString().ToLowerInvariant()}{(site.RequiredStone>0?$" + {site.DeliveredStone}/{site.RequiredStone} stone":"")} · {PriorityNames[site.Priority]}";
        if (site.WorkPaused) return "Paused · inspect to resume";
        return site.Kind switch
        {
            BuildingKind.Cottage=>"2 beds", BuildingKind.Lodge=>"4 beds",
            BuildingKind.Stockpile=>$"{_world.MaterialAt(site.Id,site.StorageMaterial)}/12 {site.StorageMaterial} · target {site.StorageTarget}",
            BuildingKind.VegetableGarden=>site.Harvest>0?$"{site.Harvest} vegetables ripe":site.Planted?$"Growing · {site.Growth:P0}":"Ready to plant",
            BuildingKind.Orchard=>site.Harvest>0?$"{site.Harvest} fruit ripe":site.Planted?$"{(site.OrchardMature?"Fruit growing":"Trees establishing")} · {site.Growth:P0}":site.OrchardMature?"Mature trees · batch held":"Ready to plant trees",
            BuildingKind.Farm=>site.Harvest>0?$"{site.Harvest} grain ripe":site.Planted?$"Growing · {site.Growth:P0}":"Ready to sow",
            BuildingKind.Bakery=>$"{site.InputGrain} grain in · {site.OutputBread} bread ready",
            BuildingKind.Quarry or BuildingKind.HuntingLodge=>_world.ReadWorkplace(site).State,
            BuildingKind.Sawmill=>$"{site.InputLogs} logs in · {site.OutputPlanks} planks ready",
            BuildingKind.ForagerHut=>$"{_world.People.Count(p=>p.WorkplaceId==site.Id)}/2 foragers working",
            BuildingKind.Bridge=>"Open crossing", _=>"Gathering place"
        };
    }
    private void UpdateVillageDirectory()
    {
        int filter=_rosterFilter.GetSelectedId(), visible=0;
        foreach(var p in _world.People)
        {
            bool show=filter==100 || (filter==101?p.Task==Work.Waiting:(int)p.Role==filter);
            _roster[p.Id].Visible=show;
            if(show) visible++;
        }
        _rosterResults.Text=visible==0?"No matching villagers. Choose All villagers to reset.":$"Showing {visible} of {_world.Population} villagers";
        int category=_buildingFilter.Selected, state=_constructionFilter.Selected;
        foreach(var (kind,button) in _kindButtons) button.Visible=category==0 || BuildingCategory(kind)==category;
        int sites=0;
        foreach(var site in _world.Cottages)
        {
            var button=_queueButtons[site.Id];
            button.Visible=(category==0 || BuildingCategory(site.Kind)==category) &&
                (state==0 || (state==1?!site.Complete:site.Complete));
            button.Text=$"{(site.Id==_selectedSite ? "› " : "")}{BuildingName(site.Kind)} {site.Id}\n{BuildingStatus(site)}";
            button.AutowrapMode=TextServer.AutowrapMode.WordSmart;
            if(button.Visible) sites++;
        }
        _buildingResults.Text=sites==0?"No matching sites. Change the filters or place a building.":$"Showing {sites} of {_world.Cottages.Count} sites";
    }
    private void MakeStorageDirectory(VBoxContainer column)
    {
        _storageDirectory=new(); column.AddChild(_storageDirectory);
        _yardLink=Button("",()=>
        {
            _placing=false; RefreshGhost(); ClearSelection(); _focus=new(_world.Stockpile.X,0,_world.Stockpile.Z); UpdateCamera();
        });
        _yardLink.AutowrapMode=TextServer.AutowrapMode.WordSmart;
        _yardLink.Alignment=HorizontalAlignment.Left; _storageDirectory.AddChild(_yardLink);
    }
    private void UpdateStorageDirectory()
    {
        _logLocations.Text="STORAGE LOCATIONS · SELECT TO VISIT";
        _yardLink.Text=$"Central stores · {_world.YardLogs} logs · {_world.YardPlanks} planks\nLogs: {_world.ReservedLogsAt(null)} reserved · {_world.IncomingLogsAt(null)} arriving\nPlanks: {_world.ReservedMaterialAt(null,Inlanders.Simulation.Resource.Planks)} reserved · {_world.IncomingMaterialAt(null,Inlanders.Simulation.Resource.Planks)} arriving";
        _yardLink.Text+=$"\nStone: {_world.YardStone} central · {_world.ReservedMaterialAt(null,Inlanders.Simulation.Resource.Stone)} reserved · {_world.IncomingMaterialAt(null,Inlanders.Simulation.Resource.Stone)} arriving";
        _yardLink.Text+="\nCentral food: "+string.Join(" · ",World.EdibleKinds.Select(k=>$"{_world.CentralFood(k)} {k.ToString().ToLowerInvariant()}"));
        var stores=_world.Cottages.Where(c=>c.Kind is BuildingKind.Stockpile or BuildingKind.Pantry && c.Complete).ToArray();
        foreach(int id in _storageLinks.Keys.Where(id=>!stores.Any(c=>c.Id==id)).ToArray())
        {
            var old=_storageLinks[id]; _storageDirectory.RemoveChild(old); old.QueueFree(); _storageLinks.Remove(id);
        }
        foreach(var site in stores)
        {
            if(!_storageLinks.TryGetValue(site.Id,out var button))
            {
                int id=site.Id;
                button=Button("",()=>
                {
                    var target=_world.Cottages.FirstOrDefault(c=>c.Id==id);
                    if(target==null) return;
                    _placing=false; RefreshGhost(); SelectBuilding(id);
                    _focus=new(target.Cell.X,0,target.Cell.Z); UpdateCamera();
                });
                button.Alignment=HorizontalAlignment.Left; button.AutowrapMode=TextServer.AutowrapMode.WordSmart;
                button.AddThemeFontSizeOverride("font_size",14);
                _storageDirectory.AddChild(button); _storageLinks[id]=button;
            }
            button.Text=site.Kind==BuildingKind.Pantry ? $"Pantry {site.Id} · {site.PantryFood.Sum()}/24 food · target {site.PantryTarget}\n{World.EdibleKinds.Sum(k=>_world.FoodReservedAt(site.Id,k))} reserved · {_world.FoodIncoming(site.Id)} arriving"+(site.DemolitionRequested?" · closing":"") :
                $"Stockpile {site.Id} · {_world.MaterialAt(site.Id,site.StorageMaterial)}/12 {site.StorageMaterial} · target {site.StorageTarget}\n{_world.ReservedMaterialAt(site.Id,site.StorageMaterial)} reserved · {_world.IncomingMaterialAt(site.Id,site.StorageMaterial)} arriving";
        }
    }
}
