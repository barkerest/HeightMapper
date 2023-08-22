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

    public static implicit operator int(IntegerOption self) => self._integerValue;

    public static int operator -(IntegerOption self)               => -(self._integerValue);
    
    public static int operator -(IntegerOption a, int           b) => a._integerValue - b;
    public static int operator +(IntegerOption a, int           b) => a._integerValue + b;
    public static int operator *(IntegerOption a, int           b) => a._integerValue * b;
    public static int operator /(IntegerOption a, int           b) => a._integerValue / b;
    public static int operator -(int           a, IntegerOption b) => a - b._integerValue;
    public static int operator +(int           a, IntegerOption b) => a + b._integerValue;
    public static int operator *(int           a, IntegerOption b) => a * b._integerValue;
    public static int operator /(int           a, IntegerOption b) => a / b._integerValue;

    public static bool operator ==(IntegerOption a, int           b) => a._integerValue == b;
    public static bool operator !=(IntegerOption a, int           b) => a._integerValue == b;
    public static bool operator <(IntegerOption  a, int           b) => a._integerValue < b;
    public static bool operator >(IntegerOption  a, int           b) => a._integerValue > b;
    public static bool operator >=(IntegerOption a, int           b) => a._integerValue >= b;
    public static bool operator <=(IntegerOption a, int           b) => a._integerValue <= b;
    public static bool operator ==(int           a, IntegerOption b) => a               == b._integerValue;
    public static bool operator !=(int           a, IntegerOption b) => a               == b._integerValue;
    public static bool operator <(int            a, IntegerOption b) => a               < b._integerValue;
    public static bool operator >(int            a, IntegerOption b) => a               > b._integerValue;
    public static bool operator >=(int           a, IntegerOption b) => a               >= b._integerValue;
    public static bool operator <=(int           a, IntegerOption b) => a               <= b._integerValue;
}
