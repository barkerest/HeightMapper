using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Flat : GeneratorBase
{
    private readonly IntegerOption _height = new IntegerOption("Height", 2560, 0, ushort.MaxValue);

    public Flat()
    {
        SetOptions(_height);
    }
    
    /// <inheritdoc />
    public override void Generate(Map map)
    {
        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
            map[x, y] = (ushort)_height.IntegerValue;
    }
}
