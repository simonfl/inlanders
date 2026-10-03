using Godot;
using System.Collections.Generic;
public partial class Game
{
    private Button _landSurvey=null!;
    private HBoxContainer _villageTools=null!;
    private readonly Dictionary<int,Button> _villageLinks=new();
    private void MakePublicNavigation(VBoxContainer column)
    {
        _villageTools=new();column.AddChild(_villageTools);
        foreach(int index in new[]{0,4,3})
        {
            int page=index;var b=Button(index==4?"Supplies":MenuNames[index],()=>ToggleDrawer(page));b.AddThemeFontSizeOverride("font_size",14);b.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;
            b.TooltipText=index switch{0=>"People and advanced staffing [V]",4=>"Detailed supplies and work [I]",_=>"Save, load, sound and controls [O]"};_villageLinks[index]=b;_villageTools.AddChild(b);
        }
        _landSurvey=Button("Look over the land",ToggleResourceSurvey);column.AddChild(_landSurvey);
    }
    private void RenderPublicNavigation()
    {
        bool small=_world.PublicPlace!=null;
        foreach(int i in new[]{0,3,4})_menuButtons[i].Visible=!small;
        _villageTools.Visible=small;_landSurvey.Visible=small;
        if(small)_menuButtons[2].TooltipText="Watch or finish this village; People, Supplies and Options are here [G].";
    }
}
