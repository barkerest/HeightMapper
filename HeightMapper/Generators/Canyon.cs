using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Canyon : GeneratorBase
{
    private readonly IntegerOption _bottom = new IntegerOption("Bottom Height", 120, 0, ushort.MaxValue);
    private readonly IntegerOption _top    = new IntegerOption("Top Height", short.MaxValue, 0, ushort.MaxValue);
    private readonly IntegerOption _ratio  = new IntegerOption("Bottom Ratio", 60, 10, 80);

    public Canyon()
    {
        SetOptions(_bottom, _ratio);
    }

    /// <inheritdoc />
    public override void Generate(Map map)
    {
        var buf          = new byte[2];
        
        var topHeight    = (ushort)_top.IntegerValue;
        var bottomHeight = (ushort)_bottom.IntegerValue;
        
        if (topHeight < bottomHeight) (topHeight, bottomHeight) = (bottomHeight, topHeight);

        var bottomWidth = (int)(map.Width * (_ratio.IntegerValue * 0.01));

        if (bottomWidth < 4) bottomWidth = 4;

        var leftWidth  = (map.Width - bottomWidth) / 2;
        var rightWidth = map.Width - bottomWidth - leftWidth;

        if (leftWidth  < 4) leftWidth  = 4;
        if (rightWidth < 4) rightWidth = 4;
        bottomWidth = map.Width - leftWidth - rightWidth;

        if (bottomWidth < 4) throw new InvalidOperationException();

        // jitter is the ability of the walls to wander off course a bit.
        var jitter = bottomWidth > rightWidth ? rightWidth / 3 : bottomWidth / 3;

        // slope is how fast we'll go from topHeight to bottomHeight.
        var slope            = bottomWidth > rightWidth ? rightWidth / 10 : bottomWidth / 10;
        
        if (slope < 1) slope = 1;

        var slopeHeights = new ushort[slope];
        var heightDiff   = (ushort)(topHeight - bottomHeight);
        for (var n = 0; n < slope; n++)
        {
            slopeHeights[n] = (ushort)(bottomHeight + heightDiff * (1.0 - n / (slope + 1.0)));
        }
        
        int rx = map.Width - rightWidth, lx = leftWidth, rs, ls;
        for (var y = 0; y < map.Height; y++)
        {
            Random.NextBytes(buf);

            lx += buf[0] switch
                  {
                      < 32  => -2,
                      < 64  => -1,
                      < 96  => 1,
                      < 128 => 2,
                      _     => 0,
                  };
            if (lx < leftWidth - jitter) lx = leftWidth - jitter;
            if (lx > leftWidth + jitter) lx = leftWidth + jitter;

            rx += buf[1] switch
                  {
                      < 32  => -2,
                      < 64  => -1,
                      < 96  => 1,
                      < 128 => 2,
                      _     => 0,
                  };
            
            if (rx < map.Width - rightWidth - jitter) rx = map.Width - rightWidth - jitter;
            if (rx > map.Width - rightWidth + jitter) rx = map.Width - rightWidth + jitter;
            ls = lx - slope;
            rs = rx + slope;

            for (var x = 0; x < map.Width; x++)
            {
                if (x < ls)
                {
                    map[x, y] = topHeight;
                }
                else if (x < lx)
                {
                    var slopePosition = x - ls;
                    map[x, y] = slopeHeights[slopePosition];
                }
                else if (x <= rx)
                {
                    map[x, y] = bottomHeight;
                }
                else if (x <= rs)
                {
                    var slopePosition = rs - x;
                    map[x, y] = slopeHeights[slopePosition];
                }
                else
                {
                    map[x, y] = topHeight;
                }
            }
        }
    }
}
