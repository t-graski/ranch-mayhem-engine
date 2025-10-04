using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public static class KeyboardManager
{
    public static bool IsInTextBox { get; set; }
    public static Keys PressedKey { get; set; } = Keys.None;
    private static readonly Dictionary<Keys, Action> _keyBindings = new();

    public static bool RegisterBinding(Keys key, Action handler)
    {
        if (!_keyBindings.TryAdd(key, handler))
        {
            Logger.Log(
                $"{typeof(KeyboardManager)}::RegisterBinding key={KeyboardInput.GetCharFromKey(key)} already exists and will be ignored.",
                LogLevel.Warning
            );
            return false;
        }

        return true;
    }

    public static bool RegisterBinding(Keys key, string pageId) => RegisterBinding(
        key,
        () =>
        {
            var page = RanchMayhemEngine.UiManager?.GetPage(pageId);
            page?.ToggleVisibility();
            Logger.Log($"toggling page {pageId}");
        }
    );

    public static void ResetPressedKey()
    {
        PressedKey = Keys.None;
    }

    public static void Update()
    {
        if (IsInTextBox) return;

        var currentState = KeyboardInput.CurrentState;
        var pressed = currentState.GetPressedKeys();
        if (pressed.Length == 0) return;

        var key = pressed[0];
        if (!KeyboardInput.IsNewKeyPress(key)) return;

        PressedKey = key;

        if (_keyBindings.TryGetValue(key, out var action))
        {
            action?.Invoke();
        }
    }
}
