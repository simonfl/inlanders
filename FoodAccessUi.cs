using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private bool _foodAccessPlanning;
    private Cell? _foodAccessAt,_foodAccessA;
    private World? _foodAccessWorld;
    private PanelContainer _foodAccessPanel=null!;
    private Label _foodAccessText=null!;
    private Button _foodAccessEntry=null!,_foodAccessPin=null!,_foodAccessClear=null!;
    private Label _foodAccessALabel=null!,_foodAccessBLabel=null!;
    private Line2D _foodAccessLine=null!,_foodAccessALine=null!;
    private FoodAccessRoute[] _foodAccessARoutes=System.Array.Empty<FoodAccessRoute>();
    private FoodAccessRoute[] _foodAccessRoutes=System.Array.Empty<FoodAccessRoute>();
    private float _foodAccessRefresh;
    private void MakeFoodAccessUi()
    {
        _foodAccessEntry=Button("Compare food access on the ground",()=>{
            CloseManagementUi();_placing=false;RefreshGhost();_showFoodMap=false;
            _foodAccessPlanning=true;_foodAccessWorld=_world;_foodAccessAt=null;_foodAccessA=null;_foodAccessRefresh=0;
        });_foundingGoals.AddChild(_foodAccessEntry);
        _foodAccessLine=new(){Width=3,DefaultColor=new("efd28a")};_hud.AddChild(_foodAccessLine);
        _foodAccessALine=new(){Width=3,DefaultColor=new("a7d5e3")};_hud.AddChild(_foodAccessALine);
        _foodAccessALabel=Text("A",18);_foodAccessALabel.MouseFilter=Control.MouseFilterEnum.Ignore;_hud.AddChild(_foodAccessALabel);
        _foodAccessBLabel=Text("B",18);_foodAccessBLabel.MouseFilter=Control.MouseFilterEnum.Ignore;_hud.AddChild(_foodAccessBLabel);
        _foodAccessPanel=HudPanel(_hud);var column=new VBoxContainer();_foodAccessPanel.AddChild(column);
        _foodAccessText=Text("",14,true);_foodAccessText.CustomMinimumSize=new(270,0);column.AddChild(_foodAccessText);
        _foodAccessPin=Button("Keep this spot as A",()=>{_foodAccessA=_foodAccessAt;_foodAccessRefresh=0;});column.AddChild(_foodAccessPin);
        _foodAccessClear=Button("Clear A",()=>{_foodAccessA=null;_foodAccessRefresh=0;});column.AddChild(_foodAccessClear);
        column.AddChild(Button("Done [Esc]",()=>_foodAccessPlanning=false));_foodAccessPanel.Hide();
    }
    private void RenderFoodAccess()
    {
        if(_foodAccessWorld!=_world || _atMainMenu || _placing || _watching || _drawer.Visible || _inspector.Visible)_foodAccessPlanning=false;
        _foodAccessPanel.Visible=_foodAccessLine.Visible=_foodAccessPlanning;
        _foodAccessALine.Visible=_foodAccessALabel.Visible=_foodAccessPlanning && _foodAccessA!=null;
        _foodAccessBLabel.Visible=_foodAccessPlanning && _foodAccessAt!=null;
        if(!_foodAccessPlanning)return;
        _foodAccessPanel.Position=new(_hud.Size.X-310,92);_foodAccessPanel.Size=new(294,0);
        if(_uiTime>=_foodAccessRefresh)
        {
            _foodAccessRefresh=_uiTime+.5f;_foodAccessRoutes=_foodAccessAt is Cell at?_world.ReadFoodAccess(at):System.Array.Empty<FoodAccessRoute>();
            _foodAccessARoutes=_foodAccessA is Cell a?_world.ReadFoodAccess(a):System.Array.Empty<FoodAccessRoute>();
            string Spot(string label,FoodAccessRoute[] routes)=>label+"\n"+(routes.Length==0?"No route to a food store.":
                $"{routes[0].Store.Name.Replace(" store","")}: {routes[0].Route.Length} steps\n{routes[0].Store.Available} free · "+(routes[0].ServesSharedPlace?"shared-place reach":"beyond shared-place reach"));
            _foodAccessText.Text="COMPARE FOOD ACCESS\n"+(_foodAccessAt==null?"Click clear ground. Keep one spot as A, then inspect another.":
                _foodAccessA==null?Spot("B · current spot",_foodAccessRoutes):Spot("A · kept spot",_foodAccessARoutes)+"\n\n"+Spot("B · current spot",_foodAccessRoutes))+
                "\n\nNearest stocked source, or nearest empty store.\nBlue A · gold B\nCurrent routes, not a supply forecast.";
            _foodAccessPin.Disabled=_foodAccessAt==null || _foodAccessRoutes.Length==0;
            _foodAccessPin.Text=_foodAccessA==null?"Keep this spot as A":"Replace A with this spot";
            _foodAccessClear.Visible=_foodAccessA!=null;
        }
        _foodAccessALine.Points=_foodAccessA is Cell origin && _foodAccessARoutes.Length>0?new[]{origin}.Concat(_foodAccessARoutes[0].Route).Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.18f))).ToArray():System.Array.Empty<Vector2>();
        if(_foodAccessA is Cell markerA)_foodAccessALabel.Position=_camera.UnprojectPosition(OnGround(markerA.X,markerA.Z,.2f))+new Vector2(-8,-25);
        if(_foodAccessAt is Cell markerB)_foodAccessBLabel.Position=_camera.UnprojectPosition(OnGround(markerB.X,markerB.Z,.2f))+new Vector2(5,-25);
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
