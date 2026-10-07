using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeGroupArrangement(bool fields=false)
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<8;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Point(Cell cell){_focus=OnGround(cell.X,cell.Z);_camera.Size=22;UpdateCamera();await Frames();await Click(_camera.UnprojectPosition(OnGround(cell.X,cell.Z)));await Frames();}
        var homes=fields?new[]{_world.Cottages.Single(c=>c.Kind==BuildingKind.Cottage && c.Cell==new Cell(-3,5)),_world.Cottages.Single(c=>c.Kind==BuildingKind.VegetableField && c.Cell==new Cell(-1,-1))}:_world.Cottages.Where(c=>c.Kind==BuildingKind.Cottage && c.Cell.X<0).ToArray();var ids=homes.Select(c=>c.Id).ToArray();
        string before=_world.SaveJson();await OpenMenu(2);await Frames();await UiClick(_groupEntry);await Frames();
        foreach(var home in homes)await Point(home.Cell);
        Check(_groupMembers.SequenceEqual(ids),"Group world selection failed");await UiClick(_groupPick);await Frames();await UiClick(_groupLeft);await Frames();
        var target=_world.Map.Land.OrderBy(c=>(c.Point-homes[0].Cell.Point).LengthSquared()).First(c=>_world.PreviewGroup(ids,c,3).Result!=null);
        await Point(target);Check(_groupPlan?.Result!=null && !_groupApply.Disabled && _paused,"Group proposal unavailable");
        Check(_groupPanel.GetGlobalRect().End.X<=_hud.Size.X && _groupPanel.GetGlobalRect().End.Y<_hud.Size.Y-76,"Group panel overflow");
        Check(before==_world.SaveJson(),"Preview changed live village");await CaptureReviewBundle("home-group-proposal");
        await Press(Key.Escape);await Frames();Check(!_groupActive && before==_world.SaveJson(),"Group cancel mutated village");
        await OpenMenu(2);await Frames();await UiClick(_groupEntry);await Frames();foreach(var home in homes)await Point(home.Cell);
        await UiClick(_groupPick);await Frames();await UiClick(_groupLeft);await Frames();await Point(target);await UiClick(_groupApply);await Frames();
        Check(!_groupActive && ids.All(id=>_world.Cottages.Single(c=>c.Id==id).Rotation==(homes.Single(c=>c.Id==id).Rotation+3)%4),"Group apply lost orientation");
        if(fields)Check(_world.Cottages.Where(c=>ids.Contains(c.Id) && c.Kind==BuildingKind.VegetableField).All(c=>!c.Planted && c.Growth==0),"Growing field did not restart");
        Check(_world.Cottages.Single(c=>c.Id==ids[0]).Cell==target,"Group anchor differs");_world.Validate();
        await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();Check(saved==_world.SaveJson(),"Group current save differs");await CaptureReviewBundle("home-group-applied");
        GD.Print("PASS: world group selection, actual model preview/turn, atomic cancel/apply, compact controls and current save.");
    }
}
