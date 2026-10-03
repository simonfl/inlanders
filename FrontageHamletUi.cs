using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private void FrontageHamletMenu()
    {
        MenuPage("An inhabited river frontage");
        MenuButton("Landscape: river frontage · change",()=>HamletMenu(true,false,true));
        _mainColumn.AddChild(Text("Homes along the river; growing ground inland. Twelve neighbors already live here. Change the distances between home and work, or keep a place you like.",17,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,false,false,true);
            string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName),label=relaxed?"relaxed hamlet":"hamlet";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, real daily life":"Normal · materials, work and meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this river hamlet?",Start,FrontageHamletMenu);else Start();});
        }
        _mainColumn.AddChild(Text("Same people, buildings and starting reserves as the inlet; different relationships to the land. No required improvement or deadline.",15,true));
        MenuButton("Back",TransformationMenu);
    }
}
