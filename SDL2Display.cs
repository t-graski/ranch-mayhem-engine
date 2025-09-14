using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine;

internal static class SDL2Display
{
    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_Rect
    {
        public int x, y, w, h;
    }

    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GetWindowDisplayIndex(IntPtr window);

    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GetDisplayBounds(int displayIndex, out SDL_Rect rect);

    public static Rectangle GetCurrentDisplayBounds(IntPtr sdlWindow)
    {
        var idx = SDL_GetWindowDisplayIndex(sdlWindow);
        if (idx < 0) idx = 0;
        if (SDL_GetDisplayBounds(idx, out var r) != 0)
            throw new Exception("SDL_GetDisplayBounds failed");

        return new Rectangle(r.x, r.y, r.w, r.h);
    }
}
