using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeFirstPlace()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();
        Check(_mainButtons.ContainsKey("Establish life by the river") && _mainButtons.ContainsKey("Tend a river settlement"),"Starting-situation choice missing");
        foreach(string choice in new[]{"Establish life by the river","Tend a river settlement"})Check(_mainScroll.GetGlobalRect().Encloses(_mainButtons[choice].GetGlobalRect()),"Starting situation hidden below scroll");
        await CaptureReviewBundle("equal-starting-choices");await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Establish a farmstead"]);await Frames();
        await UiClick(_mainButtons["New farmstead"]);await Frames();Check(_world.PublicPlace?.PlayerFounded==true && _world.Cottages.Count==0 && _firstPlace.IsVisibleInTree(),"Primary entry lost opening choices");
        Check(!_hintPanel.Visible || !_hintPanel.GetGlobalRect().Intersects(_firstPlace.GetGlobalRect()),"Entry hint covers first choices");
        string saved=_world.SaveJson();Check(_firstPlace.GetGlobalRect().End.Y<_hud.Size.Y-76,"First choices overflow compact view");await CaptureReviewBundle("first-place-choices");
        foreach(var kind in EverydayBuildings)
        {
            await UiClick(_firstPlaceChoices[kind]);await Frames();Check(_placing && _buildKind==kind && !_firstPlace.Visible,"Direct first choice did not open placement");
            await Press(Key.Escape);await Frames();Check(!_placing && _firstPlace.Visible && saved==_world.SaveJson(),"Cancelled first choice changed village or lost alternatives");
        }
        await UiClick(_firstPlaceTimber);await Frames();Check(_placing && _clearingTrees,"Opening timber action missing");await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Opening timber cancel changed village");
        await UiClick(_firstPlaceBrowse);await Frames();Check(_drawer.Visible && _buildingFilter.Selected==0,"Full catalogue inaccessible");await Press(Key.Escape);await Frames();
        await UiClick(_firstPlaceLook);await Frames();Check(!_firstPlace.Visible && saved==_world.SaveJson(),"Look around changed the village");await Press(Key.B);await Frames();Check(_drawer.Visible,"Dismissal blocked normal build access");await Press(Key.Escape);await Frames();
        await Press(Key.F5);await Press(Key.F9);await Frames();Check(_world.SaveJson()==saved && _world.PublicPlace!.PlayerFounded,"New entry lost save identity");
        ReturnToMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Earlier beginnings"]);await Frames();await UiClick(_mainButtons["Establish a farmstead"]);await Frames();await UiClick(_mainButtons["New relaxed farmstead"]);await Frames();
        Check(_world.PublicPlace is {PlayerFounded:true,Relaxed:true} && _firstPlace.Visible && _firstPlaceChoices[BuildingKind.Cottage].Text.Contains("Free",StringComparison.OrdinalIgnoreCase),"Relaxed first choice identity/cost wrong");
        await UiClick(_firstPlaceChoices[BuildingKind.Cottage]);await Frames();_rotation=0;_focus=OnGround(0,7);_camera.Size=23;UpdateCamera();await Frames();
        var point=_camera.UnprojectPosition(OnGround(-3,7));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();Check(_ghostValid,"Chosen first home site invalid");await Click(point);await Press(Key.Escape);await Frames();
        Check(_world.Cottages.Any(c=>c.Cell==new Cell(-3,7)) && !_firstPlace.Visible,"First placement did not retire opening chooser");await CaptureReviewBundle("first-home-chosen");await ProbePublicPlaceContract();
        GD.Print("PASS: actual primary menu, shared everyday choices and cancelable timber action, all-catalogue and dismiss access, save/load, relaxed cost and first world placement.");
    }
}
