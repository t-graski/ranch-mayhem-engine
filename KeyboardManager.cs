using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public static class KeyboardManager
{
    public static bool IsInTextBox { get; set; }
    public static Keys PressedKey { get; set; }
    private static readonly Dictionary<Keys, Page> _keyBindings = new();

    public static void RegisterBinding(Keys key, Page page)
    {
        if (!_keyBindings.TryAdd(key, page))
        {
            Logger.Log(
                $"{typeof(KeyboardManager)}::RegisterBinding key={KeyboardInput.GetCharFromKey(key)} already exists and will be ignored.",
                LogLevel.Warning
            );
        }
    }

    public static void ResetPressedKey()
    {
        PressedKey = Keys.None;
    }

    public static void Update()
    {
        if (IsInTextBox) return;

        var currentState = KeyboardInput.CurrentState;

        if (currentState.GetPressedKeys().Length == 0) return;

        if (!KeyboardInput.IsNewKeyPress(currentState.GetPressedKeys()[0])) return;

        PressedKey = currentState.GetPressedKeys()[0];

        if (_keyBindings.TryGetValue(currentState.GetPressedKeys()[0], out var page))
        {
            page.ToggleVisibility();
        }
    }
}
