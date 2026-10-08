using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private void RiverLivelihoodMenu()
    {
        MenuPage("A living from the river land");
        _mainColumn.AddChild(Text("Twelve neighbors have homes and provisions. Choose where their food will come from: growing ground, the river, the woods, or a mixture.",17,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,PlayerFounded:true,RiverFrontage:true,RiverLandscape:true);
            string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName),label=relaxed?"relaxed settlement":"settlement";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, meals without hunger":"Normal · build with materials and shared work",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this settlement?",Start,RiverLivelihoodMenu);else Start();});
        }
        MenuButton("Back",TransformationMenu);
    }
}
