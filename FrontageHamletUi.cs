using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private bool _riverLandscape;
    private void FrontageHamletMenu()
    {
        MenuPage(_riverLandscape?"River farmsteads":"An inhabited river frontage");
        MenuButton(_riverLandscape?"Layout: farmsteads · change":"Layout: scattered frontage · change",()=>{_riverLandscape=!_riverLandscape;FrontageHamletMenu();});
        MenuButton("Landscape: river frontage · change",()=>HamletMenu(true,false,true));
        _mainColumn.AddChild(Text(_riverLandscape?"Twelve neighbors between woods and river. An eight-row field and two-row kitchen plot share work with a fishing landing. Equal crop capacity to the scattered frontage; different walks and harvest timing.":"Homes along the river, two inland fields and a working fishing landing. Twelve neighbors share a small outdoor place. Change a journey or a yard, or keep what you like.",17,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,false,false,true,false,_riverLandscape);
            string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName),label=relaxed?"relaxed hamlet":"hamlet";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, real daily life":"Normal · materials, work and meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this river hamlet?",Start,FrontageHamletMenu);else Start();});
        }
        _mainColumn.AddChild(Text("The landing replaces the inlet kitchen garden: 8 logs in the yard instead of12, equal total timber investment, 4 planks and72 food. Fish and crops have different timing. No required improvement or deadline.",15,true));
        MenuButton("Back",TransformationMenu);
    }
}
