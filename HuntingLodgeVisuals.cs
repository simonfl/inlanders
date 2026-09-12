using Godot;

public partial class Game
{
    private void MakeHuntingLodge(Node3D parent,int stage)
    {
        var log=new Color("887051");var roofColor=new Color("536c57");
        // Raised timber shelter: open working frontage, enclosed back and weather side.
        foreach(float x in new[]{-1.06f,1.06f})foreach(float z in new[]{-.68f,.58f})
            StoneFoot(parent,new(x,.13f,z),new(.38f,.24f,.38f));
        StoneFoot(parent,new(0,.055f,.74f),new(.80f,.08f,.30f));
        if(stage==0)return;
        foreach(float x in new[]{-1.06f,1.06f})
        {
            foreach(float z in new[]{-.68f,.58f})Box(parent,new(x,1.07f,z),new(.21f,1.82f,.21f),_frameTimber);
            Box(parent,new(x,1.96f,-.05f),new(.22f,.22f,1.67f),_frameTimber);
            TimberBeam(parent,new(x,1.43f,.58f),new(x,1.94f,.08f),.12f,log);
        }
        foreach(float z in new[]{-.68f,.58f})Box(parent,new(0,1.96f,z),new(2.44f,.22f,.22f),_frameTimber);
        if(stage==1)return;
        for(int row=0;row<9;row++)
        {
            float y=.32f+row*.17f;
            Box(parent,new(0,y,-.70f),new(2.27f,.145f,.20f),row%2==0?log:log.Darkened(.07f));
            Box(parent,new(-1.06f,y,-.04f),new(.20f,.145f,1.40f),log);
            Box(parent,new(1.06f,y,-.39f),new(.20f,.145f,.70f),log);
        }
        // An inset rear window, visible on the back; no residential chimney or door.
        CottageWindow(parent,new(.22f,1.17f,-.82f),180,false);
        Box(parent,new(-.20f,.73f,-.35f),new(1.31f,.13f,.44f),new("b79a6f"));
        foreach(float x in new[]{-.70f,.30f})Box(parent,new(x,.42f,-.35f),new(.13f,.57f,.33f),_frameTimber);
        if(stage==2)return;
        VillageRoof(parent,new(0,2.06f,-.05f),2.78f,1.99f,.96f,roofColor,true);
        // Exposed end trusses and contrasting shingle courses give the roof depth.
        foreach(float side in new[]{-1f,1f})
        {
            float x=side*1.25f;
            TimberBeam(parent,new(x,2.05f,-.89f),new(x,2.92f,-.05f),.12f,log);
            TimberBeam(parent,new(x,2.05f,.79f),new(x,2.92f,-.05f),.12f,log);
            TimberBeam(parent,new(x,2.07f,-.05f),new(x,2.94f,-.05f),.10f,_frameTimber);
        }
        foreach(float side in new[]{-1f,1f})for(int row=0;row<3;row++)for(int column=0;column<4;column++)
        {
            float step=.18f+row*.32f;
            var course=Box(parent,new(-1.035f+column*.69f,2.06f+.96f*(1-step)+.14f,-.05f+side*.995f*step),new(.675f,.075f,.47f),roofColor.Lightened(.025f+((row+column)%3)*.035f));
            course.Rotation=new(side*.768f,0,0);
        }
        // Empty equipment identifies the craft without inventing stored game.
        var rack=new Node3D{Position=new(1.20f,1.09f,-.34f),RotationDegrees=new(0,90,0)};parent.AddChild(rack);
        foreach(float x in new[]{-.17f,.17f})
        {
            TimberBeam(rack,new(x,-.45f,0),new(x+.10f,0,.10f),.04f,new("c9ae7c"));
            TimberBeam(rack,new(x+.10f,0,.10f),new(x,.45f,0),.04f,new("c9ae7c"));
            TimberBeam(rack,new(x,-.45f,0),new(x,.45f,0),.012f,new("d9cba6"));
        }
        Cylinder(parent,new(-.66f,.34f,.30f),.23f,.42f,new("a68b61"),.28f);
        Cylinder(parent,new(-.66f,.556f,.30f),.226f,.013f,_recess);
        FoodSign(parent,"HUNTING LODGE",3.45f);
    }
}
