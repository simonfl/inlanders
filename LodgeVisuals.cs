using Godot;

public partial class Game
{
    private void MakeLodge(Node3D parent,int stage)
    {
        var siding=new Color("ba996d");
        var roof=new Color("617e88");
        void Window(Vector3 at,float yaw=0)=>CottageWindow(parent,at,yaw,false);
        // A loft over the shared ground floor gives four beds a distinct silhouette.
        StoneFoot(parent,new(0,.16f,-.12f),new(2.55f,.30f,1.55f));
        StoneFoot(parent,new(0,.08f,.83f),new(.92f,.13f,.40f));
        if(stage<1)return;
        foreach(float x in new[]{-1.18f,0f,1.18f}) foreach(float z in new[]{-.83f,.59f})
            Box(parent,new(x,1.43f,z),new(.16f,2.52f,.16f),_frameTimber);
        foreach(float z in new[]{-.83f,.59f}) foreach(float y in new[]{1.50f,2.65f})
            Box(parent,new(0,y,z),new(2.60f,.16f,.17f),_frameTimber);
        foreach(float x in new[]{-1.18f,1.18f})
        {
            Box(parent,new(x,2.65f,-.12f),new(.18f,.16f,1.56f),_frameTimber);
            TimberBeam(parent,new(x,1.65f,-.75f),new(x,2.57f,.48f),.12f,_frameTimber);
        }
        if(stage<2)return;
        Box(parent,new(0,1.40f,-.12f),new(2.34f,2.26f,1.36f),siding);
        // Broad courses remain readable at village zoom without fine plank noise.
        foreach(float z in new[]{-.813f,.573f}) for(int i=0;i<8;i++)
            Box(parent,new(0,.39f+i*.28f,z),new(2.33f,.025f,.035f),siding.Darkened(.14f));
        Box(parent,new(0,.88f,.59f),new(.71f,1.26f,.07f),_recess);
        Box(parent,new(0,.85f,.635f),new(.52f,1.15f,.045f),new("735940"));
        foreach(float x in new[]{-.39f,.39f}) Box(parent,new(x,.89f,.68f),new(.11f,1.36f,.17f),_frameTimber);
        Box(parent,new(0,1.54f,.68f),new(.91f,.14f,.17f),_frameTimber);
        Box(parent,new(.18f,.88f,.67f),new(.06f,.09f,.04f),new("c1a371"));
        Window(new(.77f,1.03f,.585f));
        Window(new(-.77f,1.03f,.585f));
        foreach(float x in new[]{-.72f,.72f}) Window(new(x,2.09f,.585f));
        foreach(float side in new[]{-1f,1f})
        {
            Window(new(side*1.185f,1.10f,-.20f),side*90);
            Window(new(side*1.185f,2.09f,-.20f),side*90);
        }
        Window(new(-.20f,1.10f,-.815f),180);
        foreach(float x in new[]{-.72f,.72f}) Window(new(x,2.09f,-.815f),180);
        if(stage<3)return;
        VillageRoof(parent,new(0,2.70f,-.12f),2.88f,1.96f,.90f,roof,true);
        // An open porch keeps the shared entrance in shade without blocking its tile.
        foreach(float x in new[]{-.51f,.51f})
        {
            StoneFoot(parent,new(x,.12f,.83f),new(.23f,.20f,.23f));
            Box(parent,new(x,.90f,.83f),new(.10f,1.57f,.10f),_frameTimber);
            TimberBeam(parent,new(x,1.25f,.83f),new(x,1.71f,.53f),.09f,_frameTimber);
        }
        var canopy=Box(parent,new(0,1.72f,.75f),new(1.36f,.13f,.65f),roof);
        canopy.RotationDegrees=new(16,0,0);
        Box(parent,new(-.72f,3.20f,-.43f),new(.37f,1.45f,.40f),_stone.Darkened(.10f));
        Box(parent,new(-.72f,3.95f,-.43f),new(.51f,.16f,.53f),_stone);
        Box(parent,new(-.72f,4.04f,-.43f),new(.24f,.02f,.26f),_recess);
        FoodSign(parent,"LODGE · 4 BEDS",4.35f);
    }
}
