using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckStoragePresentation()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        int cappedNodes=-1;
        foreach(int amount in new[]{0,8,24,10000,10002,0})
        {
            // Accounted inventory fixture; never alter the production economy for an art test.
            var json=JsonNode.Parse(new World(0).SaveJson())!;
            json["Stored"]=amount;json["Planks"]=amount;
            json["SawnLogs"]=amount/2;
            json["InitialLogs"]=json["InitialLogs"]!.GetValue<int>()+amount+amount/2;
            var oldIds=_stored.GetChildren().Select(n=>n.GetInstanceId()).ToArray();
            if(amount==10002) _world=World.LoadJson(json.ToJsonString());
            else AdoptWorld(World.LoadJson(json.ToJsonString()));
            _paused=true;_world.Validate();
            for(int x=-2;x<=2;x++)_world.SetPath(new(x,5),true);
            string saved=_world.SaveJson();await Frames();
            int nodes=_stored.GetChildCount();
            if(amount==10002 && !oldIds.SequenceEqual(_stored.GetChildren().Select(n=>n.GetInstanceId())))throw new Exception("Above-cap stock change rebuilt racks");
            if(amount==0 && nodes!=0)throw new Exception("Empty yard shows timber");
            if(amount==24)cappedNodes=nodes;
            if(amount>=10000 && nodes!=cappedNodes)throw new Exception("High reserves grow yard geometry");
            if(_world.SaveJson()!=saved)throw new Exception("Storage rendering changed inventory");
            var ids=_stored.GetChildren().Select(n=>n.GetInstanceId()).ToArray();await Frames();
            if(!ids.SequenceEqual(_stored.GetChildren().Select(n=>n.GetInstanceId())))throw new Exception("Idle racks rebuilt");
            CloseManagementUi();_focus=new(-2,0,3);_camera.Size=12;UpdateCamera();_noticeUntil=0;
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/storage-yard-{amount}-{width}.png");
            }
            GD.Print($"Yard reserve {amount} of each material: {nodes} batched nodes, exact save unchanged.");
            var clock=System.Diagnostics.Stopwatch.StartNew();
            for(int i=0;i<100;i++)RenderActors(0);
            clock.Stop();
            GD.Print($"100 paused actor updates at reserve {amount}: {clock.Elapsed.TotalMilliseconds:F2}ms CPU; VSync {DisplayServer.WindowGetVsyncMode()}. Not a frame-rate benchmark.");
        }
        GD.Print("PASS: empty/ordinary/high-stock yard, bounded geometry, stable paused nodes and exact inventory at 960/1440.");
    }
}
