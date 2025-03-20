using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.Pages;

public class Crops : Page
{
    private string _selectedCrop = "";

    private Animator _detailAnimator;
    private Text _titleText;
    private Text _infoText;
    private Grid _cropGrid;
    private Container _cropDetails;

    private bool _cropDetailsVisible = false;
    private bool _shouldAnimate = false;

    public override Page Initialize()
    {
        Id = "crops";
        IsVisible = false;

        List<string> cropsNames =
        [
            "wheat", "potato", "carrot", "cucumber", "tomato", "cabbage", "watermelon", "bell_pepper", "corn",
            "strawberry", "raspberry", "blueberry", "blackberry",
            "banana", "coconut"
        ];

        List<string> amounts =
        [
            "100", "47.4k", "462k", "1M", "237", "1.2k", "34", "4535M", "34.3k", "437", "12121", "43784k", "437843",
            "34278k", "4374m"
        ];
        var contentManager = RanchMayhemEngine.ContentManager;

        List<UIComponent> cropComponents = [];

        for (var i = 0; i < cropsNames.Count; i++)
        {
            var crop = new Box($"crops-{i}", new Box.BoxOptions
            {
                Texture = contentManager.GetSprite(cropsNames[i]),
                SizePercent = new Vector2(0, 90),
                UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,
                SizeUnit = SizeUnit.Percent,
            });

            var i1 = i;
            var container = new Container($"crops-container-{i}", new UIComponentOptions
            {
                BorderColor = Color.Goldenrod,
                BorderSize = 2,
                BorderOrientation = BorderOrientation.Outside
            }, [crop])
            {
                OnClick = () => { ToggleDetailedView(cropsNames[i1], amounts[i1]); },
            };

            cropComponents.Add(container);
        }

        _cropGrid = new Grid("crop-inventory", new Grid.GridOptions
        {
            Color = Color.MediumAquamarine,
            Position = new Vector2(360, 100),
            Size = new Vector2(1200, 664),
            Columns = [1, 1, 1, 1, 1],
            ColumnGap = 25,
            Rows = [1, 1, 1],
            RowGap = 20,
            Padding = new Vector4(15),

            BorderTexture = RanchMayhemEngine.ContentManager.GetTexture("planks_oak"),
            BorderSize = 6,
            BorderOrientation = BorderOrientation.Outside
        }, cropComponents);

        _titleText = new TextBuilder("crop-title")
            .SetFontSize(16)
            .SetFontColor(Color.Orange)
            .SetUiAnchor(UIAnchor.CenterX)
            .Build();

        _infoText = new TextBuilder("crop-info")
            .SetFontColor(Color.Gray)
            .SetPosition(12, 60)
            .Build();

        _cropDetails = new ContainerBuilder("crop-details")
            .SetColor(Color.Green)
            .SetPosition(1600, 100)
            .SetSize(282, 664)
            .SetBorderTexture("planks_oak")
            .SetBorderSize(6)
            .SetBorderOrientation(BorderOrientation.Outside)
            .SetChildren([_titleText, _infoText])
            .Build();

        _cropDetails.OnClick = () => { Logger.Log("test"); };

        _detailAnimator = new Animator(_cropDetails, Animator.AnimationDirection.Right, 0.33f);

        return this;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        _cropGrid.Draw(spriteBatch);

        if (_cropDetailsVisible)
        {
            _cropDetails.Draw(spriteBatch);

            if (_shouldAnimate)
            {
                _detailAnimator.StartAnimation();
                _shouldAnimate = false;
            }
        }
    }

    public override void Update(MouseState mouseState)
    {
        _detailAnimator.Update();
        base.Update(mouseState);
    }

    private void ToggleDetailedView(string name, string amount)
    {
        if (!_cropDetailsVisible && !_selectedCrop.Equals(name))
        {
            _titleText.SetContent(name?.ToUpper() ?? "");
            _infoText.SetContent($"Amount {amount ?? "0"}");
            _detailAnimator.Reset();
            _selectedCrop = name;
            _shouldAnimate = true;
            _cropDetailsVisible = true;
        }
        else if (_cropDetailsVisible && !_selectedCrop.Equals(name))
        {
            _titleText.SetContent(name?.ToUpper() ?? "");
            _infoText.SetContent($"Amount {amount ?? "0"}");
            _selectedCrop = name;
            _shouldAnimate = false;
            _cropDetailsVisible = true;
        }
        else
        {
            _titleText.SetContent("");
            _infoText.SetContent("");
            _selectedCrop = "";
            _shouldAnimate = false;
            _cropDetailsVisible = false;
        }
    }
}