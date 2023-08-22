using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class Noise : GeneratorBase
{
    private readonly        IntegerOption         _coverage = new IntegerOption("Coverage Percent", 50, 1, 100);
    
    public Noise()
    {
        SetOptions(_coverage);
    }
    
    public override void Generate(Map map)
    {
        var buf = new byte[3];
        for (var x = 0; x < map.Width; x++)
        for (var y = 0; y < map.Height; y++)
        {
            Random.NextBytes(buf);
            if (_coverage == 100 || (buf[2] % 100) < _coverage)
            {
                map[x, y] = BitConverter.ToUInt16(buf);
            }
        }
    }
}
