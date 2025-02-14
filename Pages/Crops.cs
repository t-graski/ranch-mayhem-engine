using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class Crops : Page
{
    public override Page Initialize()
    {
        Id = "crops";

        List<string> cropsNames = ["bell_pepper", "coconut", "lychee", "blueberry", "dragon_fruit", "corn"];
        var contentManager = RanchMayhemEngine.ContentManager;

        List<UIComponent> cropComponents = [];

        foreach (var crop in cropsNames)
        {
            var box = new Box($"crops-{crop}", new UIComponentOptions
            {
                Texture = contentManager.GetTexture(crop),
                SizePercent = new Vector2(0, 100),
                SizeUnit = SizeUnit.Percent,
                UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
            });

            cropComponents.Add(box);
        }

        var cropInventory = new Grid("crop-inventory", new UIComponentOptions
            {
                Color = Color.MediumAquamarine,
                Position = new Vector2(100),
                Size = new Vector2(1720, 664)
            },
            new Grid.GridOptions
            {
                Columns = [1, 1, 1],
                ColumnGap = 0,
                Rows = [1, 1],
                RowGap = 0,
                Padding = new Vector4(5)
            }, cropComponents);

        Components.Add(cropInventory);
        return this;
    }
}