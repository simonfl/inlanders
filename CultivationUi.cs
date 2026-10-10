using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private int _plotRows=5;
    private PanelContainer _plotPanel=null!;
    private Label _plotInfo=null!;
    private Button _plotLess=null!,_plotMore=null!;
    private bool PlotActive=>_world.PublicPlace!=null && _placing && _movingSite<0 && _buildKind==BuildingKind.VegetableField && !_plantingTrees && !_clearingTrees && !_decorating && _pathTool==0 && _woodlandTool==0;
    private int PlacementRows=>_movingSite>=0?_world.Cottages.Find(c=>c.Id==_movingSite)?.PlotRows??0:PlotActive?_plotRows:0;
    private void MakeCultivationUi()
    {
        _plotPanel=HudPanel(_hud);var column=new VBoxContainer();_plotPanel.AddChild(column);column.AddChild(Text("CULTIVATE GROUND",15));
        _plotInfo=Text("",14,true);_plotInfo.CustomMinimumSize=new(266,0);column.AddChild(_plotInfo);
        var row=new HBoxContainer();column.AddChild(row);
        _plotLess=Button("Shorter [Z]",()=>ChangePlotRows(-1));row.AddChild(_plotLess);_plotMore=Button("Longer [X]",()=>ChangePlotRows(1));row.AddChild(_plotMore);_plotPanel.Hide();
    }
    private void ChangePlotRows(int change){_plotRows=System.Math.Clamp(_plotRows+change,1,8);_livelihoodSite=null;RefreshGhost();}
    private void RenderCultivationUi()
    {
        _plotPanel.Visible=PlotActive && !_drawer.Visible && !_atMainMenu;if(!_plotPanel.Visible)return;
        _plotPanel.Position=new(16,92);_plotPanel.Size=new(290,0);
        _plotInfo.Text=$"3 × {_plotRows} tiles · {_plotRows*4} vegetables/crop\n"+(_world.Creative?"Free cultivation":$"{_plotRows*2} logs")+" · one farmer\nDrag from the entrance row toward the far row, or click the chosen size. Esc cancels.\n"+(!PointerOverHud(_pointerPosition)?(_ghostValid?(_livelihoodSite?.Summary??"Leave room for homes and access."):_groundPreparation!=null?$"Shift-click plans clearance of {_groundPreparation.Trees.Length} trees/stumps, then construction here.":_placementProblem):"Z / X changes length; R rotates.");
        _plotInfo.TooltipText="Longer strips use more land, timber and harvesting work. All sizes use one farmer, collecting a row of four vegetables per trip. Preparation and crops use the real footprint.";
        _plotLess.Disabled=_plotRows==1;_plotMore.Disabled=_plotRows==8;
    }
}
