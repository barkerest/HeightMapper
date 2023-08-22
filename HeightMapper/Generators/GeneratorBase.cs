using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public abstract class GeneratorBase : IGenerator
{
    protected static Random Random = new Random();

    public static void SetRandomSeed(int seed)
    {
        Random = new Random(seed);
    }
    
    private IGeneratorOption[] _options = Array.Empty<IGeneratorOption>();

    protected void SetOptions(params IGeneratorOption[] options)
    {
        _options = options;
    }

    public int OptionCount => _options.Length;

    public string   GetOptionPrompt(int index)               => _options[index].Prompt;
    public TypeCode GetOptionType(int   index)               => _options[index].DataType;
    public object   GetOptionValue(int  index)               => _options[index].Value;
    public void     SetOptionValue(int  index, object value) => _options[index].Value = value;
    
    public abstract void Generate(Map map);
}
