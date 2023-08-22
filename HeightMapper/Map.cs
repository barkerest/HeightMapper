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
            return (ushort)((_data[i + 1] << 8) | (_data[i]));
        }
        set
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            var i = PixelIndex(x, y);
            _data[i]     = (byte)(value        & 0xFF);
            _data[i + 1] = (byte)((value >> 8) & 0xFF);
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
        _data  = (byte[])map._data.Clone();
    }
    
    public MagickImage ToImage()
    {
        var ret = new MagickImage(_data, new PixelReadSettings(Width, Height, StorageType.Short, "I"));
        ret.SetBitDepth(16, Channels.Gray);
        return ret;
    }
}
