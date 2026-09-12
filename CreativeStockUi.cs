using Godot;
using Inlanders.Simulation;
using System;
using Resource=Inlanders.Simulation.Resource;

public partial class Game
{
    private VBoxContainer _creativeStockUi=null!;
    private OptionButton _stockResource=null!;
    private SpinBox _stockQuantity=null!;
    private Button _stockApply=null!;
    private Label _stockStatus=null!,_stockResult=null!;
    private World? _stockEditWorld;
    private Resource StockEditKind=>(Resource)_stockResource.GetSelectedId();
    private void MakeCreativeStockUi(VBoxContainer parent)
    {
        _creativeStockUi=new();parent.AddChild(_creativeStockUi);
        _creativeStockUi.AddChild(Text("CREATIVE RESOURCE SETUP",16));
        _creativeStockUi.AddChild(Text("Set central storage only. Carried goods, local stock and existing reservations are protected. Changes are recorded separately from production.",13,true));
        _stockResource=new OptionButton();foreach(var kind in Enum.GetValues<Resource>())_stockResource.AddItem(kind.ToString(),(int)kind);
        _creativeStockUi.AddChild(_stockResource);_stockResource.ItemSelected+=_=>ResetStockDraft();
        _stockStatus=Text("",13,true);_creativeStockUi.AddChild(_stockStatus);
        _stockQuantity=new SpinBox{MinValue=0,MaxValue=World.CreativeStockLimit,Step=1,SizeFlagsHorizontal=Control.SizeFlags.ExpandFill};
        _stockQuantity.TooltipText="Desired central quantity; Apply rechecks worker reservations.";_creativeStockUi.AddChild(_stockQuantity);
        _stockApply=Button("Apply central stock",ApplyStockDraft);_creativeStockUi.AddChild(_stockApply);
        _creativeStockUi.AddChild(Button("Use current quantity",ResetStockDraft));
        _stockResult=Text("",13,true);_creativeStockUi.AddChild(_stockResult);
    }
    private void ResetStockDraft()
    {
        _stockQuantity.Value=Math.Clamp(_world.CreativeCentralStock(StockEditKind),0,World.CreativeStockLimit);_stockResult.Text="";
    }
    private void UpdateCreativeStockUi()
    {
        _creativeStockUi.Visible=_world.Creative;if(!_world.Creative){_stockEditWorld=null;return;}
        if(!ReferenceEquals(_stockEditWorld,_world)){_stockEditWorld=_world;ResetStockDraft();}
        var kind=StockEditKind;
        _stockStatus.Text=$"Central: {_world.CreativeCentralStock(kind)} · Reserved minimum: {_world.CreativeProtectedStock(kind)}\nSetup added: {_world.CreativeAdded(kind)} · removed: {_world.CreativeRemoved(kind)}";
    }
    private void ApplyStockDraft()
    {
        if(!ReferenceEquals(_stockEditWorld,_world)){UpdateCreativeStockUi();_stockResult.Text="Settlement changed. Review the current quantity before applying.";return;}
        _stockQuantity.Apply();int target=(int)_stockQuantity.Value;
        string? problem=_world.CreativeStockProblem(StockEditKind,target);
        if(problem!=null){_stockResult.Text=problem;UiCue(Cue.Reject);return;}
        _world.SetCreativeCentralStock(StockEditKind,target);_stockResult.Text=$"Central {StockEditKind} set to {target}.";
        RenderActors(0);RenderFoodViews();UpdateCreativeStockUi();
    }
}
