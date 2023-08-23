using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public class SetRotation : GeneratorBase
{
    private readonly IntegerOption _degrees = new IntegerOption("Degrees to rotate (clockwise)", 0, -360, 360);

    public int Degrees
    {
        get => _degrees.IntegerValue;
        set => _degrees.IntegerValue = value;
    }

    public SetRotation()
    {
        SetOptions(_degrees);
    }

    public SetRotation(int degrees) : this()
    {
        _degrees.IntegerValue = degrees;
    }
    
    public override string Description => "Sets the rotation of the map for the next generator to use.";

    public override void   Generate(Map map)
    {
        map.SetRotation(_degrees.IntegerValue);
    }
}
