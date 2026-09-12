using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Game
{
    private OptionButton _jobChoice = null!;
    private Button _followButton = null!, _workplaceButton = null!, _staffPlus = null!, _staffMinus = null!;
    private Label _workplaceStaff = null!;
    private VBoxContainer _workplaceControls = null!;
    private readonly List<Button> _workerLinks = new();
    private bool _followPerson;
    private int _jobChoicePerson = -1;
    private ScrollContainer _inspectionScroll = null!;

    private static Role? WorkplaceRole(BuildingKind kind) => kind switch
    {
        BuildingKind.Carpenter => Role.Carpenter,
        BuildingKind.HuntingLodge => Role.Hunter, BuildingKind.Quarry => Role.Quarrier, BuildingKind.ForagerHut => Role.Forager, BuildingKind.Farm or BuildingKind.VegetableGarden or BuildingKind.Orchard => Role.Farmer,
        BuildingKind.Stockpile or BuildingKind.Pantry => Role.Hauler, BuildingKind.Bakery => Role.Baker, BuildingKind.Sawmill => Role.Sawyer, BuildingKind.FishingDock => Role.Fisher, _ => null
    };
    private int? SelectedWorkplace()
    {
        if (_selectedPerson < 0) return null;
        var p = _world.People[_selectedPerson];
        return p.WorkplaceId ?? p.SiteId ?? p.HaulTargetId ?? p.StorageId ?? p.FoodDestinationId ?? p.FoodSourceId;
    }
    private void MakeJobChoice()
    {
        _peopleBackButton=Button("Back to residents [Esc]",OpenPeopleKeyboard);_personDetails.AddChild(_peopleBackButton);_peopleBackButton.Hide();
        _jobChoice = new OptionButton { CustomMinimumSize = new(0,34), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        foreach(var role in Enum.GetValues<Role>()) _jobChoice.AddItem(RoleName(role), (int)role);
        _jobChoice.ItemSelected += _ => UpdateManagementControls();
        _personDetails.AddChild(_jobChoice);
    }
    private void MakeManagementControls()
    {
        _workplaceButton = Button("Inspect workplace", () => { if (SelectedWorkplace() is int id) SelectBuilding(id); });
        _personDetails.AddChild(_workplaceButton);
        _followButton = Button("Follow villager", () => _followPerson = !_followPerson); _personDetails.AddChild(_followButton);
        _workplaceControls = new(); _buildingDetails.AddChild(_workplaceControls);
        _workplaceStaff = Text("",14,true); _workplaceControls.AddChild(_workplaceStaff);
        var row = new HBoxContainer(); _workplaceControls.AddChild(row);
        void Staff(int change) {
            var site = _world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
            if(site!=null && WorkplaceRole(site.Kind) is Role role) _world.AdjustWorkers(role,change);
        }
        _staffMinus = Button("− Role",()=>Staff(-1)); _staffPlus = Button("+ Role",()=>Staff(1));
        _staffMinus.SizeFlagsHorizontal = _staffPlus.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(_staffMinus); row.AddChild(_staffPlus);
    }
    private void UpdateManagementControls()
    {
        if (_selectedPerson >= 0) {
            var person=_world.People[_selectedPerson];
            if(_jobChoicePerson!=person.Id) { _jobChoice.Select((int)person.Role); _jobChoicePerson=person.Id; }
            _jobChoice.Disabled = _world.Food.Celebrating;
            _assignButton.Text = (_peopleKeyboard && _peopleKeyboardPerson==person.Id ? $"Assign {person.Name}: " : "Assign: ") + RoleName((Role)_jobChoice.GetSelectedId());
            _assignButton.Disabled = _world.Food.Celebrating || _jobChoice.GetSelectedId()==(int)person.Role;
        } else _jobChoicePerson=-1;
        _followButton.Text = _followPerson ? "Stop following" : "Follow villager";
        _workplaceButton.Disabled = SelectedWorkplace()==null;
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
        Role? role=site==null ? null : WorkplaceRole(site.Kind);
        _workplaceControls.Visible=site?.Complete==true && role!=null;
        if(role is Role job && site!=null) {
            int assigned=_world.People.Count(p=>p.Role==job);
            int active=_world.People.Count(p=>p.WorkplaceId==site.Id || p.StorageId==site.Id || p.HaulTargetId==site.Id || p.FoodDestinationId==site.Id || p.FoodSourceId==site.Id);
            int capacity=Buildings.Get(site.Kind).Slots;
            _workplaceStaff.Text=site.Kind is BuildingKind.Stockpile or BuildingKind.Pantry ? $"{active} visiting · {assigned} haulers village-wide\nHaulers share stockpiles and pantries. Targets reserve space for incoming loads." : $"{active}/{capacity} working here · {assigned} {RoleName(job).ToLowerInvariant()}s village-wide\nWorkers share workplaces; + uses a spare worker or transfers one from another job.";
            _staffMinus.Disabled=_world.Food.Celebrating || assigned==0;
            _staffPlus.Disabled=_world.Food.Celebrating || assigned==_world.Population;
            var candidate = _world.WorkerAdjustmentCandidate(job, 1);
            _staffPlus.TooltipText = candidate == null ? "Everyone already has this role." : $"Assign {candidate.Name} ({candidate.Role}) as {job}. Changes the village-wide role; does not pin them here.";
            _staffMinus.TooltipText = "Unassign one worker from this role across the village; they may be working elsewhere.";
        }
        foreach(var p in _world.People) {
            var button=_workerLinks[p.Id];
            button.Visible=site!=null && (p.WorkplaceId==site.Id || p.SiteId==site.Id || p.StorageId==site.Id || p.HaulTargetId==site.Id || p.FoodDestinationId==site.Id || p.FoodSourceId==site.Id);
            button.Text=$"{p.Name} · {TaskName(p.Task)}";
        }
    }
    private void UpdateFollowing()
    {
        if(_selectedPerson<0) { _followPerson=false; return; }
        _selection.Position=_people[_selectedPerson].Body.Position;
        if(_followPerson) { _focus=_selection.Position; UpdateCamera(); }
    }
}
