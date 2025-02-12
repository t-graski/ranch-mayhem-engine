using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    private Color _color;
    private Vector2 _size;

    public Box(string id, Color color, UIAnchor uiAnchor, Vector2 size) : base(id, color, uiAnchor, size)
    {
        _color = color;
        _size = size;
    }

    public Box(string id, Color color, Vector2 position, Vector2 size) : base(id, color, position, size)
    {
        _color = color;
        _size = size;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
        texture.SetData([_color]);

        RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
            new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y), _color);
    }


    public override void Update()
    {
    }
}