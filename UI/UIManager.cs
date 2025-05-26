using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ranch_mayhem_engine.UI;

public class UiManager
{
    private const int ReferenceWidth = 1920;
    private const int ReferenceHeight = 1080;

    private readonly float _globalScaleX;
    private readonly float _globalScaleY;

    public Vector2 GlobalScale;

    public GraphicsDevice GraphicsDevice { get; private set; }
    private SpriteBatch SpriteBatch { get; set; }

    private List<Page> _pages;

    private Texture2D Background { get; set; }

    public UiManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        Logger.Log($"{GetType().FullName}::ctor", LogLevel.Internal);
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;

        var viewport = GraphicsDevice.Viewport;

        _globalScaleX = (float)viewport.Width / ReferenceWidth;
        _globalScaleY = (float)viewport.Height / ReferenceHeight;
        GlobalScale = new Vector2(_globalScaleX, _globalScaleY);
    }

    public void Initialize()
    {
        _pages = [];
    }

    public void AddComponent(Page page)
    {
        _pages.Add(page);
    }

    public void SetBackground(Texture2D texture)
    {
        Background = texture;
    }

    public Page? GetPage(string id)
    {
        return _pages.Find(component => component.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
    }

    public void UpdateComponents(MouseState mouseState)
    {
        foreach (var page in _pages)
        {
            // Logger.Log($"{GetType().FullName}::UpdateComponents");
            page.Update(mouseState);
        }
    }

    public void RenderBackground()
    {
        if (Background != null)
        {
            SpriteBatch.Draw(Background, new Rectangle(0, 0, RanchMayhemEngine.Width, RanchMayhemEngine.Height),
                Color.White);
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
