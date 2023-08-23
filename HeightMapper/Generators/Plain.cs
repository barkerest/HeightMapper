using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Plain : GeneratorBase
{
    private readonly IntegerOption _avgHeight = new IntegerOption("Average Height", 120 * 64, 0, ushort.MaxValue);
    private readonly IntegerOption _slope     = new IntegerOption("Slope", 0, 0, 10);

    public ushort AverageHeight
    {
        get => (ushort)_avgHeight.IntegerValue;
        set => _avgHeight.IntegerValue = value;
    }

    public int Slope
    {
        get => _slope.IntegerValue;
        set => _slope.IntegerValue = value;
    }
    
    public Plain()
    {
        SetOptions(_avgHeight, _slope);
    }

    public Plain(ushort avgHeight, int slope) : this()
    {
        _avgHeight.IntegerValue = avgHeight;
        _slope.IntegerValue     = slope;
    }

    public override string Description => "Resets the entire map a flat terrain with an optional slope.";

    public override void Generate(Map map)
    {
        var avgHeight = (ushort)_avgHeight.IntegerValue;
        var slope     = _slope.IntegerValue;
        if (slope == 0)
        {
            for (var y = 0; y < map.Height; y++)
            for (var x = 0; x < map.Width; x++)
                map[x, y] = avgHeight;
        }
        else
        {
            // 65536 (max value) / 1024 (max height in game) * 0.01 (1 %) = 0.64
            // then multiple by the slope % to get the change per map position
            // finally multiple by the map height to get the total change over the entire map
            var heightChange = (int)(0.64 * _slope.IntegerValue * map.Height);
            var heightTop    = avgHeight + heightChange / 2;

            for (var y = 0; y < map.Height; y++)
            {
                var heightTemp = (heightTop - ((1.0 * y / map.Height) * heightChange));
                var height = (heightTemp < 0) ? (ushort)0 : (ushort)heightTemp;
                for (var x = 0; x < map.Width; x++)
                    map[x, y] = height;
            }
        }
    }
}
