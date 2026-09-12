using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async Task CheckCreativeStockUi()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        System.IO.Directory.CreateDirectory("artifacts/creative-stock");
        foreach(int width in new[]{960,1440})
        {
            AdoptWorld(World.NewCreative());_paused=true;GetWindow().Size=new(width,width==960?640:900);ToggleDrawer(3);await Frames();
            _drawerPages[3].EnsureControlVisible(_stockResource);await Frames();Check(_creativeStockUi.Visible,"Creative stock controls hidden");
            _stockResource.Select((int)Resource.Fruit);_stockResource.EmitSignal(OptionButton.SignalName.ItemSelected,(long)Resource.Fruit);await Frames();
            string before=_world.SaveJson();_stockQuantity.Value=70;await Frames();Check(_world.SaveJson()==before,"Draft edited world before Apply");
            _drawerPages[3].EnsureControlVisible(_stockApply);await Frames();await UiClick(_stockApply);await Frames();
            Check(_world.Food.Fruit==70 && _world.Food.GrownFruit==0 && _world.CreativeAdded(Resource.Fruit)==70,"Apply lost stock/ledger distinction");
            Check(_resourceValues[Resource.Fruit].GetParent<Control>().Visible,"Injected fruit missing from resource bar");
            _stockQuantity.Value=5;await Frames();await UiClick(_stockApply);await Frames();
            Check(_world.Food.Fruit==5 && _world.CreativeRemoved(Resource.Fruit)==65,"Decrease not applied honestly");
            _drawerPages[3].EnsureControlVisible(_stockStatus);await Frames();await Capture($"artifacts/creative-stock/controls-{width}.png");
            _stockQuantity.Value=999;var saved=_world.SaveJson();AdoptWorld(World.LoadJson(saved));_paused=true;await Frames();
            Check(_stockQuantity.Value==5 && _world.SaveJson()==saved,"World reload kept stale draft or changed stocks");
            AdoptWorld(World.NewScenario());_paused=true;await Frames();Check(!_creativeStockUi.Visible,"Normal mode exposes Creative stock controls");
        }
        GD.Print("PASS: Creative stock draft/apply, additions/removals, resource visibility, save/reload draft reset and normal-mode exclusion at 960/1440.");
    }
}
