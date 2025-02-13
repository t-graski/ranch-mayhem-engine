using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Grid : UIComponent
{
    private List<UIComponent> _components;

    private void InitializeGrid(List<UIComponent> components)
    {
        _components = components ?? [];
    }

    // public Grid(string id, Color color, Vector2 position, Vector2 size, UIComponent parent = null) : base(id, color,
    //     position, size, parent)
    // {
    // }
    //
    // public Grid(string id, Color color, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null) : base(id, color,
    //     uiAnchor, size, parent)
    // {
    // }
    //
    // public Grid(string id, Texture2D texture, Vector2 position, Vector2 size, UIComponent parent = null) : base(id,
    //     texture, position, size, parent)
    // {
    // }
    //
    // public Grid(string id, Texture2D texture, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null) : base(id,
    //     texture, uiAnchor, size, parent)
    // {
    // }
    //
    // public Grid(string id, Texture2D texture, Vector2 position, float scale, UIComponent parent = null) : base(id,
    //     texture, position, scale, parent)
    // {
    // }

    public override void Update()
    {
    }

    public class GridOptions
    {
    }

    public Grid(string id, UIComponentOptions options, UIComponent parent = null) : base(id, options, parent)
    {
    }
}