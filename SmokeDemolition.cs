using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckDemolitionUi()
    {
        var previous = _world; var size = GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            var w = new World(); var house = w.Place(new(3,0))!;
            for(int i=0;i<3000 && !house.Complete;i++) w.Tick(.1f);
            if(!house.Complete) throw new Exception("Demolition fixture never built");
            foreach(var person in w.People) w.Assign(person.Id,Role.Unassigned);
            AdoptWorld(w); await Frames();
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size = new(width,width==960?640:900); SelectBuilding(house.Id); await Frames();
                if(!_removalInfo.Text.Contains("Recover all 6 logs") || !_removalInfo.Text.Contains("Housing after order: 0/8")) throw new Exception("Demolition consequences missing");
                await UiClick(_removeBuildingButton); await Frames();
                if(!house.DemolitionRequested || w.Beds!=0 || !_cancelDemolition.Visible) throw new Exception("Demolition order did not stop housing or offer cancellation");
                await Capture($"artifacts/f16b-order-{width}.png");
                await UiClick(_cancelDemolition); await Frames();
                if(house.DemolitionRequested || w.Beds!=2) throw new Exception("Cancel did not restore housing");
            }
            await UiClick(_removeBuildingButton); w.Assign(0,Role.Builder);
            for(int i=0;i<2000 && house.DemolitionProgress < .5f;i++) w.Tick(.1f);
            w.Validate(); RenderActors(0); UpdateHud(); await Frames();
            if(house.DemolitionProgress < .5f || _cancelDemolition.Visible || _cottages[house.Id].Stage != 10200) throw new Exception("Dismantling progress/model/cancellation state incorrect");
            _focus = new(3,0,0); _camera.Size=10; UpdateCamera(); CloseDrawer(); await Capture("artifacts/f16b-dismantling.png");
            string midway = w.SaveJson(); var copy=World.LoadJson(midway);
            if(copy.SaveJson()!=midway) throw new Exception("Demolition save changed");
            for(int i=0;i<3000 && w.Cottages.Any(c=>c.Id==house.Id);i++) { w.Tick(.1f); w.Validate(); }
            RenderActors(0); await Frames();
            if(w.Cottages.Any(c=>c.Id==house.Id) || _cottages.ContainsKey(house.Id)) throw new Exception("Demolished model or site remained");
            if(w.Place(new(3,0))==null) throw new Exception("Demolished plot not reusable");
            foreach(var kind in new[]{BuildingKind.Bakery,BuildingKind.Stockpile})
            {
                var facilityWorld=new World(); var facility=facilityWorld.Place(new(3,0),false,kind)!;
                for(int i=0;i<3000 && !facility.Complete;i++) facilityWorld.Tick(.1f);
                AdoptWorld(facilityWorld); await Frames(); SelectBuilding(facility.Id); await Frames();
                await UiClick(_removeBuildingButton); await Frames();
                if(!facility.DemolitionRequested || kind==BuildingKind.Bakery && !_productionPause.Disabled || kind==BuildingKind.Stockpile && _storageControls.Visible)
                    throw new Exception("Demolition left production/storage controls active");
            }
            GD.Print("PASS: normal demolition controls at 1440/960, housing preview, cancel, builder work/model stages, partial save and reusable plot.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
