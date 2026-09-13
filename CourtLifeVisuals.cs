using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    // Development comparison only; ordinary court play uses the candidate.
    private bool _courtControl;
    private bool ReadableCourt=>_world.IsArrangementCourt && !_courtControl;
    private float CourtZoom(float previous)=>ReadableCourt?previous*.88f:previous;

    private void MakeDailyFurniture(PersonView view)
    {
        view.Body.AddChild(view.MealBoard);view.MealBoard.Visible=false;
        Box(view.MealBoard,new(0,.53f,-.47f),new(.82f,.08f,.48f),new("e7dbc0"));
        foreach(float x in new[]{-.28f,.28f})Box(view.MealBoard,new(x,.26f,-.47f),new(.07f,.50f,.35f),_wood);
        view.RestStool.AddChild(view.RestBack);view.RestBack.Visible=false;
        foreach(float x in new[]{-.18f,.18f})Box(view.RestBack,new(x,.54f,.17f),new(.055f,.60f,.06f),_wood);
        Box(view.RestBack,new(0,.77f,.17f),new(.43f,.13f,.06f),new("b69161"));
    }

    private void AnimateCourtMeal(PersonView view,Villager person)
    {
        view.MealBoard.Visible=person.Meal is {Carrying:true};
        view.RestStool.Visible=true;view.Rig.Position=new(0,-.20f,0);
        view.LeftLeg.Rotation=new(Mathf.Pi/2,0,-.12f);view.RightLeg.Rotation=new(Mathf.Pi/2,0,.12f);
        // The actual carried portion rests on the board; no extra serving or resource.
        float bite=(MathF.Sin(person.Timer*2.8f)+1)*.5f;
        view.Torso.Rotation=new(.12f,0,0);view.Head.Rotation=new(.24f,0,0);
        view.LeftArm.Rotation=new(1.0f,0,.14f);
        view.Arm.Rotation=new(1.05f+bite*.9f,0,-.15f);
        view.Carry.GlobalTransform=view.Body.GlobalTransform*new Transform3D(Basis.Identity,new(0,.67f,-.47f));
    }
}
