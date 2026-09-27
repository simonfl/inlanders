using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
public partial class Game
{
    private async Task ProbeLandDrawing()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        CloseManagementUi();ClearSelection();await Press(Key.B);await Frames();await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[2]));await Frames();_drawerPages[1].EnsureControlVisible(_kindButtons[BuildingKind.VegetableField]);await Frames();await UiClick(_kindButtons[BuildingKind.VegetableField]);CloseDrawer();
        _focus=OnGround(1,-5);_camera.Size=23;UpdateCamera();await Frames();string saved=_world.SaveJson();var start=_camera.UnprojectPosition(OnGround(1,-5));
        async Task Draw(Vector2 end)
        {
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=true,Position=start,GlobalPosition=start});await Frames();Input.ParseInputEvent(new InputEventMouseMotion{Position=end,GlobalPosition=end,ButtonMask=MouseButtonMask.Left});await Frames();
        }
        foreach(int turn in new[]{0,1,2,3})
        {
            var endCell=World.RotateOffset(new(1,-5),0,-2,turn);var end=_camera.UnprojectPosition(OnGround(endCell.X,endCell.Z));await Draw(end);Check(_plotRows==3 && _rotation==turn && _plotAnchor==new Cell(1,-5),"Directional strip drawing wrong");
            await Press(Key.Escape);Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=end,GlobalPosition=end});await Frames();Check(_world.SaveJson()==saved,"Cancelled rotated drawing mutated world");
        }
        var finish=_camera.UnprojectPosition(OnGround(1,-7));await Draw(finish);await CaptureReviewBundle("direct-ground-proposal");
        var uiPoint=_plotPanel.GetGlobalRect().Position+new Vector2(20,20);Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=uiPoint,GlobalPosition=uiPoint});await Frames();Check(_world.SaveJson()==saved,"Release over UI placed a field");
        await Draw(finish);Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=finish,GlobalPosition=finish});await Frames();Check(_world.Cottages.Single().PlotRows==3 && _world.Cottages.Single().Cell==new Cell(1,-5),"Release committed wrong footprint");await Press(Key.Escape);await Frames();await CaptureReviewBundle("drawn-ground-order");_world.Validate();
        GD.Print("PASS: actual four-direction cultivated drawing, pure Escape/UI-release cancellation and anchored commit.");
    }
    private async Task ProbeLandFirst()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Choose(BuildingKind kind)
        {
            await Press(Key.B);await Frames();await UiClick(_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[kind==BuildingKind.Cottage?1:2]));await Frames();_drawerPages[1].EnsureControlVisible(_kindButtons[kind]);await Frames();await UiClick(_kindButtons[kind]);CloseDrawer();_rotation=0;_focus=OnGround(1,7);_camera.Size=25;UpdateCamera();await Frames();
        }
        ShowMainMenu();await Frames();await UiClick(_mainButtons["Play"]);await Frames();await UiClick(_mainButtons["Establish a farmstead"]);await Frames();await UiClick(_mainButtons["New farmstead"]);await Frames();
        Check(_world.PublicPlace is {PlayerFounded:true,Relaxed:false} && _world.Cottages.Count==0,"Wrong ordinary entry");
        foreach(var cell in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})
        {await Choose(BuildingKind.Cottage);if(_world.Cottages.Count==0)await Press(Key.Y);await Frames();var point=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();Check(_ghostValid,"House siting failed");await Click(point);await Press(Key.Escape);await Frames();}
        foreach(var cell in new[]{new Cell(4,3),new(4,7)})
        {
            await Choose(BuildingKind.VegetableField);var start=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));var end=_camera.UnprojectPosition(OnGround(cell.X,cell.Z-2));string before=_world.SaveJson();
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=true,Position=start,GlobalPosition=start});await Frames();Input.ParseInputEvent(new InputEventMouseMotion{Position=end,GlobalPosition=end,ButtonMask=MouseButtonMask.Left});await Frames();
            Check(_plotRows==3 && _plotAnchor==cell && _hover==cell && _ghostValid,"Drag extent/anchor wrong");await CaptureReviewBundle("draw-growing-ground");
            await Press(Key.Escape);Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=end,GlobalPosition=end});await Frames();Check(_world.SaveJson()==before,"Cancelled drawing mutated village");
            Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=true,Position=start,GlobalPosition=start});await Frames();Input.ParseInputEvent(new InputEventMouseMotion{Position=end,GlobalPosition=end,ButtonMask=MouseButtonMask.Left});await Frames();Input.ParseInputEvent(new InputEventMouseButton{ButtonIndex=MouseButton.Left,Pressed=false,Position=end,GlobalPosition=end});await Frames();Check(_world.Cottages.Any(c=>c.Cell==cell && c.PlotRows==3),"Drawn strip not placed");await Press(Key.Escape);await Frames();
        }
        ClearSelection();_paused=false;_speed=6;double wallStart=_uiTime;float nextSample=0;var samples=new List<object>();
        while(_world.Food.Time<1200 && _uiTime-wallStart<250)
        {
            await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
            if(_world.Food.Time>=nextSample){nextSample+=60;samples.Add(new{time=_world.Food.Time,housed=_world.Housed,food=_world.EdibleStored,grown=_world.Food.GrownVegetables,eaten=_world.Food.EatenVegetables,foodNeeded=_world.FoodWorkNeeded,atHome=_world.People.Count(_world.AvailableAtHome)});}
        }
        _paused=true;Check(_world.Food.Time>=1200 && _world.Housed==8 && _world.Food.EatenVegetables>24 && _world.People.All(p=>p.RestVisits>0),"Whole farmstead did not reach mature daily life");
        await CaptureReviewBundle("mature-authored-farmstead");File.WriteAllText(Path.Combine(_reviewDirectory,"mature-session.json"),JsonSerializer.Serialize(new{provenance="scripted ordinary controls from public entry; explicit camera framing and6x waits; no world injection or spontaneous preference",samples},new JsonSerializerOptions{WriteIndented=true}));
        var home=_world.Cottages.First(c=>Buildings.Get(c.Kind).Beds>0);ShowWorkplaceCard(home.Id);await Frames();await UiClick(_householdResidents[1]);await Frames();Check(_dailyPerson>=0,"Mature household cannot be followed");await UiClick(_dailyHomeBack);await Frames();
        var field=_world.Cottages.First(c=>c.Kind==BuildingKind.VegetableField);_paused=false;wallStart=_uiTime;while(field.Harvest>0 && _uiTime-wallStart<75)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_paused=true;
        ShowWorkplaceCard(field.Id);await Frames();await UiClick(_workCardPause);await Frames();await UiClick(_reshapePlot);await Frames();await UiClick(_revisionLess);await Frames();await UiClick(_revisionApply);await Frames();await UiClick(_workCardPause);await Frames();Check(field.Depth==2 && !field.WorkPaused,"Mature land revision failed");
        ClearSelection();await CaptureReviewBundle("mature-land-revised");_world.Validate();
        GD.Print("PASS: public entry to four chosen homes, two ground-drawn strips, pure gesture cancellation,20 simulated minutes of actual life, household observation and chosen scripted land revision. No human preference claim.");
    }
}
