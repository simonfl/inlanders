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
    }
}
