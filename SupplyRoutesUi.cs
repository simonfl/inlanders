using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool _showSupplyRoutes;
    private Button _supplyToggle=null!;
    private Label _supplySummary=null!;
    private Label _supplyHelp=null!;
    private VBoxContainer _supplyLinks=null!;
    private MeshInstance3D? _supplyMesh;
    private float _nextSupplyRefresh;
    private World? _supplyWorld;
    private string _supplyGeometryKey="";
    private void MakeSupplyControls(VBoxContainer column)
    {
        column.AddChild(Text("SUPPLY ROUTES",12));
        _supplyToggle=Button("Show supply routes",()=> { _showSupplyRoutes=!_showSupplyRoutes; _nextSupplyRefresh=0; _drawerPages[4].ScrollVertical=(int)_supplyToggle.Position.Y; }); column.AddChild(_supplyToggle);
        _supplySummary=Text("",14,true); column.AddChild(_supplySummary);
        _supplyLinks=new(); column.AddChild(_supplyLinks);
        _supplyHelp=Text("",14,true); column.AddChild(_supplyHelp);
    }
    private void RenderSupplyRoutes()
    {
        if(_supplyWorld!=_world) { _supplyWorld=_world; _showSupplyRoutes=false; _nextSupplyRefresh=0; _supplyGeometryKey=""; }
        _supplyToggle.Text=_showSupplyRoutes?"Hide supply routes":"Show supply routes";
        bool visible=_showSupplyRoutes && !_watching && !_atMainMenu;
        if(_supplyMesh!=null) _supplyMesh.Visible=visible;
        _supplyLinks.Visible=_showSupplyRoutes;
        _supplyHelp.Visible=_showSupplyRoutes;
        _supplyHelp.Text="Loggers deliver to nearby log stores without haulers. Builders and sawyers collect there. Haulers redistribute logs toward targets.\n\nSawyers can deliver to plank piles; builders collect locally. "+(_world.HasWorkplaceFood?
            "All food producers store their edible output at their workplace first. Residents collect meals there; haulers carry surplus to pantries. Hut location affects the return journey from berry patches.":
            "Edible food goes to nearby pantry storage. Forager huts provide worker slots; berries go from the patch to a pantry, not through the hut.");
        if(!visible) { _supplySummary.Text="See current trips, then select a worker to inspect their load and task. Routes hide in Watch mode."; return; }
        if(_uiTime<_nextSupplyRefresh) return;
        _nextSupplyRefresh=_uiTime+.25f;
        var routes=_world.ReadSupplyRoutes();
        _supplySummary.Text=$"{routes.Length} active supply trips\nGold: carrying goods · blue: going to work or collect\nArrows point to the destination. Remaining trip distance, not a full-cycle estimate.";
        var buttons=_supplyLinks.GetChildren().OfType<Button>().ToList();
        while(buttons.Count<routes.Length) { var b=Button("",()=>{}); b.AutowrapMode=TextServer.AutowrapMode.WordSmart; b.AddThemeFontSizeOverride("font_size",14); _supplyLinks.AddChild(b); buttons.Add(b); }
        // Reuse controls without accumulating callbacks as jobs change.
        for(int i=0;i<buttons.Count;i++) { buttons[i].Visible=i<routes.Length; if(i>=routes.Length) continue;
            var r=routes[i]; buttons[i].Text=$"{r.Worker} → {r.Destination}\n{(r.Amount>0?$"{r.Amount} {r.Cargo}":"Hands free")} · {r.Distance:0.0} tiles left";
            buttons[i].SetMeta("worker",r.WorkerId); buttons[i].TooltipText=r.Purpose;
            if(!buttons[i].HasMeta("linked")) { var b=buttons[i]; b.Pressed+=()=>SelectPerson((int)b.GetMeta("worker")); b.SetMeta("linked",true); }
        }
        if(_supplyMesh==null) { _supplyMesh=new() { CastShadow=GeometryInstance3D.ShadowCastingSetting.Off }; AddChild(_supplyMesh); }
        string key=string.Join(";",routes.Select(r=>$"{r.WorkerId}:{r.Start}:{r.Amount}:{r.OnWater}:{string.Join(',',r.Steps)}"));
        if(key==_supplyGeometryKey && _supplyMesh.Mesh!=null) return;
        _supplyGeometryKey=key;
        var mesh=new ImmediateMesh();
        if(routes.Length>0)
        {
            mesh.SurfaceBegin(Godot.Mesh.PrimitiveType.Triangles,new StandardMaterial3D { ShadingMode=BaseMaterial3D.ShadingModeEnum.Unshaded,VertexColorUseAsAlbedo=true,CullMode=BaseMaterial3D.CullModeEnum.Disabled });
            void Segment(Vector3 a,Vector3 b)
            {
                var side=(b-a).Cross(Vector3.Up).Normalized()*.035f;
                foreach(var point in new[]{a-side,a+side,b+side,a-side,b+side,b-side}) mesh.SurfaceAddVertex(point);
            }
            foreach(var r in routes)
            {
                Vector3 Point(float x,float z)=>r.OnWater?new(x,.22f,z):OnGround(x,z,.12f);
                var point=Point(r.Start.X,r.Start.Y);
                mesh.SurfaceSetColor(r.Amount>0?new("ffd27b"):new("82c9eb"));
                foreach(var cell in r.Steps) { var next=Point(cell.X,cell.Z); Segment(point,next); point=next; }
                var before=r.Steps.Length>1?r.Steps[^2].Point:r.Start;
                var direction=(point-Point(before.X,before.Y)).Normalized();
                foreach(float angle in new[]{-.6f,.6f}) Segment(point,point-direction.Rotated(Vector3.Up,angle)*.35f);
            }
            mesh.SurfaceEnd();
        }
        var previous=_supplyMesh.Mesh; _supplyMesh.Mesh=mesh; previous?.Dispose();
    }
}
