using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private LineEdit _viewName = null!;
    private readonly Button[] _viewRecall = new Button[3], _viewSave = new Button[3], _viewClear = new Button[3];
    private void MakeCameraViewsUi(VBoxContainer column)
    {
        column.AddChild(Text("SAVED VIEWS",12));
        column.AddChild(Text("1–3 recall · Ctrl+1–3 save\nIncludes zoom and orbit; works in Watch mode. Save the village to keep views between sessions.",13,true));
        _viewName=new LineEdit { PlaceholderText="View name (optional)",MaxLength=24,CustomMinimumSize=new(0,34) }; column.AddChild(_viewName);
        for(int i=0;i<3;i++)
        {
            int slot=i; var row=new HBoxContainer(); column.AddChild(row);
            _viewRecall[i]=Button("",()=>RecallCameraView(slot)); _viewRecall[i].ClipText=true; _viewRecall[i].SizeFlagsHorizontal=Control.SizeFlags.ExpandFill; row.AddChild(_viewRecall[i]);
            _viewSave[i]=Button("Set",()=>StoreCameraView(slot),44); _viewSave[i].TooltipText="Save the current camera here; replaces this slot."; row.AddChild(_viewSave[i]);
            _viewClear[i]=Button("×",()=>_world.ClearCameraView(slot),32); _viewClear[i].TooltipText="Clear this saved view."; row.AddChild(_viewClear[i]);
        }
    }
    private void UpdateCameraViewsUi()
    {
        if(_viewName==null) return;
        for(int i=0;i<3;i++)
        {
            var view=_world.CameraViews[i];
            _viewRecall[i].Text=$"{i+1} · {view?.Name ?? "Empty"}";
            _viewRecall[i].Disabled=_viewClear[i].Disabled=view==null;
            _viewRecall[i].TooltipText=view==null?"Save a view in this slot.":$"{view.Name}\nFocus {view.X:F1}, {view.Z:F1} · zoom {view.Zoom:F1}";
        }
    }
    private void StoreCameraView(int slot)
    {
        string name=_viewName.Text.Trim();
        if(name.Length==0) name=_world.CameraViews[slot]?.Name ?? $"View {slot+1}";
        float angle=(_angle%Mathf.Tau+Mathf.Tau)%Mathf.Tau;
        if(_world.SaveCameraView(slot,new CameraView(name,_focus.X,_focus.Z,_camera.Size,angle)))
        { _viewName.Text=""; Notice($"Saved {name} in view {slot+1}."); }
        UpdateCameraViewsUi();
    }
    private void RecallCameraView(int slot)
    {
        _watchOrbit=false;
        if(_world.CameraViews[slot] is not CameraView view) { Notice($"View {slot+1} is empty. Use Ctrl+{slot+1} to save the current camera."); return; }
        _followPerson=false; _placing=false; _pathStroke=false; _lastPathCell=null; RefreshGhost();
        _focus=new(view.X,0,view.Z); _angle=view.Angle; _camera.Size=view.Zoom; UpdateCamera();
        Notice(view.Name);
    }
}
