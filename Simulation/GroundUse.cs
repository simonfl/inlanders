using System;
using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;
public sealed record GroundUse(Cell Cell,int Visits);
public sealed partial class World
{
    private readonly Dictionary<Cell,int> _groundUse=new();
    public int GroundUseRevision { get; private set; }
    public GroundUse[] ReadGroundUse(bool visibleOnly=false)=>_groundUse.Where(p=>!visibleOnly || !Blocked(p.Key)).OrderBy(p=>p.Key.Z).ThenBy(p=>p.Key.X).Select(p=>new GroundUse(p.Key,p.Value)).ToArray();
    private void RecordFootfall(Cell cell)
    {
        if(Founding?.TransformationHamlet!=true || Map.Water.Contains(cell))return;
        int count=_groundUse.GetValueOrDefault(cell);if(count>=32)return;
        _groundUse[cell]=count+1;GroundUseRevision++;
    }
    private void RestoreGroundUse(GroundUse[] marks)
    {
        if(marks==null || marks.Any(m=>!Map.Contains(m.Cell) || m.Visits is <1 or >32) || marks.Select(m=>m.Cell).Distinct().Count()!=marks.Length)
            throw new InvalidOperationException("Invalid ground-use marks");
        foreach(var mark in marks)_groundUse.Add(mark.Cell,mark.Visits);
    }
}
