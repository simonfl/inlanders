using System.IO;
using Inlanders.Simulation;
public partial class Game
{
    private string TransformationPath=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"transformation.json");
    private void TransformationMenu()
    {
        MenuPage("Between wood and water");
        _mainColumn.AddChild(Text("An inhabited hamlet · development comparison",20,true));
        _mainColumn.AddChild(Text("Twelve neighbors live close together. Two gardens grow beyond the inlet, reached by the western detour. The remaining open ground near home could become food, workshops or a place to gather.",16,true));
        _mainColumn.AddChild(Text("The first 12 logs will not fund everything. The woodlot is preserved: release selected trees for timber or clear a plot, knowing what you give up. The northern meadow offers more growing room. A crossing shortens journeys but does not increase the harvest.",16,true));
        _mainColumn.AddChild(Text("Choose your own transformation: bring food closer, spread the homes, retain the woods or work the distant ground. More neighbors are optional. There is no prescribed building sequence or timed deadline. Food keeps being consumed while you decide; pause freely.",16,true));
        if(File.Exists(TransformationPath))MenuButton("Resume hamlet",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(TransformationPath))));
        void Start()=>MenuAttempt(()=>{var w=World.NewTransformationHamlet();w.SaveFile(TransformationPath);EnterFromMenu(w);});
        MenuButton("New hamlet",()=>{if(File.Exists(TransformationPath))ConfirmMenu("Begin again?","Replace the hamlet?",Start,TransformationMenu);else Start();});
        MenuButton("Back",ComparisonMenu);
    }
}
