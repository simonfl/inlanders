using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private bool _foundClearing;
    private void WorkingClearingMenu()
    {
        MenuPage(_foundClearing?"Found the clearing":"A working clearing");
        MenuButton(_foundClearing?"Beginning: open land · change":"Beginning: inhabited · change",()=>{_foundClearing=!_foundClearing;WorkingClearingMenu();});
        _mainColumn.AddChild(Text(_foundClearing?"Same land, eight neighbors and 80 food. All building timber starts as supplies. Choose homes and a livelihood; construction and crops begin from scratch.":"Four homes and two small fields, with room to bring home, land and shore together. No required change or deadline.",16,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,true,false,false,_foundClearing,true,true);string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName);
            string label=relaxed?"relaxed clearing":"clearing";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, no hunger penalties":"Normal · spend materials; provide daily meals",15,true));
            var actions=new Godot.HBoxContainer();_mainColumn.AddChild(actions);
            void InRow(Godot.Button button){button.TooltipText=button.Text;button.Text=button.Text.StartsWith("New")?"New":"Resume";_mainColumn.RemoveChild(button);button.SizeFlagsHorizontal=Godot.Control.SizeFlags.ExpandFill;actions.AddChild(button);}
            if(File.Exists(path))InRow(MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path)))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            InRow(MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this clearing?",Start,WorkingClearingMenu);else Start();}));
        }
        MenuButton("Back",TransformationMenu);
    }
}
