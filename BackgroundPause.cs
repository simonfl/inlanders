using Godot;
public partial class Game
{
    private bool _pauseInBackground=true;
    private Button _backgroundPauseButton=null!;
    private void HandleFocusLoss()
    {
        CancelCameraDrag();CancelDecorationStroke();CancelAreaRemoval();CancelBushMove();CancelGatheringPlan();CancelTerrain();
        CancelWoodlandCare();CancelGroupArrangement();CancelHouseholdMove(false);CancelRelocation(false);
        _pathStroke=_woodlandStroke=false;_lastPathCell=_lastWoodlandCell=null;_plotAnchor=null;
        if(HasPlacePathOrigin)EndPlacePath();
        _placing=false;_pathAnchor=_pathDraftEnd=null;_pathWaypoints.Clear();RefreshGhost();
        if(_atMainMenu || !_pauseInBackground)return;
        bool wasRunning=!_paused;_paused=true;_pauseButton.Text="Resume  [Space]";
        if(wasRunning)Notice("Paused while you switched apps. Space resumes village life.");
        UpdateWatchUi();
    }
    private void MakeBackgroundPauseUi(VBoxContainer column)
    {
        _backgroundPauseButton=Button("",()=>{_pauseInBackground=!_pauseInBackground;ApplyAtmosphere();SaveAtmosphere();});column.AddChild(_backgroundPauseButton);
        _backgroundPauseButton.TooltipText="On pauses when another app gets focus. Return and press Space to resume. Off allows village life to continue in the background; unfinished pointer gestures still cancel.";
    }
}
