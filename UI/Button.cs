using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Button : UIComponent
{
    public Button(string id, Texture2D texture, Vector2 position) : this(id, texture, position,
        new Vector2(texture.Width, texture.Height))
    {
    }

    public Button(string id, Texture2D texture, Vector2 position, Vector2 size) : base(id, texture, position, size)
    {
        _onHover = HandleOnHover;
        _onClick = HandleOnClick;
    }

    private void HandleOnHover() => Console.WriteLine($"Hovering {Id}");
    private void HandleOnClick() => Console.WriteLine($"Clicked {Id}");

    public override void Update()
    {
    }
}