using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Noise : GeneratorBase
{
    private readonly IntegerOption _coverage  = new IntegerOption("Coverage Percent", 25, 1, 100);
    private readonly IntegerOption _maxChange = new IntegerOption("Max Height Change", short.MaxValue - 1, 1, short.MaxValue - 1);

    public int CoveragePercent
    {
        get => _coverage.IntegerValue;
        set => _coverage.IntegerValue = value;
    }

    public ushort MaxHeightChange
    {
        get => (ushort)_maxChange.IntegerValue;
        set => _maxChange.IntegerValue = value;
    }
    
    public Noise()
    {
        SetOptions(_coverage, _maxChange);
    }

    public Noise(int coveragePct, int maxChange)
        : this()
    {
        _coverage.IntegerValue  = coveragePct;
        _maxChange.IntegerValue = maxChange;
    }

    public override string Description => "Adds random noise to the map.";

    public override void Generate(Map map)
    {
        var buf     = new byte[5];
        var pct     = _coverage.IntegerValue;
        var maxD    = _maxChange.IntegerValue;
        var pctByte = (int)(pct * 2.55);
        var maxR    = maxD * 2 + 1;
        
        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            map.Random.NextBytes(buf);
            if (pct == 100 ||
                buf[4] < pctByte)
            {
                var d = (BitConverter.ToUInt16(buf) % maxR) - maxD;
                
                var cval = map[x, y];

                if (d == 0) continue;
                
                if (d < 0)    // going down
                {
                    d    = cval - d;

                    cval = d < 0 ? (ushort)0 : (ushort)d;
                }
                else          // going up
                {
                    d = cval + d;

                    cval = d > ushort.MaxValue ? ushort.MaxValue : (ushort)d;
                }

                map[x, y] = cval;
            }
        }
    }
}
