using System.Linq;
public partial class Game
{
    private void UpdateRestorationUi()
    {
        var c=_world.Challenge!;
        _foundingGoals.Hide();_foundingHallGoals.Hide();_objective.Hide();_goalArrival.Hide();
        _goalTitle.Text="AFTER THE FLOOD";_challengeInvite.Hide();
        _challengeAccess.Visible=!c.Complete && _world.Cottages.Any(b=>b.Kind==Inlanders.Simulation.BuildingKind.VegetableField && _world.ReadWorkplace(b).MissingAccess);
        _challengeRecover.Visible=!c.Complete;
        _challengeRecover.Text=ChallengeHousingFirst?"Choose replacement homes":ChallengePersonFirst && ChallengePerson() is {} person?"Inspect "+person.Name+" · recovery":"Inspect food production";
        _challengeWatch.Visible=!c.Continuing;_challengeWatch.Text=ChallengePerson() is {} neighbor?"Find "+neighbor.Name:"Find a neighbor";
        _challengeProgress.Text=c.Complete?"VILLAGE RESTORED\nTwelve neighbors have reachable homes and have eaten two meals during recovery. Two more village meals are in store.\nContinue building when you choose; growth is optional.":$"Restore the village for the twelve people already here. No new arrivals required.\n\nReachable homes: {_world.ChallengeHoused}/12\nResidents fed twice: {_world.RestorationFed}/12\nEveryone fed now: {_world.People.Count(p=>p.Fed)}/12\nFood for two more meals: {System.Math.Min(24,_world.EdibleStored)}/24\n\n{_world.RestorationProblem()}\n\nThe near crossing opens an island. Grow there, cross again to the old fields, or develop the home bank. Rebuild where the routes suit your village. Meals retain their progress.";
        _challengeContinue.Visible=c.Complete && !c.Continuing;_challengeContinue.Text="Village restored · continue building";_challengeLeave.Visible=c.Complete;
        _menuButtons[2].Text=_world.RestorationStatus;_menuButtons[2].TooltipText="Restoration purpose, progress and recovery [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Restoration";
    }
}
