using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _foodAccessPlanning;
    private Cell? _foodAccessAt;
    private World? _foodAccessWorld;
    private PanelContainer _foodAccessPanel=null!;
    private Label _foodAccessText=null!;
    private Button _foodAccessEntry=null!;
    private Line2D _foodAccessLine=null!;
    private FoodAccessRoute[] _foodAccessRoutes=System.Array.Empty<FoodAccessRoute>();
    private float _foodAccessRefresh;
    private void MakeFoodAccessUi()
    {
        _foodAccessEntry=Button("Compare food access on the ground",()=>{
            CloseManagementUi();_placing=false;RefreshGhost();_showFoodMap=false;
            _foodAccessPlanning=true;_foodAccessWorld=_world;_foodAccessAt=null;_foodAccessRefresh=0;
        });_foundingGoals.AddChild(_foodAccessEntry);
        _foodAccessLine=new(){Width=3,DefaultColor=new("efd28a")};_hud.AddChild(_foodAccessLine);
        _foodAccessPanel=HudPanel(_hud);var column=new VBoxContainer();_foodAccessPanel.AddChild(column);
        _foodAccessText=Text("",14,true);_foodAccessText.CustomMinimumSize=new(270,0);column.AddChild(_foodAccessText);
        column.AddChild(Button("Done [Esc]",()=>_foodAccessPlanning=false));_foodAccessPanel.Hide();
    }
    private void RenderFoodAccess()
    {
        if(_foodAccessWorld!=_world || _atMainMenu || _placing || _watching || _drawer.Visible || _inspector.Visible)_foodAccessPlanning=false;
        _foodAccessPanel.Visible=_foodAccessLine.Visible=_foodAccessPlanning;if(!_foodAccessPlanning)return;
        _foodAccessPanel.Position=new(_hud.Size.X-310,92);_foodAccessPanel.Size=new(294,0);
        if(_uiTime>=_foodAccessRefresh)
        {
            _foodAccessRefresh=_uiTime+.5f;_foodAccessRoutes=_foodAccessAt is Cell at?_world.ReadFoodAccess(at):System.Array.Empty<FoodAccessRoute>();
            _foodAccessText.Text="FOOD ACCESS\n"+(_foodAccessAt==null?"Click clear ground to compare where meals could come from.":_foodAccessRoutes.Length==0?"No walking route from this spot to a food store.":string.Join("\n\n",_foodAccessRoutes.Take(3).Select(r=>$"{r.Store.Name.Replace(" store","")}: {r.Route.Length} steps\n{r.Store.Available} free · "+(r.ServesSharedPlace?"shared-place reach":"beyond shared-place reach"))))+
                "\n\nGold: nearest stocked source, or nearest empty store.\nCurrent routes, not a supply forecast. Building may change access.\n\nClick elsewhere to compare.";
        }
        _foodAccessLine.Points=_foodAccessAt is Cell start && _foodAccessRoutes.Length>0?new[]{start}.Concat(_foodAccessRoutes[0].Route).Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.15f))).ToArray():System.Array.Empty<Vector2>();
    }
    private bool HandleFoodAccessInput(InputEvent input)
    {
        if(!_foodAccessPlanning)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){_foodAccessPlanning=false;return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){_foodAccessPlanning=false;return false;}
        if(input is not InputEventMouseButton{ButtonIndex:MouseButton.Left} click || PointerOverHud(click.Position))return false;
        if(click.Pressed && Ground(click.Position) is Vector3 p){_foodAccessAt=new(Mathf.RoundToInt(p.X),Mathf.RoundToInt(p.Z));_foodAccessRefresh=0;}
        return true;
    }
}
