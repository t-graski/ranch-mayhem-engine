using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Grid : UiComponent
{
    public List<UiComponent> Components;
    private readonly GridOptions _gridOptions;

    public Grid(
        string id, GridOptions options, List<UiComponent> components,
        UiComponent? parent = null, Effect? renderShader = null
    ) : base(
        id,
        options,
        parent,
        true,
        renderShader
    )
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
            Logger.Log($"{prefix} ColumnGap is negative ({gridOptions.ColumnGap})", LogLevel.Warning);
        }

        if (gridOptions.RowGap < 0)
        {
            Logger.Log($"{prefix} RowGap is negative ({gridOptions.RowGap})", LogLevel.Warning);
        }

        if (childrenAmount < gridOptions.Columns.Count * gridOptions.Rows.Count)
        {
            Logger.Log(
                $"{prefix} More columns/rows ({gridOptions.Columns.Count}x{gridOptions.Rows.Count}) defined than used ({childrenAmount}).",
                LogLevel.Warning
            );
        }
    }

    public void SetComponents(List<UiComponent> components)
    {
        InitializeGrid(components);
    }

    private void InitializeGrid(List<UiComponent> components)
    {
        Components = components ?? [];

        foreach (var component in Components)
        {
            component.SetParent(this);
        }

        CalculatePositions();
    }

    public override void SetParent(UiComponent parent)
    {
        base.SetParent(parent);

        foreach (var component in Components)
        {
            component.SetParent(this);
        }

        CalculatePositions();
    }

    public void CalculatePositions()
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
                // current.RecalculateSize(size, size);

                if (current.Options.UiAnchor != UiAnchor.None)
                {
                    current.RecalculateSize(current.Options.Size, size);
                    var itemPosition = current.Options.UiAnchor.CalculatePosition(current.Options.Size, size);

                    current.UpdatePosition(position + itemPosition, current.Options.Size, this, size);
                }
                else
                {
                    current.UpdatePosition(position, size, this, size);
                }

                if (current is Container container)
                {
                    // Logger.Log($"{GetType().FullName}::CalculatePositions Id={Id} updating parent location for {current.Id}");
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

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }
    }

    private IEnumerable<RenderCommand> DrawConnections()
    {
        for (var c = 0; c < _gridOptions.Columns.Count; c++)
        {
            for (var r = 0; r < _gridOptions.Rows.Count; r++)
            {
                if (IsLastRow(r) && !_gridOptions.DrawConnectionToBottom)
                {
                    continue;
                }

                var currIdx = r * _gridOptions.Columns.Count + c;
                var curr = Components[currIdx];

                if (IsLastRow(r))
                {
                    yield return new RenderCommand
                    {
                        Id = $"{Id}-connection-bottom-{curr.Id}",
                        Position = new Vector2(curr.GlobalPosition.X + (curr.Options.Size.X / 2 - 4),
                            curr.GlobalPosition.Y + curr.Options.Size.Y - 1),
                        Texture = UiManager.Pixel,
                        Color = Color.White,
                        SourceRect = null,
                        Rotation = 0f,
                        Origin = Vector2.Zero,
                        Scale = new Vector2(8,
                            GlobalPosition.Y + Options.Size.Y - (curr.GlobalPosition.Y + curr.Options.Size.Y) + 1),
                        LayerDepth = 0f,
                    };

                    continue;
                }

                if (IsFirstRow(r) && _gridOptions.DrawConnectionFromTop)
                {
                    yield return new RenderCommand
                    {
                        Id = $"{Id}-connection-top-{curr.Id}",
                        Position = new Vector2(curr.GlobalPosition.X + (curr.Options.Size.X / 2 - 4),
                            GlobalPosition.Y),
                        Texture = UiManager.Pixel,
                        Color = Color.White,
                        SourceRect = null,
                        Rotation = 0f,
                        Origin = Vector2.Zero,
                        Scale = new Vector2(8, curr.GlobalPosition.Y - GlobalPosition.Y),
                        LayerDepth = 0f,
                    };
                }


                var nextIdx = currIdx + _gridOptions.Columns.Count;
                var next = Components[nextIdx];

                yield return new RenderCommand
                {
                    Id = $"{Id}-connection-{curr.Id}-{next.Id}",
                    Position = new Vector2(curr.GlobalPosition.X + (curr.Options.Size.X / 2 - 4),
                        curr.GlobalPosition.Y + curr.Options.Size.Y - 1),
                    Texture = UiManager.Pixel,
                    Color = Color.White,
                    SourceRect = null,
                    Rotation = 0f,
                    Origin = Vector2.Zero,
                    Scale = new Vector2(8, next.GlobalPosition.Y - (curr.GlobalPosition.Y + curr.Options.Size.Y) + 1),
                    LayerDepth = 0f,
                };
            }
        }

        yield break;

        bool IsLastRow(int row) => row == _gridOptions.Rows.Count - 1;
        bool IsFirstRow(int row) => row == 0;
    }

    public IEnumerable<RenderCommand> DrawConnectionFromTo(int? fromIdx, int? toIdx, Texture2D texture2D,
        Effect? shader = null)
    {
        if (fromIdx == null && toIdx == null) yield break;
        if (fromIdx != null && (fromIdx < 0 || fromIdx >= Components.Count)) yield break;
        if (toIdx != null && (toIdx < 0 || toIdx >= Components.Count)) yield break;

        var fromComponent = fromIdx != null ? Components[fromIdx.Value] : null;
        var toComponent = toIdx != null ? Components[toIdx.Value] : null;

        const int width = 12;

        var startX = fromComponent != null
            ? fromComponent.GlobalPosition.X + (fromComponent.Options.Size.X / 2 - (width / 2))
            : toComponent != null
                ? toComponent.GlobalPosition.X + (toComponent.Options.Size.X / 2 - (width / 2))
                : GlobalPosition.X + (Options.Size.X / 2 - (width / 2));

        var startY = fromComponent != null
            ? fromComponent.GlobalPosition.Y + fromComponent.Options.Size.Y - 1
            : GlobalPosition.Y;

        var endY = toComponent != null
            ? toComponent.GlobalPosition.Y
            : GlobalPosition.Y + Options.Size.Y;


        var textureSize = new Vector2(texture2D.Width, texture2D.Height);
        var scale = new Vector2(width / textureSize.X, (endY - startY) / textureSize.Y);

        yield return new RenderCommand
        {
            Id = $"{Id}-connection-from-{fromIdx?.ToString() ?? "top"}-to-{toIdx?.ToString() ?? "bottom"}",
            Position = new Vector2(startX, startY),
            Texture = texture2D,
            Color = Color.White,
            SourceRect = null,
            Rotation = 0f,
            Origin = Vector2.Zero,
            Scale = scale,
            LayerDepth = 0f,
            Shader = shader
        };
    }

    public override void Update()
    {
        foreach (var component in Components)
        {
            component.Update();
        }
    }

    public int GetColumnsCnt() => _gridOptions.Columns.Count;
    public int GetRowsCnt() => _gridOptions.Rows.Count;

    public class GridOptions : UiComponentOptions
    {
        public List<int> Columns;
        public List<int> Rows;
        public int ColumnGap;
        public int RowGap;

        public bool ColumnConnection;

        public bool DrawConnectionFromTop;
        public bool DrawConnectionToBottom;

        public Texture2D ConnectionTexture;

        public Vector4 Padding = Vector4.Zero;
    }
}