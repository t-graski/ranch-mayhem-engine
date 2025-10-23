// =========================================================
// Ranch Mayhem — HoverCard.cs
// Author: Tobias Graski
// Created: 10/21/2025 01:10
// Project: ranch-mayhem
// 
// Copyright (c) 2025 Ranch Mayhem. All rights reserved.
// =========================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class HoverCard : UiComponent
{
    private HoverCardOptions _options;
    private readonly List<object> _content;
    private readonly List<UiComponent> _sections = [];
    private Container _container;

    public HoverCard(string id, HoverCardOptions options, List<object> content, UiComponent? parent = null,
        bool scale = true,
        Effect? renderShader = null) : base(id, options, parent, scale, renderShader)
    {
        _options = options;
        _content = content;

        ProcessContent();
        LayoutSections();
    }

    private void ProcessContent()
    {
        for (var i = 0; i < _content.Count; i++)
        {
            var item = _content[i];

            if (item is Divider)
            {
                var divider = new Box($"{Id}-divider-{i}", new Box.BoxOptions
                {
                    Size = new Vector2(100, 2),
                    Color = _options.DividerColor,
                });

                _sections.Add(divider);
            }
            else if (item is Spacer spacer)
            {
                var spacerBox = new Box($"{Id}-spacer-{i}", new Box.BoxOptions
                {
                    Size = new Vector2(1, spacer.Height),
                    Color = Color.Transparent,
                });

                _sections.Add(spacerBox);
            }
            else if (item is UiComponent component)
            {
                _sections.Add(component);
            }
        }
    }

    private void LayoutSections()
    {
        var currentY = _options.Padding;
        var maxWidth = 0f;

        // _options.Padding *= RanchMayhemEngine.UiManager.GlobalScale.Y;
        // _options.SectionSpacing *= RanchMayhemEngine.UiManager.GlobalScale.Y;

        foreach (var section in _sections)
        {
            var sectionSize = section is Text text ? text.GetUnscaledSize() : section.Options.Size;

            if (sectionSize.X > maxWidth)
            {
                maxWidth = sectionSize.X;
            }

            section.SetPosition(new Vector2(_options.Padding, currentY));

            currentY += sectionSize.Y + _options.SectionSpacing;
        }

        var totalHeight = currentY - _options.SectionSpacing + _options.Padding;
        var totalWidth = maxWidth + (_options.Padding * 2);

        foreach (var section in _sections)
        {
            if (section is Box box && section.Id.Contains("divider"))
            {
                box.SetSizePixels(new Vector2(maxWidth, 2));
            }
        }

        _container = new ContainerBuilder($"{Id}-container")
            .SetSize(totalWidth, totalHeight)
            .SetColor(Options.Color)
            .SetTexture(Options.Texture ?? UiManager.Pixel)
            .SetBorderColor(Options.BorderColor)
            .SetBorderPosition(Options.BorderPosition)
            .SetBorderSize(Options.BorderSize)
            .SetPosition(Options.Position)
            .SetChildren(_sections)
            .Build();

        currentY = _options.Padding;
        for (var i = 0; i < _sections.Count; i++)
        {
            var section = _sections[i];
            var sectionSize = section is Text text ? text.GetSize() : section.Options.Size;

            var x = _options.Padding;

            // if (section.Id.Contains("divider"))
            // {
            //     x = (totalWidth - sectionSize.X) / 2;
            // }

            // if (i == 0 && _options.CenterHeading)
            // {
            //     x = (totalWidth - sectionSize.X) / 2;
            // }

            section.SetPosition(new Vector2(x, currentY));
            currentY += sectionSize.Y + _options.SectionSpacing;
        }

        Options.Size = _container.Options.Size;
        Bounds = new Rectangle((int)_container.LocalPosition.X, (int)_container.LocalPosition.Y,
            (int)Options.Size.X, (int)Options.Size.Y);
    }

    public override void SetPosition(Vector2 position)
    {
        _container.SetPosition(position);
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        if (!IsVisible) return [];

        return _container.Draw();
    }

    public override void Update()
    {
        // foreach (var section in _sections)
        // {
        //     section.Update();
        // }
        _container.Update();
    }
}

public class HoverCardOptions : UiComponentOptions
{
    public float Padding { get; set; } = 10f;
    public float SectionSpacing { get; set; } = 5f;
    public bool CenterHeading { get; set; } = true;
    public Color DividerColor { get; set; } = Color.Gray;
}

public class Divider
{
}

public class Spacer(float height = 8f)
{
    public float Height { get; set; } = height;
}