using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    private Color _color;

    public Box(string id, Color color, UIAnchor uiAnchor, Vector2 size, UIComponent parent) : base(id, color, uiAnchor,
        size, parent)
    {
        _color = color;
    }

    public Box(string id, Color color, Vector2 position, Vector2 size) : base(id, color, position, size)
    {
        _color = color;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
        texture.SetData([_color]);
        if (_parent is null)
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)_localPosition.X, (int)_localPosition.Y, (int)Size.X, (int)Size.Y), _color);
        }
        else
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)_globalPosition.X, (int)_globalPosition.Y, (int)Size.X, (int)Size.Y), _color);
        }
    }


    public override void Update()
    {
    }
}