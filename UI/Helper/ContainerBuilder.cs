using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;
using Vector2 = System.Numerics.Vector2;

namespace ranch_mayhem_engine.UI.Helper;

public class ContainerBuilder
{
    private readonly UiComponentOptions _componentOptions;
    private readonly string _id;
    private UiComponent _parent;
    private List<UiComponent> _children;
    private Action _onClickAction;

    public ContainerBuilder(string id)
    {
        _id = id;
        _componentOptions = new UiComponentOptions();
    }

    public ContainerBuilder SetParent(UiComponent parent)
    {
        _parent = parent;
        return this;
    }

    public ContainerBuilder SetColor(Color color)
    {
        _componentOptions.Color = color;
        return this;
    }

    public ContainerBuilder SetPosition(Vector2 position)
    {
        _componentOptions.Position = position;
        return this;
    }

    public ContainerBuilder SetPosition(float x, float y) => SetPosition(new Vector2(x, y));

    public ContainerBuilder SetSize(Vector2 size)
    {
        _componentOptions.Size = size;
        return this;
    }

    public ContainerBuilder SetSize(float x, float y) => SetSize(new Vector2(x, y));

    public ContainerBuilder SetBorderTexture(Texture2D texture)
    {
        _componentOptions.BorderTexture = texture;
        return this;
    }

    public ContainerBuilder SetBorderColor(Color color)
    {
        _componentOptions.BorderColor = color;
        return this;
    }

    public ContainerBuilder SetBorderPosition(BorderPosition position)
    {
        _componentOptions.BorderPosition = position;
        return this;
    }

    public ContainerBuilder SetBorderTexture(string textureId)
    {
        _componentOptions.BorderTexture = ContentManager.GetTexture(textureId);
        return this;
    }

    public ContainerBuilder SetBorderSize(int size)
    {
        _componentOptions.BorderSize = size;
        return this;
    }

    public ContainerBuilder SetBorderOrientation(BorderOrientation orientation)
    {
        _componentOptions.BorderOrientation = orientation;
        return this;
    }

    public ContainerBuilder SetChildren(List<UiComponent> children)
    {
        _children = children;
        return this;
    }

    public ContainerBuilder SetOnClick(Action onClickAction)
    {
        _onClickAction = onClickAction;
        return this;
    }

    public Container Build()
    {
        var container = new Container(_id, _componentOptions, _children, _parent)
        {
            OnClick = _onClickAction
        };

        return container;
    }
}
