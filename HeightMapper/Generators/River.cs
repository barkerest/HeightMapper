using System.Security.Cryptography;
using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class River : GeneratorBase
{
    private readonly IntegerOption _depth      = new IntegerOption("Average Depth", 1280, 64, 3840);
    private readonly IntegerOption _width      = new IntegerOption("Average Width", 10, 1, 60);
    private readonly IntegerOption _confinePct = new IntegerOption("Confine to Middle Pct", 50, 1, 100);
    private readonly IntegerOption _slope      = new IntegerOption("Slope", 1, 1, 15);

    public River()
    {
        SetOptions(_depth, _width, _confinePct, _slope);
    }

    public River(ushort depth, int width, int confinePct, int slope)
        : this()
    {
        _depth.IntegerValue      = depth;
        _width.IntegerValue      = width;
        _confinePct.IntegerValue = confinePct;
        _slope.IntegerValue      = slope;
    }

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
        var mid = (Random.Next() % midRange) + minMidX;

        var depthChange = (int)(ushort.MaxValue * 0.01 * _slope.IntegerValue);
        var depthMin    = (ushort)(_depth.IntegerValue - depthChange / 2);

        var depths = new ushort[midW + 1];

        for (var y = 0; y < map.Height; y++)
        {
            var depth = (ushort)(depthMin + ((1.0 * y / map.Height) * depthChange));

            depths[0] = depth;
            for (var i = 1; i <= midW; i++)
            {
                depths[i] = (ushort)(depth * (1.0 - (1.0 * i / midW)));
            }

            Random.NextBytes(b);
            var d = b[0] switch
            {
                < 32  => -2,
                < 64  => -3,
                < 96  => -1,
                < 128  => 1,
                < 160  => 3,
                < 192 => 2,
                _     => 0,
            };
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
