using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private void MakeOpenOven(Node3D root,Cottage site,int stage)
    {
        // Artistic interpretation of the existing timber/rough masonry palette, not a reconstruction.
        StoneFoot(root,new(0,.14f,-.10f),new(1.58f,.26f,1.60f));
        Box(root,new(0,.03f,.62f),new(2.86f,.05f,.62f),new("a38b63"));
        if(stage<1)return;
        foreach(float x in new[]{-1.36f,-.82f})foreach(float z in new[]{-.70f,.47f})
            Box(root,new(x,.72f,z),new(.12f,1.36f,.12f),_frameTimber);
        Box(root,new(0,.44f,-.10f),new(1.42f,.42f,1.40f),new("958773"));
        Box(root,new(1.10f,.38f,0),new(.56f,.72f,1.46f),new("796647"));
        if(stage<2)return;
        var dome=Mesh(root,new SphereMesh{Radius=.73f,Height=1.46f,RadialSegments=12,Rings=6},new(0,.73f,-.13f),new("b2a68e"));
        dome.Scale=new(1,.85f,1);
        Box(root,new(0,1.28f,-.65f),new(.42f,1.14f,.47f),new("a4967e"));
        Box(root,new(0,1.88f,-.65f),new(.55f,.14f,.60f),new("8e806b"));
        Box(root,new(0,1.96f,-.65f),new(.27f,.02f,.31f),_recess);
        // The oven mouth faces the actual central worker/collection approach.
        Box(root,new(0,.69f,.60f),new(.57f,.50f,.10f),_recess);
        foreach(float x in new[]{-.38f,.38f})Box(root,new(x,.67f,.63f),new(.17f,.61f,.22f),new("c1b397"));
        Box(root,new(0,1.01f,.63f),new(.91f,.18f,.24f),new("c1b397"));
        Box(root,new(0,.40f,.76f),new(.96f,.16f,.39f),new("94866d"));
        Box(root,new(1.10f,.78f,0),new(.68f,.10f,1.53f),new("c0a47a"));
        if(stage<3)return;
        var shelter=Box(root,new(-1.07f,1.44f,-.10f),new(.83f,.13f,1.60f),new("697565"));shelter.RotationDegrees=new(0,0,-13);
        for(int i=0;i<site.InputGrain;i++)
        {
            var sack=Mesh(root,new SphereMesh{Radius=.17f,Height=.40f,RadialSegments=7,Rings=4},new(-1.07f,.25f,-.42f+i*.43f),new("c9b68a"));sack.Name="GrainSack"+i;
        }
        for(int i=0;i<site.OutputBread;i++)MakeOvenLoaf(root,new(.96f+i%2*.25f,.92f+i/4*.10f,-.48f+i/2%2*.23f),"BreadLoaf"+i);
        var glow=Box(root,new(0,.56f,.66f),new(.44f,.13f,.026f),new("e4a254"));glow.Name="OvenGlow";glow.Visible=false;
        var mat=(StandardMaterial3D)glow.MaterialOverride;mat.EmissionEnabled=true;mat.Emission=new("bf6a2d");
        FoodSign(root,"BAKERY",2.25f);
    }
    private void MakeOvenLoaf(Node3D parent,Vector3 at,string name)
    {
        var loaf=Mesh(parent,new SphereMesh{Radius=.12f,Height=.16f,RadialSegments=8,Rings=4},at,new("d9a053"));loaf.Scale=new(.8f,1,1.25f);loaf.Name=name;
    }
}
