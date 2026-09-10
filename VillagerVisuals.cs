using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private sealed class PersonView
    {
        public Node3D Body = new(), Rig = new(), Torso = new(), Head = new(), Arm = new(), LeftArm = new(),
            LeftLeg = new(), RightLeg = new(), Carry = new(), Marker = null!, Axe = new(), AxeEdge = new(), Hammer = new(), WorkBoard = new(), Spade = new(), Peel = new(), Saw = new(), Sickle = new(), SeedPouch = new(), RestStool = new(), Bow = new();
        public Resource Cargo;
        public int Count = -1;
        public float? PickupStarted;
        public float CargoObservedAt=-1;
    }

    private PersonView MakeVillager(int id)
    {
        var v = new PersonView();
        v.Body.AddChild(v.Rig); v.Rig.AddChild(v.Torso); v.Torso.Position = new(0, 0.43f, 0);
        var shirts = new[] { "bf714d", "5c8390", "a19358", "7c6c92", "7a9161", "bd8a68", "af6971", "588c83" };
        Cylinder(v.Torso, new(0, 0.21f, 0), 0.26f, 0.48f, new(shirts[id % shirts.Length]), 0.21f);
        v.Torso.AddChild(v.Head); v.Head.Position = new(0, 0.58f, 0);
        Mesh(v.Head, new SphereMesh { Radius = 0.20f, Height = 0.4f, RadialSegments = 8, Rings = 4 }, Vector3.Zero, new("e7bd8e"));
        Cylinder(v.Head, new(0, 0.19f, 0), 0.30f, 0.07f, new("dbc28c"));
        Cylinder(v.Head, new(0, 0.26f, 0), 0.19f, 0.15f, new("dbc28c"), 0.15f);
        Cylinder(v.Head,new(0,.22f,0),.188f,.055f,new(shirts[id%shirts.Length]),.18f);
        v.Body.AddChild(v.RestStool); v.RestStool.Visible=false;
        Cylinder(v.RestStool,new(0,.23f,0),.22f,.06f,_wood);
        foreach(float x in new[]{-.13f,.13f}) foreach(float z in new[]{-.12f,.12f})
            Box(v.RestStool,new(x,.10f,z),new(.045f,.20f,.045f),_wood);
        foreach (var (leg, x) in new[] { (v.LeftLeg, -0.13f), (v.RightLeg, 0.13f) })
        {
            v.Rig.AddChild(leg); leg.Position = new(x, 0.4f, 0);
            Box(leg, new(0, -0.18f, 0), new(0.16f, 0.36f, 0.18f), new("494d45"));
            Box(leg, new(0, -0.35f, -0.05f), new(0.18f, 0.10f, 0.27f), new("614c3b"));
        }
        foreach (var (arm, x) in new[] { (v.LeftArm, -0.28f), (v.Arm, 0.28f) })
        {
            v.Torso.AddChild(arm); arm.Position = new(x, 0.36f, 0);
            Box(arm, new(0, -0.17f, 0), new(0.13f, 0.4f, 0.14f), new("e7bd8e"));
        }
        foreach (var tool in new[] { v.Axe, v.Hammer, v.Spade, v.Peel })
        {
            v.Arm.AddChild(tool); tool.Position = new(0, -0.32f, 0); tool.Visible = false;
            Box(tool, new(0, 0.1f, tool==v.Axe?-.30f:tool==v.Hammer?-.23f:-.15f), new(0.055f, 0.055f, tool==v.Axe?.85f:tool==v.Hammer?.70f:.55f), _wood);
        }
        Box(v.Axe, new(-0.06f, 0.1f, -0.69f), new(0.25f, 0.10f, 0.18f), new("9ca8a4"));
        v.Axe.AddChild(v.AxeEdge); v.AxeEdge.Position=new(-.06f,.1f,-.78f);
        Box(v.Hammer, new(0, 0.1f, -0.55f), new(0.24f, 0.14f, 0.14f), new("737d7b"));
        v.Body.AddChild(v.WorkBoard); v.WorkBoard.Visible=false;
        Box(v.WorkBoard,new(0,.48f,-.6f),new(.78f,.14f,.22f),new("c8a36f"));
        foreach(float x in new[]{-.28f,.28f}) Box(v.WorkBoard,new(x,.22f,-.6f),new(.08f,.44f,.17f),_wood);
        Box(v.Spade, new(0, 0.1f, -0.43f), new(0.21f, 0.05f, 0.25f), new("89938a"));
        Box(v.Peel, new(0, 0.1f, -0.48f), new(0.32f, 0.04f, 0.34f), new("cba36d"));
        v.Arm.AddChild(v.Saw); v.Saw.Position = new(0, -0.32f, 0);
        Box(v.Saw, new(0, 0, -0.3f), new(0.04f, 0.18f, 0.6f), new("a3aaa4"));
        Box(v.Saw, new(0, 0, 0.02f), new(0.09f, 0.22f, 0.14f), _wood);
        v.Arm.AddChild(v.Sickle); v.Sickle.Position=new(0,-.32f,0);
        Box(v.Sickle,new(0,0,-.15f),new(.06f,.06f,.35f),_wood);
        for(int i=0;i<5;i++)
        {
            float a=i*.4f;
            var blade=Box(v.Sickle,new(.16f-MathF.Cos(a)*.16f,0,-.32f-MathF.Sin(a)*.16f),new(.08f,.025f,.10f),new("b4bcb5"));
            blade.Rotation=new(0,-a,0);
        }
        v.LeftArm.AddChild(v.Bow); v.Bow.Position=new(0,-.32f,0);
        for(int i=0;i<5;i++) { float a=-1+i*.5f; TimberBeam(v.Bow,new(0,MathF.Sin(a)*.45f,-.15f-MathF.Cos(a)*.18f),new(0,MathF.Sin(a+.5f)*.45f,-.15f-MathF.Cos(a+.5f)*.18f),.035f,_wood); }
        TimberBeam(v.Bow,new(0,-.38f,-.24f),new(0,.45f,-.16f),.009f,_cream);
        v.Torso.AddChild(v.SeedPouch);
        Box(v.SeedPouch,new(-.27f,.02f,-.19f),new(.23f,.25f,.18f),new("c5a16d"));
        v.Torso.AddChild(v.Carry); v.Carry.Position = new(0, 0.12f, -0.43f);
        v.Marker = Cylinder(v.Body, new(0, 0.02f, 0), 0.36f, 0.02f, new("efd49c"));
        return v;
    }

    private void RefreshCargo(PersonView view, Villager worker)
    {
        if(view.Count==0 && worker.Carried>0 && _world.Food.Time-view.CargoObservedAt<=.4f) view.PickupStarted=_world.Food.Time;
        if(worker.Carried==0) view.PickupStarted=null;
        view.CargoObservedAt=_world.Food.Time;
        view.Carry.Visible = worker.Carried > 0;
        if (view.Count == worker.Carried && view.Cargo == worker.Cargo) return;
        view.Count = worker.Carried; view.Cargo = worker.Cargo; Clear(view.Carry);
        if (worker.Carried == 0) return;
        if(worker.Cargo==Resource.Stone)
        {
            for(int i=0;i<worker.Carried;i++) StonePiece(view.Carry,new((i-.5f)*.26f,.04f,0),.18f);
            return;
        }
        if (worker.Cargo == Resource.Planks)
        {
            for (int i = 0; i < worker.Carried; i++) Plank(view.Carry, new(0, i * 0.14f, 0));
            return;
        }
        if (worker.Cargo == Resource.Logs)
        {
            for (int i = 0; i < worker.Carried; i++) Log(view.Carry, new(0, i * 0.22f, 0), 0.72f);
            return;
        }
        // Shallow baskets frame their contents; each mound/sheaf/loaf represents one carried unit.
        Box(view.Carry, new(0, -0.05f, 0), new(0.67f, 0.08f, 0.40f), new("b18a53"));
        foreach (float z in new[] { -0.21f, 0.21f }) Box(view.Carry, new(0, 0.02f, z), new(0.71f, 0.16f, 0.04f), new("c5a16d"));
        foreach (float x in new[] { -0.34f, 0.34f }) Box(view.Carry, new(x, 0.02f, 0), new(0.04f, 0.16f, 0.42f), new("c5a16d"));
        for (int i = 0; i < worker.Carried; i++)
        {
            float x = worker.Carried == 1 ? 0 : -0.17f + i % 2 * 0.34f;
            float row = worker.Carried > 2 ? -.10f + i / 2 * .20f : 0;
            if (worker.Cargo == Resource.Berries)
                for (int b = 0; b < 5; b++) Mesh(view.Carry, new SphereMesh { Radius = 0.075f, Height = 0.15f, RadialSegments = 6, Rings = 3 },
                    new(x + (b % 2 - 0.5f) * 0.12f, 0.10f + b / 4 * 0.10f, (b / 2 % 2 - 0.5f) * 0.14f), new("a74268"));
            else if (worker.Cargo == Resource.Vegetables) MakeSquash(view.Carry, new(x,.12f,0), .13f);
            else if (worker.Cargo == Resource.Game) GameParcel(view.Carry,new(x,.10f,row));
            else if (worker.Cargo == Resource.Fish) MakeFish(view.Carry,new(x,.12f,row));
            else if (worker.Cargo == Resource.Grain)
                for (int s = 0; s < 4; s++)
                {
                    var stalk = Cylinder(view.Carry, new(x + (s - 1.5f) * 0.045f, 0.18f, row), 0.018f, 0.4f, new("d5b55e"));
                    stalk.RotationDegrees = new(0, 0, (s - 1.5f) * 12);
                    Mesh(view.Carry, new SphereMesh { Radius = 0.048f, Height = 0.15f, RadialSegments = 5, Rings = 3 }, new(x + (s - 1.5f) * 0.075f, 0.39f, row), new("e4c87a"));
                }
            else
            {
                var loaf = Mesh(view.Carry, new SphereMesh { Radius = 0.14f, Height = 0.23f, RadialSegments = 8, Rings = 4 }, new(x, 0.11f + i / 2 * .20f, 0), new("cb8844"));
                loaf.Scale = new(1, 1, 1.45f);
                for (int cut = 0; cut < 3; cut++) Box(view.Carry, new(x, 0.22f, -0.09f + cut * 0.09f), new(0.13f, 0.015f, 0.025f), new("f0ce8c"));
            }
        }
    }

    private void AnimateVillager(PersonView view, Villager v)
    {
        RefreshCargo(view, v); view.Marker.Visible = v.Id == _selectedPerson;
        view.Carry.Position=new(0,.12f,-.43f);
        view.Bow.Visible=false; view.WorkBoard.Visible=false;
        view.RestStool.Visible=false;
        bool walking = v.Route.Count > 0;
        float cycle = _clock * 8 + v.Id * 1.7f, swing = MathF.Sin(cycle);
        view.Rig.Position = new(0, walking ? MathF.Abs(swing) * 0.035f : 0, 0);
        view.Torso.Rotation = Vector3.Zero; view.Head.Rotation = Vector3.Zero;
        view.LeftLeg.Rotation = new(walking ? swing * 0.55f : 0, 0, 0);
        view.RightLeg.Rotation = -view.LeftLeg.Rotation;
        view.Arm.Rotation = new(walking ? -swing * 0.35f : 0, 0, 0);
        view.LeftArm.Rotation = -view.Arm.Rotation;
        view.Axe.Visible = view.Hammer.Visible = view.Spade.Visible = view.Peel.Visible = view.Saw.Visible = view.Sickle.Visible = view.SeedPouch.Visible = false;
        if(v.Task==Work.Aboard)
        {
            bool rowing=_world.PassengerBoat(v)?.Route.Count>0;
            view.Rig.Position=new(0,-.25f,0);
            view.LeftLeg.Rotation=new(Mathf.Pi/2,0,-.08f); view.RightLeg.Rotation=new(Mathf.Pi/2,0,.08f);
            view.Arm.Rotation=new(.9f+(rowing?MathF.Sin(_clock*4)*.25f:0),0,-.15f);
            view.LeftArm.Rotation=new(view.Arm.Rotation.X,0,.15f);
            return;
        }
        if (v.Carried > 0 && v.Task!=Work.EatingMeal)
        {
            view.Arm.Rotation = view.LeftArm.Rotation = new(1.05f, 0, 0);
            view.Torso.Rotation = new(-0.08f, 0, 0); AnimateCargoHandoff(view,v); return;
        }
        if(AnimatePickupReach(view,v)) return;
        if (walking) return;
        Cell? facing = v.TreeId is int tree ? _world.Trees.FirstOrDefault(t => t.Id == tree)?.Cell :
            v.BushId is int bush ? _world.Bushes.FirstOrDefault(b => b.Id == bush)?.Cell :
            (v.SiteId ?? v.WorkplaceId ?? v.LeisureSiteId ?? (v.Task==Work.Resting ? v.HomeId : null)) is int site ? _world.Cottages.FirstOrDefault(c => c.Id == site)?.Cell : null;
        if (facing is Cell cell)
        {
            var direction = new Vector3(cell.X, 0, cell.Z) - view.Body.Position;
            if (direction.LengthSquared() > 0.01f) view.Body.Rotation = new(0, MathF.Atan2(-direction.X, -direction.Z), 0);
        }
        switch (v.Task)
        {
            case Work.Hunting:
                view.Bow.Visible=true; view.LeftArm.Rotation=new(1.5f,0,-.15f); view.Arm.Rotation=new(1.1f+MathF.Sin(v.Timer*1.5f)*.12f,.4f,.4f); view.Head.Rotation=new(.05f,-.2f,0); break;
            case Work.Quarrying:
                var deposit=_world.Map.StoneDeposits.FirstOrDefault(d=>d.Id==v.DepositId);
                if(deposit!=null) FaceVisit(view,OnGround(deposit.Cell.X,deposit.Cell.Z)-view.Body.Position);
                view.Hammer.Visible=true; AnimateAxeStroke(view,v.Timer); break;
            case Work.Sawing:
                view.Saw.Visible = true; view.Arm.Rotation = new(0.8f + swing * 0.25f, 0, 0);
                view.Torso.Rotation = new(-0.18f, 0, 0); view.LeftArm.Rotation = new(0.9f, 0, 0); break;
            case Work.Chopping:
                bool felling = _world.Trees.Any(t => t.Id == v.TreeId && !t.Felled);
                view.Axe.Visible = felling;
                if(felling) AnimateAxeStroke(view,v.Timer);
                else { view.Arm.Rotation = new(.65f+swing*.25f,0,0); view.Torso.Rotation=new(-.35f,0,0); }
                break;
            case Work.Building: case Work.Demolishing:
                if(!HasHammerWork(v)) { view.Torso.Rotation=new(-.26f,0,0); view.Arm.Rotation=view.LeftArm.Rotation=new(.7f,0,0); break; }
                view.Hammer.Visible = view.WorkBoard.Visible = true;
                float beat=v.Timer%1;
                float hammer=beat<.5f ? Mathf.SmoothStep(.03f,1.5f,beat/.5f) : beat<.7f ? Mathf.SmoothStep(1.5f,.03f,(beat-.5f)/.2f) : .03f;
                view.Arm.Rotation=new(hammer,0,0); view.LeftArm.Rotation=new(.55f,0,.1f); view.Head.Rotation=new(.13f,0,0); break;
            case Work.Planting: case Work.Harvesting:
                AnimateFieldWork(view,v); break;
            case Work.ClearingStump: case Work.PlantingTree:
                view.Spade.Visible = true; view.Torso.Rotation = new(-0.4f - swing * 0.12f, 0, 0);
                view.Arm.Rotation = new(0.6f + swing * 0.35f, 0, 0); view.LeftArm.Rotation = new(0.5f, 0, 0); break;
            case Work.Foraging:
                view.Torso.Rotation = new(-0.18f, swing * 0.12f, 0);
                view.Arm.Rotation = new(1.05f + swing * 0.35f, 0, 0);
                view.LeftArm.Rotation = new(1.05f - swing * 0.35f, 0, 0); break;
            case Work.Baking:
                view.Peel.Visible = true; view.Arm.Rotation = new(0.8f + MathF.Sin(cycle * 0.5f) * 0.22f, 0, 0);
                view.Torso.Rotation = new(-0.1f - MathF.Sin(cycle * 0.5f) * 0.08f, 0, 0); break;
            case Work.Leisure:
                AnimateSquareVisit(view,v); break;
            case Work.EatingMeal:
                view.RestStool.Visible=true; view.Rig.Position=new(0,-.20f,0);
                view.LeftLeg.Rotation=new(Mathf.Pi/2,0,-.08f); view.RightLeg.Rotation=new(Mathf.Pi/2,0,.08f);
                view.Arm.Rotation=new(1.5f+MathF.Sin(v.Timer*2)*.25f,0,-.1f); view.LeftArm.Rotation=new(.8f,0,.1f);
                view.Head.Rotation=new(.12f,0,0); break;
            case Work.Resting:
                AnimateHomeRest(view,v); break;
            case Work.Supper:
                view.Arm.Rotation = new(2.6f, 0, swing * 0.3f); view.LeftArm.Rotation = new(1.3f, 0, -swing * 0.2f); break;
            case Work.Waiting:
                float idle = (_clock + v.Id * 2.3f) % 14;
                view.Head.Rotation = new(0, MathF.Sin(_clock * 0.6f + v.Id) * 0.25f, 0);
                view.Torso.Rotation = new(0, 0, MathF.Sin(_clock * 0.8f + v.Id) * 0.025f);
                if (idle < 2) view.Arm.Rotation = new(2.7f, 0, -0.25f); // Adjust the brim of the hat.
                int happiness = _world.ReadHappiness(v).Score;
                if (happiness < 40) { view.Head.Rotation += new Vector3(.18f,0,0); view.Torso.Rotation += new Vector3(.08f,0,0); }
                else if (happiness >= 85 && idle < 3) view.Arm.Rotation = new(2.65f,0,MathF.Sin(_clock*3+v.Id)*.24f);
                break;
        }
    }
}
