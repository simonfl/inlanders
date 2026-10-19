using Godot;
using Inlanders.Simulation;
public partial class Game
{
    private PanelContainer? _challengeMapPanel;
    private TextureRect _challengeMapImage=null!;
    private Label _challengeMapSummary=null!;
    private void ShowChallengeMap(HamletProfile profile)
    {
        if(_challengeMapPanel==null)
        {
            _challengeMapPanel=HudPanel(_mainMenu);
            _challengeMapPanel.AddThemeStyleboxOverride("panel",new StyleBoxFlat{BgColor=new("243b32"),CornerRadiusTopLeft=8,CornerRadiusTopRight=8,CornerRadiusBottomLeft=8,CornerRadiusBottomRight=8,ContentMarginLeft=12,ContentMarginRight=12,ContentMarginTop=12,ContentMarginBottom=12});
            var column=new VBoxContainer();_challengeMapPanel.AddChild(column);
            column.AddChild(Text("STARTING GROUND · NORTH ↑",17,true));
            _challengeMapImage=new TextureRect{ExpandMode=TextureRect.ExpandModeEnum.IgnoreSize,StretchMode=TextureRect.StretchModeEnum.KeepAspectCentered,TextureFilter=CanvasItem.TextureFilterEnum.Nearest,SizeFlagsVertical=Control.SizeFlags.ExpandFill,CustomMinimumSize=new(240,240)};column.AddChild(_challengeMapImage);
            column.AddChild(Text("Cream: homes · gold: working plots\nDark green: trees · blue: water",13,true));
            _challengeMapSummary=Text("",16,true);column.AddChild(_challengeMapSummary);
        }
        var world=profile.Create();var map=world.Map;
        var picture=Image.CreateEmpty(map.Width,map.Depth,false,Image.Format.Rgba8);
        picture.Fill(new Color("243b32"));
        void Mark(Cell cell,Color color){if(map.Contains(cell))picture.SetPixel(cell.X-map.MinX,cell.Z-map.MinZ,color);}
        foreach(var cell in map.Land)Mark(cell,new("91a46f"));
        foreach(var cell in map.Water)Mark(cell,new("689caa"));
        foreach(var cell in world.Paths)Mark(cell,new("b9ad85"));
        foreach(var tree in world.Trees)Mark(tree.Cell,new("365b3d"));
        foreach(var building in world.Cottages)
            foreach(var cell in World.Footprint(building))Mark(cell,Buildings.Get(building.Kind).Beds>0?new("f2dbab"):new("d3aa58"));
        Mark(world.Stockpile,new("ab7350"));
        _challengeMapImage.Texture=ImageTexture.CreateFromImage(picture);
        _challengeMapSummary.Text=$"{world.Population} neighbors · {world.Beds} beds\n{world.EdibleStored} food · {world.Stored} logs\n\n"+(profile.Restoration?"Restore the existing village. Growth is optional after recovery.":profile.Repair?"A repair variation of the newcomer challenge: fields begin cut off.":"An introduction: make room and invite four neighbors when ready.");
        _challengeMapPanel.Show();LayoutChallengeMap();
    }
    private void LayoutChallengeMap()
    {
        if(_challengeMapPanel==null)return;
        _challengeMapPanel.Position=new(460,40);_challengeMapPanel.Size=new(System.Math.Max(280,_mainMenu.Size.X-500),System.Math.Max(400,_mainMenu.Size.Y-80));
    }
}
