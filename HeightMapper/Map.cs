using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImageMagick;

namespace HeightMapper;

public class Map
{
    public int Width { get; }
    
    public int Height { get; }

    private readonly byte[] _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PixelIndex(int x, int y)
        => (y * Width + x) * 2;
    
    public ushort this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            var i = PixelIndex(x, y);
            return BitConverter.ToUInt16(_data, i);
        }
        set
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            var i = PixelIndex(x, y);
            BitConverter.TryWriteBytes(_data.AsSpan(i), value);
        }
    }
    
    public Map(int width, int height)
    {
        if (width  < 16) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 16) throw new ArgumentOutOfRangeException(nameof(height));
        Width  = width;
        Height = height;
        _data  = new byte[width * height * 2];
    }

    public Map(Map map)
    {
        Width  = map.Width;
        Height = map.Height;
        _data  = new byte[Width * Height * 2];
        map._data.CopyTo(_data, 0);
    }
    
    public MagickImage ToImage()
    {
        var ret = new MagickImage(_data, new PixelReadSettings(Width, Height, StorageType.Short, "I"));
        ret.SetBitDepth(16, Channels.Gray);
        return ret;
    }
}
