using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public class Grid : UIComponent
{
    public List<UIComponent> Components;
    private GridOptions _gridOptions;

    public Grid(string id, GridOptions options, List<UIComponent> components,
        UIComponent parent = null) : base(id,
        options, parent)
    {
#if DEBUG
        ParseOptions(options, components.Count);
#endif

        _gridOptions = options;
        InitializeGrid(components);
    }

    private void ParseOptions(GridOptions gridOptions, int childrenAmount)
    {
        var prefix = $"{GetType().FullName}::ctor Id={Id}";

        if (gridOptions.ColumnGap < 0)
        {
            Logger.Log($"{prefix} ColumnGap is negative ({gridOptions.ColumnGap})", Logger.LogLevel.Warning);
        }

        if (gridOptions.RowGap < 0)
        {
            Logger.Log($"{prefix} RowGap is negative ({gridOptions.RowGap})", Logger.LogLevel.Warning);
        }

        if (childrenAmount < gridOptions.Columns.Count * gridOptions.Rows.Count)
        {
            Logger.Log(
                $"{prefix} More columns/rows ({gridOptions.Columns.Count}x{gridOptions.Rows.Count}) defined than used ({childrenAmount}).",
                Logger.LogLevel.Warning);
        }
    }

    private void InitializeGrid(List<UIComponent> components)
    {
        Components = components ?? [];

        foreach (var component in Components)
        {
            component?.SetParent(this);
        }

        CalculatePositions();
    }

    private void CalculatePositions()
    {
        var position = Vector2.Zero;
        var size = Vector2.Zero;

        for (var i = 0; i < Components.Count; i++)
        {
            var current = Components[i];

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

            if (current != null)
            {
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
                        // Logger.Log($"{GetType().FullName}::CalculatePositions Id={Id} updating parent location for {current.Id}");
                        container.UpdateParentLocation();
                    }
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

    // public override void Draw(SpriteBatch spriteBatch)
    // {
    //     base.Draw(spriteBatch);
    //     foreach (var component in _components)
    //     {
    //         component.Draw(spriteBatch);
    //         component.HandleMouse(RanchMayhemEngine.MouseState);
    //     }
    // }

    public override void Update()
    {
    }

    public class GridOptions : UIComponentOptions
    {
        public List<int> Columns;
        public List<int> Rows;
        public int ColumnGap;
        public int RowGap;

        public Vector4 Padding = Vector4.Zero;
    }
}