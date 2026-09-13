using Godot;
public partial class Game
{
    private Node3D? _commonsView;
    private string _commonsVisualKey="";
    private void UpdateCommonsView()
    {
        if(_commonsEntry==null)return;
        _commonsEntry.Visible=_world.Neighborhood?.Complete==true;
        _commonsEntry.Text=_world.Commons==null?"Make a shared place":"Rearrange shared place";
        _commonsRemove.Visible=_world.Commons!=null;
        string key=_world.Commons is {} c?$"{c.Center}:"+string.Join(';',c.Places):"";
        if(key==_commonsVisualKey && _commonsView!=null)return;
        _commonsVisualKey=key;
        if(_commonsView==null){_commonsView=new();AddChild(_commonsView);}else Clear(_commonsView);
        if(_world.Commons is not {} commons)return;
        // Low woven mats keep walking routes open, even while the place is empty.
        foreach(var p in commons.Places)
        {
            Cylinder(_commonsView,OnGround(p.X,p.Z,.025f),.43f,.045f,new("b89569"));
            Cylinder(_commonsView,OnGround(p.X,p.Z,.05f),.31f,.012f,new("758d7b"));
        }
        Box(_commonsView,OnGround(commons.Center.X,commons.Center.Z,.035f),new(.9f,.05f,.9f),new("ccaa79"));
        Box(_commonsView,OnGround(commons.Center.X,commons.Center.Z,.065f),new(.65f,.02f,.65f),new("a9634b"));
    }
}
