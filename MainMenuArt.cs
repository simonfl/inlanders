using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private TextureRect _menuVillage=null!;
    private Label _menuArtHeading=null!,_menuArtCaption=null!;
    private void MakeMenuArt()
    {
        var paper=new ColorRect{Color=new("dedfce"),MouseFilter=Control.MouseFilterEnum.Ignore};
        _mainMenu.AddChild(paper);paper.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        var viewport=new SubViewport{Size=new(1100,1000),OwnWorld3D=true,TransparentBg=true,RenderTargetUpdateMode=SubViewport.UpdateMode.Once};
        _mainMenu.AddChild(viewport);
        var root=new Node3D();viewport.AddChild(root);
        root.AddChild(new WorldEnvironment{Environment=new Godot.Environment{AmbientLightSource=Godot.Environment.AmbientSource.Color,AmbientLightColor=new("e5e7d6"),AmbientLightEnergy=.45f}});
        root.AddChild(new DirectionalLight3D{RotationDegrees=new(-48,-38,0),LightEnergy=.65f,ShadowEnabled=true});
        var ground=Cylinder(root,new(0,-.5f,0),8.5f,1f,new("8b9b64"),8.1f);
        ground.Scale=new(1,1,.8f);
        var pond=Cylinder(root,new(4.6f,.015f,2.1f),2.05f,.025f,new("829e99"));pond.Scale=new(.8f,1,1.3f);
        // A crossing lane gathers homes and food production around a small shared green.
        Box(root,new(-.5f,.02f,2),new(9,.035f,.7f),new("c4ba95"));
        Box(root,new(-.5f,.025f,-.3f),new(.7f,.04f,5.2f),new("c4ba95"));
        void Building(BuildingKind kind,Vector3 position,int id=0,float angle=0)
        {
            var model=new Node3D{Position=position,RotationDegrees=new(0,angle,0)};root.AddChild(model);
            MakeBuilding(model,new Cottage{Kind=kind,Id=id},3);HideModelLabels(model);
            if(kind==BuildingKind.VegetableGarden)MakeVegetables(model,new Cottage{Kind=kind,Harvest=8},4);
        }
        Building(BuildingKind.Bakery,new(-3,0,-.4f));
        Building(BuildingKind.Cottage,new(1.8f,0,-1),1);
        Building(BuildingKind.Cottage,new(-2.9f,0,-3.4f),2);
        Building(BuildingKind.VegetableGarden,new(-3.4f,0,3.8f));
        Building(BuildingKind.SeatingGarden,new(.4f,0,3.4f));
        foreach(var p in new[]{new Vector3(-6,0,-1),new(-5.2f,0,-3.7f),new(4.2f,0,-3.7f),new(5.4f,0,-1.5f),new(-6,0,2.4f)})
            MakeTree(p,.9f,new("6d8752")).Reparent(root,false);
        // Presentation nodes must never join live settlement animation groups.
        void Isolate(Node n){foreach(var group in n.GetGroups())n.RemoveFromGroup(group);foreach(var child in n.GetChildren())Isolate(child);}
        Isolate(root);
        var camera=new Camera3D{Projection=Camera3D.ProjectionType.Orthogonal,Size=20,Position=new(15,18,23),Current=true};root.AddChild(camera);camera.LookAt(new(0,.4f,0));
        _menuVillage=new TextureRect{Texture=viewport.GetTexture(),ExpandMode=TextureRect.ExpandModeEnum.IgnoreSize,StretchMode=TextureRect.StretchModeEnum.KeepAspectCentered,MouseFilter=Control.MouseFilterEnum.Ignore};_mainMenu.AddChild(_menuVillage);
        _menuArtHeading=Text("MAKE ROOM FOR A LITTLE LIFE",14,true);_menuArtHeading.Modulate=new("4b6254");_menuArtHeading.HorizontalAlignment=HorizontalAlignment.Center;_mainMenu.AddChild(_menuArtHeading);
        _menuArtCaption=Text("A home. A harvest. A place to gather.",16,true);_menuArtCaption.Modulate=new("4b6254");_menuArtCaption.HorizontalAlignment=HorizontalAlignment.Center;_mainMenu.AddChild(_menuArtCaption);
    }
    private void LayoutMenuArt()
    {
        float left=_mainPanel.Position.X+_mainPanel.Size.X+16,width=_mainMenu.Size.X-left-16;
        _menuVillage.Position=new(left,60);_menuVillage.Size=new(width,_mainMenu.Size.Y-120);
        _menuArtHeading.Position=new(left,48);_menuArtHeading.Size=new(width,36);
        _menuArtCaption.Position=new(left,_mainMenu.Size.Y-76);_menuArtCaption.Size=new(width,44);
    }
}
