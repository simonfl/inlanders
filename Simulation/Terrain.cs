using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class MapLayout
{
    // Shared corner grid, row-major. Empty means the original flat surface.
    public float[] Heights { get; set; } = Array.Empty<float>();
    public float CornerHeight(int x, int z) => Heights.Length == 0 ? 0 : Heights[z * (Width + 1) + x];
    public float SurfaceHeight(float x, float z)
    {
        if (Heights.Length == 0) return 0;
        float gx = Math.Clamp(x - MinX + .5f, 0, Width), gz = Math.Clamp(z - MinZ + .5f, 0, Depth);
        int ix = Math.Min((int)gx, Width - 1), iz = Math.Min((int)gz, Depth - 1);
        float u = gx - ix, v = gz - iz;
        float a = CornerHeight(ix, iz), b = CornerHeight(ix + 1, iz), c = CornerHeight(ix + 1, iz + 1), d = CornerHeight(ix, iz + 1);
        // The rendered mesh uses this same northwest-to-southeast diagonal.
        return u >= v ? a + (b - a) * u + (c - b) * v : a + (c - d) * u + (d - a) * v;
    }
    public bool LevelGround(IEnumerable<Cell> cells)
    {
        float? height = null;
        foreach (var cell in cells)
        {
            if (!Contains(cell)) return false;
            int x = cell.X - MinX, z = cell.Z - MinZ;
            foreach (float h in new[] { CornerHeight(x,z), CornerHeight(x+1,z), CornerHeight(x,z+1), CornerHeight(x+1,z+1) })
            {
                height ??= h;
                if (Math.Abs(h - height.Value) > .001f) return false;
            }
        }
        return true;
    }
    public void AuthorMeadows()
    {
        Heights = new float[(Width + 1) * (Depth + 1)];
        for (int z = 0; z <= Depth; z++) for (int x = 0; x <= Width; x++)
        {
            float px = MinX + x - .5f, pz = MinZ + z - .5f;
            float Meadow(float cx, float cz) => 1.6f * Math.Clamp((7 - Math.Max(Math.Abs(px - cx), Math.Abs(pz - cz))) / 4, 0, 1);
            Heights[z * (Width + 1) + x] = Math.Max(Meadow(-9, -10), Meadow(-2, 11));
        }
    }
    private void ValidateTerrain()
    {
        if (Heights == null || (Heights.Length != 0 && Heights.Length != (Width + 1) * (Depth + 1)) ||
            Heights.Any(h => !float.IsFinite(h) || h < 0 || h > 4)) throw new InvalidDataException("Invalid terrain heights");
        for (int z = 0; z <= Depth; z++) for (int x = 0; x <= Width; x++)
        {
            float h = CornerHeight(x,z);
            if ((x < Width && Math.Abs(h - CornerHeight(x+1,z)) > .401f) ||
                (z < Depth && Math.Abs(h - CornerHeight(x,z+1)) > .401f))
                throw new InvalidDataException("Terrain slopes must remain gently walkable");
        }
        foreach (var c in Water)
            if (!LevelGround(new[] { c }) || SurfaceHeight(c.X,c.Z) != 0)
                throw new InvalidDataException("Water must remain at the flat river level");
    }
}
