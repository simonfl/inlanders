using Godot;

public partial class Game
{
    private void MakeForagerHut(Node3D parent,int stage)
    {
        var poles=new Color("665039");var weave=new Color("bea275");
        // Separate feet leave the woodland floor visible through the open shelter.
        foreach(float x in new[]{-1.13f,1.13f}) foreach(float z in new[]{-.70f,.61f})
            StoneFoot(parent,new(x,.10f,z),new(.32f,.18f,.32f));
        if(stage<1)return;
        foreach(float x in new[]{-1.13f,1.13f})
        {
            Cylinder(parent,new(x,1.19f,-.70f),.105f,2.30f,poles);
            Cylinder(parent,new(x,.94f,.61f),.105f,1.80f,poles);
            TimberBeam(parent,new(x,2.34f,-.70f),new(x,1.84f,.61f),.17f,_frameTimber);
            TimberBeam(parent,new(x,1.23f,.61f),new(x,1.98f,.14f),.12f,poles);
        }
        Box(parent,new(0,2.32f,-.70f),new(2.52f,.18f,.16f),_frameTimber);
        Box(parent,new(0,1.82f,.61f),new(2.52f,.18f,.16f),_frameTimber);
        if(stage<2)return;
        // A low woven windbreak, open sides, and a rear sorting bench.
        foreach(float x in new[]{-1.02f,-.51f,0f,.51f,1.02f})
            Cylinder(parent,new(x,.75f,-.71f),.035f,1.26f,poles);
        for(int row=0;row<7;row++)
            Box(parent,new(0,.22f+row*.16f,-.71f+(row%2==0?.025f:-.025f)),new(2.12f,.10f,.055f),weave);
        Box(parent,new(.05f,.68f,-.43f),new(1.80f,.14f,.48f),new("b09365"));
        foreach(float x in new[]{-.68f,.78f}) Box(parent,new(x,.36f,-.43f),new(.13f,.62f,.36f),_frameTimber);
        foreach(float x in new[]{-.75f,0f,.75f})
            TimberBeam(parent,new(x,2.37f,-.89f),new(x,1.75f,.76f),.11f,poles);
        if(stage<3)return;
        // Deep overlapping roof courses give the inexpensive lean-to a clear edge.
        for(int row=0;row<4;row++)
        {
            float z=-.72f+row*.43f;
            for(int panel=-1;panel<=1;panel++)
            {
                // Each course sits slightly above the previous slope; coplanar overlaps flicker.
                // Warm bark roofing separates the shelter from meadow and foliage.
                var course=Box(parent,new(panel*.94f,2.38f-row*.150f,z),new(.93f,.17f,.53f),new Color("89664f").Lightened(((row+panel+3)%3)*.035f));
                course.RotationDegrees=new(20.8f,0,0);
            }
        }
        foreach(float x in new[]{-1.34f,1.34f})
            TimberBeam(parent,new(x,2.54f,-.94f),new(x,1.84f,.85f),.10f,poles);
        // Empty gathering equipment, not a fictitious stock of stored berries.
        foreach(var at in new[]{new Vector3(-.53f,.88f,-.41f),new(.58f,.22f,-.30f)})
        {
            Cylinder(parent,at,.20f,.31f,weave,.24f);
            Cylinder(parent,at+new Vector3(0,.157f,0),.198f,.013f,_recess);
            foreach(float y in new[]{-.09f,.03f,.12f}) Cylinder(parent,at+new Vector3(0,y,0),.215f,.025f,poles,.235f);
        }
        // Two spare picking poles hang at the rear; the entrance stays open.
        foreach(float x in new[]{.48f,.74f})
            TimberBeam(parent,new(x,.83f,-.66f),new(x+.15f,1.70f,-.66f),.055f,poles);
        FoodSign(parent,"FORAGERS",2.85f);
    }
}
