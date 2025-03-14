using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class Page
{
    public string Id { get; protected set; }
    protected bool IsVisible { get; set; } = false;
    protected List<UIComponent> Components { get; set; } = [];

    public abstract Page Initialize();

    protected void AddComponent(UIComponent component)
    {
        if (Components.Find(c => c.Id.Equals(component.Id)) != null)
        {
            Logger.Log(
                $"{GetType().FullName}::AddComponent Id={component.Id} Adding component with already existing Id. This might cause unexpected issues.",
                Logger.LogLevel.Warning);
        }

        Components.Add(component);
    }

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

    public virtual void ToggleVisibility() => IsVisible = !IsVisible;
}