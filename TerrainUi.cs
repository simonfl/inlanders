using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private bool _terrainEditing,_terrainDragging,_terrainAfter=true;
    private Cell? _terrainFirst,_terrainLast;
    private World? _terrainWorld;
    private TerrainEditPreview? _terrainPreview;
    private MapLayout? _terrainRenderMap;
    private Button _terrainEntry=null!,_terrainApply=null!,_terrainUndoButton=null!,_terrainCompare=null!;
    private SpinBox _terrainTarget=null!;
    private Label _terrainText=null!,_terrainUndoText=null!;
    private Button _terrainClose=null!;
    private Cell? _terrainMarkedBlocker;
    private PanelContainer _terrainPanel=null!;
    private Node3D _terrainMarks=null!;
    private float _terrainRefresh;
    private string _terrainDrawKey="";
    private void MakeTerrainUi()
    {
        _terrainEntry=Button("Level terrace · Creative",BeginTerrain);_buildSections[1].AddChild(_terrainEntry);
        _terrainPanel=HudPanel(_hud);_terrainPanel.Hide();var column=new VBoxContainer();_terrainPanel.AddChild(column);
        column.AddChild(Text("LEVEL TERRACE",16));
        column.AddChild(Text("Drag bare ground, including room for entrances. Middle-drag / WASD: camera.",14,true));
        var row=new HBoxContainer();column.AddChild(row);row.AddChild(Text("Elevation",14));
        _terrainTarget=new SpinBox{MinValue=0,MaxValue=4,Step=.4,Value=.4,CustomMinimumSize=new(150,36)};row.AddChild(_terrainTarget);
        _terrainTarget.ValueChanged+=_=>{_terrainRefresh=0;UpdateTerrainUi();};
        _terrainText=Text("",14,true);column.AddChild(_terrainText);
        _terrainCompare=Button("Show before",()=>{_terrainAfter=!_terrainAfter;_terrainDrawKey="";_terrainRefresh=0;UpdateTerrainUi();});column.AddChild(_terrainCompare);
        _terrainApply=Button("Apply terrace",ConfirmTerrain);column.AddChild(_terrainApply);
        _terrainUndoButton=Button("Undo last terrain change",UndoTerrainUi);column.AddChild(_terrainUndoButton);
        _terrainUndoText=Text("",14,true);column.AddChild(_terrainUndoText);
        _terrainClose=Button("Close [Esc]",CancelTerrain);column.AddChild(_terrainClose);_terrainMarks=new();AddChild(_terrainMarks);
    }
    private void BeginTerrain()
    {
        if(!_world.Creative)return;
        CloseManagementUi();_placing=false;RefreshGhost();CancelCameraDrag();
        _terrainEditing=true;_terrainWorld=_world;_terrainAfter=true;_terrainPanel.Show();_terrainRefresh=0;UpdateTerrainUi();
    }
    private void RefreshTerrainRendering()
    {
        RebuildLandscape();_wildlifeKey="";RenderWildlife();RefreshGhost();RefreshSelection();
    }
    private void CancelTerrain()
    {
        if(!_terrainEditing)return;
        _terrainEditing=false;_terrainDragging=false;_terrainFirst=null;_terrainLast=null;_terrainPreview=null;_terrainWorld=null;
        _terrainPanel.Hide();Clear(_terrainMarks);_terrainDrawKey="";CancelCameraDrag();
        if(_terrainRenderMap!=null){_terrainRenderMap=null;RefreshTerrainRendering();}
    }
    private void UpdateTerrainUi()
    {
        _terrainEntry.Visible=_world.Creative;
        if(!_terrainEditing)return;
        if(_terrainWorld!=_world || !_world.Creative || _placing || _watching){CancelTerrain();return;}
        _terrainPanel.Position=new(_hud.Size.X-300,92);_terrainPanel.Size=new(284,0);
        if(_uiTime<_terrainRefresh)return;_terrainRefresh=_uiTime+.25f;
        var previous=_terrainPreview;
        _terrainPreview=_terrainFirst is Cell first && _terrainLast is Cell last?_world.PreviewTerrain(first,last,(float)_terrainTarget.Value):null;
        var preview=_terrainPreview;
        _terrainApply.Disabled=preview==null || preview.Problem!=null || preview.ChangedCells.Count==0 || _terrainDragging;
        var undoProblem=_world.TerrainUndoProblem();_terrainUndoButton.Disabled=undoProblem!=null;_terrainUndoButton.TooltipText=undoProblem??"Restore the previous terrain; keeps elapsed time and goods.";
        _terrainUndoText.Text=undoProblem==null?"Undo needs clear ground. Kept until next edit or load.":$"Undo: {undoProblem}";
        var blocker=preview?.Blocker ?? (preview==null?_world.TerrainUndoBlocker():null);
        _terrainCompare.Disabled=preview==null || preview.Heights.Count==0;
        _terrainCompare.Text=_terrainAfter?"Show before":"Show after";
        _terrainText.Text=preview==null?(undoProblem==null?"Terrace applied. Choose another plot, or Undo to restore previous heights.":"Choose a terrace. Gold marks the level plot; blue marks its sloped border."):
            $"{(_terrainAfter?"AFTER":"BEFORE")} · {preview.ChangedCells.Count} affected tiles\n"+(preview.Problem??(preview.ChangedCells.Count==0?"Already at this elevation.":"Gold: level plot. Blue: sloped border. Ready to apply."));
        if(blocker!=null)_terrainText.Text="Red cross: blocking tile.\n"+(preview?.Problem??"Choose another plot or clear the Undo obstruction below.");
        string key=$"{_terrainFirst}:{_terrainLast}:{_terrainTarget.Value}:{_terrainAfter}:{preview?.Problem}:{blocker}";
        bool sameHeights=previous==null && preview==null || previous!=null && preview!=null && previous.Heights.SequenceEqual(preview.Heights);
        if(key==_terrainDrawKey && sameHeights)return;_terrainDrawKey=key;Clear(_terrainMarks);
        _terrainRenderMap=_terrainAfter && preview is {Problem:null,Heights.Count:>0}?new MapLayout{MinX=_world.Map.MinX,MinZ=_world.Map.MinZ,Width=_world.Map.Width,Depth=_world.Map.Depth,Heights=preview.Heights.ToArray()}:null;
        RefreshTerrainRendering();
        _terrainMarkedBlocker=blocker?.Cell;
        var marks=new SurfaceTool();marks.Begin(Godot.Mesh.PrimitiveType.Triangles);int count=0;
        var a=_terrainFirst??default;var b=_terrainLast??default;
        var cells=preview==null?Array.Empty<Cell>():preview.ChangedCells.Concat(_world.Map.Land.Where(c=>c.X>=Math.Min(a.X,b.X)&&c.X<=Math.Max(a.X,b.X)&&c.Z>=Math.Min(a.Z,b.Z)&&c.Z<=Math.Max(a.Z,b.Z))).Distinct().ToArray();
        foreach(var cell in cells)
        {
            bool selected=cell.X>=Math.Min(a.X,b.X)&&cell.X<=Math.Max(a.X,b.X)&&cell.Z>=Math.Min(a.Z,b.Z)&&cell.Z<=Math.Max(a.Z,b.Z);
            Color color=new(selected?"eac04a":"429fd0");
            // Narrow corner-following rims leave the terrain itself visible.
            var corners=new[]{OnGround(cell.X-.49f,cell.Z-.49f,.025f),OnGround(cell.X+.49f,cell.Z-.49f,.025f),OnGround(cell.X+.49f,cell.Z+.49f,.025f),OnGround(cell.X-.49f,cell.Z+.49f,.025f)};
            for(int i=0;i<4;i++)
            {
                var p=corners[i];var q=corners[(i+1)%4];var inward=(new Vector3(cell.X,0,cell.Z)-(p+q)/2) with{Y=0};inward=inward.Normalized()*.05f;
                Triangle(marks,p,q+inward,q,color);Triangle(marks,p,p+inward,q+inward,color);count++;
            }
        }
        if(blocker is {Cell:var blocked})
        {
            // A cross stays distinct from both plot and apron rims, including hidden access tiles.
            foreach(float sign in new[]{-1f,1f})
            {
                var p=OnGround(blocked.X-.4f,blocked.Z-.4f*sign,.065f);var q=OnGround(blocked.X+.4f,blocked.Z+.4f*sign,.065f);
                var side=new Vector3(-sign,0,1).Normalized()*.065f;var color=new Color("ff5440");
                Triangle(marks,p-side,q+side,q-side,color);Triangle(marks,p-side,p+side,q+side,color);count++;
            }
        }
        if(count>0){var mesh=SurfaceMesh(_terrainMarks,marks);var material=(StandardMaterial3D)mesh.MaterialOverride;material.ShadingMode=BaseMaterial3D.ShadingModeEnum.Unshaded;material.NoDepthTest=true;}
    }
    private void ConfirmTerrain()
    {
        if(!_terrainEditing || _terrainWorld!=_world || _terrainPreview==null)return;
        string? problem=_world.TerrainApplyProblem(_terrainPreview);
        if(problem!=null || !_world.ApplyTerrain(_terrainPreview)){_terrainRefresh=0;UpdateTerrainUi();if(_terrainPreview?.Problem==null)_terrainText.Text=problem??"Terrain could not be changed. Preview again.";UiCue(Cue.Reject);return;}
        _terrainRenderMap=null;_terrainFirst=null;_terrainLast=null;_terrainPreview=null;_terrainDrawKey="";Clear(_terrainMarks);RefreshTerrainRendering();
        _terrainRefresh=0;UpdateTerrainUi();UiCue(Cue.Place);
    }
    private void UndoTerrainUi()
    {
        if(!_terrainEditing || _terrainWorld!=_world)return;
        var problem=_world.TerrainUndoProblem();
        if(problem!=null || !_world.UndoTerrain()){_terrainRefresh=0;UpdateTerrainUi();_terrainUndoText.Text=$"Undo: {problem??"Terrain could not be restored."}";return;}
        _terrainRenderMap=null;_terrainFirst=null;_terrainLast=null;_terrainPreview=null;_terrainDrawKey="";Clear(_terrainMarks);RefreshTerrainRendering();
        _terrainRefresh=0;UpdateTerrainUi();UiCue(Cue.Place);
    }
    private bool HandleTerrainInput(InputEvent input)
    {
        if(!_terrainEditing)return false;
        if(input is InputEventKey{Pressed:true,Keycode:Key.Escape} || input is InputEventMouseButton{Pressed:true,ButtonIndex:MouseButton.Right}){CancelTerrain();return true;}
        if(input is InputEventKey{Pressed:true} key && key.Keycode is Key.B or Key.V or Key.G or Key.I or Key.O or Key.H or Key.U or Key.T or Key.C or Key.P){CancelTerrain();return false;}
        if(input is InputEventMouseMotion motion && _terrainDragging)
        {if(!PointerOverHud(motion.Position) && Ground(motion.Position) is Vector3 p)_terrainLast=new(Mathf.RoundToInt(p.X),Mathf.RoundToInt(p.Z));_terrainRefresh=0;UpdateTerrainUi();return true;}
        if(input is not InputEventMouseButton{ButtonIndex:MouseButton.Left} button)return false;
        if(!button.Pressed && _terrainDragging){_terrainDragging=false;_terrainRefresh=0;UpdateTerrainUi();return true;}
        if(PointerOverHud(button.Position))return false;
        if(button.Pressed && Ground(button.Position) is Vector3 point)
        {_terrainFirst=_terrainLast=new Cell(Mathf.RoundToInt(point.X),Mathf.RoundToInt(point.Z));_terrainDragging=true;_terrainRefresh=0;UpdateTerrainUi();}
        return true;
    }
}
