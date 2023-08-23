namespace HeightMapper;

public struct RotationMatrix
{
    public readonly float Row1Column1;
    public readonly float Row1Column2;
    public readonly float Row2Column1;
    public readonly float Row2Column2;

    public RotationMatrix(int degreesRotation)
    {
        while (degreesRotation < 0) degreesRotation    += 360;
        while (degreesRotation >= 360) degreesRotation -= 360;
        
        var radians                                    = Math.PI / 180.0 * degreesRotation;
        Row1Column1 = (float)Math.Cos(radians);
        Row1Column2 = -(float)Math.Sin(radians);
        Row2Column1 = (float)Math.Sin(radians);
        Row2Column2 = (float)Math.Cos(radians);
    }

    public (int x, int y) Apply(int x, int y)
    {
        var dx = x * Row1Column1 + y * Row1Column2;
        var dy = x * Row2Column1 + y * Row2Column2;
        return ((int)Math.Round(dx), (int)Math.Round(dy));
    }
}
