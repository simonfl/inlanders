using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckDecorationBrush()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        Vector2 Screen(Cell c)=>_camera.UnprojectPosition(OnGround(c.X,c.Z));
        void ButtonAt(Vector2 p,bool down,MouseButton button=MouseButton.Left)=>Input.ParseInputEvent(new InputEventMouseButton{Position=p,GlobalPosition=p,ButtonIndex=button,Pressed=down});
        void Motion(Vector2 p,MouseButtonMask mask=MouseButtonMask.Left)=>Input.ParseInputEvent(new InputEventMouseMotion{Position=p,GlobalPosition=p,ButtonMask=mask});
        async Task Start(Cell c){Motion(Screen(c),0);await Frames();ButtonAt(Screen(c),true);await Frames();}
        async Task End(Cell c){ButtonAt(Screen(c),false);await Frames();Check(!_decorationStroke,"Release kept stroke active");}
        void Tool(DecorationKind kind,bool remove=false){_decorationChoice.Select((int)kind);BeginDecorating(remove);}
        async Task ClearDecorations(){CancelDecorationStroke();foreach(var d in _world.Decorations.ToArray())_world.RemoveDecoration(d.Cell);await Frames();}
        _goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        foreach(int width in new[]{1440,960})
        {
            var w=World.NewCreative();foreach(var t in w.Trees.ToArray())w.SetClearing(t.Cell,true);
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;
            GetWindow().Size=new(width,width==960?640:900);_focus=new(3,0,-3);_camera.Size=16;_angle=0;UpdateCamera();await Frames();
            Tool(DecorationKind.Fence);string before=w.SaveJson();Motion(Screen(new(1,-4)),0);await Frames();
            Check(w.SaveJson()==before,"Hover painted decorations");
            await Start(new(1,-4));Motion(Screen(new(6,-4)));await Frames();
            Check(w.Decorations.Count==6 && Enumerable.Range(1,6).All(x=>w.Decorations.Any(d=>d.Cell==new Cell(x,-4))),"Fast stroke skipped tiles");
            Check((int)_fenceBodies[new(3,-4)].GetMeta("connections")==5,"Stroke did not update fence joins");
            string painted=w.SaveJson();Motion(Screen(new(1,-4)));await Frames();
            Check(w.SaveJson()==painted && _decorationStrokeProblem==null,"Revisiting stroke cells changed state or raised refusal");
            await End(new(1,-4));Motion(Screen(new(6,-5)),0);await Frames();Check(w.SaveJson()==painted,"Unheld motion painted");
            var loaded=World.LoadJson(painted);Check(loaded.SaveJson()==painted,"Painted save differs");
            Tool(DecorationKind.Fence,true);await Start(new(6,-4));Motion(Screen(new(1,-4)));await End(new(1,-4));Check(w.Decorations.Count==0,"Reverse erasing missed run");
            // A known diagonal staircase; its reverse must erase exactly those same cells.
            Tool(DecorationKind.Fence);await Start(new(2,-5));Motion(Screen(new(5,-2)));await End(new(5,-2));
            var diagonal=new[]{new Cell(2,-5),new(3,-5),new(3,-4),new(4,-4),new(4,-3),new(5,-3),new(5,-2)};
            Check(w.Decorations.Select(d=>d.Cell).ToHashSet().SetEquals(diagonal),"Diagonal staircase wrong");
            Tool(DecorationKind.Fence,true);await Start(new(5,-2));Motion(Screen(new(2,-5)));await End(new(2,-5));Check(w.Decorations.Count==0,"Reverse diagonal erasing differs");
            // Refused overlap is retained, accepted cells on both sides are still painted.
            Check(w.PlaceDecoration(new(3,-4),DecorationKind.Shrub),"Overlap fixture failed");
            Tool(DecorationKind.Fence);await Start(new(1,-4));Motion(Screen(new(6,-4)));await Frames();
            Check(w.Decorations.Count==6 && w.Decorations.Single(d=>d.Cell==new Cell(3,-4)).Kind==DecorationKind.Shrub,"Brush replaced another kind or aborted later cells");
            Check(_decorationStrokeProblem!=null && _notice.Contains("Remove the existing decoration"),"Skipped tile lacks reason");
            double until=_noticeUntil;Motion(Screen(new(3,-4)));await Frames();Check(_noticeUntil==until,"Repeat motion retriggered refusal notice");await End(new(3,-4));
            await ClearDecorations();
            // Every cancellation must prevent a later held-motion event from continuing.
            foreach(string stop in new[]{"hud","escape","focus","tool","world","mask","rotation"})
            {
                Tool(DecorationKind.Flowers);await Start(new(2,-4));
                if(stop=="hud")Motion(_menuButtons[0].GetGlobalRect().GetCenter());
                if(stop=="escape")await Press(Key.Escape);
                if(stop=="focus")_Notification((int)NotificationWMWindowFocusOut);
                if(stop=="tool")Tool(DecorationKind.Pebbles);
                if(stop=="world"){w=World.LoadJson(w.SaveJson());AdoptWorld(w);_paused=true;CloseManagementUi();_focus=new(3,0,-3);_camera.Size=16;_angle=0;UpdateCamera();}
                if(stop=="mask")Motion(Screen(new(2,-4)),0);
                if(stop=="rotation")await Press(Key.R);
                await Frames();Check(!_decorationStroke,"Stroke survived "+stop);
                string stopped=w.SaveJson();Motion(Screen(new(6,-4)));await Frames();Check(w.SaveJson()==stopped,"Held motion painted after "+stop);await End(new(6,-4));await ClearDecorations();
            }
            Tool(DecorationKind.Sunflowers);await Start(new(2,-4));Motion(Screen(new(5,-4)));await End(new(5,-4));Check(w.Decorations.Count==4,"Creative sunflower brush failed");await ClearDecorations();
            Tool(DecorationKind.Fence);_rotation=1;await Start(new(2,-4));await End(new(2,-4));Check(w.Decorations.Single().Rotated,"Single-click rotation lost");await ClearDecorations();
            // Camera drag with the tool ready must pan without drawing.
            Tool(DecorationKind.Fence);var oldFocus=_focus;var p=Screen(new(3,-3));before=w.SaveJson();
            ButtonAt(p,true,MouseButton.Right);Motion(p+new Vector2(50,20),MouseButtonMask.Right);ButtonAt(p+new Vector2(50,20),false,MouseButton.Right);await Frames();
            Check(_focus!=oldFocus && w.SaveJson()==before,"Decoration tool broke right-drag camera isolation");
            _focus=new(4,0,-3);_angle=.72f;_camera.Size=16;UpdateCamera();
            var home=w.Place(new(4,-3),0,BuildingKind.Cottage)??throw new Exception("Garden cottage failed");
            Tool(DecorationKind.Fence);await Start(new(2,0));Motion(Screen(new(2,-6)));await Frames();Motion(Screen(new(6,-6)));await Frames();Motion(Screen(new(6,0)));await End(new(6,0));
            await Start(new(2,0));Motion(Screen(new(6,0)));await Frames();
            Check(Enumerable.Range(-6,7).All(z=>w.Decorations.Any(d=>d.Cell==new Cell(2,z)) && w.Decorations.Any(d=>d.Cell==new Cell(6,z))),"Garden side strokes missing");
            Check(w.DecorationProblem(new(5,0),DecorationKind.Fence)!=null && !w.Decorations.Any(d=>d.Cell==new Cell(5,0)),"Brush sealed the garden");await End(new(6,0));
            Check(!w.PlaceDecoration(home.Entrance,DecorationKind.Fence),"Home entrance protection changed");
            Tool(DecorationKind.Flowers);await Start(new(3,-5));Motion(Screen(new(5,-5)));await End(new(5,-5));await Press(Key.Escape);_noticeUntil=0;await Frames();
            await Capture($"artifacts/decoration-brush-garden-{width}.png");w.Validate();
            w=World.NewScenario();AdoptWorld(w);_paused=true;CloseManagementUi();_focus=new(3,0,-3);_camera.Size=16;_angle=0;UpdateCamera();await Frames();
            Tool(DecorationKind.Sunflowers);await Start(new(2,-4));Motion(Screen(new(5,-4)));await End(new(5,-4));Check(w.Decorations.Count==0 && !w.SunflowersUnlocked,"Brush bypassed sunflower reward lock");
        }
        GD.Print("PASS: decoration click/fast/diagonal/retraced strokes, erasing, refusals, lock, rotation, save isolation, HUD/Escape/focus/tool/world/mask cancellation, camera pan and protected garden at 960/1440.");
    }
}
