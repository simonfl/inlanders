using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private LivelihoodSite? _livelihoodSite;
    private World? _livelihoodWorld;
    private (Cell,int,BuildingKind) _livelihoodKey;
    private double _nextLivelihoodPreview;
    private bool LivelihoodPreviewActive=>_world.PublicPlace!=null && _placing && _movingSite<0 && !_plantingTrees && !_clearingTrees && !_decorating && _pathTool==0 && _woodlandTool==0 && _ghostValid && !PointerOverHud(_pointerPosition);
    private void RefreshLivelihoodPreview()
    {
        if(!LivelihoodPreviewActive){_livelihoodSite=null;return;}
        var key=(_hover,_rotation,_buildKind);
        if(_livelihoodSite==null || _livelihoodWorld!=_world || key!=_livelihoodKey || _uiTime>=_nextLivelihoodPreview)
        {_livelihoodWorld=_world;_livelihoodKey=key;_nextLivelihoodPreview=_uiTime+.5;_livelihoodSite=_world.ReadLivelihoodSite(_hover,_rotation,_buildKind);}
        if(_livelihoodSite is not {} site)return;
        for(int i=0;i<site.Route.Length;i++)
        {var c=site.Route[i];GroundPatch(_ghostCells,c.X,c.Z,.23f,.23f,new("89c7cd"),.14f);}
        if(site.Route.Length>0){var c=site.Route[^1];var mark=new Node3D{Position=OnGround(c.X,c.Z,.2f)};_ghostCells.AddChild(mark);FoodSign(mark,site.Destination,.5f);}
    }
}
