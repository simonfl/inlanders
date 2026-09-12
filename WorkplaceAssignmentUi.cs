using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private Label _assignmentState=null!,_assignmentHelp=null!;
    private OptionButton _assignmentChoice=null!;
    private Button _assignmentApply=null!,_assignmentVisit=null!;
    private World? _assignmentWorld;
    private int _assignmentPerson=-1,_assignmentRole=-1,_assignmentActual=-1;
    private string _assignmentChoices="";
    private void MakeWorkplaceAssignmentControls()
    {
        _assignmentState=Text("",14,true);_personDetails.AddChild(_assignmentState);
        _assignmentChoice=new(){CustomMinimumSize=new(0,34),SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,FitToLongestItem=false};_personDetails.AddChild(_assignmentChoice);
        _assignmentChoice.ItemSelected+=_=>UpdateWorkplaceAssignmentControls();
        _assignmentHelp=Text("",13,true);_personDetails.AddChild(_assignmentHelp);
        _assignmentApply=Button("Apply workplace",()=>
        {
            if(_selectedPerson<0)return;
            int id=_assignmentChoice.GetSelectedId();
            _world.SetWorkplaceAssignment(_selectedPerson,id==0?null:id);UpdateWorkplaceAssignmentControls();
        });_personDetails.AddChild(_assignmentApply);
        _assignmentVisit=Button("Inspect assigned workplace",()=>
        {if(_selectedPerson>=0 && _world.People[_selectedPerson].AssignedWorkplaceId is int id)SelectBuilding(id);});_personDetails.AddChild(_assignmentVisit);
    }
    private void UpdateWorkplaceAssignmentControls()
    {
        if(_selectedPerson<0){_assignmentPerson=-1;return;}
        var person=_world.People[_selectedPerson];bool supported=World.SupportsWorkplaceAssignment(person.Role);
        var sites=_world.Cottages.Where(c=>c.Complete && !c.DemolitionRequested && Buildings.Get(c.Kind).Worker==person.Role && Buildings.Get(c.Kind).Slots>0).OrderBy(c=>c.Id).ToArray();
        int actual=person.AssignedWorkplaceId??0;
        bool reset=_assignmentWorld!=_world || _assignmentPerson!=person.Id || _assignmentRole!=(int)person.Role || _assignmentActual!=actual;
        string key=string.Join(",",sites.Select(c=>c.Id));
        if(reset || key!=_assignmentChoices)
        {
            int draft=reset?actual:_assignmentChoice.GetSelectedId();_assignmentChoice.Clear();_assignmentChoice.AddItem("Automatic",0);
            foreach(var site in sites)_assignmentChoice.AddItem($"{Buildings.Get(site.Kind).Name} {site.Id} ({site.Cell.X}, {site.Cell.Z})",site.Id);
            int index=_assignmentChoice.GetItemIndex(draft);_assignmentChoice.Select(index<0?0:index);
            _assignmentChoices=key;_assignmentWorld=_world;_assignmentPerson=person.Id;_assignmentRole=(int)person.Role;_assignmentActual=actual;
        }
        _assignmentState.Text="WORKPLACE\n"+_world.ReadWorkplaceAssignment(person);
        _assignmentChoice.Visible=_assignmentApply.Visible=_assignmentHelp.Visible=supported;
        _assignmentChoice.Disabled=_world.Food.Celebrating;
        _assignmentVisit.Visible=person.AssignedWorkplaceId!=null;
        int chosen=_assignmentChoice.GetSelectedId();
        string? problem=_world.WorkplaceAssignmentProblem(person.Id,chosen==0?null:chosen);
        var target=sites.FirstOrDefault(c=>c.Id==chosen);
        _assignmentChoice.TooltipText=target==null?"Automatic":$"{Buildings.Get(target.Kind).Name} {target.Id} ({target.Cell.X}, {target.Cell.Z})";
        _assignmentHelp.Text=problem ?? (target==null?"Automatic chooses available workplaces after the current job.":
            $"{_world.AssignedWorkers(target.Id)}/{Buildings.Get(target.Kind).Slots} assigned. Works only here; waits if paused, growing, out of inputs or at target. Meals and breaks continue. Changes apply after the current job."+
            (person.Role==Role.Forager?" Berry collection remains village-wide.":person.Role==Role.Carpenter?" Home orders remain village-wide.":""));
        _assignmentApply.Disabled=problem!=null || chosen==actual;
        _assignmentApply.TooltipText=problem??"Keep the current work and cargo; use this workplace for the next job.";
    }
}
