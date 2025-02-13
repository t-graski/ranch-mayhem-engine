using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Container : UIComponent
{
    private List<UIComponent> _components;

    public bool IsShown { get; private set; } = true;

    private void InitializeContainer(List<UIComponent> components)
    {
        _components = components ?? [];

        foreach (var component in _components)
        {
            component.SetParent(this);
        }
    }

    // public Container(string id, Color color, Vector2 position, Vector2 size, UIComponent parent = null,
    //     List<UIComponent> components = null) : base(id,
    //     color, position, size, parent)
    // {
    //     InitializeContainer(components);
    // }
    //
    // public Container(string id, Color color, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null,
    //     List<UIComponent> components = null) : base(id,
    //     color, uiAnchor, size, parent)
    // {
    //     InitializeContainer(components);
    // }
    //
    // public Container(string id, Texture2D texture, Vector2 position, Vector2 size, UIComponent parent = null,
    //     List<UIComponent> components = null) : base(id,
    //     texture, position, size, parent)
    // {
    //     InitializeContainer(components);
    // }
    //
    // public Container(string id, Texture2D texture, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null,
    //     List<UIComponent> components = null) : base(
    //     id, texture, uiAnchor, size, parent)
    // {
    //     InitializeContainer(components);
    // }
    //
    // public Container(string id, Texture2D texture, Vector2 position, float scale, UIComponent parent = null,
    //     List<UIComponent> components = null) : base(id,
    //     texture, position, scale, parent)
    // {
    //     InitializeContainer(components);
    // }
    //
    //
    // public override void Draw(SpriteBatch spriteBatch)
    // {
    //     if (!IsShown) return;
    //     var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
    //     texture.SetData([_color]);
    //     RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
    //         new Rectangle((int)_localPosition.X, (int)_localPosition.Y, (int)Size.X, (int)Size.Y), _color);
    //
    //     foreach (var component in _components)
    //     {
    //         component.Draw(spriteBatch);
    //     }
    // }

    public override void Update()
    {
    }

    public Container(string id, UIComponentOptions options, UIComponent parent = null) : base(id, options, parent)
    {
    }
}