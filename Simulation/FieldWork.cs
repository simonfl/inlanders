using System;
namespace Inlanders.Simulation;

public sealed partial class World
{
    public static bool IsVegetablePlot(BuildingKind kind)=>kind is BuildingKind.VegetableGarden or BuildingKind.VegetableField;
    public static int VegetableYield(Cottage field)=>field.PlotRows>0?field.PlotRows*4:VegetableYield(field.Kind);
    public static float VegetableRow(Cottage field,int plant)=>field.PlotRows>0?-(field.PlotRows-1)/2f+Math.Clamp(plant/4,0,field.PlotRows-1):VegetableRow(field.Kind,plant);
    public static int VegetableYield(BuildingKind kind)=>kind==BuildingKind.VegetableField?20:8;
    public static float VegetableRow(BuildingKind kind,int plant)=>kind==BuildingKind.VegetableField?-1.8f+Math.Clamp(plant/4,0,4)*.9f:plant/4==0?-.45f:.45f;
    // Six harvest rows on the five-tile grain plot. Positions are relative to its centre.
    public static float GrainRow(int remaining) => -1.9f + Math.Clamp(remaining-1,0,5)*.76f;
    public static float FieldWalkSeconds(Cottage field, bool sowing) => field.Kind==BuildingKind.Farm
        ? (3-(sowing?0:GrainRow(field.Harvest)))/1.5f : field.Kind==BuildingKind.VegetableField?((field.Depth+1)/2f-(sowing?0:VegetableRow(field,field.Harvest-1)))/1.5f:0;
    public static float FieldWorkSeconds(Cottage field, bool sowing) => (sowing?4:2)+2*FieldWalkSeconds(field,sowing);
}
