using System.Runtime.CompilerServices;
using ImageMagick;

namespace HeightMapper;

public class Map
{
    private readonly int    _width;
    private readonly int    _centerX;
    private readonly int    _centerY;
    private readonly int    _height;
    private readonly byte[] _data;

    private int            _rotateDegrees = 0;
    private RotationMatrix _rotateMatrix;
    private int            _rotateWidth;
    private int            _rotateHeight;
    private int            _rotateCenterX;
    private int            _rotateCenterY;

    public int Width => _rotateDegrees == 0 ? _width : _rotateWidth;

    public int Height => _rotateDegrees == 0 ? _height : _rotateHeight;

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PixelIndex(int x, int y)
    {
        if (_rotateDegrees == 0) return (y * _width + x) * 2;
        
        // rotate the "fake" point to get the "real" point.
        (x, y) = (x - _rotateCenterX, y - _rotateCenterY);
        (x, y) = _rotateMatrix.Apply(x, y);
        (x, y) = (x + _centerX, y + _centerY);

        if (x < 0 || x >= _width || y < 0 || y >= _height) return -1;
        return (y * _width + x) * 2;
    }

    public Map SetRotation(int degrees)
    {
        while (degrees < 0) degrees    += 360;
        while (degrees >= 360) degrees -= 360;
        _rotateDegrees = degrees;
        _rotateMatrix  = new RotationMatrix(degrees);

        var c1  = _rotateMatrix.Apply(_centerX - _width, _centerY   - _height);
        var c2  = _rotateMatrix.Apply(_centerX - _width, _height    - _centerY);
        var c3  = _rotateMatrix.Apply(_width   - _centerX, _height  - _centerY);
        var c4  = _rotateMatrix.Apply(_width   - _centerX, _centerY - _height);
        var xs  = new[] { c1.x, c2.x, c3.x, c4.x };
        var ys  = new[] { c1.y, c2.y, c3.y, c4.y };
        var min = (x: xs.Min(), y: ys.Min());
        var max = (x: xs.Max(), y: ys.Max());
        _rotateWidth   = max.x - min.x + 1;
        _rotateHeight  = max.y - min.y + 1;
        _rotateCenterX = _rotateWidth  / 2;
        _rotateCenterY = _rotateHeight / 2;
        return this;
    }

    public Map Apply(IGenerator generator)
    {
        generator.Generate(this);
        return this;
    }

    public Map Apply<TGenerator>(params object[] options) where TGenerator : class, IGenerator, new()
    {
        var generator = new TGenerator();
        
        for (var i = 0; i < options.Length; i++)
        {
            generator.SetOptionValue(i, options[i]);
        }

        generator.Generate(this);
        return this;
    }
    
    public Map Apply<TGenerator>(Action<TGenerator>? config = null) where TGenerator : class, IGenerator, new()
    {
        var generator = new TGenerator();
        config?.Invoke(generator);
        generator.Generate(this);
        return this;
    }
    
    public ushort this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            var i = PixelIndex(x, y);
            if (i < 0) return 0;
            return BitConverter.ToUInt16(_data, i);
        }
        set
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            var i = PixelIndex(x, y);
            if (i >= 0) BitConverter.TryWriteBytes(_data.AsSpan(i), value);
        }
    }
    
    public Map(int width, int height)
    {
        if (width  < 16) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 16) throw new ArgumentOutOfRangeException(nameof(height));
        _width   = width;
        _height  = height;
        _centerX = _width  / 2;
        _centerY = _height / 2;
        _data    = new byte[_width * _height * 2];
    }

    public Map(Map map)
    {
        _width         = map._width;
        _height        = map._height;
        _centerX       = map._centerX;
        _centerY       = map._centerY;
        _rotateDegrees = map._rotateDegrees;
        _rotateWidth   = map._rotateWidth;
        _rotateHeight  = map._rotateHeight;
        _rotateMatrix  = map._rotateMatrix;
        _rotateCenterX = map._rotateCenterX;
        _rotateCenterY = map._rotateCenterY;
        
        _data          = new byte[_width * _height * 2];
        map._data.CopyTo(_data, 0);
    }
    
    public MagickImage ToImage()
    {
        var ret = new MagickImage(_data, new PixelReadSettings(_width, _height, StorageType.Short, "I"));
        ret.SetBitDepth(16, Channels.Gray);
        return ret;
    }
}
