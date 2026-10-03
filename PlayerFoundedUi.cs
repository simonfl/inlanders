using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private bool _frontageStart;
    private void PlayerFoundedMenu()
    {
        MenuPage(_frontageStart?"River frontage":"A place of our own");
        MenuButton(_frontageStart?"Landscape: river frontage · change":"Landscape: inlet · change",()=>{_frontageStart=!_frontageStart;PlayerFoundedMenu();});
        _mainColumn.AddChild(Text(_frontageStart?"Open land along a river bend; a rising woodlot inland. Choose how homes and growing ground meet the shore.":"Two banks around a narrow inlet. Nearby ground or a longer journey to the opposite shore.",15,true));
        _mainColumn.AddChild(Text("Eight neighbors, open land and supplies to begin.",20,true));
        _mainColumn.AddChild(Text("Choose homes and a livelihood. Begin with 48 logs, 4 planks and 120 food portions. Shared workers build and tend the village.",16,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,true,true,_frontageStart);string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
            string label=relaxed?"relaxed farmstead":"farmstead";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, daily life without hunger penalties":"Normal · timber, work and real meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var world=profile.Create();world.SaveFile(path);EnterFromMenu(world);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this farmstead?",Start,PlayerFoundedMenu);else Start();});
        }
        _mainColumn.AddChild(Text("Keep the village small or invite neighbors when you want. No required building sequence or deadline. You can finish for now and return later.",15,true));
        MenuButton("Shape an inhabited hamlet",()=>HamletMenu(true,false,true));
        MenuButton("Back",TransformationMenu);
    }
}
