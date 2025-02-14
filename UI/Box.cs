using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    public Box(string id, UIComponentOptions options, UIComponent parent = null, bool scale = true) : base(id, options,
        parent, scale)
    {
        OnClick = () =>
        {
            var component = RanchMayhemEngine.UIManager.GetComponent("test-container");
            if (component is Container container)
            {
                container.ToggleVisibility();
            }
        };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Options.Texture != null) base.Draw(spriteBatch);
        else
        {
            var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
            texture.SetData([Options.Color]);
            if (Parent is null)
            {
                RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                    new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, (int)Options.Size.X, (int)Options.Size.Y),
                    Options.Color);
            }
            else
            {
                RanchMayhemEngine.UIManager.SpriteBatch.Draw(texture,
                    new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                        (int)Options.Size.Y), Options.Color);
            }
        }
    }


    public override void Update()
    {
    }
}