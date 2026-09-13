using System.IO;
using Inlanders.Simulation;

public partial class Game
{
    private string CreativeCourtPath=>Path.Combine(Path.GetDirectoryName(_creativeSavePath)!,"creative-court.json");
    private void CreativeCourtMenu()
    {
        MenuPage("Earlier free court · eight residents");
        _mainColumn.AddChild(Text("Arrange Willow court freely. Buildings are instant and free; move or remove as many as you like. Residents share work, collect real food, go home and meet. Missing meals never slows work or lowers mood.",16,true));
        _mainColumn.AddChild(Text("What would you like to make? Open a gathering place, shape a lane, or bring homes closer to their gardens. Welcoming more residents is optional in Goals.",16,true));
        if(File.Exists(CreativeCourtPath))MenuButton("Resume free arrangement",()=>MenuAttempt(()=>EnterFromMenu(World.LoadFile(CreativeCourtPath))));
        void Start()=>MenuAttempt(()=>{var world=World.NewCreativeCourt();world.SaveFile(CreativeCourtPath);EnterFromMenu(world);});
        MenuButton("New free arrangement",()=>{if(File.Exists(CreativeCourtPath))ConfirmMenu("New free arrangement","Replace your free-arrangement village with a fresh court?",Start,CreativeCourtMenu);else Start();});
        MenuButton("Back",ComparisonMenu);
    }
}
