using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    public Box(string id, UIComponentOptions options, UIComponent parent = null) : base(id, options,
        parent)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
        texture.SetData([_options.Color]);
        if (Parent is null)
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)_localPosition.X, (int)_localPosition.Y, (int)_options.Size.X, (int)_options.Size.Y),
                _options.Color);
        }
        else
        {
            RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                new Rectangle((int)_globalPosition.X, (int)_globalPosition.Y, (int)_options.Size.X,
                    (int)_options.Size.Y), _options.Color);
        }
    }


    public override void Update()
    {
    }
}