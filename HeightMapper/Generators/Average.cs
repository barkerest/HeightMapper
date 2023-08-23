using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Average : GeneratorBase
{
    private readonly IntegerOption _distance = new IntegerOption("Averaging Distance", 16, 1, 64);

    public int AveragingDistance
    {
        get => _distance.IntegerValue;
        set => _distance.IntegerValue = value;
    }
    
    public Average()
    {
        SetOptions(_distance);
    }

    public Average(int distance)
        : this()
    {
        _distance.IntegerValue = distance;
    }

    public override string Description =>
        "Computes the average height for a pixel based on surrounding pixels (map rotation should be 0).";

    public override void Generate(Map map)
    {
        var copy         = new Map(map);
        var maxX         = map.Width - 1;
        var maxY         = map.Height - 1;
        var diam         = _distance.IntegerValue * 2 + 1;
        var summedPixels = diam * diam;

        for (var y = 0; y < map.Height; y++)
        for (var x = 0; x < map.Width; x++)
        {
            long sum = 0;

            for (var ix = 0; ix < diam; ix++)
            for (var iy = 0; iy < diam; iy++)
            {
                var sourceX = x - _distance.IntegerValue + ix;
                var sourceY = y - _distance.IntegerValue + iy;

                sourceX = sourceX < 0 ? 0 : sourceX > maxX ? maxX : sourceX;
                sourceY = sourceY < 0 ? 0 : sourceY > maxY ? maxY : sourceY;

                sum += copy[sourceX, sourceY];
            }

            map[x, y] = (ushort)(sum / summedPixels);
        }
    }
}
