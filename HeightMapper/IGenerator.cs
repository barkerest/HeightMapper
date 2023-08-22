﻿namespace HeightMapper;

public interface IGenerator
{
    public int OptionCount { get; }

    public string GetOptionPrompt(int index);

    public TypeCode GetOptionType(int index);

    public object GetOptionValue(int index);

    public void SetOptionValue(int index, object value);

    public void Generate(Map map);
}
