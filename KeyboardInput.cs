using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine;

public static class KeyboardInput
{
    private static KeyboardState _previousState;
    private static HashSet<Keys> _pressedKeys = [];

    public static void Update()
    {
        var currentState = Keyboard.GetState();
        _pressedKeys.Clear();

        foreach (var key in currentState.GetPressedKeys())
        {
            _pressedKeys.Add(key);
        }

        _previousState = currentState;
    }

    public static bool IsNewKeyPress(Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key) && !_previousState.IsKeyDown(key);
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