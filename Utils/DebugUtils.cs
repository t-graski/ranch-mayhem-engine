using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.Utils;

public static class DebugUtils
{
    public static float FrameTimeMs;
    public static float Fps;
    public static int TextureSwaps;

    private static double _accumTime;
    private static int _fpsFrames;
    private static long _allocStart;

    public static void BeginFrame(GameTime gameTime)
    {
        FrameTimeMs = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        TextureSwaps = 0;
    }

    public static void EndFrame(GameTime gameTime)
    {
        _accumTime += gameTime.ElapsedGameTime.TotalSeconds;
        _fpsFrames++;

        if (_accumTime >= 0.5)
        {
            Fps = (float)(_fpsFrames / _accumTime);
            _fpsFrames = 0;
            _accumTime = 0;
        }
    }

    public static string GpuName(GraphicsDevice gd) => gd.Adapter.Description;
}
