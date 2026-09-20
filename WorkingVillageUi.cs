using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private string WorkingVillagePath=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"working-village.json");
    private void WorkingVillageMenu()
    {
        MenuPage("The long way home");
        _mainColumn.AddChild(Text("Le détour · An inhabited village to reshape",20,true));
        _mainColumn.AddChild(Text("Eight neighbors already have homes, a grain field and an oven. The inlet lies between their homes and their work. Everyone can walk around its western end, but meals and work mean long journeys.",16,true));
        _mainColumn.AddChild(Text("Watch a resident, then decide what is worth changing: shorten a crossing, rearrange homes, or live more from the nearby shore. Fields use real land; workplaces can move after pausing. The woodland can remain or make room. All buildings are available.",16,true));
        _mainColumn.AddChild(Text("This is an open arrangement challenge, not a building checklist. Keep eight neighbors or grow by choice. Finish when satisfied after ordinary food is flowing, or stay and improve it.",16,true));
        if(File.Exists(WorkingVillagePath))MenuButton("Resume · The long way home",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(WorkingVillagePath))));
        void Start()=>MenuAttempt(()=>{var w=World.NewWorkingVillage();w.SaveFile(WorkingVillagePath);EnterFromMenu(w);});
        MenuButton("New · The long way home",()=>{if(File.Exists(WorkingVillagePath))ConfirmMenu("Start this village again?","Replace this village?",Start,WorkingVillageMenu);else Start();});
        MenuButton("Back",RiverFarmsteadMenu);
    }
}
