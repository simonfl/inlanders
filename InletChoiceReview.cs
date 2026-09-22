using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeInletChoice()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Earlier prototypes"]);await Frames();
        await UiClick(_mainButtons["Between wood and water · prototype"]);await Frames();Check(_menuPageTitle==new HamletProfile(false).Title,"Archived compact link opens another place");
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: cultivated bank"]);await Frames();Check(_menuPageTitle==new HamletProfile(false,true).Title,"Matched control link missing");
        await UiClick(_mainButtons["Other starting layouts"]);await Frames();await UiClick(_mainButtons["Compare: across the inlet"]);await Frames();
        await CaptureReviewBundle("inlet-choice-entry");await UiClick(_mainButtons["New hamlet"]);await Frames();
        var profile=new HamletProfile(false,true,false,true);Check(_world.PublicPlace==profile && CurrentSavePath.EndsWith(profile.SaveName),"Inlet entry or save slot wrong");
        await CaptureReviewBundle("inlet-choice-opening");
        var home=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage);ShowWorkplaceCard(home.Id);await Frames();await UiClick(_workCardYard);await Frames();
        int side=Enumerable.Range(0,4).First(i=>_world.FurnishHomeYardProblem(home.Id,i)==null);await UiClick(_yardSides[side]);await Frames();await UiClick(_yardFurnish);await Frames();
        Check(home.ImprovementRequested,"Inlet furnishing unavailable");await UiClick(_workCardFurnish);await Frames();
        ClearSelection();var field=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);Check(_world.SetWorkplacePaused(field.Id,true),"Field pause failed");
        var destination=_world.Map.Land.First(c=>c!=field.Cell && _world.RelocationProblem(field.Id,c,field.Rotation)==null);
        Check(_world.MoveBuilding(field.Id,destination,field.Rotation),"Inlet field move rejected");_world.Validate();
        string saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Inlet save continuation differs");
        Reset();await Frames();Check(_world.PublicPlace==profile && _world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField).Cell==new Cell(1,-5),"Inlet restart lost layout");
        ReturnToMainMenu();await Frames();Check(_atMainMenu,"Return to menu failed: inspect atomic-save diagnostics; do not treat as missing controls.");await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["New relaxed hamlet"]);await Frames();
        Check(_world.PublicPlace==profile with {Relaxed=true},"Inlet relaxed mode lost layout");await ProbePublicPlaceContract();
        string continueSlot=_continuePath,live=_world.SaveJson();
        try
        {
            // An existing file cannot be a directory: deterministic second-stage failure, no permission changes.
            _world.SaveFile(CurrentSavePath);_continuePath=System.IO.Path.Combine(CurrentSavePath,"invalid.json");
            Check(!SaveSession() && !_atMainMenu && _notice.StartsWith("Settlement saved for F9, but Continue"),"Partial save success was reported as total failure");
            Check(World.LoadFile(CurrentSavePath).SaveJson()==live && _world.SaveJson()==live,"Partial save failure lost slot or live state");
            await CaptureReviewBundle("saved-slot-continue-failure");
            _lastAutosaved=null;AdvanceAutosave(120);
            Check(_notice.StartsWith("Autosave is available in Options, but Continue"),"Autosave partial success misreported");
            Check(World.LoadFile(AutosavePath).SaveJson()==live && _world.SaveJson()==live,"Autosave failure lost slot/live state");
        }
        finally{_continuePath=continueSlot;}

        GD.Print("PASS: inlet public entry, paid yard action, movable real field, exact save/restart and relaxed identity (scripted UI).");
    }
}
