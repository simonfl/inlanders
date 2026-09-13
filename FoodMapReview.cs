using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbeFoodMap()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<6;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        ClearSelection();string saved=_world.SaveJson();await OpenMenu(4);await UiClick(_foodMapToggle);await Frames();
        Check(_showFoodMap && !_drawer.Visible,"Food view did not open on world");
        foreach(var s in _world.ReadFoodMap())Check(_foodMapLabels[s.Id??-1].Label.Text.Contains($"{s.Available} free · {s.Claimed} claimed"),"Food label differs from live store");
        Check(_world.SaveJson()==saved,"Food observation changed simulation");
        ClearSelection();_noticeUntil=0;await CaptureReviewBundle("food-in-world");
        await OpenMenu(4);await UiClick(_foodMapToggle);await Frames();
        Check(!_showFoodMap && _foodMapLabels.Values.All(l=>!l.Panel.Visible),"Food view failed to close");
        Check(_world.SaveJson()==saved,"Closing food view changed simulation");
        CloseDrawer();
        if(_world.HasWorkplaceFood)
        {
            var site=_world.Cottages.First(c=>c.Complete && _world.IsWorkplaceFoodStore(c));SelectBuilding(site.Id);await Frames();
            int before=site.LocalFoodReserve;await UiClick(_pantryMore);await Frames();
            Check(site.LocalFoodReserve==Math.Min(24,before+4),"Producer retention control failed");
            await CaptureReviewBundle("local-food-reserve");await Press(Key.F5);string policy=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.SaveJson()==policy,"Retention save/load differs");
            SelectBuilding(site.Id);await Frames();await UiClick(_pantryLess);await Frames();Check(_world.Cottages.First(c=>c.Id==site.Id).LocalFoodReserve==before,"Retention decrease failed");ClearSelection();
        }
        GD.Print("PASS: optional world food view matches stores, closes and preserves world state");
    }
}
