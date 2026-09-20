using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task ProbeBuildingGroups()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        string initial=_world.SaveJson();
        Check(_kindButtons.Count(p=>p.Value.IsVisibleInTree())==2,"Homes should show two choices");
        await CaptureReviewBundle("catalog-homes");
        foreach(int category in new[]{2,3,4,5,0,1})
        {
            var button=_categoryButtons.First(b=>b.Text.TrimStart('›',' ')==BuildingCategoryNames[category]);
            await UiClick(button,6);await Frames();
            Check(_buildingFilter.Selected==category,"Category click lost");
            var visible=_kindButtons.Where(p=>p.Value.IsVisibleInTree()).Select(p=>p.Key).ToArray();
            Check(visible.Length==Enum.GetValues<BuildingKind>().Count(k=>category==0 || BuildingCategory(k)==category),"Category omits buildings");
            Check(visible.All(k=>category==0 || BuildingCategory(k)==category),"Category leaks unrelated buildings");
            Check(_buildingGroups.All(g=>g.Panel.IsVisibleInTree()==(category==0 || g.Category==category)),"Empty or hidden category heading");
            if(category==2)
            {
                Check(visible.Contains(BuildingKind.Pantry),"Pantry missing from food");
                var viewport=_drawerPages[1].GetGlobalRect();
                foreach(var kind in new[]{BuildingKind.ForagerHut,BuildingKind.VegetableGarden,BuildingKind.FishingDock})
                    Check(viewport.Encloses(_kindButtons[kind].GetGlobalRect()),"First food alternatives cannot be compared at once");
                Check(BuildingDescription(BuildingKind.FishingDock).Contains("stored at the dock"),"Local fish guidance stale");
                Check(BuildingStaff(BuildingKind.ForagerHut)=="Shared workers","Shared staffing guidance stale");
                await CaptureReviewBundle("catalog-food");
                await UiClick(_kindButtons[BuildingKind.Bakery],6);await Frames();
                Check(_placing && _buildKind==BuildingKind.Bakery,"Grouped card did not start preview");
                await Press(Key.Escape);await Frames();
            }
        }
        await Press(Key.B);await Frames();
        Check(_catalogKeyboard && _buildingFilter.IsVisibleInTree(),"Keyboard category navigation unavailable");
        _buildingFilter.GrabFocus();await Press(Key.Right);await Frames();
        Check(_buildingFilter.Selected==2,"Keyboard category change failed");
        await Press(Key.Escape);await Frames();
        Check(_world.SaveJson()==initial,"Catalog browsing changed village");
        GD.Print("PASS: grouped catalog held-click categories, all buildings, food pantry, preview/cancel and keyboard navigation; village unchanged.");
    }
}
