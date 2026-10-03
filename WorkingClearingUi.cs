using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private void WorkingClearingMenu()
    {
        MenuPage("A working clearing");
        _mainColumn.AddChild(Text("Eight neighbors have homes. Their small cultivated strips lie apart from them, toward the woods. The shore and open ground leave room to make this place your own.",18,true));
        _mainColumn.AddChild(Text("Four homes, two small working strips, 16 logs, 4 planks and 80 food portions. Keep a modest village, bring home and work closer, or try a river livelihood. No required change or deadline.",15,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,false,false,true,true);string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
            string label=relaxed?"relaxed clearing":"clearing";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free editing, real daily life":"Normal · real materials, work and meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this clearing?",Start,WorkingClearingMenu);else Start();});
        }
        MenuButton("Back",TransformationMenu);
    }
}
