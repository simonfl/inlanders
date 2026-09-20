using Godot;
public partial class Game
{
    private Button _hamletCompare=null!,_hamletCompareDone=null!;
    private PanelContainer _hamletComparePanel=null!;
    private void MakeHamletComparisonUi()
    {
        _hamletCompare=Button("Compare with the opening layout",()=>{
            CloseManagementUi();_noticeUntil=0;_courtShowBefore=true;
        });_foundingGoals.AddChild(_hamletCompare);
        _hamletComparePanel=HudPanel(_hud);var column=new VBoxContainer();_hamletComparePanel.AddChild(column);
        var legend=Text("YOUR CHANGES\nGold · opening footprints\nBlue · moved or added places\nDaily life continues; nothing is rewound.",14,true);legend.CustomMinimumSize=new(280,0);column.AddChild(legend);
        _hamletCompareDone=Button("Return to live view [Esc]",()=>_courtShowBefore=false);column.AddChild(_hamletCompareDone);_hamletComparePanel.Hide();
    }
    private void RenderHamletComparison()
    {
        bool hamlet=_world.PublicPlace!=null;
        _hamletCompare.Visible=hamlet;
        if(hamlet && (_atMainMenu || _watching || _placing || _drawer.Visible))_courtShowBefore=false;
        bool show=hamlet && _courtShowBefore;
        if(hamlet && _courtStartingLayout!=null)_courtStartingLayout.Visible=show;
        _hamletComparePanel.Visible=show;
        _hamletComparePanel.Position=new(16,92);_hamletComparePanel.Size=new(306,0);
    }
}
