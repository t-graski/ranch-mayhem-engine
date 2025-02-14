using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Grid : UIComponent
{
    private List<UIComponent> _components;
    private GridOptions _gridOptions;

    private void InitializeGrid(List<UIComponent> components)
    {
        _components = components ?? [];

        foreach (var component in _components)
        {
            component?.SetParent(this);
        }

        CalculatePositions();
    }

    private void CalculatePositions()
    {
        var position = Vector2.Zero;
        var size = Vector2.Zero;

        for (var i = 0; i < _components.Count; i++)
        {
            var current = _components[i];

            var isFirstColumn = i % _gridOptions.Columns.Count == 0;
            var isLastColumn = (i + 1) % _gridOptions.Columns.Count == 0;
            var isFirstRow = i / _gridOptions.Columns.Count == 0;
            var isLastRow = i / _gridOptions.Columns.Count == _gridOptions.Rows.Count - 1;

            var column = i % _gridOptions.Columns.Count;
            var row = i / _gridOptions.Columns.Count;

            var columnSum = _gridOptions.Columns.Sum();
            var rowSum = _gridOptions.Rows.Sum();

            var scaledGaps = ScaleToGlobal(new Vector2(_gridOptions.ColumnGap, _gridOptions.RowGap));
            var scaledPadding = ScaleToGlobal(_gridOptions.Padding);

            size.X = (float)_gridOptions.Columns[column] / columnSum * (Options.Size.X -
                                                                        (_gridOptions.Columns.Count - 1) *
                                                                        scaledGaps.X - scaledPadding.Y -
                                                                        scaledPadding.W);

            size.Y = (float)_gridOptions.Rows[row] / rowSum * (Options.Size.Y -
                                                               (_gridOptions.Rows.Count - 1) * scaledGaps.Y -
                                                               scaledPadding.X - scaledPadding.Z);
            if (isFirstRow && isFirstColumn)
            {
                position.Y += scaledPadding.X;
                position.X += scaledPadding.W;
            }

            if (current.Options.UiAnchor != UIAnchor.None)
            {
                current.RecalculateSize(current.Options.Size, size);
                var itemPosition = current.Options.UiAnchor.CalculatePosition(current.Options.Size, size);

                current.UpdatePosition(position + itemPosition, current.Options.Size, this, size);
            }
            else
            {
                current.UpdatePosition(position, size, this, size);

                if (current is Container container)
                {
                    container.UpdateParentLocation();
                }
            }

            if (!isLastColumn)
            {
                position.X += size.X + scaledGaps.X;
            }

            if (isLastColumn)
            {
                position.X = scaledPadding.W;
                position.Y += size.Y + scaledGaps.Y;
            }
        }
    }

    public Grid(string id, UIComponentOptions options, GridOptions gridOptions, List<UIComponent> components,
        UIComponent parent = null) : base(id,
        options, parent)
    {
        _gridOptions = gridOptions;
        InitializeGrid(components);
    }

    public override void Draw(SpriteBatch spriteBatch)
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

        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
            component.HandleMouse(RanchMayhemEngine.MouseState);
        }
    }

    public override void Update()
    {
    }

    public class GridOptions
    {
        public List<int> Columns;
        public List<int> Rows;
        public int ColumnGap;
        public int RowGap;

        public Vector4 Padding = Vector4.Zero;
    }
}