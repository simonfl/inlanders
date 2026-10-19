using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private bool _riverRelaxed;
    private void RiverLivelihoodMenu()=>RiverSessionMenu(true);
    private void RiverSessionMenu(bool establish)
    {
        MenuPage("Free play · river settlement");
        MenuButton(establish?"Food setup: choose your own · change":"Food setup: working fields & landing · change",()=>RiverSessionMenu(!establish));
        _mainColumn.AddChild(Text("One river settlement, with optional prepared food production. Choose your own purpose; there is no challenge to finish.",15,true));
        _mainColumn.AddChild(Text(establish?"Twelve neighbors have homes and provisions. Choose growing ground, river work, woodland gathering, or a mixture.":"Twelve neighbors live between woods and river. A long field, kitchen plot and fishing landing provide their food.",17,true));
        MenuButton(_riverRelaxed?"Mode: Relaxed · change":"Mode: Normal · change",()=>{_riverRelaxed=!_riverRelaxed;RiverSessionMenu(establish);});
        _mainColumn.AddChild(Text(_riverRelaxed?"Free, instant building. Real work and meals, without hunger penalties.":"Build with materials and shared work. Neighbors collect and eat actual food.",15,true));
        var profile=new HamletProfile(_riverRelaxed,true,PlayerFounded:establish,RiverFrontage:true,RiverLandscape:true);
        string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
        if(File.Exists(path))MenuButton("Resume settlement",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
        void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
        MenuButton("New settlement",()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this settlement?",Start,()=>RiverSessionMenu(establish));else Start();});
        if(DeveloperPlaces)MenuButton("Earlier beginnings",EarlierBeginningsMenu);
        MenuButton("Back",ShowMainMenu);
    }
}
