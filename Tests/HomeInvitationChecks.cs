using Inlanders.Simulation;
static class HomeInvitationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/home-invitation");
        foreach(bool relaxed in new[]{false,true})
        {
            var w=World.NewPlayerFounded(relaxed);
            foreach(var at in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})Check(w.Place(at,0,BuildingKind.Cottage)!=null,"Founders homes rejected");
            for(int i=0;i<2000 && w.Housed<8;i++)w.Tick(.1f);Check(w.Housed==8 && w.Population==8,"Original neighbors not housed or forced growth");
            var near=w.Place(new(4,3),0,BuildingKind.Cottage)!;var chosen=w.Place(new(1,-5),0,BuildingKind.Cottage)!;
            string before=w.SaveJson();if(!relaxed)Check(!w.InviteToHome(chosen.Id) && before==w.SaveJson(),"Unfinished invitation changed village");
            for(int i=0;i<2000 && !chosen.Complete;i++)w.Tick(.1f);
            Check(near.Complete && chosen.Complete && w.Population==8,"Available homes forced arrival");
            w.SaveFile($"artifacts/home-invitation/{relaxed}-before.json");before=w.SaveJson();
            Check(w.HomeInvitationProblem(chosen.Id)==null && w.SaveJson()==before,"Invitation inspection mutates village");
            Check(w.InviteToHome(chosen.Id) && w.Population==10 && w.People.Skip(8).All(p=>p.HomeId==chosen.Id),"Arrivals did not choose selected home");
            Check(w.People.Take(8).All(p=>p.HomeId!=chosen.Id),"Existing neighbors displaced");
            before=w.SaveJson();Check(!w.InviteToHome(chosen.Id) && before==w.SaveJson(),"Full home accepted duplicate invitation");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<2400;i++){w.Tick(.1f);copy.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.SaveJson()==copy.SaveJson() && w.People.Skip(8).All(p=>p.RestVisits>0 && w.Founding!.Settled.Contains(p.Id)),"New household did not eat/rest or continuation differed");
            w.SaveFile($"artifacts/home-invitation/{relaxed}-settled.json");
        }
        Console.WriteLine("PASS: optional home-specific invitations, real beds/arrivals/meals/rest, rejected repeat, no displaced residents and exact continuation in both modes.");
    }
}
