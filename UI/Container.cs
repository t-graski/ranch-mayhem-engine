using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ranch_mayhem_engine.UI;

public class Container : UiComponent
{
    private List<UiComponent> _components;
    public Texture2D Background { get; private set; }


    public Container(
        string id, UiComponentOptions options, List<UiComponent> components, UiComponent? parent = null, Texture2D? background = null
    ) :
        base(id, options, parent)
    {
        InitializeContainer(components);
        Background = background;
    }

    private void InitializeContainer(List<UiComponent> components)
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

    public override void SetParent(UiComponent parent)
    {
        base.SetParent(parent);
        UpdateParentLocation();
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }

        foreach (var component in _components)
        {
            foreach (var command in component.Draw())
            {
                component.HandleMouse(RanchMayhemEngine.MouseState);
                yield return command;
            }

            // component.Draw(spriteBatch);
            // component.HandleMouse(RanchMayhemEngine.MouseState);
        }
    }

    public List<UiComponent> GetChildren() => _components;

    public T GetChildById<T>(string id) where T : UiComponent => (T)_components.FirstOrDefault(c => c.Id.Equals(id))!;

    public T GetFirstChild<T>() where T : UiComponent => (T)_components.FirstOrDefault()!;

    public T GetNthChild<T>(int index) where T : UiComponent => (T)_components[index];

    public override void Update()
    {
    }
}
