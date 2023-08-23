using HeightMapper.Generators.Options;

namespace HeightMapper.Generators;

public abstract class GeneratorBase : IGenerator
{
    private IGeneratorOption[] _options = Array.Empty<IGeneratorOption>();

    protected void SetOptions(params IGeneratorOption[] options)
    {
        _options = options;
    }

    public abstract string Description { get; }
    
    public int    OptionCount => _options.Length;

    public string   GetOptionPrompt(int index)               => _options[index].Prompt;
    public TypeCode GetOptionType(int   index)               => _options[index].DataType;
    public object   GetOptionValue(int  index)               => _options[index].Value;
    public void     SetOptionValue(int  index, object value) => _options[index].Value = value;

    public bool WillResetMap { get; protected set; } = false;

    public abstract void Generate(Map map);
}
