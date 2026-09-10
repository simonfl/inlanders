using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeSeatingGarden(Node3D parent,int stage)
    {
        // Everything permanent fits the single reserved tile; visitors bring stools
        // to real, reserved walkable destinations outside the planted bed.
        Box(parent,new(0,.07f,0),new(.86f,.14f,.86f),new("8f7350"));
        if(stage==0) return;
        foreach(float x in new[]{-.39f,.39f})
            Box(parent,new(x,.18f,0),new(.08f,.18f,.86f),_wood);
        foreach(float z in new[]{-.39f,.39f})
            Box(parent,new(0,.18f,z),new(.86f,.18f,.08f),_wood);
        if(stage<2) return;
        foreach(float x in new[]{-.32f,.32f})
            Box(parent,new(x,.64f,-.28f),new(.055f,1.1f,.055f),_wood);
        foreach(float y in new[]{.5f,.78f,1.06f})
            Box(parent,new(0,y,-.28f),new(.72f,.045f,.045f),_wood);
        if(stage<3) return;
        foreach(float x in new[]{-.23f,0,.23f})
        {
            Box(parent,new(x,.32f,.1f),new(.18f,.23f,.34f),new("67864e"));
            Box(parent,new(x,.47f,.08f),new(.13f,.10f,.13f),new(x==0?"e2bd70":"c17c83"));
            Box(parent,new(x,.78f,-.25f),new(.13f,.5f,.12f),new("789753"));
        }
        FoodSign(parent,"SEATING GARDEN",1.55f);
    }
}
