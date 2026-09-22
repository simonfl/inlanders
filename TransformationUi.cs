using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private string TransformationPath=>TransformationSavePath(_world.Creative);
    private string TransformationSavePath(bool relaxed)=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,new HamletProfile(relaxed).SaveName);
    private void TransformationMenu()=>HamletMenu(true,false,true);
    private void HamletMenu(bool cultivatedBank,bool groupedFarmsteads=false,bool acrossTheInlet=false)
    {
        MenuPage(new HamletProfile(false,cultivatedBank,groupedFarmsteads,acrossTheInlet).Title);
        _mainColumn.AddChild(Text("Twelve neighbors. A working hamlet to make your own.",20,true));
        _mainColumn.AddChild(Text(acrossTheInlet?"Most crops grow beyond the inlet; the homes share a crowded bank. Keep this place, bring home and work closer, or change the journey between them.":groupedFarmsteads?"Homes face smaller working clearings beside fields and a kitchen garden. The same people and productive land as the cultivated bank, arranged differently.":cultivatedBank?"Homes share two working fields and a kitchen garden. Four planks can furnish one home. Keep what you like; change what you want.":"A compact hamlet with kitchen gardens. Four planks can furnish one home. Keep what you like; change what you want.",16,true));
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new HamletProfile(relaxed,cultivatedBank,groupedFarmsteads,acrossTheInlet);
            string path=Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,profile.SaveName),label=relaxed?"relaxed hamlet":"hamlet";
            _mainColumn.AddChild(Text(relaxed?"Relaxed · free building, real daily life, no hunger penalty":"Normal · real materials, work and meals",15,true));
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=profile.Create();w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this "+label+"?",Start,()=>HamletMenu(cultivatedBank,groupedFarmsteads,acrossTheInlet));else Start();});
        }
        if(!acrossTheInlet)MenuButton("Compare: across the inlet",()=>HamletMenu(true,false,true));
        MenuButton(cultivatedBank?"Compare: compact hamlet":"Compare: cultivated bank",()=>HamletMenu(!cultivatedBank));
        if(groupedFarmsteads || acrossTheInlet)MenuButton("Compare: cultivated bank",()=>HamletMenu(true));
        if(!groupedFarmsteads)MenuButton("Compare: grouped farmsteads",()=>HamletMenu(true,true));
        MenuButton("About this place",()=>{
            MenuPage("Life between wood and water");
            _mainColumn.AddChild(Text("All openings have twelve residents, twelve logs, four planks and72 starting food. The bank and grouped farmsteads have identical buildings and36 cultivated tiles/48 vegetables per full crop; the compact village has18 tiles/24 vegetables. More land brings larger crops and more collection work. The woodlot is preserved; release chosen trees when you need timber. The meadow beyond the inlet has growing room. A crossing shortens journeys but grows no food.",16,true));
            _mainColumn.AddChild(Text("Choose what you want to improve. Pause a kitchen garden to move it; growing crops need fresh sowing. Homes and most paused workplaces can move too. Shared places use ordinary meals from nearby stores.",16,true));
            _mainColumn.AddChild(Text("Normal and relaxed share this place, recipes and daily behavior. Relaxed also allows free editing. Both have their own saves. Make a place you would like to keep. No prescribed building sequence or deadline.",16,true));
            MenuButton("Back",()=>HamletMenu(cultivatedBank,groupedFarmsteads,acrossTheInlet));
        });
        MenuButton("Back",ShowMainMenu);
    }
}
