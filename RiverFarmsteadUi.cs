using System.IO;
using Inlanders.Simulation;

public partial class Game
{
    private string RiverFarmsteadPath => Path.Combine(Path.GetDirectoryName(_creativeSavePath)!, "river-farmstead.json");
    private void RiverFarmsteadMenu()
    {
        MenuPage("Une place à nous");
        _mainColumn.AddChild(Text("A place of our own · New France",20,true));
        _mainColumn.AddChild(Text("The river brought eight neighbors to this bank. One house, a little timber and provisions are the beginning of a home—not yet a livelihood.",16,true));
        _mainColumn.AddChild(Text("Build homes and choose food: work the open ground, reach the fishing water, or use the woodlot. Provisions cover about six minutes for eight people. Workers share jobs automatically. All buildings are available.",16,true));
        _mainColumn.AddChild(Text("Remain a small settlement or invite more neighbors. Once homes and food are working, finish whenever you are satisfied, or keep shaping the village.",16,true));
        if(File.Exists(RiverFarmsteadPath)) MenuButton("Resume · A place of our own",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(RiverFarmsteadPath))));
        void Start()=>MenuAttempt(()=>{var w=World.NewRiverFarmstead();w.SaveFile(RiverFarmsteadPath);EnterFromMenu(w);});
        MenuButton("New · A place of our own",()=>{if(File.Exists(RiverFarmsteadPath))ConfirmMenu("Start a new farmstead?","Replace this farmstead?",Start,RiverFarmsteadMenu);else Start();});
        MenuButton("Another village · The long way home",WorkingVillageMenu);
        MenuButton("Back",ShowMainMenu);
    }
}
