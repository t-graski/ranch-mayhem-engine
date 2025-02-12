using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class UIManager(SpriteBatch spriteBatch)
{
    private List<UIComponent> _components;
    private SpriteBatch _spriteBatch = spriteBatch;

    public void Initialize()
    {
        _components = [];
    }

    public void AddComponent(UIComponent component)
    {
        _components.Add(component);
    }

    public void UpdateComponents(MouseState mouseState)
    {
        foreach (var component in _components)
        {
            component.HandleMouse(mouseState);
            component.Update();
        }
    }

    public void RenderComponents()
    {
        foreach (var component in _components)
        {
            component.Draw(_spriteBatch);
        }
    }
}