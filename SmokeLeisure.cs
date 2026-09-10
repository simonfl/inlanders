using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckLeisureUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            foreach(var kind in new[]{BuildingKind.Square,BuildingKind.SeatingGarden})
            {
            AdoptWorld(new World(40)); _paused=true;
            var square=_world.Place(new(3,0),false,kind)!;
            for(int i=0;i<4000 && !_world.People.Any(p=>p.Task==Work.Leisure);i++) { _world.Tick(.1f); _world.Validate(); }
            if(!_world.People.Any(p=>p.Task==Work.Leisure)) throw new Exception("No rendered square visit");
            SelectBuilding(square.Id); _focus=new(3,0,1); _camera.Size=12; UpdateCamera();
            foreach(var width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); await Frames();
                if(kind==BuildingKind.SeatingGarden && (_cottages[square.Id].Body.Position.X!=square.Cell.X || _cottages[square.Id].Body.Position.Z!=square.Cell.Z)) throw new Exception("Garden model is not centered on its one tile");
                if(!_siteInfo.Text.Contains($"/{Buildings.Get(kind).RecreationSlots} visitors")) throw new Exception("Missing square visitor feedback");
                string saved=_world.SaveJson(); await Frames();
                if(saved!=_world.SaveJson()) throw new Exception("Paused leisure advanced");
                await Capture($"artifacts/recreation-{kind}-{width}.png");
            }
            var person=_world.People.First(p=>p.Task==Work.Leisure);
            if(_people[person.Id].Axe.Visible || _people[person.Id].Carry.Visible) throw new Exception("Leisure has work props");
            AdoptWorld(World.LoadJson(_world.SaveJson())); _paused=true; await Frames();
            if(!_world.People.Any(p=>p.Task==Work.Leisure)) throw new Exception("Visit not restored");
            if(kind==BuildingKind.SeatingGarden && !_people[_world.People.First(p=>p.Task==Work.Leisure).Id].RestStool.Visible) throw new Exception("Garden visitor is not seated");
            }
            GD.Print("PASS: square/garden visitor inspectors, social/seated poses, pause and saved visits at wide/narrow sizes.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
