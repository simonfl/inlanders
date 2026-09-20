using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private string TransformationPath=>TransformationSavePath(_world.Creative);
    private string TransformationSavePath(bool relaxed)=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,relaxed?"transformation-relaxed.json":"transformation.json");
    private void TransformationMenu()
    {
        MenuPage("Between wood and water");
        _mainColumn.AddChild(Text("An inhabited hamlet · development comparison",20,true));
        _mainColumn.AddChild(Text("Twelve neighbors live close together. Two kitchen gardens occupy the ground beside the homes; a third grows beyond the inlet. Keep this working arrangement, or make room near home for shared meals and move food work elsewhere.",16,true));
        _mainColumn.AddChild(Text("There are 12 logs for your first changes. The woodlot is preserved: release selected trees for timber or clear a plot, knowing what you give up. The northern meadow offers more growing room. A crossing shortens journeys but does not increase the harvest.",16,true));
        _mainColumn.AddChild(Text("Choose your own transformation: bring food closer, spread the homes, retain the woods or work the distant ground. More neighbors are optional. There is no prescribed building sequence or timed deadline. Food keeps being consumed while you decide; pause freely.",16,true));
        _mainColumn.AddChild(Text("Normal uses real materials and building work. Relaxed uses the exact same village, food recipes and daily life, with free instant construction and no hunger penalties. Each has its own save.",15,true));
        foreach(bool relaxed in new[]{false,true})
        {
            string path=TransformationSavePath(relaxed),label=relaxed?"relaxed hamlet":"hamlet";
            if(File.Exists(path))MenuButton("Resume "+label,()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(path))));
            void Start()=>MenuAttempt(()=>{var w=World.NewTransformationHamlet(relaxed);w.SaveFile(path);EnterFromMenu(w);});
            MenuButton("New "+label,()=>{if(File.Exists(path))ConfirmMenu("Begin again?","Replace this "+label+"?",Start,TransformationMenu);else Start();});
        }
        MenuButton("Back",ComparisonMenu);
    }
}
