using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Container : UIComponent
{
    private List<UIComponent> _components;

    public Container(string id, UIComponentOptions options, List<UIComponent> components, UIComponent parent = null) :
        base(id, options, parent)
    {
        InitializeContainer(components);
    }

    private void InitializeContainer(List<UIComponent> components)
    {
        _components = components ?? [];
        UpdateParentLocation();
    }

    public void UpdateParentLocation()
    {
        foreach (var component in _components)
        {
            // Logger.Log(
            // $"{GetType().FullName}::UpdateParentLocation Id:{Id} updating id:{component.Id} parent global: {GlobalPosition} parent local: {LocalPosition}");
            component.SetParent(this);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
            component.HandleMouse(RanchMayhemEngine.MouseState);
        }
    }

    public List<UIComponent> GetChildren() => _components;

    public T? GetChildById<T>(string id) where T : UIComponent => (T)_components.FirstOrDefault(c => c.Id.Equals(id));

    public T? GetFirstChild<T>() where T : UIComponent => (T)_components.FirstOrDefault();

    public override void Update()
    {
    }
}