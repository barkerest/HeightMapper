using HeightMapper.Generators;
using ImageMagick;

namespace HeightMapper;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("HeightMapper v0.1");

        var map = new Map(1081, 1081)
                  .Apply(new Plain(120 * 64, 4))
                  .SetRotation(-30)
                  .Apply<Cliff>(900 * 64, 25)
                  .SetRotation(0);
        
        var river = new MiddleRiver(70 * 64, 55, 50, courseLean: -1);
        map.Apply(river);
        river.CourseLean = 1;
        map.Apply(river);


        var img = map
                  .SetRotation(80)
                  .Apply(new Cliff(20 * 64, 20))
                  .SetRotation(130)
                  .Apply(new Cliff(20 * 64, 20))
                  .SetRotation(180)
                  .Apply(new Cliff(20 * 64, 20))
                  .SetRotation(0)
                  .Apply(new Noise(25, 50 * 64))
                  .Apply(new Average(4))
                  .ToImage();

        File.WriteAllBytes(@"C:\Temp\test.png", img.ToByteArray(MagickFormat.Png));
    }
}
