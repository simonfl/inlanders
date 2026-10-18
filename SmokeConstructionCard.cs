using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private async void RunConstructionCardSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
            AdoptWorld(World.NewRiverFrontage(false,true,true));_paused=true;CloseManagementUi();GetWindow().Size=new(960,640);
            var at=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,BuildingKind.Lodge)==null);
            var site=_world.Place(at,0,BuildingKind.Lodge)!;
            for(int i=0;i<1200 && _world.ReadConstruction(site).Missing==null;i++)_world.Tick(.1f);
            ShowWorkplaceCard(site.Id);await Frames();
            if(!_constructionSupply.Visible || !_workCardText.Text.Contains("planks"))throw new Exception("Missing supply action");
            if(_workCard.GetGlobalRect().End.Y>_hud.Size.Y-65)throw new Exception("Construction card overflows");
            await UiClick(_constructionPriority);if(site.Priority!=2)throw new Exception("Priority ignored");
            await Capture("artifacts/201-construction-960.png");await UiClick(_constructionSupply);await Frames();
            if(!_placing || _buildKind!=BuildingKind.Sawmill)throw new Exception("Supply action did not plan sawmill");
            await Press(Key.Escape);
            var sawAt=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,BuildingKind.Sawmill)==null);_world.Place(sawAt,0,BuildingKind.Sawmill);
            _world.SetTreePreserved(_world.Trees.First(t=>t.Preserved && t.Growth==1).Cell,false);
            for(int i=0;i<6000 && !site.Complete;i++)_world.Tick(.1f);
            if(!site.Complete)throw new Exception("Supplying sawmill never finished home");_world.Validate();
            GD.Print("PASS compact construction blocker, build-first action, actual supply planning and completion after real sawmill production.");GetTree().Quit();
        }
        catch(Exception e){GD.PrintErr(e);GetTree().Quit(1);}
    }
}
