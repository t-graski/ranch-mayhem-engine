using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI.Helper;

public class UiComponentBuilder<TBuilder> where TBuilder : UiComponentBuilder<TBuilder>
{
    protected readonly UiComponentOptions _componentOptions;
    protected readonly string _id;
    protected UiComponent _parent;
    protected UiComponent _hoverUiComponent;
    protected bool _visible;

    protected Action _onClickAction;
    protected Action _onHoverAction;

    protected UiComponentBuilder(string id, UiComponentOptions options)
    {
        _id = id;
        _componentOptions = options;
    }

    public TBuilder SetVisible(bool visible)
    {
        _visible = visible;
        return (TBuilder)this;
    }

    public TBuilder SetHoverUiComponent(UiComponent hover)
    {
        _hoverUiComponent = hover;
        return (TBuilder)this;
    }

    public TBuilder SetParent(UiComponent parent)
    {
        _parent = parent;
        return (TBuilder)this;
    }

    public TBuilder SetColor(Color color)
    {
        _componentOptions.Color = color;
        return (TBuilder)this;
    }

    public TBuilder SetPosition(Vector2 position)
    {
        _componentOptions.Position = position;
        return (TBuilder)this;
    }

    public TBuilder SetPosition(float x, float y) => SetPosition(new Vector2(x, y));

    public TBuilder SetUiAnchor(UiAnchor anchor)
    {
        _componentOptions.UiAnchor = anchor;
        return (TBuilder)this;
    }

    public TBuilder CenterXY()
    {
        _componentOptions.UiAnchor = UiAnchor.CenterX | UiAnchor.CenterY;
        return (TBuilder)this;
    }

    public TBuilder SetUiAnchorOffSet(Vector2 offset)
    {
        _componentOptions.UiAnchorOffset = offset;
        return (TBuilder)this;
    }

    public TBuilder SetUiAnchorOffset(float x, float y) => SetUiAnchorOffSet(new Vector2(x, y));

    public TBuilder SetSize(Vector2 size)
    {
        _componentOptions.Size = size;
        _componentOptions.SizeUnit = SizeUnit.Pixels;
        return (TBuilder)this;
    }

    public TBuilder SetSize(float x, float y) => SetSize(new Vector2(x, y));

    public TBuilder SetSizePercent(Vector2 size)
    {
        _componentOptions.SizePercent = size;
        _componentOptions.SizeUnit = SizeUnit.Percent;
        return (TBuilder)this;
    }

    public TBuilder SetSizePercent(float x, float y) => SetSizePercent(new Vector2(x, y));

    public TBuilder SetScale(Vector2 scale)
    {
        _componentOptions.Scale = scale;
        return (TBuilder)this;
    }

    public TBuilder SetScale(float x, float y) => SetScale(new Vector2(x, y));

    public TBuilder SetBorderColor(Color color)
    {
        _componentOptions.BorderColor = color;
        return (TBuilder)this;
    }

    public TBuilder SetBorderTexture(Texture2D texture)
    {
        _componentOptions.BorderTexture = texture;
        return (TBuilder)this;
    }

    public TBuilder SetBorderCornerTexture(Texture2D texture)
    {
        _componentOptions.BorderCornerTexture = texture;
        return (TBuilder)this;
    }

    public TBuilder SetBorderSize(int size)
    {
        _componentOptions.BorderSize = size;
        return (TBuilder)this;
    }

    public TBuilder SetBorderOrientation(BorderOrientation orientation)
    {
        _componentOptions.BorderOrientation = orientation;
        return (TBuilder)this;
    }

    public TBuilder SetBorderPosition(BorderPosition position)
    {
        _componentOptions.BorderPosition = position;
        return (TBuilder)this;
    }

    public TBuilder SetBorder(Texture2D texture, int size)
    {
        SetBorderTexture(texture);
        SetBorderSize(size);
        return (TBuilder)this;
    }

    public TBuilder SetBorder(Texture2D texture, int size, BorderOrientation borderOrientation,
        BorderPosition borderPosition)
    {
        SetBorderTexture(texture);
        SetBorderSize(size);
        SetBorderOrientation(borderOrientation);
        SetBorderPosition(borderPosition);
        return (TBuilder)this;
    }
}