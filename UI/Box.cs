using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    public Box(string id, BoxOptions options, UIComponent parent = null, bool scale = true) : base(id, options,
        parent, scale)
    {
    }

    public class BoxOptions : UIComponentOptions
    {
    }

    public override void Update()
    {
    }
}