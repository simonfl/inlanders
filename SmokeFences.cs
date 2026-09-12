using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckFences()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Hover(Cell c)
        {
            Input.ParseInputEvent(new InputEventMouseMotion { Position=_camera.UnprojectPosition(OnGround(c.X,c.Z)),GlobalPosition=_camera.UnprojectPosition(OnGround(c.X,c.Z)) });
            await Frames();Check(_hover==c,"Hover missed fence tile");
        }
        int Mask(Node3D body)=>(int)body.GetMeta("connections");
        _goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        var w=World.NewCreative();foreach(var t in w.Trees.ToArray())w.SetClearing(t.Cell,true);
        AdoptWorld(w);_paused=true;_noticeUntil=0;CloseManagementUi();
        GetWindow().Size=new(1440,900);_focus=new(3,0,-3);_camera.Size=15;_angle=.72f;UpdateCamera();
        var center=new Cell(3,-3);
        // Every neighbor combination, including straight, corner, T and cross.
        for(int mask=0;mask<16;mask++)
        {
            foreach(var d in w.Decorations.ToArray())w.RemoveDecoration(d.Cell);
            for(int i=0;i<4;i++)if((mask&(1<<i))!=0)
            {
                var d=FenceDirections[i];Check(w.PlaceDecoration(new(center.X+d.X,center.Z+d.Z),DecorationKind.Fence,i%2==0),"Neighbor placement failed");
            }
            _decorationChoice.Select((int)DecorationKind.Fence);BeginDecorating(false);await Hover(center);
            string before=w.SaveJson();RefreshGhost();await Frames();
            Check(_ghostValid && Mask(_ghostModel)==mask,"Preview connections wrong");
            Check(w.SaveJson()==before,"Preview changed simulation");
            Check(_fenceBodies.Values.All(b=>!b.Visible),"Old neighbor models remain behind preview");
            await Click(_pointerPosition);await Press(Key.Escape);await Frames();
            Check(w.Decorations.Any(d=>d.Cell==center) && Mask(_fenceBodies[center])==mask,"Clicked fence disagrees with preview");
            Check(_fenceBodies.Values.All(b=>b.Visible),"Cancel left fences hidden");
            string saved=w.SaveJson();w=World.LoadJson(saved);Check(w.SaveJson()==saved,"Fence save differs");AdoptWorld(w);_paused=true;CloseManagementUi();
            _focus=new(3,0,-3);_camera.Size=15;_angle=.72f;UpdateCamera();await Frames();
            Check(Mask(_fenceBodies[center])==mask,"Reload changed joins");
            BeginDecorating(true);await Hover(center);Check(_ghostValid,"Remove preview invalid");
            Check(_fenceBodies.Values.All(b=>!b.Visible),"Removal preview did not replace affected models");
            Check(_ghostModel.GetChildren().OfType<Node3D>().All(b=>Mask(b)==0),"Removal left neighbor joined to removed center");
            await Click(_pointerPosition);await Press(Key.Escape);await Frames();
            Check(!w.Decorations.Any(d=>d.Cell==center) && _fenceBodies.Values.All(b=>b.Visible && Mask(b)==0),"Removal did not update neighbor runs");
        }
        foreach(var d in w.Decorations.ToArray())w.RemoveDecoration(d.Cell);
        _decorationChoice.Select((int)DecorationKind.Fence);BeginDecorating(false);await Hover(center);await Press(Key.R);
        bool rotated=_rotation%2!=0;await Click(_pointerPosition);await Press(Key.Escape);await Frames();
        Check(w.Decorations.Single().Rotated==rotated,"Isolated R orientation lost");
        Check(!w.PlaceDecoration(w.YardAccess,DecorationKind.Fence),"Fence covered yard entrance");
        Check(!w.PlaceDecoration(World.At(w.People[0]),DecorationKind.Fence),"Fence covered worker");
        foreach(var d in w.Decorations.ToArray())w.RemoveDecoration(d.Cell);
        var home=w.Place(new(4,-3),0,BuildingKind.Cottage)??throw new Exception("Garden home failed");
        Check(!w.PlaceDecoration(home.Entrance,DecorationKind.Fence),"Fence covered home entrance");
        // Two sides and a front opening: the arrangement keeps the cottage reachable.
        foreach(var c in Enumerable.Range(2,5).Select(x=>new Cell(x,-6)).Concat(Enumerable.Range(-5,6).Select(z=>new Cell(2,z))).Concat(Enumerable.Range(-5,6).Select(z=>new Cell(6,z))))
            Check(w.PlaceDecoration(c,DecorationKind.Fence),"Garden boundary refused at "+c);
        foreach(var c in new[]{new Cell(3,0),new Cell(5,0)})Check(w.PlaceDecoration(c,DecorationKind.Fence),"Front boundary refused");
        Check(!w.PlaceDecoration(new(4,0),DecorationKind.Fence),"Fence sealed cottage access");
        w.PlaceDecoration(new(3,-5),DecorationKind.Flowers);w.PlaceDecoration(new(5,-5),DecorationKind.Shrub);
        await Frames();
        foreach(int width in new[]{1440,960})
        {
            GetWindow().Size=new(width,width==960?640:900);_focus=new(3,0,-3);_camera.Size=15;UpdateCamera();await Frames();
            await Capture($"artifacts/fences-garden-{width}.png");
        }
        // Raised terrain uses upright posts and rails sampling identical shared boundaries.
        w=World.NewCreative(true);foreach(var t in w.Trees.ToArray())w.SetClearing(t.Cell,true);
        AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;
        var slope=w.Map.Land.First(c=>w.DecorationProblem(c,DecorationKind.Fence)==null && Math.Abs(Height(c.X-.5f,c.Z)-Height(c.X+.5f,c.Z))>.05f && w.DecorationProblem(new(c.X+1,c.Z),DecorationKind.Fence)==null);
        Check(w.PlaceDecoration(slope,DecorationKind.Fence) && w.PlaceDecoration(new(slope.X+1,slope.Z),DecorationKind.Fence),"Slope fences failed");
        _focus=OnGround(slope.X,slope.Z);_camera.Size=10;UpdateCamera();await Frames();
        Check(_fenceBodies.Values.All(b=>b.Basis.IsEqualApprox(Basis.Identity)),"Slope posts tilt");
        await Capture("artifacts/fences-slope.png");w.Validate();
        GD.Print("PASS: all 16 fence joins, real placement/removal, isolated rotation, preview isolation, cancellation, reload, protected access, cottage garden and raised terrain.");
    }
}
