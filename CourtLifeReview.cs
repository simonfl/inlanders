using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeCourtLife()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        CloseDrawer();ClearSelection();_paused=true;_noticeUntil=0;
        _focus=OnGround(3,3);_camera.Size=CourtZoom(32);UpdateCamera();
        var observed=new HashSet<string>();
        for(int i=0;i<3600 && observed.Count<3;i++)
        {
            _world.Tick(.1f);
            string? activity=_world.People.Any(p=>p.Task==Work.Planting && p.Timer is >.6f and <1.8f) && !observed.Contains("work")?"work":
                _world.People.Any(p=>p.Task==Work.EatingMeal) && !observed.Contains("meal")?"meal":
                _world.People.Any(p=>p.Task==Work.Resting) && !observed.Contains("rest")?"rest":null;
            if(activity==null)continue;
            observed.Add(activity);RenderActors(0);UpdateHud();await Frames();
            string saved=_world.SaveJson();
            foreach(var p in _world.People)
            {
                Check(_people[p.Id].MealBoard.Visible==(ReadableCourt && p.Task==Work.EatingMeal && p.Meal is {Carrying:true}),"Meal board without an actual meal");
                Check(_people[p.Id].RestBack.Visible==(ReadableCourt && p.Task==Work.Resting),"Rest chair leaked into another activity");
                if(ReadableCourt && p.Task==Work.Planting && p.Timer<2.2f)Check(_people[p.Id].Spade.Visible,"Digging has no ground-contact tool");
            }
            await CaptureReviewBundle("court-life-"+activity+(_courtControl?"-control":"-candidate"));
            // Both presentations must render the exact same authoritative world.
            bool control=_courtControl;var headings=_people.Select(p=>p.Body.Rotation).ToArray();
            _courtControl=!control;CreateActors();for(int n=0;n<_people.Count;n++)_people[n].Body.Rotation=headings[n];_camera.Size=CourtZoom(32);UpdateCamera();RenderActors(0);await Frames();
            Check(_world.SaveJson()==saved,"Presentation changed the simulation");
            await CaptureReviewBundle("court-life-"+activity+(_courtControl?"-control":"-candidate"));
            _courtControl=control;CreateActors();for(int n=0;n<_people.Count;n++)_people[n].Body.Rotation=headings[n];_camera.Size=CourtZoom(32);UpdateCamera();RenderActors(0);await Frames();
            Check(_world.SaveJson()==saved,"Restoring presentation changed the simulation");
        }
        Check(observed.Count==3,"Did not observe real work, eating and rest within six simulated minutes");
        ClearSelection();await Frames();Check(!_dailyCard.Visible && !_inspector.Visible,"Activity view needs a card");
        _world.Validate();
        GD.Print("PASS: actual planting/eating/resting, furniture lifecycle, control/candidate rendering isolation, cards closed and unchanged current saves");
    }
}
