using System;
using Inlanders.Simulation;

public static class CameraViewChecks
{
    static void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
    public static void Run()
    {
        var w=World.NewLargeMap(); string before=w.SaveJson();
        var view=new CameraView("East orchard",8,5,18,1.2f);
        Check(w.SaveCameraView(0,view) && w.SaveCameraView(2,view with { Name="North",Z=-8 }),"Valid view rejected");
        string saved=w.SaveJson(); var clone=World.LoadJson(saved);
        Check(clone.SaveJson()==saved && clone.CameraViews[0]==view,"Camera view not saved exactly");
        Check(!w.SaveCameraView(3,view) && !w.SaveCameraView(-1,view),"Invalid slot accepted");
        foreach(var bad in new[]{view with { Name="" },view with { X=float.NaN },view with { X=999 },
            view with { Zoom=1 },view with { Zoom=999 },view with { Angle=float.PositiveInfinity },view with { Angle=-1 }})
            Check(!w.SaveCameraView(1,bad),"Invalid camera view accepted");
        Check(w.ClearCameraView(0) && w.ClearCameraView(2) && !w.ClearCameraView(2),"Clear view failed");
        Check(w.SaveJson()==before,"Camera views changed gameplay state");
        var original=new World();
        Check(original.CameraViews[0]==null,"View leaked into another settlement");
        Console.WriteLine("PASS: named camera slots, exact saves, map bounds, zoom/angle validation, clearing and gameplay isolation.");
    }
}
