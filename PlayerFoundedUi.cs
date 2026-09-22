using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private void PlayerFoundedMenu()
    {
        MenuPage("A place of our own");
        _mainColumn.AddChild(Text("Eight neighbors, open land and supplies to begin.",20,true));
        _mainColumn.AddChild(Text("Choose where homes belong and how this place will feed itself. You have48 logs,4 planks and120 food portions. Gardens, grain with an oven, and fishing make different uses of the land. Shared workers build and tend them; no job assignments are needed.",16,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,true,true);string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
            string label=relaxed?"relaxed farmstead":"farmstead";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, daily life without hunger penalties":"Normal · timber, work and real meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var world=profile.Create();world.SaveFile(path);EnterFromMenu(world);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this farmstead?",Start,PlayerFoundedMenu);else Start();});
        }
        _mainColumn.AddChild(Text("Keep the village small or invite neighbors when you want. No required building sequence or deadline. You can finish for now and return later.",15,true));
        MenuButton("Back",TransformationMenu);
    }
}
