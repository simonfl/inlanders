using Godot;
using Inlanders.Simulation;
using System;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeSceneStudy()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        string saved=_world.SaveJson();bool original=_storybookScene;
        var focus=_focus;float angle=_angle,zoom=_camera.Size;
        CloseDrawer();ToggleDrawer(3);await Frames();
        // Exercise the normal option through pointer input, including rebuilding the scene twice.
        await UiClick(_sceneStudyButton);await Frames();
        Check(_storybookScene!=original,"Scene comparison option did not switch");
        await UiClick(_sceneStudyButton);await Frames();
        Check(_storybookScene==original && _world.SaveJson()==saved,"Scene comparison changed world state");
        Check(_focus==focus && _angle==angle && _camera.Size==zoom,"Scene comparison changed camera framing");
        CloseDrawer();
        if(original && _world.Neighborhood!=null)
        {
            BeginPlacement(BuildingKind.Cottage);await Frames();
            Check(_studyBoundary!=null && _studyBoundary.Visible,"Usable-land boundary missing while building");
            await CaptureReviewBundle();await Press(Key.Escape);await Frames();
            Check(!_placing && _studyBoundary!=null && !_studyBoundary.Visible,"Build boundary survived placement cancellation");
            if(_world.Neighborhood.Complete)
            {
                ToggleDrawer(2);await Frames();
                float compactHeight=_drawer.Size.Y;
                Check(CompactNeighborhoodGoals && !_visitorPanel.Visible,"Completed view was not concise");
                await UiClick(_studyDetailsButton);await Frames();
                Check(!CompactNeighborhoodGoals && _drawer.Size.Y>=compactHeight,"Village details did not expand");
                await UiClick(_studyDetailsButton);await Frames();
                Check(CompactNeighborhoodGoals && _drawer.Size.Y==compactHeight,"Village details did not collapse");
                CloseDrawer();
            }
        }
        Check(_world.SaveJson()==saved,"Presentation controls mutated simulation");
        GD.Print("PASS: scene comparison preserves world/camera; contextual boundary and completed details controls");
    }
}
