using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeGroupedFarmsteads()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: grouped farmsteads"]);await Frames();
        await CaptureReviewBundle("grouped-farmsteads-entry");await UiClick(_mainButtons["New hamlet"]);await Frames();
        var profile=new HamletProfile(false,true,true);Check(_world.PublicPlace==profile && CurrentSavePath.EndsWith(profile.SaveName),"Grouped entry or save slot wrong");
        await CaptureReviewBundle("grouped-farmsteads-opening");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);ShowWorkplaceCard(home.Id);await Frames();await UiClick(_workCardYard);await Frames();
        int side=Enumerable.Range(0,4).First(i=>_world.FurnishHomeYardProblem(home.Id,i)==null);await UiClick(_yardSides[side]);await Frames();await UiClick(_yardFurnish);await Frames();
        Check(home.ImprovementRequested,"Grouped furnishing unavailable");await UiClick(_workCardFurnish);await Frames();
        ClearSelection();var field=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);Check(_world.SetWorkplacePaused(field.Id,true),"Field pause failed");
        var destination=_world.Map.Land.First(c=>c!=field.Cell && _world.RelocationProblem(field.Id,c,field.Rotation)==null);
        Check(_world.MoveBuilding(field.Id,destination,field.Rotation),"Grouped field move rejected");_world.Validate();
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Grouped save continuation differs");
        Reset();await Frames();Check(_world.PublicPlace==profile && _world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField).Cell==new Cell(5,11),"Grouped restart lost layout");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: grouped farmsteads"]);await Frames();await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();
        Check(_world.PublicPlace==profile with {Relaxed=true},"Grouped relaxed mode lost layout");await ProbePublicPlaceContract();
        GD.Print("PASS: grouped public entry, paid yard action, movable real field, exact save/restart and relaxed identity (scripted UI).");
    }
}
