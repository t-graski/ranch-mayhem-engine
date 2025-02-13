using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Button : UIComponent
{
    // public Button(string id, Texture2D texture, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null) : base(id,
    //     texture, uiAnchor, size, parent)
    // {
    //     _onHover = HandleOnHover;
    //     _onClick = HandleOnClick;
    //     _offHover = HandleOffHover;
    //     _offClick = HandleOffClick;
    // }
    //
    //
    // public Button(string id, Texture2D texture, Vector2 position, Vector2 size) : base(id, texture, position, size)
    // {
    //     _onHover = HandleOnHover;
    //     _onClick = HandleOnClick;
    // }
    //
    // private void HandleOnHover()
    // {
    //     Console.WriteLine($"{GetType().FullName}::HandleOnHover {Id}");
    //     _texture = RanchMayhemEngine.ContentManager.GetTexture("button_hover");
    // }
    //
    // private void HandleOnClick()
    // {
    //     Console.WriteLine($"{GetType().FullName}::HandleOnClick {Id}");
    //     _texture = RanchMayhemEngine.ContentManager.GetTexture("button_click");
    // }
    //
    // private void HandleOffHover()
    // {
    //     Console.WriteLine($"{GetType().FullName}::HandleOffHover {Id}");
    //     _texture = RanchMayhemEngine.ContentManager.GetTexture("button");
    // }
    //
    // private void HandleOffClick()
    // {
    //     Console.WriteLine($"{GetType().FullName}::HandleOffClick {Id}");
    //     _texture = RanchMayhemEngine.ContentManager.GetTexture("button_hover");
    // }

    public override void Update()
    {
    }

    public Button(string id, UIComponentOptions options, UIComponent parent = null) : base(id, options, parent)
    {
    }
}