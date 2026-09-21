using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeCommonsCard()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        FrameMap();await Frames();
        var center=_world.Map.Land.First(c=>_world.CommonsProblem(c)==null && _world.CommonsFoodNearby(c) &&
            !PointerOverHud(_camera.UnprojectPosition(OnGround(c.X,c.Z))) &&
            _people.All(p=>_camera.UnprojectPosition(p.Body.Position+Vector3.Up*.6f).DistanceTo(_camera.UnprojectPosition(OnGround(c.X,c.Z)))>30));
        Check(_world.SetCommons(center),"Shared place fixture failed");UpdateCommonsView();await Frames();
        await Click(_camera.UnprojectPosition(OnGround(center.X,center.Z)));await Frames();Check(_commonsCard.Visible && !_drawer.Visible,"Shared place world click missed card");
        string before=_world.SaveJson();await CaptureReviewBundle("shared-place-world-card");
        await UiClick(_commonsCardMove);await Frames();Check(_gatherPlanning && _planningCommons,"Shared place move did not preview");await Press(Key.Escape);await Frames();Check(_world.SaveJson()==before,"Cancelled shared-place move changed world");
        ShowCommonsCard();await Frames();await UiClick(_commonsCardWatch);await Frames();Check(_watching && !_followPerson && _world.SaveJson()==before,"Shared place watch mutated world");await Press(Key.Escape);await Frames();
        ShowCommonsCard();await Frames();await UiClick(_commonsCardRemove);await Frames();Check(_world.Commons==null && !_commonsCard.Visible,"Shared place remove failed");_world.Validate();
    }
}
