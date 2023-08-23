using System.Security.Cryptography;
using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class MiddleRiver : GeneratorBase
{
    private readonly IntegerOption _depth      = new IntegerOption("Average Depth", 1280, 64, 3840);
    private readonly IntegerOption _width      = new IntegerOption("Average Width", 10, 1, 60);
    private readonly IntegerOption _confinePct = new IntegerOption("Confine to Middle Pct", 50, 1, 100);
    
    private readonly IntegerOption _explicitStart = new IntegerOption(
        "Explicit Start (-1 = disable)",
        -1,
        -1
    );

    private readonly IntegerOption _courseLean = new IntegerOption(
        "Course Change Lean (-1 = to the left, 0 = mid, 1 = to the right)",
        0,
        -1,
        1
    );

    public ushort Depth
    {
        get => (ushort)_depth.IntegerValue;
        set => _depth.IntegerValue = value;
    }

    public int Width
    {
        get => _width.IntegerValue;
        set => _width.IntegerValue = value;
    }

    public int ConfinePercent
    {
        get => _confinePct.IntegerValue;
        set => _confinePct.IntegerValue = value;
    }

    public int ExplicitStart
    {
        get => _explicitStart.IntegerValue;
        set => _explicitStart.IntegerValue = value;
    }

    public int CourseLean
    {
        get => _courseLean.IntegerValue;
        set => _courseLean.IntegerValue = value;
    }
    
    public MiddleRiver()
    {
        SetOptions(_depth, _width, _confinePct, _explicitStart, _courseLean);
    }

    public MiddleRiver(ushort depth, int width, int confinePct, int explicitStart = -1, int courseLean = 0)
        : this()
    {
        _depth.IntegerValue         = depth;
        _width.IntegerValue         = width;
        _confinePct.IntegerValue    = confinePct;
        _explicitStart.IntegerValue = explicitStart;
        _courseLean.IntegerValue    = courseLean;
    }

    public override string Description => "Generates a river running down the middle of the map.";

    public override void Generate(Map map)
    {
        var confineWidth = map.Width * _confinePct.IntegerValue * 0.01;
        if (confineWidth < _width.IntegerValue) return; // nothing to do.
        var midW     = _width.IntegerValue / 2;
        var minX     = (int)((map.Width - confineWidth) / 2);
        var maxX     = (int)(minX + confineWidth);
        var minMidX  = minX + midW;
        var maxMidX  = maxX - midW;
        var midRange = maxMidX - minMidX + 1;
        var b        = new byte[1];

        // starting point.
        var mid = (map.Random.Next() % midRange) + minMidX;
        if (_explicitStart.IntegerValue >= minMidX && _explicitStart.IntegerValue <= maxMidX)
        {
            // use the explicit value.
            mid = _explicitStart.IntegerValue;
        }
        else
        {
            // set the explicit value (allows the generated point to be reused.
            _explicitStart.IntegerValue = mid;
        }

        var depth    = (ushort)_depth.IntegerValue;
        var depths   = new ushort[midW + 1];
        var lean     = _courseLean.IntegerValue;
        var momentum = lean;
        
        depths[0] = depth;
        for (var i = 1; i <= midW; i++)
        {
            depths[i] = (ushort)(depth * (1.0 - (1.0 * i / midW)));
        }

        for (var y = 0; y < map.Height; y++)
        {
            map.Random.NextBytes(b);
            var d = b[0] switch
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

            momentum = lean == 0 ? d : lean;
            
            mid += d;
            if (mid < minMidX) mid = minMidX;
            if (mid > maxMidX) mid = maxMidX;
            var startX             = mid - midW;
            var endX               = mid + midW;
            for (var x = startX; x <= endX; x++)
            {
                var index = (x < mid) ? (mid - x) : (x - mid);
                map[x, y] = (ushort)(map[x, y] - depths[index]);
            }
        }
    }
}
