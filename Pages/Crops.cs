﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

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

        List<string> cropsNames = ["wheat", "potato", "carrot", "cucumber", "tomato"];
        List<string> amounts = ["100", "47.4k", "462k", "1M", "237"];
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

            // TODO: Text CenterX doesn't work properly
            var cropText = new Text($"crops-{i}-text", new Text.TextOptions
            {
                Content = "Amount: 0",
                FontSize = 12,
                FontColor = Color.Orange,
                UiAnchor = UIAnchor.Bottom | UIAnchor.CenterX
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

        _titleText = new Text("crop-title", new Text.TextOptions
        {
            Content = "",
            FontSize = 16,
            FontColor = Color.Orange,
            UiAnchor = UIAnchor.CenterX
        });

        _infoText = new Text("crop-info", new Text.TextOptions
        {
            Content = "",
            FontSize = 10,
            FontColor = Color.Gray,
            Position = new Vector2(8, 60)
        });

        _cropDetails = new Container("crop-details", new UIComponentOptions
        {
            Color = Color.Green,
            Position = new Vector2(1600, 100),
            Size = new Vector2(282, 664),

            BorderTexture = RanchMayhemEngine.ContentManager.GetTexture("planks_oak"),
            BorderSize = 6,
            BorderOrientation = BorderOrientation.Outside
        }, [_titleText, _infoText]);

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

            Logger.Log(
                $"global pos container: {_cropDetails.GlobalPosition}, title global pos: {_titleText.GlobalPosition}");
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