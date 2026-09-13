using Godot;
using System;
using System.Collections.Generic;

public partial class Game
{
    private bool _storybookScene;
    private readonly List<(Vector3 A,Vector3 B,Vector3 Outward,float Width)> _storybookEdges=new();
    private Button _sceneStudyButton=null!;
    private void ToggleSceneStudy()
    {
        _storybookScene=!_storybookScene;
        ApplyAtmosphere();CreateActors();RenderActors(0);RefreshGhost();RefreshSelection();UpdateHud();LayoutHud();ApplyWorldLabels();UpdateLabelButtons();
        _sceneStudyButton.Text=_storybookScene?"Scene study: storybook":"Scene study: original";
    }
    private void StorybookCrown(Node3D crown,Vector3 at)
    {
        float variation=(MathF.Sin(at.X*2.7f+at.Z*1.9f)+1)*.5f;
        var baseColor=new Color("547653").Lerp(new("839558"),variation);
        // Layered, offset volumes retain a visible trunk and break the repeated egg silhouette.
        foreach(var (position,radius,height,tint) in new[]{
            (new Vector3(-.32f,.55f,.08f),.73f,1.25f,-.06f),
            (new Vector3(.36f,.70f,-.15f),.79f,1.42f,.015f),
            (new Vector3(-.07f,1.18f,.04f),.68f,1.27f,.07f)})
            Mesh(crown,new SphereMesh{Radius=radius,Height=height,RadialSegments=7,Rings=4},position,baseColor.Lightened(tint));
    }
}
