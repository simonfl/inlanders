using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunVillagerPresentationSmoke()
    {
        const string folder="artifacts/villagers-256";
        Directory.CreateDirectory(folder);
        void Check(bool ok,string message){if(!ok)throw new Exception(message);}
        async Task Frames(){for(int i=0;i<3;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        try
        {
            SetProcess(false);_paused=true;GetWindow().Size=new(960,640);
            AdoptWorld(World.NewRiverClosing());CloseManagementUi();_hud.Hide();_watchRoot.Hide();
            _landscape.Hide();_dynamic.Hide();
            var stage=new Node3D();AddChild(stage);
            Box(stage,new(0,-.08f,1),new(12,.12f,9),new("a9b28e"));
            var portraits=new PersonView[8];
            for(int i=0;i<portraits.Length;i++)
            {
                var view=MakeVillager(i);portraits[i]=view;stage.AddChild(view.Body);
                view.Body.Position=new((i%4-1.5f)*1.65f,0,i/4*2.1f);
                view.Body.Rotation=new(0,Mathf.Pi+.2f,0);
                AnimateVillager(view,new Villager{Id=i,Task=Work.Waiting});
                Check(view.LeftKnee.GetParent()==view.LeftLeg && view.LeftKnee.GetChildCount()>0,"Knee lost during batching");
            }
            _focus=new(0,.5f,1);_angle=.2f;_camera.Size=7.8f;UpdateCamera();await Frames();
            await Capture(folder+"/lineup.png");
            for(int frame=0;frame<8;frame++)
            {
                for(int i=0;i<portraits.Length;i++)
                {
                    var worker=new Villager{Id=i,Task=i>=4?Work.EatingMeal:Work.ToStockpile,Timer=frame*.15f};
                    if(i<4){worker.Route.Enqueue(new(1,1));if(i==1){worker.Carried=1;worker.Cargo=Inlanders.Simulation.Resource.Logs;}}
                    portraits[i].WalkBlend=1;portraits[i].GaitPhase=frame*Mathf.Tau/8;
                    AnimateVillager(portraits[i],worker);
                    if(i>=4)Check(portraits[i].LeftKnee.Rotation.X<-.8f,"Seated knee did not bend");
                }
                await Frames();await Capture($"{folder}/poses-{frame:D2}.png");
            }
            stage.QueueFree();await Frames();_landscape.Show();_dynamic.Show();
            // Real simulation locomotion, at both ordinary and accelerated speed.
            foreach(int speed in new[]{1,6})
            {
                var w=World.NewRiverClosing();w.Place(World.RecoveryCrossing,0,BuildingKind.Bridge);
                w.ApplyWoodlandCare(new(-11,5),new(6,12),WoodlandIntent.Renew,false);
                for(int i=0;i<80;i++)w.Tick(.1f);
                AdoptWorld(w);var control=World.LoadJson(w.SaveJson());_paused=true;RenderActors(0);
                _focus=new(-1,0,2);_camera.Size=12;_angle=.72f;UpdateCamera();_speed=speed;_paused=false;
                float phaseTravel=0;var timer=System.Diagnostics.Stopwatch.StartNew();double renderMs=0;
                for(int frame=0;frame<120;frame++)
                {
                    if(frame%3==0)for(int tick=0;tick<speed;tick++){w.Tick(.1f);control.Tick(.1f);}
                    _clock+=speed/30f;
                    float before=_people[0].GaitPhase;
                    long start=timer.ElapsedTicks;RenderActors(1/30f);renderMs+=(timer.ElapsedTicks-start)*1000d/System.Diagnostics.Stopwatch.Frequency;
                    phaseTravel+=Math.Abs(_people[0].GaitPhase-before);
                    if(frame%6==0){await Frames();await Capture($"{folder}/walk-{speed}x-{frame/6:D2}.png");}
                }
                Check(phaseTravel>.1f,"Actual movement did not animate legs");
                Check(w.SaveJson()==control.SaveJson(),"Presentation changed simulation");
                _paused=true;RenderActors(0);var phase=_people[0].GaitPhase;var knee=_people[0].LeftKnee.Transform;
                for(int i=0;i<10;i++)RenderActors(.1f);
                Check(phase==_people[0].GaitPhase && knee==_people[0].LeftKnee.Transform,"Pause changed walking pose");
                GD.Print($"VILLAGERS {speed}x: render mean {renderMs/120:0.00}ms; simulation unchanged; gait and pause verified.");
            }
            var actor=_people[0];var villager=_world.People[0];
            actor.Body.Position=OnGround(villager.Position.X,villager.Position.Y);float stillPhase=actor.GaitPhase;
            _paused=false;_clock+=2;RenderActors(.1f);
            Check(actor.GaitPhase==stillPhase,"Clock advanced gait without distance");
            GD.Print("PASS: villager lineup, walking/carry/seated pose sequences, joint batching, distance gait, pause and 1x/6x simulation independence.");
            GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr("VILLAGER PRESENTATION FAIL: "+e);GetTree().Quit(1);}
    }
}
