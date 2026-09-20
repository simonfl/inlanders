using Godot;
using System.Linq;
public partial class Game
{
    private Node3D? _homeYards;
    private string _homeYardKey="";
    private void UpdateHomeYards()
    {
        var places=_world.Cottages.SelectMany(c=>_world.HomeYardPlaces(c)).ToArray();
        string key=string.Join(';',places)+":"+string.Join(';',places.Select(c=>Height(c.X,c.Z)));
        if(_homeYards!=null && key==_homeYardKey)return;
        _homeYardKey=key;
        if(_homeYards==null){_homeYards=new();AddChild(_homeYards);}else Clear(_homeYards);
        foreach(var c in places)
        {
            Box(_homeYards,OnGround(c.X,c.Z,.025f),new(.88f,.05f,.88f),new("a08b69"));
            for(int i=0;i<4;i++)Box(_homeYards,OnGround(c.X-.3f+i*.2f,c.Z,.055f),new(.17f,.03f,.78f),new("b79b72"));
        }
    }
}
