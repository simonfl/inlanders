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
        _recoveryFood.Visible=!c.Complete;_recoveryFood.Text="Choose food supply · fields, fishing or gathering";
        _challengeRecover.Text=ChallengeHousingFirst?"Choose replacement homes":ChallengePersonFirst && ChallengePerson() is {} person?"Inspect "+person.Name+" · recovery":"Inspect food production";
        _challengeWatch.Visible=!c.Continuing && (c.Complete || _world.People.Any(p=>!p.Fed));_challengeWatch.Text=ChallengePerson() is {} neighbor?"Find "+neighbor.Name:"Find a neighbor";
        _challengeProgress.Text=c.Complete?"VILLAGE RESTORED\nTwelve neighbors have reachable homes and have eaten two meals during recovery. Two more village meals are in store.\nContinue building when you choose; growth is optional.":$"Twelve existing neighbors. No new arrivals required.\n\nHomes: {_world.ChallengeHoused}/12 · Fed now: {_world.People.Count(p=>p.Fed)}/12\nFed twice: {_world.RestorationFed}/12 · Food stored: {System.Math.Min(24,_world.EdibleStored)}/24\n\n{_world.RestorationProblem()}\n\n{_world.RecoverySupplySummary}\n\nChoose what to tackle first. Reconnect the ripe fields or establish new food elsewhere; crops stay on their land. Meals retain their progress.";
        _challengeContinue.Visible=c.Complete && !c.Continuing;_challengeContinue.Text="Village restored · continue building";_challengeLeave.Visible=c.Complete;
        _menuButtons[2].Text=_world.RestorationStatus;_menuButtons[2].TooltipText="Restoration purpose, progress and recovery [G]";
        if(_tabs.CurrentTab==2)_drawerTitle.Text="Restoration";
    }
}
