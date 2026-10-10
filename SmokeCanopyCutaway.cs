using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunCanopyCutawaySmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(int n=20){for(int i=0;i<n;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;GetWindow().Size=new(960,640);CloseManagementUi();
            var tree=_world.Trees.OrderBy(t=>t.Cell.Point.LengthSquared()).First();var p=_world.People[0];_world.Assign(0,Role.Logger);p.Position=tree.Access.Point;
            _focus=OnGround(tree.Cell.X,tree.Cell.Z);_camera.Size=17;UpdateCamera();await Frames();
            var view=_trees[tree.Id];bool found=false;
            for(int i=0;i<16;i++){_angle=i*Mathf.Tau/16;UpdateCamera();if(CanopyObscures(view,PresentedPerson(0)+Vector3.Up*.65f)){found=true;break;}}
            if(!found)throw new Exception("No occluded resident angle");await Frames();string before=_world.SaveJson();
            await Capture("artifacts/194-canopy-before-960.png");ShowDailyLife(0);await Frames(30);
            if(view.Cutaway?.Amount<.9f || view.Cutaway==null || _world.SaveJson()!=before)throw new Exception("Selected resident not revealed purely");
            await Capture("artifacts/194-canopy-person-960.png");ClearSelection();await Frames(30);
            if(view.Cutaway.Amount!=0 || view.Cutaway.Pieces.Any(x=>x.Mesh.MaterialOverride!=x.Original) || _world.SaveJson()!=before)throw new Exception("Canopy not restored");
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);UpdateCamera();await Frames();
                var ground=_world.Map.Land.Where(c=>(c.Point-tree.Cell.Point).LengthSquared()<=9).First(c=>CanopyObscures(view,OnGround(c.X,c.Z,.15f)) && !PointerOverHud(_camera.UnprojectPosition(OnGround(c.X,c.Z))));
                BeginPlacement(BuildingKind.SeatingGarden);var pointer=_camera.UnprojectPosition(OnGround(ground.X,ground.Z));
                Input.ParseInputEvent(new InputEventMouseMotion{Position=pointer,GlobalPosition=pointer});await Frames(30);
                if(view.Cutaway.Amount<.9f || before!=_world.SaveJson())throw new Exception("Ground preview not revealed purely");
                await Capture($"artifacts/194-canopy-placement-{width}.png");await Press(Key.Escape);await Frames(30);
                if(_trees.Values.Any(v=>v.Cutaway?.Amount>0) || before!=_world.SaveJson())throw new Exception("Placement cancellation did not restore canopies");
            }
            _world.SetTreePreserved(tree.Cell,false);ShowDailyLife(0);_followPerson=true;_paused=false;_speed=3;double start=_uiTime;
            while(_uiTime-start<25 && !tree.Felled)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            _paused=true;if(!tree.Felled)throw new Exception("Revealed logger never worked");_world.Validate();ClearSelection();await Frames(30);
            if(_trees.Values.Any(v=>v.Cutaway?.Amount>0))throw new Exception("Cutaway survived deselection");
            GD.Print("PASS canopy reveal for selected resident, original material restoration, pure paused inspection, ground previews/cancel at960/1440 and actual followed logging.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}
