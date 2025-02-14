using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class UIManager
{
    private const int ReferenceWidth = 1920;
    private const int ReferenceHeight = 1080;

    private float _globalScaleX;
    private float _globalScaleY;

    public Vector2 GlobalScale;

    public GraphicsDevice GraphicsDevice { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; }

    private List<UIComponent> _components;

    public UIManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        Console.WriteLine($"{GetType().FullName}::ctor");
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;

        var viewport = GraphicsDevice.Viewport;

        _globalScaleX = (float)viewport.Width / ReferenceWidth;
        _globalScaleY = (float)viewport.Height / ReferenceHeight;
        GlobalScale = new Vector2(_globalScaleX, _globalScaleY);
        Console.WriteLine($"Setting global scale to {GlobalScale}");
    }

    public void Initialize()
    {
        _components = [];
    }

    public void AddComponent(UIComponent component)
    {
        Console.WriteLine($"adding component with id {component.Id}");
        _components.Add(component);
    }

    public void AddComponents(IEnumerable<UIComponent> components)
    {
        _components.AddRange(components);
    }

    public UIComponent GetComponent(string id)
    {
        return _components.Find(component => component.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
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
        // if (!RanchMayhemEngine.IsFocused) return;

        foreach (var component in _components)
        {
            component.Draw(SpriteBatch);
        }
    }
}