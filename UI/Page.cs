using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class Page
{
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
    protected bool IsVisible { get; set; } = false;
    protected List<UiComponent> Components { get; set; } = [];

    public abstract Page Initialize();

    protected void AddComponent(UiComponent component)
    {
        if (Components.Find(c => c.Id.Equals(component.Id)) != null)
        {
            Logger.Log(
                $"{GetType().FullName}::AddComponent Id={component.Id} Adding component with already existing Id. This might cause unexpected issues.",
                LogLevel.Warning);
        }

        component.IsVisible = IsVisible;
        Components.Add(component);
    }

    // public virtual void Draw(SpriteBatch spriteBatch)
    // {
    //     if (!IsVisible) return;
    //
    //     foreach (var component in Components)
    //     {
    //         component.Draw(spriteBatch);
    //     }
    // }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void Update(MouseState mouseState)
    {
        foreach (var component in Components)
        {
            component.Update();
            component.HandleMouse(mouseState);
        }
    }

    public virtual void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        Components.ForEach(c => c.IsVisible = IsVisible);
    }
}