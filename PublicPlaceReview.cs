using Godot;
using System;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbePublicPlaceContract()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var archived=Inlanders.Simulation.World.NewRiverFarmstead();
        Check(!CanContinuePlace(archived,false) && CanContinuePlace(archived,true),"Continue can bypass public place boundary");
        foreach(bool relaxed in new[]{false,true})
        {
            var profile=new Inlanders.Simulation.HamletProfile(relaxed,_world.Founding!.CultivatedBank,_world.Founding.GroupedFarmsteads);
            AdoptWorld(profile.Create());Check(CanContinuePlace(_world,false),"Continue rejects retained place");UpdateHud();await Frames();
            await Press(Key.O);await Frames();Check(!_legacyMapOptions.IsVisibleInTree(),"Public Options exposes legacy maps");
            string saved=_world.SaveJson();OpenLargeMap();OpenOriginalMap();Check(saved==_world.SaveJson(),"Public map guard escaped retained place");CloseDrawer();
            Check(!relaxed || _foodStatus.Text=="Relaxed" && !_foodStatus.GetParent<Control>().TooltipText.Contains("disabled"),"Relaxed daily-life description regressed");
            await Press(Key.G);await Frames();Check(_goalTitle.Text.StartsWith(profile.Title),"Place name differs from profile");CloseDrawer();
        }
        GD.Print("PASS: both public constraints retain Options boundary, truthful meal description and place identity.");
    }
}
