using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine;

public static class KeyboardInput
{
    private static KeyboardState _previousState;
    public static KeyboardState CurrentState;

    public static void Update()
    {
        _previousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public static bool IsNewKeyPress(Keys key)
    {
        return CurrentState.IsKeyDown(key) && !_previousState.IsKeyDown(key);
    }

    public static char? GetCharFromKey(Keys key)
    {
        return key switch
        {
            >= Keys.A and <= Keys.Z => key.ToString().ToLower()[0],
            >= Keys.D0 and <= Keys.D9 => (char)('0' + (key - Keys.D0)),
            _ => null
        };
    }
}