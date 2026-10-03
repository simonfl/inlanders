using Godot;
using Inlanders.Simulation;
using System.Linq;
public partial class Game
{
    private Button _placePath=null!;
    private HBoxContainer _placePathRow=null!;
    private int _pathOrigin=-1;
    private World? _pathOriginWorld;
    private void MakePlacePath(VBoxContainer column)
    {
        _placePathRow=new();column.AddChild(_placePathRow);_turnHomeButton.Reparent(_placePathRow);
        _placePath=Button("Path from here",()=>{
            var site=_world.Cottages.FirstOrDefault(c=>c.Id==_workCardSite);if(site==null)return;
            int id=site.Id;var start=site.Entrance;CloseManagementUi();TogglePaths(3);_placing=true;
            _pathOrigin=id;_pathOriginWorld=_world;_pathAnchor=start;_connectionWorld=null;RefreshGhost();
        });_placePath.TooltipText="Preview a free walking path from this entrance to another place or clear ground. Click to connect; Esc returns without changing paths.";_placePathRow.AddChild(_placePath);
    }
    private void EndPlacePath()
    {
        int id=_pathOrigin;bool same=_pathOriginWorld==_world;_pathOrigin=-1;_pathOriginWorld=null;_pathAnchor=null;_connectionWorld=null;
        _placing=false;_pathTool=0;RefreshGhost();if(same && _world.Cottages.Any(c=>c.Id==id))ShowWorkplaceCard(id);
    }
    private void RenderPlacePath(Cottage site)
    {
        _placePathRow.Visible=_world.PublicPlace!=null && site.Complete && _yardPreviewSide<0 && _reshapingPlot<0 && (Buildings.Get(site.Kind).Beds==0 || _homeOptions);
        _placePath.Disabled=_world.PathProblem(site.Entrance)!=null || _world.Food.Celebrating;
    }
}
