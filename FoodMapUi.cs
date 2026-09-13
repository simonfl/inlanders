using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;
public partial class Game
{
    private bool _showFoodMap;
    private World? _foodMapWorld;
    private Button _foodMapToggle=null!;
    private readonly Dictionary<int,(PanelContainer Panel,Label Label,Line2D Line)> _foodMapLabels=new();
    private float _nextFoodMap;
    private FoodMapStore[] _foodMapStores=System.Array.Empty<FoodMapStore>();
    private void ToggleFoodMap()
    {
        _showFoodMap=!_showFoodMap;_nextFoodMap=0;_nextSupplyRefresh=0;
        if(_showFoodMap){CloseDrawer();Notice("Food view: free portions, claimed portions and incoming deliveries. Gold routes carry food; blue routes collect. Toggle in Economy to close.");}
    }
    private void RenderFoodMap()
    {
        if(_foodMapWorld!=_world){_foodMapWorld=_world;_showFoodMap=false;_nextFoodMap=0;foreach(var item in _foodMapLabels.Values){item.Panel.QueueFree();item.Line.QueueFree();}_foodMapLabels.Clear();}
        _foodMapToggle.Text=_showFoodMap?"Hide food in the world":"Show food in the world";
        bool visible=_showFoodMap && !_watching && !_atMainMenu && !_drawer.Visible && !_inspector.Visible;
        foreach(var item in _foodMapLabels.Values){item.Panel.Visible=false;item.Line.Visible=false;}
        if(!_showFoodMap)return;
        bool refresh=_uiTime>=_nextFoodMap;if(refresh)_nextFoodMap=_uiTime+.5f;
        if(refresh)_foodMapStores=_world.ReadFoodMap();var stores=_foodMapStores;var ids=stores.Select(s=>s.Id??-1).ToHashSet();
        foreach(int id in _foodMapLabels.Keys.Where(id=>!ids.Contains(id)).ToArray()){_foodMapLabels[id].Panel.QueueFree();_foodMapLabels[id].Line.QueueFree();_foodMapLabels.Remove(id);}
        var occupied=new List<Rect2>();
        foreach(var s in stores)
        {
            int key=s.Id??-1;
            if(!_foodMapLabels.TryGetValue(key,out var item))
            {
                var line=new Line2D{Width=1.5f,DefaultColor=new("d9d6b0")};_hud.AddChild(line);
                var panel=HudPanel(_hud);panel.MouseFilter=Control.MouseFilterEnum.Ignore;panel.AddThemeStyleboxOverride("panel",HudStyle("223831e8",5));
                var label=Text("",14,true);label.MouseFilter=Control.MouseFilterEnum.Ignore;label.CustomMinimumSize=new(178,0);panel.AddChild(label);
                item=(panel,label,line);_foodMapLabels[key]=item;refresh=true;
            }
            if(refresh)item.Label.Text=$"{s.Name}\n{s.Available} free · {s.Claimed} claimed"+(s.Incoming>0?$" · +{s.Incoming} coming":"")+(!s.Reachable?"\nNo route from yard":s.Paused?"\nWork paused":"");
            item.Label.Modulate=!s.Reachable?new("ec907b"):s.Available>0?new("efe1b0"):new("c1d6df");
            var anchor=_camera.UnprojectPosition(OnGround(s.Access.X,s.Access.Z,.3f));
            if(!visible || anchor.X<0 || anchor.X>_hud.Size.X || anchor.Y<90 || anchor.Y>_hud.Size.Y-80)continue;
            var size=new Vector2(188,System.Math.Max(48,item.Panel.GetCombinedMinimumSize().Y));
            var position=new Vector2(Mathf.Clamp(anchor.X-size.X/2,8,_hud.Size.X-size.X-8),Mathf.Clamp(anchor.Y-size.Y-20,90,_hud.Size.Y-85-size.Y));
            var rect=new Rect2(position,size);
            for(int attempt=0;attempt<12 && occupied.Any(r=>r.Grow(4).Intersects(rect));attempt++)
            {
                float y=position.Y+(attempt%2==0?1:-1)*(attempt/2+1)*(size.Y+6);
                rect.Position=new(position.X,Mathf.Clamp(y,90,_hud.Size.Y-85-size.Y));
            }
            if(occupied.Any(r=>r.Grow(3).Intersects(rect)))continue;
            occupied.Add(rect);item.Panel.Position=rect.Position;item.Panel.Size=size;item.Panel.Show();
            item.Line.Points=new[]{anchor,rect.GetCenter()};item.Line.Show();

        }
    }
}
