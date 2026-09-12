using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckDecorationClusterRendering(World dense)
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);}
        int Vertices(Node root)=>root.GetChildren().Sum(n=>(n is MeshInstance3D mesh?mesh.Mesh.GetFaces().Length:0)+Vertices(n));
        int Meshes(Node root)=>root.GetChildren().Sum(n=>(n is MeshInstance3D?1:0)+Meshes(n));
        var motion=_foliageMotion;_foliageMotion=false;ApplyAtmosphere();
        AdoptWorld(World.LoadJson(dense.SaveJson()));_paused=true;CloseManagementUi();_noticeUntil=0;
        _focus=new(3,0,3);_camera.Size=32;_angle=.72f;UpdateCamera();_showWorldLabels=false;ApplyWorldLabels();UpdateHud();
        _batchDecorationClusters=false;_decorationRevision=-1;RenderDecorations();await Frames();
        string saved=_world.SaveJson();int vertices=Vertices(_decorationView),meshes=Meshes(_decorationView);
        var before=GetViewport().GetTexture().GetImage();before.SavePng("artifacts/large-village/decoration-before.png");
        _batchDecorationClusters=true;_decorationRevision=-1;RenderDecorations();await Frames();
        var after=GetViewport().GetTexture().GetImage();after.SavePng("artifacts/large-village/decoration-after.png");
        var a=before.GetData();var b=after.GetData();
        if(a.Length!=b.Length || Vertices(_decorationView)!=vertices || Meshes(_decorationView)>=meshes || saved!=_world.SaveJson())throw new Exception("Decoration batching changed geometry/state or failed to reduce meshes");
        int changed=0;for(int i=0;i<a.Length;i++)if(Math.Abs(a[i]-b[i])>4)changed++;
        double fraction=changed/(double)a.Length;if(fraction>.001)throw new Exception($"Decoration batching changed visible pixels: {fraction:P3}");
        GD.Print($"DECORATION BATCH: meshes {meshes} to {Meshes(_decorationView)}, identical {vertices/3} triangles; pixels/channels over 4/255 difference {fraction:P4}; identical saved simulation.");
        _foliageMotion=motion;ApplyAtmosphere();
    }
}
