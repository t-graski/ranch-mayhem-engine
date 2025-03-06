using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public class KeyboardManager
{
    public bool IsInTextBox { get; set; }
    private readonly Dictionary<Keys, Page> _keyBindings = new();

    public void RegisterBinding(Keys key, Page page)
    {
        if (!_keyBindings.TryAdd(key, page))
        {
            Logger.Log(
                $"{GetType().FullName}::RegisterBinding key={KeyboardInput.GetCharFromKey(key)} already exists and will be ignored.",
                Logger.LogLevel.Warning);
        }
    }

    public void Update()
    {
        if (IsInTextBox) return;

        var currentState = KeyboardInput.CurrentState;

        if (currentState.GetPressedKeys().Length == 0) return;

        if (!KeyboardInput.IsNewKeyPress(currentState.GetPressedKeys()[0])) return;

        if (_keyBindings.TryGetValue(currentState.GetPressedKeys()[0], out var page))
        {
            page.ToggleVisibility();
        }
    }
}