using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class Crops : Page
{
    public override Page Initialize()
    {
        Id = "crops";
        IsVisible = false;

        List<string> cropsNames = ["wheat", "potato", "carrot", "cucumber", "tomato"];
        var contentManager = RanchMayhemEngine.ContentManager;

        List<UIComponent> cropComponents = [];

        for (var i = 0; i < cropsNames.Count; i++)
        {
            var crop = new Box($"crops-{i}", new Box.BoxOptions
            {
                Texture = contentManager.GetSprite(cropsNames[i]),
                SizePercent = new Vector2(0, 80),
                UiAnchor = UIAnchor.CenterX,
                SizeUnit = SizeUnit.Percent,
            });

            // TODO: Text CenterX doesn't work properly
            var cropText = new Text($"crops-{i}-text", new Text.TextOptions
            {
                Content = "Amount: 0",
                FontSize = 12,
                FontColor = Color.Orange,
                UiAnchor = UIAnchor.Bottom
            });

            var container = new Container($"crops-container-{i}", new UIComponentOptions
            {
                BorderColor = Color.Goldenrod,
                BorderSize = 2,
                BorderOrientation = BorderOrientation.Outside
            }, [crop, cropText]);

            cropComponents.Add(container);
        }

        var cropInventory = new Grid("crop-inventory", new Grid.GridOptions
        {
            Color = Color.MediumAquamarine,
            Position = new Vector2(360, 100),
            Size = new Vector2(1200, 664),
            Columns = [1, 1, 1, 1, 1],
            ColumnGap = 25,
            Rows = [1, 1, 1],
            RowGap = 20,
            Padding = new Vector4(15),

            // BorderTexture = RanchMayhemEngine.ContentManager.GetTexture("planks_oak"),
            // BorderSize = 6,
            // BorderOrientation = BorderOrientation.Outside
        }, cropComponents);


        Components.Add(cropInventory);

        return this;
    }
}