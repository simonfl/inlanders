using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private VBoxContainer _identityControls=null!,_identityDetails=null!;
    private Button _identityToggle=null!;
    private readonly Dictionary<CivicIdentity,Button> _identityChoices=new();
    private World? _identityWorld;
    private int _identitySite=-1;
    private static string IdentityName(CivicIdentity identity)=>identity==CivicIdentity.PlantedCourt?"Planted court":identity.ToString();
    private void MakeCivicIdentityControls()
    {
        _identityControls=new();_buildingDetails.AddChild(_identityControls);
        _identityToggle=Button("Civic identity",()=>_identityDetails.Visible=!_identityDetails.Visible);_identityControls.AddChild(_identityToggle);
        _identityDetails=new(){Visible=false};_identityControls.AddChild(_identityDetails);
        foreach(var identity in Enum.GetValues<CivicIdentity>())
        {
            var button=Button(IdentityName(identity),()=>_world.SetCivicIdentity(_selectedSite,identity));
            _identityChoices[identity]=button;_identityDetails.AddChild(button);
        }
        _identityDetails.AddChild(Text("Free appearance choice. Every identity provides the same 8 recreation places. Court planting is ornamental; it supplies no timber or wildlife habitat.",13,true));
    }
    private void UpdateCivicIdentityControls()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && c.Kind==BuildingKind.GatheringHall);
        _identityControls.Visible=site!=null;if(site==null)return;
        if(_identityWorld!=_world || _identitySite!=site.Id){_identityDetails.Hide();_identityWorld=_world;_identitySite=site.Id;}
        _identityToggle.Text="Identity · "+IdentityName(site.Identity);
        foreach(var (identity,button) in _identityChoices)button.Disabled=site.DemolitionRequested || identity==site.Identity;
    }
}
