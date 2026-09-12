using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async Task CheckLooseStock()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        World Fixture(Resource material,int amount,bool salvage)
        {
            var data=JsonNode.Parse(new World(0).SaveJson())!;
            var tree=data["Trees"]![0]!.DeepClone();
            tree["Logs"]=amount;tree["Material"]=(int)material;tree["Salvage"]=salvage;tree["Felled"]=true;
            data["Trees"]=new JsonArray(tree);
            data["InitialLogs"]=material==Resource.Logs?amount:material==Resource.Planks?amount/2:0;
            data["SawnLogs"]=material==Resource.Planks?amount/2:0;
            if(material==Resource.Stone)
            {
                data["QuarriedStone"]=amount;
                data["Map"]!["StoneDeposits"]=new JsonArray(new JsonObject { ["Id"]=0,["Cell"]=new JsonObject{["X"]=5,["Z"]=-4},["Capacity"]=amount,["Remaining"]=0 });
            }
            var w=World.LoadJson(data.ToJsonString());foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);return w;
        }
        foreach(var (material,salvage) in new[]{(Resource.Logs,false),(Resource.Logs,true),(Resource.Planks,true),(Resource.Stone,true)})
        {
            var w=Fixture(material,10000,salvage);AdoptWorld(w);_paused=true;await Frames();
            var tree=w.Trees[0];var view=_trees[tree.Id];
            Check(view.Pile.GetChildCount()<=4,"Large loose pile has unbounded nodes");
            Check(view.Amount?.Text==$"10000 {material.ToString().ToLowerInvariant()}","Exact quantity label missing");
            foreach(var mesh in view.Pile.GetChildren().OfType<MeshInstance3D>())
                Check(mesh.Position.Y+mesh.GetAabb().End.Y<1.2f,"Loose pile still forms a tower");
            string saved=w.SaveJson();await Frames();Check(w.SaveJson()==saved,"Paused rendering changed source stock");
            var ids=view.Pile.GetChildren().Select(n=>n.GetInstanceId()).ToArray();
            w.Assign(0,Role.Logger);
            for(int i=0;i<3000 && tree.Logs==10000;i++){w.Tick(.1f);w.Validate();}
            Check(tree.Logs==9998 && w.People[0].Carried==2 && w.People[0].Cargo==material,"Real collection lost source/cargo quantity");
            await Frames();Check(ids.SequenceEqual(view.Pile.GetChildren().Select(n=>n.GetInstanceId())),"Above-cap collection rebuilt geometry");
            Check(view.Amount!.Text.StartsWith("9998 "),"Collection did not update exact count");
            _focus=OnGround(tree.Cell.X,tree.Cell.Z);_camera.Size=10;UpdateCamera();_noticeUntil=0;CloseManagementUi();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/loose-{material}-{salvage}-{width}.png");
            }
            saved=w.SaveJson();AdoptWorld(World.LoadJson(saved));await Frames();Check(_world.SaveJson()==saved,"In-flight collection reload differed");
            // Small stock crosses the display threshold, drains fully, then clears normally.
            w=Fixture(material,14,salvage);AdoptWorld(w);await Frames();tree=w.Trees[0];w.Assign(0,Role.Logger);
            for(int i=0;i<3000 && tree.Logs>12;i++)w.Tick(.1f);
            await Frames();Check(tree.Logs==12 && _trees[tree.Id].Amount==null,"Threshold label did not disappear");
            for(int i=0;i<12000 && (tree.Logs>0 || w.People[0].Carried>0);i++){w.Tick(.1f);w.Validate();}
            Check(tree.Logs==0 && w.People[0].Carried==0,"Loose stock never drained");await Frames();
            Check(material switch {Resource.Logs=>w.YardLogs==14,Resource.Planks=>w.YardPlanks==14,_=>w.Stone==14},"Collected stock missing from yard");
            if(salvage)Check(!_trees.ContainsKey(tree.Id),"Drained salvage view remained");
            else
            {
                Check(_trees[tree.Id].Pile.GetChildCount()==1,"Empty tree lost stump or retains timber");
                Check(w.SetClearing(tree.Cell,true),"Drained stump cannot be cleared");
                for(int i=0;i<3000 && w.Trees.Contains(tree);i++){w.Tick(.1f);w.Validate();}
                await Frames();Check(!_trees.ContainsKey(tree.Id),"Cleared stump view remained");
            }
            GD.Print($"PASS: {material}, salvage {salvage}: bounded 10,000-unit display, exact live pickup/label, 14-to-zero collection, reload and removal.");
        }
    }
}
