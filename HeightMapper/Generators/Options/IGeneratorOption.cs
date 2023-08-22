namespace HeightMapper.Generators.Options;

public interface IGeneratorOption
{
    public TypeCode DataType { get; }
    
    public string Prompt { get; }
    
    public object Value { get; set; }
}
