using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private int _homePlotSide=-1;
    private PanelContainer _homePlotPanel=null!;
    private Button _homePlotChoice=null!;
    private Label _homePlotInfo=null!;
    private bool HomePlotActive=>_world.PublicPlace!=null && _placing && _movingSite<0 && !_plantingTrees && !_clearingTrees && !_decorating && _pathTool==0 && _woodlandTool==0 && Buildings.Get(_buildKind).Beds>0;
    private void MakeHomePlotUi()
    {
        _homePlotPanel=HudPanel(_hud);var column=new VBoxContainer();_homePlotPanel.AddChild(column);column.AddChild(Text("HOME AND OUTDOOR LIFE",15));
        _homePlotChoice=Button("",CycleHomePlot);column.AddChild(_homePlotChoice);
        _homePlotInfo=Text("",14,true);_homePlotInfo.CustomMinimumSize=new(266,0);column.AddChild(_homePlotInfo);_homePlotPanel.Hide();
    }
    private void CycleHomePlot(){_homePlotSide=(_homePlotSide+2)%5-1;RefreshGhost();}
    private void RenderHomePlotUi()
    {
        _homePlotPanel.Visible=HomePlotActive && !_drawer.Visible && !_atMainMenu;if(!_homePlotPanel.Visible)return;
        _homePlotPanel.Position=new(16,92);_homePlotPanel.Size=new(290,0);
        _homePlotChoice.Text=_homePlotSide<0?"House only · add yard [Y]":"Yard: "+World.YardSideName(_homePlotSide)+" [Y]";
        _homePlotInfo.Text=_homePlotSide<0?"A yard is optional. Preview it now or furnish later from the home.":
            "Home + yard · "+(_world.Creative?"free":BuildCost(_buildKind)+" + "+Buildings.Get(_buildKind).Beds*2+" planks")+". Workers furnish after someone moves in. Residents mend and take nearby meals here.";
        if(_homePlotSide>=0 && !PointerOverHud(_pointerPosition))_homePlotInfo.Text+="\n"+(_world.PreviewHomePlot(_hover,_rotation,_buildKind,_homePlotSide).Problem??"Blue furniture shows the usable yard. R rotates the home; Y changes the side.");
    }
    private void RefreshHomePlotPreview()
    {
        if(!HomePlotActive || _homePlotSide<0 || !_ghostValid)return;
        foreach(var cell in _world.PreviewHomePlot(_hover,_rotation,_buildKind,_homePlotSide).Yard)MakeHomeYardFurniture(_ghostCells,cell,true);
    }
}
