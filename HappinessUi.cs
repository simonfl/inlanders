using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private Button _happinessButton = null!;
    private Label _happinessReasons = null!;
    private void MakeHappinessUi()
    {
        _happinessButton=Button("",()=>_happinessReasons.Visible=!_happinessReasons.Visible);
        _happinessButton.TooltipText="Show the reasons behind this villager's happiness.";
        _personDetails.AddChild(_happinessButton);
        _happinessReasons=Text("",14,true); _happinessReasons.Hide(); _personDetails.AddChild(_happinessReasons);
    }
    private void UpdateHappinessUi(Villager person)
    {
        var report=_world.ReadHappiness(person);
        _happinessButton.Text=$"{report.Mood} · {report.Score}/100";
        _happinessReasons.Text=report.Reasons+"\n\nA completed square break lasts two minutes. Happiness changes idle reactions; it adds no work penalty.";
    }
}
