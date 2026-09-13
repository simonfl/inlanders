using Inlanders.Simulation;

// Evidence helper only: no changes to player placement permission or historic comparisons.
enum ReviewPlacementPolicy { Exact, NearestAllFacings, FacingFirst }
sealed record ReviewPlacement(BuildingKind Kind, Cell Requested, Cell Actual, int Rotation,
    ReviewPlacementPolicy Policy, string Region, float DisplacementSquared)
{
    public static ReviewPlacement? Find(World w,BuildingKind kind,Cell requested,
        Func<Cell,int,bool> inRegion,string region,ReviewPlacementPolicy policy=ReviewPlacementPolicy.NearestAllFacings,int facing=0)
    {
        if(facing is <0 or >3)throw new ArgumentOutOfRangeException(nameof(facing));
        var candidates=w.Map.Land.SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
            .Where(p=>inRegion(p.cell,p.rotation));
        if(policy==ReviewPlacementPolicy.Exact)candidates=candidates.Where(p=>p.cell==requested && p.rotation==facing);
        var ordered=policy==ReviewPlacementPolicy.FacingFirst?
            candidates.OrderBy(p=>(p.rotation-facing+4)%4).ThenBy(p=>(p.cell.Point-requested.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X):
            candidates.OrderBy(p=>(p.cell.Point-requested.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>(p.rotation-facing+4)%4);
        // Search nearest first: expensive route checks stop at the first legal candidate.
        var found=ordered.FirstOrDefault(p=>w.PlacementProblem(p.cell,p.rotation,kind)==null);
        return found==null?null:new(kind,requested,found.cell,found.rotation,policy,region,(found.cell.Point-requested.Point).LengthSquared());
    }
}
