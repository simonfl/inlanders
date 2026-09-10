using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private VBoxContainer _storageControls = null!;
    private Button _targetLess = null!, _targetMore = null!;
    private Label _targetLabel = null!, _logLocations = null!;
    private Button _storageMaterial=null!;

    private void MakeStorageControls()
    {
        _storageControls = new();
        _storageMaterial=Button("",()=> { var site=_world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite); if(site!=null) _world.SetStorageMaterial(site.Id,site.StorageMaterial==Inlanders.Simulation.Resource.Logs?Inlanders.Simulation.Resource.Planks:Inlanders.Simulation.Resource.Logs); });
        _buildingDetails.AddChild(_storageMaterial);
        _buildingDetails.AddChild(_storageControls);
        _targetLabel = Text("",14,true); _storageControls.AddChild(_targetLabel);
        var row = new HBoxContainer(); _storageControls.AddChild(row);
        void Adjust(int delta)
        {
            var site = _world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite);
            if (site != null) _world.SetStorageTarget(site.Id, Math.Clamp(site.StorageTarget+delta,0,World.StockpileCapacity));
        }
        _targetLess = Button("− 2 target",()=>Adjust(-2)); _targetMore = Button("+ 2 target",()=>Adjust(2));
        _targetLess.SizeFlagsHorizontal = _targetMore.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(_targetLess); row.AddChild(_targetMore);
    }
    private void UpdateStorageControls()
    {
        var site = _world.Cottages.FirstOrDefault(c=>c.Id==_selectedSite && c.Kind==BuildingKind.Stockpile);
        _storageMaterial.Visible=site!=null;
        _storageControls.Visible = site?.Complete==true && !site.DemolitionRequested;
        if(site==null) return;
        _storageMaterial.Text=$"Store {site.StorageMaterial} · switch to {(site.StorageMaterial==Inlanders.Simulation.Resource.Logs?"planks":"logs")}";
        _storageMaterial.Disabled=_world.StorageMaterialProblem(site.Id)!=null;
        _storageMaterial.TooltipText=_world.StorageMaterialProblem(site.Id) ?? "An empty pile can store logs or planks. Capacity stays at 12.";
        _targetLabel.Text=$"Keep {site.StorageTarget} {site.StorageMaterial.ToString().ToLowerInvariant()} here (capacity 12).\nHaulers refill from central or surplus stockpiles, and return excess. Target 0 drains the pile; committed loads still finish. Stop incoming producer deliveries before switching material.";
        _targetLess.Disabled=site.StorageTarget==0 || _world.Food.Celebrating;
        _targetMore.Disabled=site.StorageTarget==World.StockpileCapacity || _world.Food.Celebrating;
    }
    private void MakeStockpile(Node3D root, Cottage site, int stage)
    {
        Box(root,new(0,.045f,0),new(2.8f,.09f,1.8f),new("a0906c"));
        foreach(float x in new[]{-1.3f,1.3f}) foreach(float z in new[]{-.8f,.8f})
            Box(root,new(x,.2f,z),new(.1f,.4f,.1f),_wood);
        if(stage<1) return;
        foreach(float z in new[]{-.7f,.7f}) Box(root,new(0,.14f,z),new(2.6f,.16f,.14f),_wood);
        if(stage<2) return;
        foreach(float x in new[]{-1.25f,1.25f})
        {
            Box(root,new(x,.55f,-.5f),new(.12f,1.1f,.12f),_wood);
            Box(root,new(x,.55f,.5f),new(.12f,1.1f,.12f),_wood);
        }
        if(stage<3) return;
        Box(root,new(0,.68f,-.83f),new(2.6f,.14f,.08f),_wood);
        for(int i=0;i<site.StoredLogs+site.StoredPlanks;i++)
        {
            var at=new Vector3(-.68f+i%2*1.25f,.28f+i/6*.23f,-.48f+i/2%3*.46f);
            if(site.StorageMaterial==Inlanders.Simulation.Resource.Planks) Plank(root,at); else Log(root,at,.95f);
        }
        FoodSign(root,site.StorageMaterial==Inlanders.Simulation.Resource.Planks?"PLANK STOCKPILE":"LOG STOCKPILE",1.55f);
    }
}
