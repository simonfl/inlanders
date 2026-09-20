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
        foreach(var c in places)MakeHomeYardFurniture(_homeYards,c,false);
    }
    private void MakeHomeYardFurniture(Node3D root,Inlanders.Simulation.Cell c,bool preview)
    {
        Color wood=preview?new("63c5cf"):new("92734e"),legs=preview?new("366e82"):_frameTimber;
        if(!preview)Box(root,OnGround(c.X,c.Z,.025f),new(.88f,.05f,.88f),new("a08b69"));
        // The same geometry in the proposal and finished yard; center remains free for residents.
        Box(root,OnGround(c.X,c.Z-.33f,.34f),new(.74f,.10f,.20f),wood);
        foreach(float x in new[]{-.25f,.25f})Box(root,OnGround(c.X+x,c.Z-.33f,.17f),new(.09f,.30f,.16f),legs);
        Box(root,OnGround(c.X+.33f,c.Z+.25f,.16f),new(.18f,.27f,.20f),preview?wood:new("b19769"));
    }
}
