using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class Page
{
    public string Id { get; set; }
    public List<UIComponent> Components { get; set; } = [];
    public bool IsVisible { get; set; } = false;

    public abstract Page Initialize();

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        foreach (var component in Components)
        {
            component.Draw(spriteBatch);
        }
    }

    public virtual void Update(MouseState mouseState)
    {
        foreach (var component in Components)
        {
            component.Update();
            component.HandleMouse(mouseState);
        }
    }

    public void ToggleVisibility() => IsVisible = !IsVisible;
}