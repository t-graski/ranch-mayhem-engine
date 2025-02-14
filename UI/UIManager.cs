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

    private List<Page> _pages;

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
        _pages = [];
    }

    public void AddComponent(Page page)
    {
        Console.WriteLine($"adding component with id {page.Id}");
        _pages.Add(page);
    }

    public Page GetPage(string id)
    {
        return _pages.Find(component => component.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
    }

    public void UpdateComponents(MouseState mouseState)
    {
        foreach (var page in _pages)
        {
            page.Update(mouseState);
        }
    }

    public void RenderComponents()
    {
        // if (!RanchMayhemEngine.IsFocused) return;

        foreach (var page in _pages)
        {
            page.Draw(SpriteBatch);
        }
    }
}