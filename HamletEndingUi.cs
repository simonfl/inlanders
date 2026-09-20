using Godot;
public partial class Game
{
    private PanelContainer _hamletEnding=null!;
    private Button _endingWatch=null!,_endingShape=null!,_endingLeave=null!;
    private void WatchHamlet(){CloseDrawer();ClearSelection();_speed=1;_paused=false;ToggleWatch();}
    private void ReopenHamlet(){if(_world.PublicPlace!=null){_world.Founding!.Finished=false;SaveWorld();UpdateHud();}CloseDrawer();_paused=false;}
    private void MakeHamletEndingUi()
    {
        _hamletEnding=HudPanel(_hud);var column=new VBoxContainer();_hamletEnding.AddChild(column);
        var title=Text("A PLACE TO KEEP\nFinished for now. You can always return and change it.",16,true);title.CustomMinimumSize=new(280,0);column.AddChild(title);
        _endingWatch=Button("Watch village life",WatchHamlet);column.AddChild(_endingWatch);
        _endingShape=Button("Keep shaping",ReopenHamlet);column.AddChild(_endingShape);
        _endingLeave=Button("Save and leave",ReturnToMainMenu);column.AddChild(_endingLeave);_hamletEnding.Hide();
    }
    private void RenderHamletEnding()
    {
        _hamletEnding.Visible=_world.PublicPlace!=null && _world.Founding!.Finished && !_atMainMenu && !_watching && !_placing && !_drawer.Visible && !_inspector.Visible && !_courtShowBefore;
        _hamletEnding.Position=new(16,92);_hamletEnding.Size=new(306,0);
    }
}
