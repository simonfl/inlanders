using System;
namespace Inlanders.Simulation;

public sealed partial class World
{
    // Six harvest rows on the five-tile grain plot. Positions are relative to its centre.
    public static float GrainRow(int remaining) => -1.9f + Math.Clamp(remaining-1,0,5)*.76f;
    public static float FieldWalkSeconds(Cottage field, bool sowing) => field.Kind==BuildingKind.Farm
        ? (3-(sowing?0:GrainRow(field.Harvest)))/1.5f : 0;
    public static float FieldWorkSeconds(Cottage field, bool sowing) => (sowing?4:2)+2*FieldWalkSeconds(field,sowing);
}
