using Godot;
using Inlanders.Simulation;
using System.Linq;
using System.Text.Json;

public partial class Game
{
    private int _movingSite=-1;
    private bool _moveFromCard;
    private World? _moveWorld;
    private Button _moveButton=null!;
    private float _nextMoveRefresh;
    private (Cell Cell,int Rotation,string Problem)? _moveCheck;
    private void DiscardRelocation(){if(_resumeMovedWork && _movingSite>=0)_moveWorld?.SetWorkplacePaused(_movingSite,false);_resumeMovedWork=false;_movingSite=-1;_moveWorld=null;_moveCheck=null;}
    private void CancelRelocation(bool inspect)
    {
        CancelMoveWait();int id=_movingSite;if(id<0)return;bool card=_moveFromCard;DiscardRelocation();_placing=false;RefreshGhost();
        if(inspect && _world.Cottages.Any(c=>c.Id==id)){if(card)ShowWorkplaceCard(id);else SelectBuilding(id);}
    }
    private void BeginRelocation()
    {
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);if(site==null)return;
        if(_world.RelocationProblem(site.Id) is string problem){Notice(problem);return;}
        bool card=_workCardSite==site.Id;CloseManagementUi();BeginPlacement(site.Kind);_moveFromCard=card;
        _movingSite=site.Id;_moveWorld=_world;_rotation=site.Rotation;_hover=site.Cell;_nextMoveRefresh=0;_moveCheck=null;
        RefreshGhost();Notice(site.Kind==BuildingKind.Farm || World.IsVegetablePlot(site.Kind)?"Choose new ground. Growing crops restart; ripe crops and stored goods stay. R rotates; Esc cancels.":"Choose a destination. R rotates; Escape or right-click cancels.");
    }
    private string MovePreviewProblem(Cell cell)
    {
        if(_moveCheck is {} cached && cached.Cell==cell && cached.Rotation==_rotation)return cached.Problem;
        string problem=_world.RelocationProblem(_movingSite,cell,_rotation)??"";
        _moveCheck=(cell,_rotation,problem);return problem;
    }
    private void UpdateRelocation()
    {
        UpdateMoveIntent();if(_movingSite<0)return;
        if(!ReferenceEquals(_moveWorld,_world) || !_world.Cottages.Any(c=>c.Id==_movingSite)){CancelRelocation(false);return;}
        if(!_placing || _plantingTrees || _clearingTrees || _decorating || _pathTool>0 || _woodlandTool>0){DiscardRelocation();return;}
        if(_uiTime>=_nextMoveRefresh){_nextMoveRefresh=_uiTime+.25f;_moveCheck=null;RefreshGhost();}
    }
    private bool HandleRelocationInput(InputEvent input)
    {
        if(_movingSite<0)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right})
        {CancelRelocation(true);return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P)
            CancelRelocation(false);
        return false;
    }
    private void ConfirmRelocation(Cell at)
    {
        int id=_movingSite;
        if(_world.RelocationProblem(id,at,_rotation) is string problem){_moveCheck=null;RefreshGhost();Notice(problem);UiCue(Cue.Reject);return;}
        if(!_world.MoveBuilding(id,at,_rotation))return;
        DiscardRelocation();_placing=false;RefreshGhost();
        // A deliberate move rebuilds presentation once, without adopting/reloading the
        // simulation, changing pause/speed or resetting the simulation accumulator.
        CreateActors();RenderActors(0);RenderFoodViews();RebuildQueue();if(_moveFromCard)ShowWorkplaceCard(id);else SelectBuilding(id);RefreshSelection();
        var moved=_world.Cottages.Single(c=>c.Id==id);
        UiCue(Cue.Place);Notice((World.IsVegetablePlot(moved.Kind) || moved.Kind==BuildingKind.Farm)?(moved.WorkPaused?"Field moved and still paused. Resume to work here; growing crops need fresh sowing.":"Field moved. Growing crops need fresh sowing."):_world.Founding!=null?"Moved for free. Neighbors will use the new location.":"Building moved. Its identity, goods and improvements are retained.");
    }
    private string RelocationModelKey(Cottage site)=>"move:"+JsonSerializer.Serialize(site);
}
