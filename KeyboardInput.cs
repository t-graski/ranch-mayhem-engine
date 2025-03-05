using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine;

public static class KeyboardInput
{
    private static KeyboardState _previousState;
    private static KeyboardState _currentState;


    public static void Update()
    {
        _previousState = _currentState;
        _currentState = Keyboard.GetState();

        var keys = "";
        foreach (var key in _currentState.GetPressedKeys())
        {
            keys += $"{GetCharFromKey(key)} ";
        }

        if (IsNewKeyPress(Keys.A))
        {
            Logger.Log($"{DateTimeOffset.Now.ToUnixTimeMilliseconds()} current {keys}");
        }
    }

    public static bool IsNewKeyPress(Keys key)
    {
        return _currentState.IsKeyDown(key) && !_previousState.IsKeyDown(key);
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