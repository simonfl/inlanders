using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeMultipleSharedPlaces()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Choose(Cell at){_focus=OnGround(at.X,at.Z);_camera.Size=20;UpdateCamera();await Frames();await Click(_camera.UnprojectPosition(OnGround(at.X,at.Z)));await Frames();}
        foreach(var (center,seats) in new[]{(new Cell(0,7),2),(new Cell(0,1),4)})
        {
            await OpenMenu(2);await Frames();await UiClick(_foundingCommons);await Frames();await UiClick(_commonsSizeButtons[seats/2-1]);await Frames();
            if(seats==4){await UiClick(_commonsShape);await Frames();await Press(Key.R);await Frames();}
            await Choose(center);
            Check(!_gatherPlanStart.Disabled,"Independent shared-place proposal refused");await UiClick(_gatherPlanStart);await Frames();
            Check(SelectedSharedPlace?.Center==center && SelectedSharedPlace.Places.Length==seats,"Wrong place selected after creation");
        }
        Check(_world.SharedPlaces[1].Layout==SharedPlaceLayout.Line && _world.SharedPlaces[1].Rotation==1,"Line direction ignored");
        var line=_world.SharedPlaces[1];var gap=new Cell((int)line.Places.Average(p=>p.X),(int)line.Places.Average(p=>p.Z));
        ClearSelection();await Choose(gap);Check(SelectedSharedPlace==line,"Visible ground between line seats missed selection");
        var oldFocus=_focus;float oldZoom=_camera.Size;await UiClick(_commonsCardWatch);await Frames();await Press(Key.H);await Frames();
        Check(SelectedSharedPlace==line && _focus==oldFocus && _camera.Size==oldZoom,"Shared Watch lost place/camera");
        string beforePath=_world.SaveJson();var pathFocus=_focus;float pathZoom=_camera.Size;
        await UiClick(_commonsCardPath);await Frames();Check(_pathAnchor==line.Center && _placing,"Shared path did not start at selected place");
        await Press(Key.Escape);await Frames();Check(beforePath==_world.SaveJson() && SelectedSharedPlace==line && _focus==pathFocus && _camera.Size==pathZoom,"Shared path cancel lost world/place/camera");
        await UiClick(_commonsCardPath);await Frames();var destination=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage).Entrance;
        _focus=OnGround(destination.X,destination.Z);_camera.Size=20;UpdateCamera();await Frames();
        var pathPoint=_camera.UnprojectPosition(OnGround(destination.X,destination.Z));
        Input.ParseInputEvent(new InputEventMouseMotion{Position=pathPoint,GlobalPosition=pathPoint});await Frames();
        Check(_ghostValid && _connectionRoute.Count>1,"Shared approach preview missing");await CaptureReviewBundle("shared-place-path-proposal");
        await Click(pathPoint);await Frames();await UiClick(_pathProposalApply);await Frames();
        Check(!_placing && SelectedSharedPlace==line && _world.Paths.Contains(line.Center) && _world.Paths.Contains(destination) && _focus==pathFocus && _camera.Size==pathZoom,"Shared approach apply/return failed");
        Check(_world.SharedPlaces.Count==2,"Adding replaced existing place");FrameMap();await Frames();await CaptureReviewBundle("two-independent-shared-places");
        var original=_world.SharedPlaces[0].Center;var second=_world.SharedPlaces[1];var secondSeats=second.Places.ToArray();ClearSelection();await Choose(original);
        Check(_commonsCard.Visible && SelectedSharedPlace?.Center==original,"Ground selection did not choose first place");string saved=_world.SaveJson();
        await UiClick(_commonsCardMove);await Frames();await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Cancelled selected move mutated world");
        ShowCommonsCard(original);await Frames();await UiClick(_commonsCardMove);await Frames();
        var target=_world.Map.Land.Where(c=>c!=original && _world.CommonsProblem(c,2,original)==null).OrderBy(c=>(c.Point-original.Point).LengthSquared()).First();
        await Choose(target);await UiClick(_gatherPlanStart);await Frames();
        Check(_world.SharedPlaces.Count==2 && SelectedSharedPlace?.Center==target && second.Places.SequenceEqual(secondSeats),"Moving selected place changed another");
        await UiClick(_commonsCardRemove);await Frames();Check(_world.SharedPlaces.Count==1 && _world.Commons==second,"Selected removal removed both places");
        saved=_world.SaveJson();await Press(Key.F5);await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Independent-place save/load");
        _world.Validate();await CaptureReviewBundle("independent-place-after-removal");GD.Print("PASS independent shared-place native add, select, move/cancel, remove and current save");
    }
}
