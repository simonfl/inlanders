using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private async void RunPlankStorageSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<8;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=World.NewCampaign(2); var mill=w.Place(new(3,-3),false,BuildingKind.Sawmill)!; var pile=w.Place(new(6,-3),false,BuildingKind.Stockpile)!;
            AdoptWorld(w); _paused=true; SelectBuilding(pile.Id); await Frames();
            if(!_storageMaterial.Visible || _storageMaterial.Disabled) throw new Exception("Planned stockpile material control unavailable");
            _storageMaterial.EmitSignal(BaseButton.SignalName.Pressed);
            if(pile.StorageMaterial!=Resource.Planks) throw new Exception("Planned plank selection failed");
            w.Assign(6,Role.Sawyer);
            for(int i=0;i<6000 && pile.StoredPlanks<4;i++) { w.Tick(.1f); w.Validate(); }
            if(pile.StoredPlanks<4) throw new Exception("Rendered plank pile did not receive output");
            await Frames();
            if(_cottages[pile.Id].Stage<1000 || !_storageMaterial.Disabled) throw new Exception("Plank model or occupied-switch protection missing");
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); _focus=new(5,0,-2); _camera.Size=14; UpdateCamera(); await Frames();
                _inspectionScroll.EnsureControlVisible(_storageMaterial); await Frames();
                await Capture($"artifacts/f07b2-planks-{width}.png");
            }
            string saved=w.SaveJson(); AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
            if(_world.SaveJson()!=saved) throw new Exception("Plank UI reload changed stocks");
            w=_world; pile=w.Cottages.Single(c=>c.Id==pile.Id);
            w.Assign(6,Role.Unassigned); w.SetStorageTarget(pile.Id,0); w.Assign(7,Role.Hauler);
            for(int i=0;i<6000 && w.StorageMaterialProblem(pile.Id)!=null;i++) { w.Tick(.1f); w.Validate(); }
            SelectBuilding(pile.Id); await Frames();
            if(_storageMaterial.Disabled) throw new Exception("Drained pile cannot change material");
            _storageMaterial.EmitSignal(BaseButton.SignalName.Pressed); await Frames();
            _storageMaterial.EmitSignal(BaseButton.SignalName.Pressed); await Frames();
            if(pile.StorageMaterial!=Resource.Logs || _cottages[pile.Id].Stage>=1000) throw new Exception("Empty pile material/model did not change");
            await CheckStoneStorageUi();
            GD.Print("PASS: plank regression and stone selection, hauling, stacks, reservations, current save and drain at 960/1440."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
    private async System.Threading.Tasks.Task CheckStoneStorageUi()
    {
        async System.Threading.Tasks.Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        foreach(int width in new[]{960,1440})
        {
            var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var pile=w.Place(new(3,0),width==960?1:3,BuildingKind.Stockpile)!;
            AdoptWorld(w);_paused=true;GetWindow().Size=new(width,width==960?640:900);SelectBuilding(pile.Id);await Frames();
            _inspectionScroll.EnsureControlVisible(_storageMaterial);await Frames();
            await UiClick(_storageMaterial);await Frames();await UiClick(_storageMaterial);await Frames();
            if(pile.StorageMaterial!=Resource.Stone || !_storageMaterial.Text.Contains("Stone"))throw new Exception("Stone selection through UI failed");
            w.SetCreativeCentralStock(Resource.Stone,20);w.SetStorageTarget(pile.Id,12);w.Assign(0,Role.Hauler);
            for(int i=0;i<6000 && pile.StoredStone<12;i++){w.Tick(.1f);w.Validate();}
            await Frames();
            if(pile.StoredStone!=12 || _cottages[pile.Id].Stage<2000 || !_storageMaterial.Disabled || !_siteInfo.Text.Contains("Stone") || _shownStone!=w.YardStone)throw new Exception("Stone model, central/local split or inspector missing");
            _focus=new(3,0,0);_camera.Size=12;UpdateCamera();await Frames();await Capture($"artifacts/stone-staging/stone-pile-{width}.png");
            string saved=w.SaveJson();AdoptWorld(World.LoadJson(saved));_paused=true;SelectBuilding(pile.Id);await Frames();
            if(_world.SaveJson()!=saved)throw new Exception("Stone UI reload changed inventory");
            w=_world;pile=w.Cottages.Single(c=>c.Id==pile.Id);w.SetStorageTarget(pile.Id,0);
            for(int i=0;i<6000 && w.StorageMaterialProblem(pile.Id)!=null;i++){w.Tick(.1f);w.Validate();}
            await Frames();_inspectionScroll.EnsureControlVisible(_storageMaterial);await Frames();await UiClick(_storageMaterial);await Frames();
            if(pile.StorageMaterial!=Resource.Logs || _cottages[pile.Id].Stage>=1000)throw new Exception("Stone drain/switch did not refresh model");
        }
    }
}
