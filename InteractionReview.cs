using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeInteraction()
    {
        async Task Frames(int count=3){for(int i=0;i<count;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var home=_world.Cottages.First(c=>c.Complete && c.Kind==BuildingKind.Cottage);
        foreach(bool running in new[]{false,true})
        foreach(string action in new[]{"move","path","brush","camera","save"})
        {
            _paused=true;CloseManagementUi();_placing=false;DiscardRelocation();_focus=OnGround(0,1);_camera.Size=23;UpdateCamera();
            if(action=="move"){SelectBuilding(home.Id);BeginRelocation();}
            if(action=="brush")TogglePaths(1);
            if(action=="path"){TogglePaths(3);ClickPathConnection(home.Entrance);}
            await Frames(5);string before=_world.SaveJson();
            _speed=1;_paused=!running;_frameTraces.Clear();_traceFrames=true;
            var direct=new System.Collections.Generic.List<double>();
            for(int i=0;i<120;i++)
            {
                var clock=System.Diagnostics.Stopwatch.StartNew();
                if(action=="camera"){_focus.X+=(i%2==0?.2f:-.2f);UpdateCamera();}
                else if(action=="save"){if(i%12==0)SaveWorld();}
                else
                {
                    var cell=new Cell(-7+i%9,-5+(i/9)%4*3);
                    var point=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));
                    ReviewInput(new InputEventMouseMotion{Position=point,GlobalPosition=point});
                    if(action=="brush" && i%12==0)ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=true});
                    if(action=="brush" && i%12==11)ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=false});
                }
                direct.Add(clock.Elapsed.TotalMilliseconds);await Frames(2);
            }
            await Frames();_traceFrames=false;_paused=true;
            string name=action+(running?"-running":"-paused");
            File.WriteAllText(Path.Combine(_reviewDirectory,name+"-frames.json"),JsonSerializer.Serialize(_frameTraces,new JsonSerializerOptions{IncludeFields=true}));
            File.WriteAllText(Path.Combine(_reviewDirectory,name+"-calls.json"),JsonSerializer.Serialize(direct));
            if(!running && action is "move" or "path" && before!=_world.SaveJson())throw new Exception("Preview changed paused world");
            await Press(Key.Escape);ClearSelection();_world.Validate();
        }
        GD.Print("PASS: paused/running interaction samples; paused route/move previews preserve world.");
    }
}
