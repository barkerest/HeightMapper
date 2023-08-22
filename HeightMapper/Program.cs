
using HeightMapper.Generators;
using ImageMagick;

namespace HeightMapper;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("HeightMapper v0.1");

        var m = new Map(64, 64);
        new Canyon().Generate(m);
        var img = m.ToImage();
        File.WriteAllBytes(@"C:\Temp\test.png", img.ToByteArray(MagickFormat.Png));
    }
}