using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private PanelContainer _workCard=null!;
    private Label _workCardText=null!;
    private Button _workCardPause=null!,_workCardMove=null!,_workCardDetails=null!,_workCardWorker=null!,_workCardDiner=null!,_workCardCancel=null!;
    private Button _workCardFurnish=null!,_workCardYard=null!;

    private int _workCardSite=-1;
    private World? _workCardWorld;
    private float _nextWorkCard;
    private bool _workCardRight;
    private bool UsesWorkCard(Cottage site)=>_world.PublicPlace!=null || (_world.Founding!=null || _world.IsArrangementCourt) && site.Complete &&
        (World.ProductionOutput(site.Kind)!=null || site.Kind is BuildingKind.Carpenter or BuildingKind.Pantry);
    private Villager? CardWorker()=>_world.People.FirstOrDefault(p=>p.WorkplaceId==_workCardSite || p.SiteId==_workCardSite || p.HomeId==_workCardSite || p.LeisureSiteId==_workCardSite);
    private Villager? CardDiner()=>_world.People.FirstOrDefault(p=>p.Meal is { } m && (m.Reserved || m.Carrying) && m.SourceId==_workCardSite);
    private void ShowWorkplaceCard(int id)
    {
        ClearSelection();CloseDrawer();_workCardSite=_selectedSite=id;_workCardWorld=_world;_nextWorkCard=0;
        var site=_world.Cottages.First(c=>c.Id==id);
        _workCardRight=_camera.UnprojectPosition(BuildingPosition(site.Cell,site.Rotation,site.Kind)).X<_hud.Size.X/2;RefreshSelection();
    }
    private void MakeWorkplaceCard()
    {
        _workCard=HudPanel(_hud);var column=new VBoxContainer();_workCard.AddChild(column);
        _workCardText=Text("",14,true);_workCardText.CustomMinimumSize=new(306,0);column.AddChild(_workCardText);
        var people=new HBoxContainer();column.AddChild(people);
        void Watch(Villager? person){if(person==null)return;_workCardSite=-1;ShowDailyLife(person.Id);_followPerson=true;}
        _workCardWorker=Button("Watch work",()=>Watch(CardWorker()));people.AddChild(_workCardWorker);
        _workCardDiner=Button("Follow meal",()=>Watch(CardDiner()));people.AddChild(_workCardDiner);
        var actions=new HBoxContainer();column.AddChild(actions);
        _workCardPause=Button("Pause",()=>{var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site!=null){if(site.Complete)_world.SetWorkplacePaused(site.Id,!site.WorkPaused);else _world.SetConstructionPaused(site.Id,!site.ConstructionPaused);}_nextWorkCard=0;});actions.AddChild(_workCardPause);
        _workCardMove=Button("Move",BeginRelocation);actions.AddChild(_workCardMove);
        _workCardDetails=Button("Details",()=>{int id=_workCardSite;_workCardSite=-1;SelectBuilding(id);});actions.AddChild(_workCardDetails);
        actions.AddChild(Button("×",ClearSelection));
        _workCardFurnish=Button("Furnish yard",()=>{var home=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(home!=null){if(home.ImprovementRequested)_world.CancelImprovement(home.Id);else _world.RequestImprovement(home.Id);}_nextWorkCard=0;});column.AddChild(_workCardFurnish);
        _workCardYard=Button("Arrange yard",BeginYardPreview);column.AddChild(_workCardYard);MakeYardPreview(column);
        _workCardCancel=Button("Cancel this construction",()=>{if(_world.Cancel(_workCardSite)){ClearSelection();RebuildQueue();}});column.AddChild(_workCardCancel);_workCard.Hide();
    }
    private void RenderWorkplaceCard()
    {
        if(_workCardWorld!=_world)_workCardSite=-1;
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);
        bool show=site!=null && _selectedSite==site.Id && !_atMainMenu && !_placing && !_watching && !_drawer.Visible && !_inspector.Visible;
        _workCard.Visible=show;if(!show || site==null){StopYardPreview();return;}
        _workCard.Size=new(330,0);
        _workCard.Position=new(Mathf.Max(0,_workCardRight?_hud.Size.X-346:Mathf.Min(16,_hud.Size.X-330)),92);
        if(_uiTime<_nextWorkCard)return;_nextWorkCard=_uiTime+.3f;
        var worker=CardWorker();var diner=CardDiner();
        string detail;
        if(!site.Complete)detail=$"{site.Delivered}/{site.Required} materials · {site.Construction:P0} built"+(site.RequiredStone>0?$"\n{site.DeliveredStone}/{site.RequiredStone} stone":"")+"\n"+(site.ConstructionPaused?"Construction paused; supplies stay here.":"Shared workers build when supplies are available.");
        else if(Buildings.Get(site.Kind).Beds>0)detail=$"{_world.People.Count(p=>p.HomeId==site.Id)}/{Buildings.Get(site.Kind).Beds} neighbors live here";
        else if(Buildings.Get(site.Kind).RecreationSlots>0)detail=$"{_world.People.Count(p=>p.LeisureSiteId==site.Id)} neighbors visiting\nA place for ordinary breaks.";
        else {var report=_world.ReadWorkplace(site);detail=report.State+"\n"+(worker!=null?worker.Name+": "+worker.Status:report.Detail.Split('\n')[0]);}
        _workCardText.Text=BuildingName(site.Kind).ToUpperInvariant()+"\n"+detail;
        _workCardYard.Visible=_yardPreviewSide<0 && _world.PublicPlace!=null && site.Complete && Buildings.Get(site.Kind).Beds>0;
        _workCardYard.Text="Arrange yard · preview";
        _workCardYard.Disabled=site.ImprovementRequested || site.DemolitionRequested;
        _workCardYard.TooltipText="Preview the four sides before choosing. Residents use the chosen ground for quiet work and nearby meals after furnishing. Existing furniture moves free.";
        _workCardFurnish.Visible=_yardPreviewSide<0 && site.Complete && Buildings.Get(site.Kind).Beds>0 && !site.Improved;
        _workCardFurnish.Text=site.ImprovementRequested?"Cancel furnishing":_world.Creative?"Furnish yard · free":$"Furnish yard · {World.ComfortCost(site)} planks";
        _workCardFurnish.Disabled=_yardPreviewSide>=0 || !site.ImprovementRequested && _world.ImprovementProblem(site.Id)!=null;
        _workCardFurnish.TooltipText=_world.ImprovementProblem(site.Id)??"Shared workers deliver planks and furnish the chosen ground beside this home.";
        if(site.Complete && Buildings.Get(site.Kind).Beds>0)_workCardText.Text+="\n"+HomeCardOutcome(site);
        _workCardWorker.Text=Buildings.Get(site.Kind).Beds>0?"Watch resident":!site.Complete?"Watch builder":"Watch work";
        _workCardCancel.Visible=!site.Complete && !site.DemolitionRequested;
        if(site.Complete && (_world.IsWorkplaceFoodStore(site) || site.Kind==BuildingKind.Pantry))
        {
            int available=World.EdibleKinds.Sum(k=>_world.FoodAvailableAt(site.Id,k));
            _workCardText.Text+=$"\n\n{available} meal portions available here\n"+(diner!=null?diner.Name+" is collecting or carrying a meal.":"No meal collection in progress.");
        }
        _workCardDiner.Visible=Buildings.Get(site.Kind).Beds==0;
        RenderYardPreview(site);
        _workCardWorker.Disabled=worker==null;_workCardDiner.Disabled=diner==null;
        _workCardWorker.TooltipText=worker==null?"No worker is currently using this workplace.":"Follow "+worker.Name;
        _workCardDiner.TooltipText=diner==null?"Available when a resident collects a meal here.":"Follow "+diner.Name;
        _workCardPause.Visible=!site.Complete || World.ProductionOutput(site.Kind)!=null || site.Kind==BuildingKind.Carpenter;
        _workCardPause.Text=(site.Complete?site.WorkPaused:site.ConstructionPaused)?"Resume":"Pause";_workCardPause.Disabled=site.DemolitionRequested || _world.Food.Celebrating;
        _workCardMove.Visible=site.Complete && _yardPreviewSide<0;
        _workCardWorker.Visible=_yardPreviewSide<0;_workCardDetails.Visible=_yardPreviewSide<0;
        _workCardMove.Disabled=_world.RelocationProblem(site.Id)!=null;_workCardMove.TooltipText=_world.RelocationProblem(site.Id)??"Choose a new location.";
    }
}
