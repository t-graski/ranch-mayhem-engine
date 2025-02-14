using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Container : UIComponent
{
    private List<UIComponent> _components;

    private void InitializeContainer(List<UIComponent> components)
    {
        _components = components ?? [];
        UpdateParentLocation();
    }

    public void UpdateParentLocation()
    {
        foreach (var component in _components)
        {
            component.SetParent(this);
        }
    }

    public Container(string id, UIComponentOptions options, List<UIComponent> components, UIComponent parent = null) :
        base(id, options, parent)
    {
        InitializeContainer(components);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
        texture.SetData([Options.Color]);
        if (Parent is null)
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, (int)Options.Size.X, (int)Options.Size.Y),
                Options.Color);
        }
        else
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                    (int)Options.Size.Y), Options.Color);
        }

        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
            component.HandleMouse(RanchMayhemEngine.MouseState);
        }
    }

    public override void Update()
    {
    }
}