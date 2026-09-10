using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    // Pair only people who have arrived. Greedy pairing is mutual and stable
    // for the same saved set of visitors; walking arrivals never become partners.
    private Villager? SquareCompanion(Villager person)
    {
        var visitors=_world.People.Where(p=>p.Task==Work.Leisure && p.LeisureSiteId==person.LeisureSiteId).OrderBy(p=>p.Id).ToList();
        while(visitors.Count>1)
        {
            var first=visitors[0]; visitors.RemoveAt(0);
            var other=visitors.Where(p=>(p.Position-first.Position).LengthSquared()<=6.25f)
                .OrderBy(p=>(p.Position-first.Position).LengthSquared()).ThenBy(p=>p.Id).FirstOrDefault();
            if(other==null) continue;
            visitors.Remove(other);
            if(first.Id==person.Id) return other;
            if(other.Id==person.Id) return first;
        }
        return null;
    }

    private static void FaceVisit(PersonView view,Vector3 direction)
    {
        if(new Vector2(direction.X,direction.Z).LengthSquared()>.001f)
            view.Body.Rotation=new(0,MathF.Atan2(-direction.X,-direction.Z),0);
    }

    private void AnimateSquareVisit(PersonView view,Villager person)
    {
        var companion=SquareCompanion(person);
        if(companion==null)
        {
            var square=_world.Cottages.FirstOrDefault(c=>c.Id==person.LeisureSiteId);
            if(square!=null) FaceVisit(view,view.Body.Position-new Vector3(square.Cell.X,view.Body.Position.Y,square.Cell.Z));
            view.Head.Rotation=new(.04f,MathF.Sin(_world.Food.Time*.5f+person.Id)*.15f,0);
            view.Arm.Rotation=new(.12f,0,-.06f); view.LeftArm.Rotation=new(.12f,0,.06f);
            return;
        }
        FaceVisit(view,new Vector3(companion.Position.X,view.Body.Position.Y,companion.Position.Y)-view.Body.Position);
        float time=_world.Food.Time+Math.Min(person.Id,companion.Id)*.37f;
        bool speaker=((int)(time/3)%2==0)==(person.Id<companion.Id);
        float phase=time%3;
        float gesture=phase<1.1f?MathF.Sin(phase/1.1f*Mathf.Pi):0;
        view.Arm.Rotation=new(.12f+(speaker ? .65f*gesture : 0),0,-.06f-(speaker ? .16f*gesture : 0));
        view.LeftArm.Rotation=new(.12f,0,.06f);
        view.Head.Rotation=new(speaker ? .04f : .04f+.09f*gesture,0,0);
        view.Torso.Rotation=new(-.025f,0,0);
    }

    private void AnimateHomeRest(PersonView view,Villager person)
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==person.HomeId);
        if(home==null) return;
        FaceVisit(view,view.Body.Position-new Vector3(home.Cell.X,view.Body.Position.Y,home.Cell.Z));
        view.RestStool.Visible=true;
        view.Rig.Position=new(0,-.20f,0);
        view.LeftLeg.Rotation=new(Mathf.Pi/2,0,-.08f); view.RightLeg.Rotation=new(Mathf.Pi/2,0,.08f);
        float settle=Mathf.SmoothStep(0,1,person.Timer/.8f);
        view.Head.Rotation=new((person.Id%3==0 ? .24f : .12f)*settle,0,0);
        view.Arm.Rotation=new(.85f,0,-.12f); view.LeftArm.Rotation=new(.85f,0,.12f);
        view.Torso.Rotation=new(.06f,0,MathF.Sin(_world.Food.Time*.8f+person.Id)*.012f);
    }
}
