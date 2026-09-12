using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private VBoxContainer _finishControls=null!,_finishDetails=null!;
    private Button _finishToggle=null!;
    private readonly Dictionary<CottageFinish,Button> _finishOptions=new();
    private int _finishSite=-1;
    private World? _finishWorld;
    private static (Color Roof,Color Wall) CottagePalette(CottageFinish finish) => finish switch
    {
        CottageFinish.Sage=>(new("768478"),new("e2cfaa")),
        CottageFinish.Ochre=>(new("a28d63"),new("e2cfaa")),
        CottageFinish.Slate=>(new("596f83"),new("e0d9be")),
        CottageFinish.Rose=>(new("966b69"),new("dfc7b0")),
        _=>(new("ae7156"),new("e2cfaa"))
    };
    private void MakeCottageFinishControls()
    {
        _finishControls=new();_buildingDetails.AddChild(_finishControls);
        _finishToggle=Button("Cottage finish",()=>_finishDetails.Visible=!_finishDetails.Visible);_finishControls.AddChild(_finishToggle);
        _finishDetails=new(){Visible=false};_finishControls.AddChild(_finishDetails);
        var choices=new GridContainer{Columns=2};_finishDetails.AddChild(choices);
        foreach(var finish in Enum.GetValues<CottageFinish>())
        {
            var choice=Button(finish.ToString(),()=>_world.SetCottageFinish(_selectedSite,finish));
            choice.SizeFlagsHorizontal=Control.SizeFlags.ExpandFill;
            if(finish!=CottageFinish.Automatic)
            {
                var palette=CottagePalette(finish);
                choice.Icon=new GradientTexture2D{Width=16,Height=16,FillFrom=new(0,0),FillTo=new(0,1),Gradient=new Gradient{Colors=new[]{palette.Roof,palette.Wall},Offsets=new[]{.49f,.51f}}};
            }
            choices.AddChild(choice);_finishOptions[finish]=choice;
        }
        _finishDetails.AddChild(Text("Free cosmetic choice. Changes this cottage's roof and plaster as they are built. Automatic restores its original colors. No effect on comfort or work.",13,true));
    }
    private void UpdateCottageFinishControls()
    {
        var home=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && c.Kind==BuildingKind.Cottage);
        _finishControls.Visible=home!=null;if(home==null)return;
        if(_finishWorld!=_world || _finishSite!=home.Id){_finishDetails.Hide();_finishWorld=_world;_finishSite=home.Id;}
        _finishToggle.Text=$"Finish · {(home.Finish==CottageFinish.Automatic?$"Automatic ({World.ResolvedCottageFinish(home)})":home.Finish.ToString())}";
        foreach(var choice in _finishOptions)choice.Value.Disabled=home.DemolitionRequested || choice.Key==home.Finish;
    }
}
