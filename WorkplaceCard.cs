using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private PanelContainer _workCard=null!;
    private Label _workCardText=null!;
    private Button _workCardPause=null!,_workCardMove=null!,_workCardDetails=null!,_workCardWorker=null!,_workCardDiner=null!,_workCardCancel=null!;
    private Button _workCardFurnish=null!,_workCardYard=null!,_workCardWatchPlace=null!;

    private int _workCardSite=-1;
    private World? _workCardWorld;
    private float _nextWorkCard;
    private bool _workCardRight, _homeOptions, _homeMore;
    private Button _homeMoreButton=null!;
    private Button _homeOptionsButton=null!,_workCardSupply=null!;
    private bool UsesWorkCard(Cottage site)=>_world.PublicPlace!=null || (_world.Founding!=null || _world.IsArrangementCourt) && site.Complete &&
        (World.ProductionOutput(site.Kind)!=null || site.Kind is BuildingKind.Carpenter or BuildingKind.Pantry);
    private Villager? CardWorker()=>_world.People.FirstOrDefault(p=>p.WorkplaceId==_workCardSite || p.SiteId==_workCardSite || p.HomeId==_workCardSite || p.LeisureSiteId==_workCardSite);
    private Villager? CardDiner()=>_world.People.FirstOrDefault(p=>p.Meal is { } m && (m.Reserved || m.Carrying) && m.SourceId==_workCardSite);
    private void ShowWorkplaceCard(int id)
    {
        ClearSelection();CloseDrawer();_homeOptions=false;_homeMore=false;_workCardSite=_selectedSite=id;_workCardWorld=_world;_nextWorkCard=0;
        var site=_world.Cottages.First(c=>c.Id==id);
        _workCardRight=_camera.UnprojectPosition(BuildingPosition(site)).X<_hud.Size.X/2;RefreshSelection();
    }
    private void MakeWorkplaceCard()
    {
        _workCard=HudPanel(_hud);var column=new VBoxContainer();_workCard.AddChild(column);
        _workCardText=Text("",14,true);_workCardText.CustomMinimumSize=new(306,0);column.AddChild(_workCardText);MakeHouseholdUi(column);
        var people=new HBoxContainer();column.AddChild(people);
        void Watch(Villager? person){if(person==null)return;int origin=_workCardSite;_workCardSite=-1;ShowDailyLife(person.Id,origin);_followPerson=true;}
        _workCardWorker=Button("Watch work",()=>Watch(CardWorker()));people.AddChild(_workCardWorker);
        _workCardWatchPlace=Button("Watch this place",()=>{
            var home=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(home==null)return;
            CloseDrawer();
            if(Buildings.Get(home.Kind).Beds>0)FrameHomeYard(home,home.YardSide,false);
            else{_focus=BuildingPosition(home);_camera.Size=home.Kind is BuildingKind.Farm or BuildingKind.VegetableField?17:14;_followPerson=false;_watchOrbit=false;UpdateCamera();}
            ToggleWatch();
        });column.AddChild(_workCardWatchPlace);
        _workCardWatchPlace.TooltipText="Stay with this place as people come and go. H or Esc returns to management. Pause and speed stay as you set them.";
        _workCardDiner=Button("Follow meal",()=>Watch(CardDiner()));people.AddChild(_workCardDiner);
        var actions=new HBoxContainer();column.AddChild(actions);
        _workCardPause=Button("Pause",()=>{var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site!=null){if(site.Complete)_world.SetWorkplacePaused(site.Id,!site.WorkPaused);else _world.SetConstructionPaused(site.Id,!site.ConstructionPaused);}_nextWorkCard=0;});actions.AddChild(_workCardPause);
        _workCardMove=Button("Move",ChooseMoveIntent);actions.AddChild(_workCardMove);
        _workCardDetails=Button("Details",()=>{int id=_workCardSite;_workCardSite=-1;SelectBuilding(id);});actions.AddChild(_workCardDetails);
        actions.AddChild(Button("×",ClearSelection));
        _workCardFurnish=Button("Furnish yard",()=>{var home=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(home!=null){if(home.ImprovementRequested || home.PlannedYard)_world.CancelImprovement(home.Id);else _world.RequestImprovement(home.Id);}_nextWorkCard=0;});column.AddChild(_workCardFurnish);
        _workCardYard=Button("Arrange yard",BeginYardPreview);column.AddChild(_workCardYard);MakeYardPreview(column);
        var arrangement=new GridContainer{Columns=2};column.AddChild(arrangement);
        _workCardMove.Reparent(arrangement);_workCardYard.Reparent(arrangement);
        foreach(var control in new[]{_workCardMove,_workCardYard})control.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;
        _workCardSupply=Button("Show food stores",ToggleFoodMap);column.AddChild(_workCardSupply);
        MakePlotRevision(column);
        _homeOptionsButton=Button("Change this home",()=>{_homeOptions=!_homeOptions;_homeMore=false;_nextWorkCard=0;});column.AddChild(_homeOptionsButton);
        MakeHomeTurn(column);MakePlacePath(column);MakeHomeAppearance(column);
        _homeMoreButton=Button("Household & details",()=>{_homeMore=!_homeMore;_nextWorkCard=0;});column.AddChild(_homeMoreButton);
        _householdChange.Reparent(column);_workCardDetails.Reparent(column);
        MakeHomeInvitation(column);MakePlaceJourneys(column);
        _workCardCancel=Button("Cancel this construction",()=>{if(_world.Cancel(_workCardSite)){ClearSelection();CreateActors();RenderActors(0);RebuildQueue();}});column.AddChild(_workCardCancel);_workCard.Hide();
    }
    private void RenderWorkplaceCard()
    {
        RenderHomeTurn();
        if(_workCardWorld!=_world)_workCardSite=-1;
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);
        if(site!=null && _yardPreviewHome==site.Id && _selectedSite==site.Id && !_atMainMenu && !_placing && !_watching && !_drawer.Visible && !_inspector.Visible)
        {_workCard.Hide();RenderYardPreview(site);return;}
        bool show=_turnHome<0 && !(_showFoodMap && _world.PublicPlace!=null) && _householdFrom<0 && site!=null && _selectedSite==site.Id && !_atMainMenu && !_placing && !_watching && !_drawer.Visible && !_inspector.Visible;
        _workCard.Visible=show;if(!show || site==null){StopYardPreview();EndPlotRevision();return;}
        _workCard.Size=new(330,0);
        _workCard.Position=new(Mathf.Max(0,_workCardRight?_hud.Size.X-346:Mathf.Min(16,_hud.Size.X-330)),92);
        if(_uiTime<_nextWorkCard)return;_nextWorkCard=_uiTime+.3f;
        var worker=CardWorker();var diner=CardDiner();
        RenderPlotRevision(site);RenderHouseholdUi(site);RenderHomeAppearance(site);RenderPlacePath(site);
        bool publicHome=_world.PublicPlace!=null && site.Complete && Buildings.Get(site.Kind).Beds>0;
        _turnHomeButton.Visible=publicHome && _homeOptions && _yardPreviewSide<0;
        _homeOptionsButton.Visible=publicHome && _yardPreviewSide<0;_homeOptionsButton.Text=_homeOptions?"Back to household life":"Change this home";
        _homeMoreButton.Visible=publicHome && _homeOptions && _yardPreviewSide<0;
        _homeMoreButton.Text=_homeMore?"Fewer options":"Household & details";
        string detail;
        if(!site.Complete)detail=$"{site.Delivered}/{site.Required} materials · {site.Construction:P0} built"+(site.RequiredStone>0?$"\n{site.DeliveredStone}/{site.RequiredStone} stone":"")+"\n"+(site.ConstructionPaused?"Construction paused; supplies stay here.":"Shared workers build when supplies are available.");
        else if(Buildings.Get(site.Kind).Beds>0)detail=$"{_world.People.Count(p=>p.HomeId==site.Id)}/{Buildings.Get(site.Kind).Beds} neighbors live here";
        else if(Buildings.Get(site.Kind).RecreationSlots>0)detail=$"{_world.People.Count(p=>p.LeisureSiteId==site.Id)} neighbors visiting\nA place for ordinary breaks.";
        else {var report=_world.ReadWorkplace(site);detail=report.State+"\n"+(worker!=null?worker.Name+": "+worker.Status:report.Detail.Split('\n')[0]);}
        _workCardSupply.Visible=_world.PublicPlace!=null && site.Complete && _reshapingPlot<0 && (_world.IsWorkplaceFoodStore(site) || site.Kind is BuildingKind.Farm or BuildingKind.Pantry);
        _workCardSupply.Text=_showFoodMap?"Hide food stores":"Show food stores";
        if(site.Complete && _world.PublicPlace!=null && _world.ReadWorkplace(site).RestingForFood)detail=$"Food work is resting\n{_world.EdibleStored} portions stored for {_world.Population} neighbors. Shared workers return as supplies fall.";
        if(site.Kind==BuildingKind.VegetableField)detail+=$"\n3 × {site.Depth} tiles · {World.VegetableYield(site)} vegetables/crop";
        if(_reshapingPlot==site.Id)detail="Choose how much ground to cultivate.";
        if(site.PlannedYard)detail+="\n"+_world.PlannedYardSummary(site);
        _workCardText.Text=BuildingName(site.Kind).ToUpperInvariant()+"\n"+detail;
        _workCardYard.Visible=(!publicHome || _homeOptions) && _yardPreviewSide<0 && _world.PublicPlace!=null && site.Complete && Buildings.Get(site.Kind).Beds>0;
        _workCardYard.Text="Arrange yard";
        _workCardYard.Disabled=site.ImprovementRequested || site.DemolitionRequested;
        _workCardYard.TooltipText="Preview the four sides before choosing. Residents use the chosen ground for quiet work and nearby meals after furnishing. Meals use the shorter eligible trip to home or shared ground. Existing furniture moves free.";
        _workCardFurnish.Visible=(!publicHome || _homeOptions && (site.ImprovementRequested || site.PlannedYard)) && _yardPreviewSide<0 && (site.Complete || site.PlannedYard) && Buildings.Get(site.Kind).Beds>0 && !site.Improved;
        _workCardFurnish.Text=site.PlannedYard?"Cancel planned yard":site.ImprovementRequested?"Cancel furnishing":_world.Creative?"Furnish yard · free":$"Furnish yard · {World.ComfortCost(site)} planks";
        _workCardFurnish.Disabled=_yardPreviewSide>=0 || !site.ImprovementRequested && !site.PlannedYard && _world.ImprovementProblem(site.Id)!=null;
        _workCardFurnish.TooltipText=site.PlannedYard?"Cancel the future yard order; keep the house. No furnishing supplies are sent before the home is occupied.":_world.ImprovementProblem(site.Id)??"Shared workers deliver planks and furnish the chosen ground beside this home.";
        if(site.Complete && Buildings.Get(site.Kind).Beds>0 && !_homeOptions)_workCardText.Text+="\n"+HomeCardOutcome(site);
        _workCardWorker.Text=Buildings.Get(site.Kind).Beds>0?"Follow resident":!site.Complete?"Follow builder":"Follow worker";
        _workCardCancel.Text=site.ExtensionFromRows>0?"Cancel extension · keep original field":"Cancel this construction";
        _workCardCancel.Visible=!site.Complete && !site.DemolitionRequested;
        if(_reshapingPlot!=site.Id && site.Complete && (_world.IsWorkplaceFoodStore(site) || site.Kind==BuildingKind.Pantry))
        {
            int available=World.EdibleKinds.Sum(k=>_world.FoodAvailableAt(site.Id,k));
            _workCardText.Text+=$"\n\n{available} meal portions available here\n"+(diner!=null?diner.Name+" is collecting or carrying a meal.":"No meal collection in progress.");
        }
        _workCardDiner.Visible=Buildings.Get(site.Kind).Beds==0 && (_world.PublicPlace==null || _placeJourneySite==site.Id);
        _workCardWatchPlace.Visible=(!publicHome || !_homeOptions) && _world.PublicPlace!=null && site.Complete && _yardPreviewSide<0 && _reshapingPlot!=site.Id && _placeJourneySite!=site.Id;
        RenderYardPreview(site);RenderHomeInvitation(site);
        _workCardWorker.Disabled=worker==null;_workCardDiner.Disabled=diner==null;
        _workCardWorker.TooltipText=worker==null?"No worker is currently using this workplace.":"Follow "+worker.Name;
        _workCardDiner.TooltipText=diner==null?"Available when a resident collects a meal here.":"Follow "+diner.Name;
        _workCardPause.Visible=_reshapingPlot!=site.Id && (!site.Complete || World.ProductionOutput(site.Kind)!=null || site.Kind==BuildingKind.Carpenter);
        _workCardPause.Text=(site.Complete?site.WorkPaused:site.ConstructionPaused)?"Resume":"Pause";_workCardPause.Disabled=site.DemolitionRequested || _world.Food.Celebrating || _waitingMove==site.Id;
        _workCardMove.Visible=(!publicHome || _homeOptions) && _reshapingPlot!=site.Id && site.Complete && _yardPreviewSide<0;
        _workCardWorker.Visible=_yardPreviewSide<0 && (_world.PublicPlace==null || _placeJourneySite==site.Id);_workCardDetails.Visible=_reshapingPlot!=site.Id && _yardPreviewSide<0 && (!publicHome || _homeOptions && _homeMore);
        _workCardMove.Text=_waitingMove==site.Id?"Cancel move":!site.WorkPaused && (World.ProductionOutput(site.Kind)!=null || site.Kind==BuildingKind.Carpenter) && _world.PublicPlace!=null?"Pause & move":"Move";
        string? moveProblem=_world.PublicPlace!=null?_world.RelocationIntentProblem(site.Id):_world.RelocationProblem(site.Id);
        _workCardMove.Disabled=moveProblem!=null;_workCardMove.TooltipText=moveProblem??"Preview another location. Work pauses and resumes when you place or cancel; an already paused workplace stays paused. Growing crops need fresh sowing after moving.";
    }
}
