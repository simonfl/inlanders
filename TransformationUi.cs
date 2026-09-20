using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private string TransformationPath=>TransformationSavePath(_world.Creative);
    private string TransformationSavePath(bool relaxed)=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,new HamletProfile(relaxed).SaveName);
    private void TransformationMenu()=>HamletMenu(false);
    private void HamletMenu(bool cultivatedBank)
    {
        MenuPage(cultivatedBank?"The cultivated bank":"Between wood and water");
        _mainColumn.AddChild(Text("Twelve neighbors. A working hamlet to make your own.",20,true));
        _mainColumn.AddChild(Text(cultivatedBank?"Homes share a working river bank. Keep what you like; change what you want. Finishing and new arrivals are optional.":"A hamlet between woodland and river. Keep what you like; change what you want. Finishing and new arrivals are optional.",16,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,cultivatedBank);
            string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName),label=relaxed?"relaxed hamlet":"hamlet";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, real daily life, no hunger penalty":"Normal · real materials, work and meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this "+label+"?",Start,()=>HamletMenu(cultivatedBank));else Start();});
        }
        MenuButton(cultivatedBank?"Compare: compact hamlet":"Compare: cultivated bank",()=>HamletMenu(!cultivatedBank));
        MenuButton("About this place",()=>{
            MenuPage("Life between wood and water");
            _mainColumn.AddChild(Text("The woodlot is preserved; release chosen trees when you need timber. The meadow beyond the inlet has growing room. A crossing shortens journeys but grows no food.",16,true));
            _mainColumn.AddChild(Text("Choose what you want to improve. Pause a kitchen garden to move it; growing crops need fresh sowing. Homes and most paused workplaces can move too. Shared places use ordinary meals from nearby stores.",16,true));
            _mainColumn.AddChild(Text("Normal and relaxed share this place, recipes and daily behavior. Relaxed also allows free editing. Both have their own saves. Make a place you would like to keep. No prescribed building sequence or deadline.",16,true));
            MenuButton("Back",()=>HamletMenu(cultivatedBank));
        });
        MenuButton("Back",ShowMainMenu);
    }
}
