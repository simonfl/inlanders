using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private static string OrchardDescription(Cottage c)=>$"Orchard · 1 shared farmer slot\n{(c.OrchardMature?"Mature trees retained":c.Planted?"Establishing fruit trees":"Waiting for a farmer to plant trees")}\n{c.Harvest} fruit ripe · 8 per harvest\n"+
        (c.Planted && c.Harvest==0?$"{c.Growth:P0} · about {(1-c.Growth)*(c.OrchardMature?60:180):0}s until ripe\n":"")+
        "First fruit: 3 minutes after planting. Then 60s growth after each harvest; no resowing. Keep quick food during establishment. Clearing loses mature trees; ripe fruit is recovered.";
    private static Vector3 OrchardTreePosition(int tree)=>new(tree%2==0?-.78f:.78f,0,tree/2==0?-.46f:.46f);
    private static Vector3 OrchardFruitPosition(int fruit)=>OrchardTreePosition(fruit/2)+new Vector3(fruit%2==0?-.25f:.25f,1.15f,.32f);
    private void MakeFruit(Node3D root,Vector3 at,float radius)
    {
        Mesh(root,new SphereMesh{Radius=radius,Height=radius*1.8f,RadialSegments=7,Rings=4},at,new("bd5544"));
        Cylinder(root,at+new Vector3(0,radius,0),.018f,.10f,_wood);
    }
    private void MakeOrchardPlot(Node3D root,int stage)
    {
        foreach(float x in new[]{-1.35f,1.35f})foreach(float z in new[]{-.85f,.85f})
            Box(root,new(x,.18f,z),new(.07f,.36f,.07f),_wood);
        if(stage<1)return;
        for(int i=0;i<4;i++)Cylinder(root,OrchardTreePosition(i)+new Vector3(0,.025f,0),.42f,.045f,new("8a7952"));
        if(stage<2)return;
        Box(root,new(0,.16f,-.84f),new(.65f,.22f,.35f),new("99764d"));
        if(stage<3)return;
        Cylinder(root,new(1.2f,.20f,.60f),.14f,.32f,new("8f9d91"));
    }
    private void MakeOrchardTrees(Node3D root,Cottage site,int stage)
    {
        if(!site.Planted && !site.OrchardMature)return;
        float size=site.OrchardMature?1:stage==1?.3f:stage==2?.55f:.8f;
        for(int i=0;i<4;i++)
        {
            var tree=new Node3D{Position=OrchardTreePosition(i),Scale=Vector3.One*size};root.AddChild(tree);
            Cylinder(tree,new(0,.60f,0),.075f,1.2f,_wood,.045f);
            TimberBeam(tree,new(0,.6f,0),new(.32f,1.05f,0),.06f,_wood);
            TimberBeam(tree,new(0,.7f,0),new(-.3f,1.10f,.10f),.06f,_wood);
            Mesh(tree,new SphereMesh{Radius=.55f,Height=1.02f,RadialSegments=7,Rings=4},new(0,1.26f,0),new(i%2==0?"6e894b":"7b9853"));
        }
        for(int i=0;i<site.Harvest;i++)MakeFruit(root,OrchardFruitPosition(i),.12f);
        BatchStaticGeometry(root);
    }
}
