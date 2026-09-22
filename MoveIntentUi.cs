using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private int _waitingMove=-1;
    private World? _waitingMoveWorld;
    private bool _resumeMovedWork;
    private bool MoveIntentActive=>_waitingMove>=0 || _resumeMovedWork;
    private void SettleMoveBeforeSave(){if(MoveIntentActive)CancelRelocation(false);}
    private void CancelMoveWait()
    {
        if(_waitingMove>=0)_waitingMoveWorld?.SetWorkplacePaused(_waitingMove,false);
        _waitingMove=-1;_waitingMoveWorld=null;
    }
    private void ChooseMoveIntent()
    {
        if(_waitingMove>=0){CancelMoveWait();_nextWorkCard=0;return;}
        var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);if(site==null)return;
        if(_world.PublicPlace==null || site.WorkPaused || World.ProductionOutput(site.Kind)==null && site.Kind!=BuildingKind.Carpenter){BeginRelocation();return;}
        if(_world.RelocationIntentProblem(site.Id) is {} problem){Notice(problem);return;}
        if(!_world.SetWorkplacePaused(site.Id,true))return;
        if(_world.RelocationProblem(site.Id)==null){BeginRelocation();_resumeMovedWork=true;}
        else{_waitingMove=site.Id;_waitingMoveWorld=_world;Notice("Waiting for the fisher to return. Cancel move resumes this workplace.");}
        _nextWorkCard=0;
    }
    private void UpdateMoveIntent()
    {
        if(_waitingMove<0)return;
        if(_waitingMoveWorld!=_world || _selectedSite!=_waitingMove || _atMainMenu){CancelMoveWait();return;}
        if(_world.RelocationProblem(_waitingMove)!=null)return;
        _waitingMove=-1;_waitingMoveWorld=null;BeginRelocation();_resumeMovedWork=true;
    }
}
