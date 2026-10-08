using Godot;
using Inlanders.Simulation;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private GroupRecovery? _groupRecovery;
    private World? _groupRecoveryWorld;
    private bool _groupRecovering;
    private Button _groupRecoverEntry=null!;
    private GroupPlan CurrentGroupPlan()=>_groupRecovering && _groupRecovery!=null?_world.PreviewGroupRecovery(_groupRecovery):_world.PreviewGroup(_groupMembers.ToArray(),_groupTarget!.Value,_groupTurn,_groupCarryPaths);
    private void BeginGroupRecovery()
    {
        if(_groupRecovery==null || _groupRecoveryWorld!=_world)return;
        var check=_world.PreviewGroupRecovery(_groupRecovery);if(check.Result==null){Notice(check.Problem!);return;}
        BeginGroupArrangement();_groupRecovering=true;_groupSelecting=false;
        _groupMembers.AddRange(_groupRecovery.Applied.Select(c=>c.Id));_groupTarget=_groupRecovery.Anchor;_groupTurn=_groupRecovery.Turn;RefreshGroupProposal();
    }
    private bool _groupCarryPaths,_groupShowBefore;
    private Button _groupCompare=null!,_groupFrame=null!,_groupWatch=null!;
    private Button _groupPaths=null!;
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
        _groupRecoverEntry=Button("Restore last group arrangement",BeginGroupRecovery);parent.AddChild(_groupRecoverEntry);
        _groupRecoverEntry.TooltipText="Preview the previous positions from this session. Time, food and work keep their current state. Growing crops restart. Another group move replaces this recovery; loading ends it.";
        _groupPanel=HudPanel(_hud);var column=new VBoxContainer();_groupPanel.AddChild(column);
        _groupText=Text("",14,true);_groupText.CustomMinimumSize=new(270,0);column.AddChild(_groupText);
        _groupPick=Button("Preview selected places",()=>{if(_groupSelecting){if(_groupMembers.Count<2)return;_groupSelecting=false;_groupTarget=_world.Cottages.Single(c=>c.Id==_groupMembers[0]).Cell;}else{_groupSelecting=true;_groupTarget=null;}RefreshGroupProposal();});column.AddChild(_groupPick);
        var turns=new HBoxContainer();column.AddChild(turns);
        _groupLeft=Button("Turn left",()=>{_groupTurn=(_groupTurn+3)%4;RefreshGroupProposal();});turns.AddChild(_groupLeft);
        _groupRight=Button("Turn right",()=>{_groupTurn=(_groupTurn+1)%4;RefreshGroupProposal();});turns.AddChild(_groupRight);
        _groupPaths=Button("Paths: leave in place",()=>{_groupCarryPaths=!_groupCarryPaths;RefreshGroupProposal();});column.AddChild(_groupPaths);
        _groupPaths.TooltipText="Carry marked path tiles within the selected places’ extent. Other places’ access and outside paths stay; boundary junctions stay too. You may need to reconnect the moved group.";
        var comparison=new HBoxContainer();column.AddChild(comparison);
        _groupCompare=Button("Show current",()=>_groupShowBefore=!_groupShowBefore);comparison.AddChild(_groupCompare);
        _groupFrame=Button("Frame group",()=>FrameGroupArrangement(true));comparison.AddChild(_groupFrame);
        var apply=new HBoxContainer();column.AddChild(apply);
        _groupApply=Button("Apply arrangement",()=>ApplyGroupArrangement(false));apply.AddChild(_groupApply);
        _groupWatch=Button("Apply & watch",()=>ApplyGroupArrangement(true));apply.AddChild(_groupWatch);
        foreach(var button in new[]{_groupCompare,_groupFrame,_groupApply,_groupWatch})button.AddThemeFontSizeOverride("font_size",14);
        column.AddChild(Button("Cancel [Esc]",CancelGroupArrangement));_groupPanel.Hide();_groupModels=new();AddChild(_groupModels);
    }
    private void BeginGroupArrangement()
    {
        if(_world.PublicPlace==null)return;CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _groupWasPaused=_paused;_paused=true;_pauseButton.Disabled=true;_pauseButton.Text="Paused while arranging";
        _groupRecovering=false;_groupWorld=_world;_groupActive=_groupSelecting=true;_groupMembers.Clear();_groupCarryPaths=false;_groupTarget=null;_groupTurn=0;RefreshGroupProposal();
    }
    private void CancelGroupArrangement()
    {
        if(!_groupActive)return;
        foreach(var id in _groupMembers){if(_cottages.TryGetValue(id,out var view))view.Body.Show();if(_cropViews.TryGetValue(id,out var crop))crop.Body.Show();}
        _groupActive=false;_groupPanel.Hide();_groupModels.Hide();_groupPlan=null;
        _paused=_groupWasPaused;_pauseButton.Disabled=false;_pauseButton.Text=_paused?"Resume  [Space]":"Pause  [Space]";
    }
    private void ApplyGroupArrangement(bool watch)
    {
        if(_groupTarget is not Cell target || _groupSelecting)return;
        var plan=CurrentGroupPlan();
        if(plan.Result==null){_groupPlan=plan;return;}
        if(_groupRecovering){_groupRecovery=null;_groupRecoveryWorld=null;}
        else{_groupRecovery=_world.RememberGroup(plan.Result,_groupMembers.ToArray(),_groupTurn);_groupRecoveryWorld=plan.Result;}
        CancelGroupArrangement();_world=plan.Result;ClearSelection();CreateActors();RenderActors(0);RebuildQueue();Notice("Places rearranged together. Growing crops restart; ripe harvest stays.");
        if(watch)
        {
            _localWatchWorld=_world;_localWatchSite=_groupMembers[0];_localWatchCommons=null;_localWatchFocus=_focus;_localWatchAngle=_angle;_localWatchZoom=_camera.Size;
            FrameGroupArrangement(false);_paused=false;_pauseButton.Text="Pause";ToggleWatch();
        }
    }
    private void RefreshGroupProposal()
    {
        foreach(var id in _groupMembers){if(_cottages.TryGetValue(id,out var view))view.Body.Show();if(_cropViews.TryGetValue(id,out var crop))crop.Body.Show();}
        _groupShowBefore=false;Clear(_groupModels);_groupModels.Show();
        _groupPlan=_groupTarget is Cell target && !_groupSelecting?CurrentGroupPlan():null;
        if(_groupCarryPaths && _groupMembers.Count>=2)
        {
            var pivot=_world.Cottages.Single(c=>c.Id==_groupMembers[0]).Cell;
            foreach(var at in _world.GroupPaths(_groupMembers.ToArray()))
            {
                GroundPatch(_groupModels,at.X,at.Z,.55f,.55f,new("c48d68"),.12f);
                if(_groupTarget is Cell destination && _groupPlan?.Result!=null){var moved=World.RotateOffset(destination,at.X-pivot.X,at.Z-pivot.Z,_groupTurn);GroundPatch(_groupModels,moved.X,moved.Z,.6f,.6f,new("75c7d0"),.14f);}
            }
        }
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
    private void FrameGroupArrangement(bool comparison)
    {
        var sites=_world.Cottages.Where(c=>_groupMembers.Contains(c.Id));
        if(comparison && _groupPlan?.Result is {} result)sites=sites.Concat(result.Cottages.Where(c=>_groupMembers.Contains(c.Id)));
        var cells=sites.SelectMany(c=>World.Footprint(c).Append(c.Entrance)).ToArray();if(cells.Length==0)return;
        _focus=OnGround((cells.Min(c=>c.X)+cells.Max(c=>c.X))*.5f,(cells.Min(c=>c.Z)+cells.Max(c=>c.Z))*.5f);_camera.Size=22;UpdateCamera();
        var points=cells.SelectMany(c=>new[]{_camera.UnprojectPosition(OnGround(c.X,c.Z)),_camera.UnprojectPosition(OnGround(c.X,c.Z,3))}).ToArray();
        float available=_hud.Size.X-(comparison?340:40);
        float scale=Mathf.Max((points.Max(p=>p.X)-points.Min(p=>p.X)+70)/available,(points.Max(p=>p.Y)-points.Min(p=>p.Y)+80)/(_hud.Size.Y-180));
        _camera.Size=Mathf.Clamp(22*scale,14,MaximumZoom);UpdateCamera();
        if(comparison){_focus+=CameraDragPoint(_hud.Size/2)-CameraDragPoint(new((_hud.Size.X-340)/2,_hud.Size.Y/2));UpdateCamera();}
    }
    private bool HandleGroupArrangementInput(InputEvent input)
    {
        if(!_groupActive)return false;
        if(input is InputEventKey{Pressed:true,Echo:false} key)
        {
            if(key.Keycode==Key.Escape){CancelGroupArrangement();return true;}
            if(key.Keycode==Key.Space)return true;
            if(key.Keycode==Key.R && !_groupSelecting && !_groupRecovering){_groupTurn=(_groupTurn+(key.ShiftPressed?3:1))%4;RefreshGroupProposal();return true;}
            if(key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.P or Key.C or Key.T or Key.U or Key.F5 or Key.F9){CancelGroupArrangement();return false;}
        }
        if(input is InputEventMouseButton{ButtonIndex:MouseButton.Left,Pressed:true} click && !PointerOverHud(click.Position))
        {
            if(_groupRecovering)return true;
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
        _groupEntry.Visible=_world.PublicPlace!=null;_groupRecoverEntry.Visible=_world.PublicPlace!=null && _groupRecovery!=null && _groupRecoveryWorld==_world;
        if(!_groupActive)return;if(_groupWorld!=_world || _atMainMenu){CancelGroupArrangement();return;}
        _paused=true;_groupPanel.Show();_groupPanel.Position=new(_hud.Size.X-310,92);_groupPanel.Size=new(294,0);
        _groupModels.Visible=!_groupShowBefore;
        if(_groupPlan?.Result!=null && !_groupSelecting)foreach(var id in _groupMembers){if(_cottages.TryGetValue(id,out var view))view.Body.Visible=_groupShowBefore;if(_cropViews.TryGetValue(id,out var crop))crop.Body.Visible=_groupShowBefore;}
        _groupText.Text=(_groupShowBefore?"CURRENT ARRANGEMENT\n":"PROPOSED FARMSTEAD\n")+(_groupSelecting?$"{_groupMembers.Count} selected. Click homes or fields to add/remove. Choose at least two.\nTime is paused; no changes until Apply.":(_groupRecovering?"Restore previous positions, keeping elapsed time, food and work.\nGrowing crops restart; ripe goods stay.\n":"Click ground to move; R turns the group.\nGrowing crops restart; ripe goods stay.\n")+(_groupRecovering?"Previous approaches return where still clear.\n":_groupCarryPaths?"Marked paths move; outside approaches stay.\n":"Paths stay on their ground.\n")+(_groupPlan?.Problem??"The whole arrangement fits. Moving is free."));
        _groupPick.Text=_groupSelecting?"Preview selected places":"Change selection";_groupPick.Disabled=_groupSelecting && _groupMembers.Count<2;
        _groupLeft.Visible=_groupRight.Visible=_groupApply.Visible=_groupPaths.Visible=_groupCompare.Visible=_groupFrame.Visible=_groupWatch.Visible=!_groupSelecting;
        _groupPick.Visible=!_groupRecovering;_groupLeft.Visible=_groupRight.Visible=_groupPaths.Visible=!_groupSelecting && !_groupRecovering;
        _groupCompare.Text=_groupShowBefore?"Show proposal":"Show current";_groupCompare.Disabled=_groupPlan?.Result==null;
        _groupPaths.Text=_groupCarryPaths?$"Paths: carry {_world.GroupPaths(_groupMembers.ToArray()).Length} tiles":"Paths: leave in place";
        _groupWatch.Disabled=_groupApply.Disabled=_groupShowBefore || _groupPlan?.Result==null || _groupTarget==_world.Cottages.FirstOrDefault(c=>c.Id==_groupMembers.FirstOrDefault(-1))?.Cell && _groupTurn==0;
    }
}
