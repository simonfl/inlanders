using Godot;
using Inlanders.Simulation;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private bool _groupActive,_groupSelecting,_groupWasPaused;
    private World? _groupWorld;
    private readonly List<int> _groupMembers=new();
    private Cell? _groupTarget;
    private int _groupTurn;
    private GroupPlan? _groupPlan;
    private Button _groupEntry=null!,_groupPick=null!,_groupApply=null!,_groupLeft=null!,_groupRight=null!;
    private PanelContainer _groupPanel=null!;
    private Label _groupText=null!;
    private Node3D _groupModels=null!;
    private void MakeGroupArrangementUi(VBoxContainer parent)
    {
        _groupEntry=Button("Arrange a farmstead group",BeginGroupArrangement);parent.AddChild(_groupEntry);
        _groupPanel=HudPanel(_hud);var column=new VBoxContainer();_groupPanel.AddChild(column);
        _groupText=Text("",14,true);_groupText.CustomMinimumSize=new(270,0);column.AddChild(_groupText);
        _groupPick=Button("Preview selected places",()=>{if(_groupSelecting){if(_groupMembers.Count<2)return;_groupSelecting=false;_groupTarget=_world.Cottages.Single(c=>c.Id==_groupMembers[0]).Cell;}else{_groupSelecting=true;_groupTarget=null;}RefreshGroupProposal();});column.AddChild(_groupPick);
        var turns=new HBoxContainer();column.AddChild(turns);
        _groupLeft=Button("Turn left",()=>{_groupTurn=(_groupTurn+3)%4;RefreshGroupProposal();});turns.AddChild(_groupLeft);
        _groupRight=Button("Turn right",()=>{_groupTurn=(_groupTurn+1)%4;RefreshGroupProposal();});turns.AddChild(_groupRight);
        _groupApply=Button("Use this arrangement",ApplyGroupArrangement);column.AddChild(_groupApply);
        column.AddChild(Button("Cancel [Esc]",CancelGroupArrangement));_groupPanel.Hide();_groupModels=new();AddChild(_groupModels);
    }
    private void BeginGroupArrangement()
    {
        if(_world.PublicPlace==null)return;CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _groupWasPaused=_paused;_paused=true;_pauseButton.Disabled=true;_pauseButton.Text="Paused while arranging";
        _groupWorld=_world;_groupActive=_groupSelecting=true;_groupMembers.Clear();_groupTarget=null;_groupTurn=0;RefreshGroupProposal();
    }
    private void CancelGroupArrangement()
    {
        if(!_groupActive)return;
        foreach(var id in _groupMembers){if(_cottages.TryGetValue(id,out var view))view.Body.Show();if(_cropViews.TryGetValue(id,out var crop))crop.Body.Show();}
        _groupActive=false;_groupPanel.Hide();_groupModels.Hide();_groupPlan=null;
        _paused=_groupWasPaused;_pauseButton.Disabled=false;_pauseButton.Text=_paused?"Resume  [Space]":"Pause  [Space]";
    }
    private void ApplyGroupArrangement()
    {
        if(_groupTarget is not Cell target || _groupSelecting)return;
        var plan=_world.PreviewGroup(_groupMembers.ToArray(),target,_groupTurn);
        if(plan.Result==null){_groupPlan=plan;return;}
        CancelGroupArrangement();_world=plan.Result;ClearSelection();CreateActors();RenderActors(0);RebuildQueue();Notice("Places rearranged together. Growing crops restart; ripe harvest stays.");
    }
    private void RefreshGroupProposal()
    {
        foreach(var id in _groupMembers){if(_cottages.TryGetValue(id,out var view))view.Body.Show();if(_cropViews.TryGetValue(id,out var crop))crop.Body.Show();}
        Clear(_groupModels);_groupModels.Show();
        _groupPlan=_groupTarget is Cell target && !_groupSelecting?_world.PreviewGroup(_groupMembers.ToArray(),target,_groupTurn):null;
        foreach(var id in _groupMembers)
        {
            var original=_world.Cottages.Single(c=>c.Id==id);var proposed=_groupPlan?.Result?.Cottages.Single(c=>c.Id==id);
            var site=proposed??original;
            foreach(var cell in World.Footprint(site))GroundPatch(_groupModels,cell.X,cell.Z,.94f,.94f,new("d8c57c"),.09f);
            if(proposed==null)continue;
            if(_cottages.TryGetValue(id,out var actual))actual.Body.Hide();if(_cropViews.TryGetValue(id,out var crop))crop.Body.Hide();
            var model=new Node3D{Position=BuildingPosition(site.Cell,site.Rotation,site.Kind,.04f,site.PlotRows),RotationDegrees=new(0,site.Rotation*90,0)};_groupModels.AddChild(model);MakeBuilding(model,site,3);
            if(site.Kind==BuildingKind.Farm || World.IsVegetablePlot(site.Kind))
            {
                int stage=site.Harvest>0?4:site.Planted?1+(int)(site.Growth*2.9f):0;
                if(World.IsVegetablePlot(site.Kind))MakeVegetables(model,site,stage);else MakeCrops(model,site,stage);
            }
            GroundPatch(_groupModels,site.Entrance.X,site.Entrance.Z,.85f,.85f,new("75c7d0"),.1f);
        }
    }
    private bool HandleGroupArrangementInput(InputEvent input)
    {
        if(!_groupActive)return false;
        if(input is InputEventKey{Pressed:true,Echo:false} key)
        {
            if(key.Keycode==Key.Escape){CancelGroupArrangement();return true;}
            if(key.Keycode==Key.Space)return true;
            if(key.Keycode==Key.R && !_groupSelecting){_groupTurn=(_groupTurn+(key.ShiftPressed?3:1))%4;RefreshGroupProposal();return true;}
            if(key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.F5 or Key.F9){CancelGroupArrangement();return false;}
        }
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left,Pressed:true} click && !PointerOverHud(click.Position))
        {
            if(Ground(click.Position) is Vector3 p)
            {
                var cell=new Cell(Mathf.RoundToInt(p.X),Mathf.RoundToInt(p.Z));
                if(_groupSelecting)
                {
                    var site=_world.Cottages.FirstOrDefault(c=>World.Footprint(c).Contains(cell));
                    if(site!=null && _world.GroupEligible(site)){if(!_groupMembers.Remove(site.Id) && _groupMembers.Count<16)_groupMembers.Add(site.Id);}
                }
                else _groupTarget=cell;
                RefreshGroupProposal();
            }
            return true;
        }
        return false;
    }
    private void RenderGroupArrangementUi()
    {
        _groupEntry.Visible=_world.PublicPlace!=null;
        if(!_groupActive)return;if(_groupWorld!=_world || _atMainMenu){CancelGroupArrangement();return;}
        _paused=true;_groupPanel.Show();_groupPanel.Position=new(_hud.Size.X-310,92);_groupPanel.Size=new(294,0);
        _groupText.Text="ARRANGE A FARMSTEAD\n"+(_groupSelecting?$"{_groupMembers.Count} selected. Click homes or fields to add/remove. Choose at least two.\nTime is paused; no changes until Apply.":"Click ground for the first selected place’s anchor. R turns the whole group.\nHouseholds and furnishings stay. Growing crops restart; ripe harvest and stored goods stay. Existing paths stay on their ground.\n"+(_groupPlan?.Problem??"The whole arrangement fits. Moving is free."));
        _groupPick.Text=_groupSelecting?"Preview selected places":"Change selection";_groupPick.Disabled=_groupSelecting && _groupMembers.Count<2;
        _groupLeft.Visible=_groupRight.Visible=_groupApply.Visible=!_groupSelecting;
        _groupApply.Disabled=_groupPlan?.Result==null || _groupTarget==_world.Cottages.FirstOrDefault(c=>c.Id==_groupMembers.FirstOrDefault(-1))?.Cell && _groupTurn==0;
    }
}
