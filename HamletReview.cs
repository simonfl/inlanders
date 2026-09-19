using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeHamlet()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        string before=_world.SaveJson();
        Check(UseLandscapeContext && _studyBoundary!=null && !_studyBoundary.Visible,"Hamlet context/boundary incorrect");
        Check(!WorldLabelsVisible,"Unselected labels crowd hamlet");
        ToggleDrawer(1);SelectBuildSection(0);_buildingFilter.Select(1);UpdateVillageDirectory();await Frames();
        await UiClick(_kindButtons[BuildingKind.Cottage],6);await Frames();
        Check(_placing && _studyBoundary!.Visible,"Buildable boundary missing during placement");
        Check(_world.PlacementProblem(new(_world.Map.MaxX+3,0),0)!=null,"Scenic ground became buildable");
        await CaptureReviewBundle("hamlet-buildable-boundary");
        await Press(Key.Escape);CloseDrawer();await Frames();
        Check(!_studyBoundary!.Visible,"Boundary remains after cancelling");
        int home=_world.Cottages.First(c=>c.Kind==BuildingKind.Lodge || c.Kind==BuildingKind.Cottage).Id;
        SelectBuilding(home);await Frames();ApplyWorldLabels();
        foreach(Node3D label in GetTree().GetNodesInGroup("world_labels"))
            if(label.Visible)Check(_cottages[home].Body.IsAncestorOf(label),"Unrelated label visible");
        ClearSelection();await Frames();
        Check(_world.SaveJson()==before,"Presentation navigation edited village");
        GD.Print("PASS: hamlet context, tool-only boundary, rejected scenic-ground placement, contextual labels and unchanged village.");
        await ProbeFoundingRearrangement();
    }

    private async Task ProbeFoundingRearrangement()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var site=_world.Cottages.First(c=>c.Kind==BuildingKind.Cottage && c.Complete);
        SelectBuilding(site.Id);await Frames();
        Check(_moveButton.IsVisibleInTree() && !_moveButton.Disabled,"Founding move action missing");
        string before=_world.SaveJson();await UiClick(_moveButton,6);await Press(Key.R);await Press(Key.Escape);await Frames();
        Check(_movingSite<0 && _world.SaveJson()==before,"Move cancellation edited founding");
        SelectBuilding(site.Id);await Frames();await UiClick(_moveButton,6);
        var target=_world.Map.Land.OrderBy(c=>(c.Point-site.Cell.Point).LengthSquared()).First(c=>c!=site.Cell && _world.RelocationProblem(site.Id,c,_rotation)==null);
        _focus=OnGround(target.X,target.Z);_camera.Size=18;UpdateCamera();
        var point=_camera.UnprojectPosition(OnGround(target.X,target.Z));
        ReviewInput(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
        Check(_ghostValid && _hover==target,"Move preview failed");
        await CaptureReviewBundle("founding-rearrangement-preview");
        ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=true});await Frames();
        ReviewInput(new InputEventMouseButton{Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=false});await Frames();
        Check(site.Cell==target && _movingSite<0 && _paused,"Pointer move failed");_world.Validate();
        SaveWorld();string moved=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==moved,"Moved founding save differs");
        GD.Print("PASS: founding move inspector, held click, rotate/cancel, pointer placement and F9.");
        ClearSelection();CloseDrawer();
    }
}
