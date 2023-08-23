namespace HeightMapper.Generators.Options;

public class IntegerOption : IGeneratorOption
{
    /// <inheritdoc />
    public TypeCode DataType => TypeCode.Int32;
    
    
    /// <inheritdoc />
    public string Prompt { get; }

    public IntegerOption(string prompt, int initialValue, int? minimum = null, int? maximum = null)
    {
        Prompt = prompt;
        if (minimum.HasValue) Minimum = minimum.Value;
        if (maximum.HasValue) Maximum = maximum.Value;
        IntegerValue = initialValue;
    }

    /// <summary>
    /// The minimum value.
    /// </summary>
    public int Minimum { get; } = int.MinValue;

    /// <summary>
    /// The maximum value.
    /// </summary>
    public int Maximum { get; } = int.MaxValue;


    private int _integerValue;

    public int IntegerValue
    {
        get => _integerValue;
        set => _integerValue = value < Minimum ? Minimum : value > Maximum ? Maximum : value;
    }

    /// <inheritdoc />
    public object Value
    {
        get => IntegerValue;
        set => IntegerValue = (int)value;
    }
}
