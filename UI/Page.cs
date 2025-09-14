using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class Page
{
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
    public bool IsVisible { get; set; } = false;
    private List<UiComponent> Components { get; set; } = [];

    public abstract Page Initialize();

    protected void AddComponent(UiComponent component)
    {
        if (Components.Find(c => c.Id.Equals(component.Id)) != null)
        {
            Logger.Log(
                $"{GetType().FullName}::AddComponent Id={component.Id} Adding component with already existing Id. This might cause unexpected issues.",
                LogLevel.Warning
            );
        }

        component.IsVisible = IsVisible;
        Components.Add(component);
    }

    public abstract void Dispose();

    protected void EnableCloseButton()
    {
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

    public abstract void Draw();

    public virtual void Update(MouseState mouseState)
    {
        foreach (var component in Components)
        {
            component.Update();
            component.HandleMouse(mouseState);
        }
    }

    public virtual void ToggleVisibility(bool forceInvisible = false)
    {
        if (forceInvisible)
        {
            IsVisible = false;
            Components.ForEach(c => c.IsVisible = false);
            return;
        }

        IsVisible = !IsVisible;
        Logger.Log($"Toggling visibility for {string.Join(", ", Components.Select(c => c.Id))}");
        Components.ForEach(c => c.IsVisible = IsVisible);
    }
}
