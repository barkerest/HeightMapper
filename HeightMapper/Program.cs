
using HeightMapper.Generators;
using ImageMagick;

namespace HeightMapper;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("HeightMapper v0.1");

        var        m = new Map(1081, 1081);
        
        new Canyon(ushort.MaxValue, 120 * 64, 55, 4).Generate(m);
        new River(80 * 64, 55, 40, 2).Generate(m);
        new Noise(25, 50 * 64).Generate(m);
        new Average(4).Generate(m);
        
        var img = m.ToImage();
        File.WriteAllBytes(@"C:\Temp\test.png", img.ToByteArray(MagickFormat.Png));
    }
}