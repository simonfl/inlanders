using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeSharedVisitors()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var place=_world.SharedPlaces.First();ShowCommonsCard(place.Center);await Frames();
        await UiClick(_commonsPeople);await Frames();
        var visitors=_world.ReadSharedVisitors(place.Center);Check(visitors.Length>0,"No actual visitors");
        string saved=_world.SaveJson();var focus=_focus;float zoom=_camera.Size,angle=_angle;
        Check(_commonsVisitors.Count(b=>b.Visible)==visitors.Length,"Visitor buttons mismatch");
        Check(_commonsCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Shared card overflow");await CaptureReviewBundle("shared-visitors");
        await UiClick(_commonsVisitors[0]);await Frames();Check(_dailyPerson==visitors[0].Person && _dailySharedOrigin==place.Center,"Wrong visitor/origin");
        await CaptureReviewBundle("shared-visitor-follow");await UiClick(_dailyHomeBack);await Frames();
        Check(_selectedCommonsCenter==place.Center && _commonsCard.Visible && _focus==focus && _camera.Size==zoom && _angle==angle,"Shared-place return lost context");
        Check(saved==_world.SaveJson(),"Visitor inspection mutated world");
        Check(_world.ReadSharedVisitors(new(999,999)).Length==0,"Invented visitors for missing place");
        GD.Print("PASS: actual shared visitors, compact card, individual follow and pure camera/place return.");
    }
}
