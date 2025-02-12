using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Button : UIComponent
{
    public Button(Texture2D texture, Vector2 position) : base(texture,
        position)
    {
        _onHover = () => Console.WriteLine("Hovering");
        _onClick = () => Console.WriteLine("Clicked");
    }
}