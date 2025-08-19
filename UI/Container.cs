using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Container : UiComponent
{
    private List<UiComponent> _components;
    public Texture2D Background { get; private set; }

    public Texture2D Overlay { get; private set; }
    public Effect OverlayShader { get; private set; }
    public Vector2 OverlayOffset { get; private set; }

    public Container(
        string id, UiComponentOptions options, List<UiComponent> components, UiComponent? parent = null, Texture2D? background = null,
        Effect? renderShader = null
    ) :
        base(id, options, parent, true, renderShader)
    {
        InitializeContainer(components);
        Background = background;
    }

    private void InitializeContainer(List<UiComponent> components)
    {
        _components = components ?? [];
        UpdateParentLocation();
    }

    public void SetOverlay(Texture2D texture, Vector2 overlayOffset, Effect? shader = null)
    {
        Overlay = texture;
        OverlayShader = shader;
        OverlayOffset = overlayOffset;
    }

    public void ClearOverlay()
    {
        Overlay = null;
        OverlayShader = null;
        OverlayOffset = Vector2.Zero;
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

    public override void ToggleAnimating()
    {
        base.ToggleAnimating();
        foreach (var component in _components)
        {
            component.IsAnimating = !component.IsAnimating;
        }
    }

    public override void HandleParentGlobalPositionChange(Vector2 position)
    {
        base.HandleParentGlobalPositionChange(position);
        foreach (var component in _components)
        {
            component.HandleParentGlobalPositionChange(position);
        }
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


        if (Overlay is not null)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-container-overlay",
                Texture = Overlay,
                Position = GlobalPosition + ScaleToGlobal(OverlayOffset),
                SourceRect = null,
                Color = Color.White,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = Options.Scale,
                Effects = SpriteEffects.None,
                LayerDepth = 0f,
                Shader = OverlayShader
            };
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
