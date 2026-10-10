using System;
namespace Inlanders.Simulation;
public sealed record SessionView(float X,float Y,float Z,float Zoom,float Angle,int Person=-1,int Site=-1,bool Addition=false,Cell? Commons=null);
public sealed partial class World
{
    public SessionView? SessionView {get;set;}
    private void ValidateSessionView()
    {
        if(SessionView is not {} v)return;
        if(!float.IsFinite(v.X) || !float.IsFinite(v.Y) || !float.IsFinite(v.Z) || !float.IsFinite(v.Zoom) || !float.IsFinite(v.Angle) || v.Zoom<1 || v.Zoom>MaximumViewZoom || v.Person < -1 || v.Site < -1)
            throw new InvalidOperationException("Invalid session view");
    }
}
