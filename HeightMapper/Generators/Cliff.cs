using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Cliff : GeneratorBase
{
    private readonly IntegerOption _height     = new IntegerOption("Cliff Height", short.MaxValue, 0, ushort.MaxValue);
    private readonly IntegerOption _mapPercent = new IntegerOption("Map Percentage", 10, 5, 50);

    public ushort Height
    {
        get => (ushort)_height.IntegerValue;
        set => _height.IntegerValue = value;
    }

    public int PercentFromEdge
    {
        get => _mapPercent.IntegerValue;
        set => _mapPercent.IntegerValue = value;
    }
    
    public Cliff()
    {
        SetOptions(_height, _mapPercent);
    }

    public Cliff(ushort height, int mapPercent) : this()
    {
        _height.IntegerValue     = height;
        _mapPercent.IntegerValue = mapPercent;
    }
    
    public override string Description => "Adds a cliff to the map.";

    public override void   Generate(Map map)
    {
        var b         = new byte[1];
        var height    = (ushort)_height.IntegerValue;
        var thickness = (int)(map.Height * 0.01 * _mapPercent.IntegerValue);
        var jitter    = (int)((_mapPercent.IntegerValue > 20 ? 20 : _mapPercent.IntegerValue) * 0.01 * map.Height) / 3;
        var minEdge   = thickness - jitter;
        var maxEdge   = thickness + jitter;

        if (minEdge < 0) minEdge             = 0;
        if (maxEdge > map.Height - 1) maxEdge = map.Height - 1;
        
        // current edge.
        var edge  = (map.Random.Next() % (jitter * 2 + 1)) - jitter + thickness;
        var slope = jitter < 4 ? 1 : ((jitter + 2) / 4);
        var momentum  = 0;

        for (var x = 0; x < map.Width; x++)
        {
            var ey = edge - slope;
            var bh = map[x, edge];
            var dh = height - bh;
            for (var y = 0; y < edge; y++)
            {
                if (y < ey)
                {
                    map[x, y] = height;
                }
                else
                {
                    var n = y  - ey;
                    var h = (ushort)(bh + (dh * (1.0 - n / (slope + 1.0))));
                    map[x, y] = h;
                }
            }

            map.Random.NextBytes(b);
            var drift = b[0] switch
                        {
                            < 16  => -3,
                            < 32  => -2,
                            < 48  => -1,
                            < 64  => 1,
                            < 80  => 2,
                            < 96  => 3,
                            < 112 => momentum < 0 ? -3 : momentum > 0 ? 3 : 0,
                            < 128 => momentum < 0 ? -2 : momentum > 0 ? 2 : 0,
                            < 144 => momentum < 0 ? -1 : momentum > 0 ? 1 : 0,
                            _     => 0,
                        };
            momentum = drift;
            
            var temp = edge + drift;
            edge = temp < minEdge ? minEdge : temp > maxEdge ? maxEdge : temp;
        }

    }
}
