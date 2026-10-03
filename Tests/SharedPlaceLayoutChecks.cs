using Inlanders.Simulation;
static class SharedPlaceLayoutChecks
{
    public static void Run()
    {
        foreach(bool relaxed in new[]{false,true})foreach(int seats in new[]{2,6})foreach(int rotation in Enumerable.Range(0,4))
        {
            void Check(bool ok,string why){if(!ok)throw new Exception(why);}
            var w=World.NewWorkingClearing(relaxed);var at=new Cell(0,1);string before=w.SaveJson();
            Check(!w.AddCommons(at,seats,SharedPlaceLayout.Line,4) && w.SaveJson()==before,"Invalid direction changed world");
            var proposed=w.CommonsPlaces(at,seats,null,SharedPlaceLayout.Line,rotation);
            Check(proposed.Length==seats && w.AddCommons(at,seats,SharedPlaceLayout.Line,rotation),"Line proposal failed");
            Check(w.Commons!.Places.SequenceEqual(proposed),"Built seating differs from proposal");
            for(int i=0;i<1200;i++){w.Tick(.1f);if(i%100==0)w.Validate();}
            Check(w.Commons.FirstDiner!=null,"Line had no actual meal");var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}Check(w.SaveJson()==copy.SaveJson(),"Line save continuation");
            Check(w.MoveCommons(at,at,seats,SharedPlaceLayout.Gathered,rotation),"Cannot regroup seating");w.Validate();
            Console.WriteLine($"PASS shared line {relaxed}/{seats}/{rotation}: matching actual seats, real meals, exact save and regroup");
        }
    }
}
