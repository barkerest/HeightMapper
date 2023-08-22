using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Blur : GeneratorBase
{
    private readonly IntegerOption      _distance  = new IntegerOption("Blur Distance", 16, 1, 64);
    
    public Blur()
    {
        SetOptions(_distance);
    }
    
    private void AddCornerBits(List<(int x, int y)> corner, int offsetX, int offsetY)
    {
        corner.Add((_distance  - offsetX, _distance  - offsetY));
        corner.Add((_distance  - offsetX, -_distance + offsetY));
        corner.Add((-_distance + offsetX, -_distance + offsetY));
        corner.Add((-_distance + offsetX, _distance  - offsetY));
    }

    public override void Generate(Map map)
    {
        var copy         = new Map(map);
        var mx           = map.Width     - 1;
        var my           = map.Height    - 1;
        var diam       = _distance * 2 + 1;
        var summedPixels = diam * diam;
        var corner       = new List<(int x, int y)>();

        if (_distance > 4)
        {
            AddCornerBits(corner, 0, 0);
        }

        if (_distance > 8)
        {
            AddCornerBits(corner, 0, 1);
            AddCornerBits(corner, 1, 0);
        }

        if (_distance > 16)
        {
            AddCornerBits(corner, 1, 1);
            AddCornerBits(corner, 0, 2);
            AddCornerBits(corner, 2, 0);
        }

        if (_distance > 32)
        {
            AddCornerBits(corner, 2, 2);
            AddCornerBits(corner, 1, 2);
            AddCornerBits(corner, 2, 1);
            AddCornerBits(corner, 0, 3);
            AddCornerBits(corner, 3, 0);
        }

        summedPixels -= corner.Count;

        for (var y = 0; y < map.Height; y++)
        for (var x = 0; x < map.Width; x++)
        {
            uint sum = 0;
            
            for (var ix = 0; ix <= diam; ix++)
            for (var iy = 0; iy <= diam; iy++)
            {
                if (corner.Any(c => c.x == ix && c.y == iy)) continue;
                
                var px = x - _distance + ix;
                var py = y - _distance + iy;
                
                px = px < 0 ? 0 : px > mx ? mx : px;
                py = py < 0 ? 0 : py > my ? my : py;
                
                sum += copy[px, py];
            }

            map[x, y] = (ushort)(sum / summedPixels);
        }
    }
}
