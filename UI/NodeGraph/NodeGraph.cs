// =========================================================
// Ranch Mayhem — NodeGraph.cs
// Author: Tobias Graski
// Created: 10/19/2025 22:10
// Project: ranch-mayhem
// 
// Copyright (c) 2025 Ranch Mayhem. All rights reserved.
// =========================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;

namespace ranch_mayhem_engine.UI;

public class NodeGraph : UiComponent
{
    private NodeGraphOptions _options;
    private readonly List<Node> _nodes = [];
    private readonly Dictionary<string, int> _idx = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<List<int>> _children = [];
    private readonly List<bool> _state = [];
    private readonly List<Vector2> _worldPosition = [];
    private readonly List<Vector2> _baseSize = [];

    private Vector2 _cameraPosition = Vector2.Zero;
    private float _camZoom = 1f;
    private bool _drag;
    private Vector2 _lastMouse;

    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    // public int Points { get; private set; }

    public Action<Node>? OnAllocated;
    public Action<Node>? OnClickNode;

    public bool DebugShowAllNodes { get; set; } = false;

    public NodeGraph(string id, NodeGraphOptions options, UiComponent? parent = null, bool scale = true,
        Effect? renderShader = null) : base(id, options, parent, scale, renderShader)
    {
        _options = options;
    }

    // public void SetPoints(int p) => Points = Math.Max(0, p);

    public void Load(List<Node> nodes)
    {
        _nodes.Clear();
        _idx.Clear();
        _children.Clear();
        _state.Clear();
        _worldPosition.Clear();
        _baseSize.Clear();

        var rootCount = 0;

        for (var i = 0; i < nodes.Count; i++)
        {
            var n = nodes[i];
            _idx[n.Id] = i;
            if (n.ParentId is null)
            {
                rootCount++;
            }
        }

        if (rootCount != 1)
        {
            Logger.Log($"{GetType().Name}::Load requires exactly ONE root (ParentId == null). Found {rootCount}",
                LogLevel.Error);
        }

        _nodes.AddRange(nodes);

        for (var i = 0; i < _nodes.Count; i++)
        {
            _children.Add([]);
            _state.Add(false);
            _worldPosition.Add(Vector2.Zero);
        }

        for (var i = 0; i < _nodes.Count; i++)
        {
            var p = _nodes[i].ParentId;
            if (p is null) continue;

            if (_idx.TryGetValue(p, out var pIdx))
            {
                _children[pIdx].Add(i);
            }
            else
            {
                Logger.Log($"{GetType().Name} Parent 'p' not found for '{_nodes[i].Id}", LogLevel.Warning);
            }
        }

        var root = RootIndex();
        if (root >= 0)
        {
            Logger.Log($"Root node: {_nodes[root].Id} Allocated at start: {_nodes[root].StartAllocated}");
            _state[root] = _nodes[root].StartAllocated;
        }

        foreach (var n in _nodes)
        {
            n.Component?.SetParent(this);
        }

        LayoutNodes();
        CalculateBounds();
        CenterCameraOn(root);
        ApplyLayoutToComponents();
        UpdateAvailability();
    }

    private void CalculateBounds()
    {
        if (_worldPosition.Count == 0)
        {
            _minBounds = Vector2.Zero;
            _maxBounds = Vector2.Zero;

            return;
        }

        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;

        for (var i = 0; i < _worldPosition.Count; i++)
        {
            var pos = _worldPosition[i];

            var size = _nodes[i].PreferredSize ?? _nodes[i].Component.Options.Size;
            if (size == Vector2.Zero)
            {
                size = new Vector2(50, 50);
            }

            var halfWidth = size.X / 2f;
            var halfHeight = size.Y / 2f;

            minX = MathF.Min(minX, pos.X - halfWidth);
            minY = MathF.Min(minY, pos.Y - halfHeight);
            maxX = MathF.Max(maxX, pos.X + halfWidth);
            maxY = MathF.Max(maxY, pos.Y + halfHeight);
        }

        var padding = _options.PanPadding;
        _minBounds = new Vector2(minX - padding, minY - padding);
        _maxBounds = new Vector2(maxX + padding, maxY + padding);
    }

    // TODO: animate this
    public void RecenterCamera()
    {
        var root = RootIndex();
        if (root >= 0)
        {
            _cameraPosition = _worldPosition[root];
            _camZoom = 1f;
            ApplyLayoutToComponents();
        }
    }

    private void ClampCamera()
    {
        if (!_options.EnablePanLimits) return;

        // FIXED: Use actual viewport size (Options.Size) not window size
        var viewportWidth = Options.Size.X;
        var viewportHeight = Options.Size.Y;

        // Calculate visible area in world space based on zoom
        var viewWidth = viewportWidth / _camZoom;
        var viewHeight = viewportHeight / _camZoom;

        var worldWidth = _maxBounds.X - _minBounds.X;
        var worldHeight = _maxBounds.Y - _minBounds.Y;

        // CHANGED: Add generous overflow for pan limits
        var overflowX = viewWidth * 0.9f; // Allow 90% of view to be empty
        var overflowY = viewHeight * 0.9f;

        if (worldWidth > viewWidth)
        {
            // Content is larger than view - allow scrolling with overflow
            var minCamX = _minBounds.X + viewWidth / 2f - overflowX;
            var maxCamX = _maxBounds.X - viewWidth / 2f + overflowX;
            _cameraPosition.X = Math.Clamp(_cameraPosition.X, minCamX, maxCamX);
        }
        else
        {
            // Content is smaller than view - center it but allow overflow
            var centerX = (_minBounds.X + _maxBounds.X) / 2f;
            _cameraPosition.X = Math.Clamp(_cameraPosition.X, centerX - overflowX, centerX + overflowX);
        }

        if (worldHeight > viewHeight)
        {
            // Content is larger than view - allow scrolling with overflow
            var minCamY = _minBounds.Y + viewHeight / 2f - overflowY;
            var maxCamY = _maxBounds.Y - viewHeight / 2f + overflowY;
            _cameraPosition.Y = Math.Clamp(_cameraPosition.Y, minCamY, maxCamY);
        }
        else
        {
            // Content is smaller than view - center it but allow overflow
            var centerY = (_minBounds.Y + _maxBounds.Y) / 2f;
            _cameraPosition.Y = Math.Clamp(_cameraPosition.Y, centerY - overflowY, centerY + overflowY);
        }
        // if (!_options.EnablePanLimits) return;
        //
        // var viewWidth = Options.Size.X / _camZoom;
        // var viewHeight = Options.Size.Y / _camZoom;
        //
        // var overflowX = viewWidth * 0.5f;
        // var overflowY = viewHeight * 0.5f;
        //
        // var minCamX = _minBounds.X - overflowX;
        // var maxCamX = _maxBounds.X + overflowX;
        // var minCamY = _minBounds.Y - overflowY;
        // var maxCamY = _maxBounds.Y + overflowY;
        //
        // _cameraPosition.X = Math.Clamp(_cameraPosition.X, minCamX, maxCamX);
        // _cameraPosition.Y = Math.Clamp(_cameraPosition.Y, minCamY, maxCamY);
        //
        // var worldWidth = _maxBounds.X - _minBounds.X;
        // var worldHeight = _maxBounds.Y - _minBounds.Y;
        //
        // if (worldWidth > viewWidth)
        // {
        //     var minCamX = _minBounds.X + viewWidth / 2f;
        //     var maxCamX = _maxBounds.X - viewWidth / 2f;
        //     _cameraPosition.X = Math.Clamp(_cameraPosition.X, minCamX, maxCamX);
        // }
        // else
        // {
        //     _cameraPosition.X = (_minBounds.X + _maxBounds.X) / 2;
        // }
        //
        // if (worldHeight > viewHeight)
        // {
        //     var minCamY = _minBounds.Y + viewHeight / 2f;
        //     var maxCamY = _maxBounds.Y - viewHeight / 2f;
        //     _cameraPosition.Y = Math.Clamp(_cameraPosition.Y, minCamY, maxCamY);
        // }
        // else
        // {
        //     _cameraPosition.Y = (_minBounds.Y + _maxBounds.Y) / 2;
        // }
    }

    public void ApplyLayoutToComponents()
    {
        for (var i = 0; i < _nodes.Count; i++)
        {
            var comp = _nodes[i].Component;
            var baseSize = comp.Options.Size;

            if (baseSize == Vector2.Zero && _nodes[i].PreferredSize.HasValue)
            {
                baseSize = _nodes[i].PreferredSize!.Value;
            }

            if (baseSize == Vector2.Zero)
            {
                baseSize = new Vector2(120, 48);
            }

            var screen = WorldToScreen(_worldPosition[i]);
            var tl = screen - baseSize / 2f;

            comp.LocalPosition = tl - GlobalPosition;
            comp.GlobalPosition = tl;

            comp.Options.Size = baseSize;
            comp.UpdateBounds(this);

            if (comp is Box box)
            {
                box.HandleParentGlobalPositionChange(comp.GlobalPosition);
            }
        }
    }

    public void AllocateNode(string id)
    {
        if (!_idx.TryGetValue(id, out var i)) return;
        _state[i] = true;
        _nodes[i].Component?.ChangeTexture(ContentManager.GetAtlasSprite("wheat")!);
        OnAllocated?.Invoke(_nodes[i]);
        UpdateAvailability();
    }

    public IEnumerable<UiComponent> VisibleComponents()
    {
        var clip = GetClipRect();
        for (var i = 0; i < _nodes.Count; i++)
        {
            var comp = _nodes[i].Component;
            if (comp is null) continue;
            var visible = IsVisible(i) && Intersects(comp, clip);
            comp.IsVisible = visible;

            if (visible)
            {
                yield return comp;
            }
        }
    }

    public IEnumerable<RenderCommand> EnumerateEdgeCommands()
    {
        var tex = _options.EdgeTexture ?? UiManager.Pixel;

        for (var p = 0; p < _nodes.Count; p++)
        {
            foreach (var ch in _children[p])
            {
                if (!IsVisible(p) || !IsVisible(ch)) continue;

                var a = WorldToScreen(_worldPosition[p]);
                var b = WorldToScreen(_worldPosition[ch]);

                var dir = b - a;
                var len = dir.Length();

                if (len < 1f) continue;

                var rot = MathF.Atan2(dir.Y, dir.X);

                var rc = new RenderCommand
                {
                    Id = $"{Id}-edge-{p}-{ch}",
                    Texture = tex,
                    Position = a,
                    Rotation = rot,
                    Scale = _options.EdgeTexture != null
                        ? new Vector2(len / _options.EdgeTexture.Width,
                            _options.EdgeThickness / _options.EdgeTexture.Height)
                        : new Vector2(len, _options.EdgeThickness),
                    Color = _options.EdgeColor,
                    Shader = _options.EdgeShader
                };

                yield return rc;
            }
        }
    }

    private IEnumerable<RenderCommand> EnumerateEdges()
    {
        var texture = _options.EdgeTexture ?? UiManager.Pixel;
        var color = _options.EdgeColor;

        for (var p = 0; p < _nodes.Count; p++)
        {
            foreach (var ch in _children[p])
            {
                if (!IsVisible(p) || !IsVisible(ch)) continue;

                var a = WorldToScreen(_worldPosition[p]);

                var b = WorldToScreen(_worldPosition[ch]);

                foreach (var cmd in DrawLine(texture, a, b, _options.EdgeThickness, color))
                {
                    yield return cmd;
                }
            }
        }
    }

    private IEnumerable<RenderCommand> DrawLine(Texture2D texture, Vector2 a, Vector2 b, float thickness, Color color)
    {
        var d = b - a;
        var len = d.Length();
        if (len < 0.5f) yield break;

        yield return new RenderCommand
        {
            Id = $"{Id}-line-{a}-{b}",
            Texture = texture,
            Position = a,
            Rotation = MathF.Atan2(d.Y, d.X),
            Origin = Vector2.Zero,
            Scale = new Vector2(len, thickness),
            Color = color
        };
    }

    public void SetZoom(float z)
    {
        _camZoom = Math.Clamp(z, _options.ZoomMin, _options.ZoomMax);
        ClampCamera();
        ApplyLayoutToComponents();
    }

    public Vector2 WorldToScreen(Vector2 world) =>
        // GlobalPosition + Options.Size / 2 + (world - _cameraPosition) * _camZoom;
        GlobalPosition + Options.Size / 2 + (world - _cameraPosition) * _camZoom;

    public Vector2 ScreenToWorld(Vector2 screen) =>
        // (screen - (GlobalPosition + Options.Size / 2)) / _camZoom + _cameraPosition;
        (screen - (GlobalPosition + Options.Size / 2)) / _camZoom + _cameraPosition;

    private Rectangle GetClipRect()
    {
        var pad = _options.ClipPaddingPx;

        var rect = new Rectangle(
            (int)GlobalPosition.X + pad,
            (int)GlobalPosition.Y + pad,
            (int)Options.Size.X - pad * 2,
            (int)Options.Size.Y - pad * 2
        );

        // Logger.Log($"ClipRect: {rect} GrahpGlobalPos: {GlobalPosition} Size: {Options.Size}");
        // Logger.Log(
        //     $"UiManager Scale: {RanchMayhemEngine.UiManager.GlobalScale} WindowSize: {RanchMayhemEngine.WindowedSize}");

        return rect;
    }

    private static bool Intersects(UiComponent c, Rectangle clip)
    {
        var r = new Rectangle(
            (int)c.GlobalPosition.X,
            (int)c.GlobalPosition.Y,
            (int)c.Options.Size.X,
            (int)c.Options.Size.Y
        );

        return r.Intersects(clip);
    }

    private IEnumerable<RenderCommand> EnumerateClippedEdges()
    {
        var tex = _options.EdgeTexture ?? UiManager.Pixel;
        var clip = GetClipRect();

        for (int p = 0; p < _nodes.Count; p++)
        {
            foreach (var ch in _children[p])
            {
                if (!IsVisible(p) || !IsVisible(ch)) continue;

                var a = WorldToScreen(_worldPosition[p]);
                var b = WorldToScreen(_worldPosition[ch]);

                // quick reject using edge AABB vs clip
                var minX = (int)MathF.Floor(MathF.Min(a.X, b.X));
                var minY = (int)MathF.Floor(MathF.Min(a.Y, b.Y));
                var maxX = (int)MathF.Ceiling(MathF.Max(a.X, b.X));
                var maxY = (int)MathF.Ceiling(MathF.Max(a.Y, b.Y));
                var aabb = new Rectangle(minX, minY, Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));
                if (!aabb.Intersects(clip)) continue; // NEW: don’t draw edge if fully outside

                var dir = b - a;
                var len = dir.Length();
                if (len < 1f) continue;

                yield return new RenderCommand
                {
                    Id = $"{Id}-edge-{p}-{ch}",
                    Texture = tex,
                    Position = a,
                    Rotation = MathF.Atan2(dir.Y, dir.X),
                    Scale = _options.EdgeTexture != null
                        ? new Vector2(len / _options.EdgeTexture.Width,
                            _options.EdgeThickness / _options.EdgeTexture.Height)
                        : new Vector2(len, _options.EdgeThickness),
                    Color = _options.EdgeColor,
                    Shader = _options.EdgeShader
                };
            }
        }
    }

    public override void Update()
    {
        if (!RanchMayhemEngine.IsFocused) return;
        var mouse = MouseInput.CurrentState.Position.ToVector2();
        var wheel = MouseInput.CurrentState.ScrollWheelValue - MouseInput.PreviousState.ScrollWheelValue;

        if (wheel != 0)
        {
            var before = ScreenToWorld(mouse);
            var step = wheel > 0 ? _options.ZoomStep : -_options.ZoomStep;
            _camZoom = Math.Clamp(_camZoom + step, _options.ZoomMin, _options.ZoomMax);
            var after = ScreenToWorld(mouse);
            _cameraPosition += (before - after);
            ClampCamera();
            ApplyLayoutToComponents();
        }

        var lmbDown = MouseInput.CurrentState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

        if (lmbDown && IsInside(mouse))
        {
            var delta = (mouse - _lastMouse) / _camZoom;
            if (_drag)
            {
                _cameraPosition -= delta;
                ClampCamera();
                ApplyLayoutToComponents();
            }

            _drag = true;
            _lastMouse = mouse;
        }
        else
        {
            _drag = false;
            _lastMouse = mouse;
        }

        var clip = GetClipRect();
        for (var i = 0; i < _nodes.Count; i++)
        {
            var c = _nodes[i].Component;

            if (c is null) continue;

            c.HandleMouse(RanchMayhemEngine.MouseState);

            if (!c.IsVisible)
            {
                // TODO: disable interaction
                c.CanTriggerClick = false;
                continue;
            }

            if (!Intersects(c, clip))
            {
                // TODO: disable interaction
                c.CanTriggerClick = false;
                continue;
            }

            // TODO: enable interaction
            if (c.CanTriggerClick)
            {
                c.CanTriggerClick = false;
                OnClickNode?.Invoke(_nodes[i]);
            }
        }
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }

        // This could be potentially dangerous
        // if another draw call is called while the scissors is active
        // have to see if this actually works
        if (_options.ClipToBounds)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-clip-push",
                ScissorPush = true,
                ScissorRect = GetClipRect()
            };
        }

        foreach (var enumerateClippedEdge in EnumerateEdges())
        {
            yield return enumerateClippedEdge;
        }

        var clip = GetClipRect();

        for (var i = 0; i < _nodes.Count; i++)
        {
            var comp = _nodes[i].Component;
            if (comp is null) continue;
            var visible = IsVisible(i) && Intersects(comp, clip);
            comp.IsVisible = visible;

            if (visible)
            {
                foreach (var renderCommand in comp.Draw())
                {
                    yield return renderCommand;
                }
            }
        }

        if (_options.ClipToBounds)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-clip-pop",
                ScissorPop = true
            };
        }
    }

    private int RootIndex()
    {
        for (var i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].ParentId is null)
            {
                return i;
            }
        }

        return -1;
    }

    private void CenterCameraOn(int idx) => _cameraPosition = idx >= 0 ? _worldPosition[idx] : Vector2.Zero;

    private void LayoutNodes()
    {
        for (var i = 0; i < _nodes.Count; i++)
        {
            var node = _nodes[i];

            if (node.WorldPosition.HasValue)
            {
                _worldPosition[i] = node.WorldPosition.Value;
            }
            else if (node.ParentId is null)
            {
                _worldPosition[i] = new Vector2(_options.RootX, _options.RootY);
            }
            else
            {
                var parentIDx = _idx[node.ParentId];
                var parentPos = _worldPosition[parentIDx];

                var angle = node.Angle ?? 0f;
                var distance = node.Distance ?? 150f;

                var radians = MathHelper.ToRadians(angle);
                var offset = new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * distance;

                _worldPosition[i] = parentPos + offset;
            }
        }
    }

    private void AutoLayout()
    {
        var root = RootIndex();
        if (root < 0) return;

        var depth = new Dictionary<int, int> { [root] = 0 };
        var q = new Queue<int>();
        q.Enqueue(root);

        while (q.Count > 0)
        {
            var u = q.Dequeue();
            foreach (var v in _children[u].Where(v => !depth.ContainsKey(v)))
            {
                depth[v] = depth[u] + 1;
                q.Enqueue(v);
            }
        }

        var groups = new Dictionary<int, List<int>>();

        foreach (var (key, value) in depth)
        {
            if (!groups.TryGetValue(value, out var list))
            {
                groups[value] = list = [];
            }

            list.Add(key);
        }

        _worldPosition[root] = new Vector2(_options.RootX, _options.RootY);

        foreach (var (L, list) in groups.OrderBy(k => k.Key))
        {
            if (L == 0) continue;

            var y = _options.RootY + L * _options.LevelGap;

            // split by side
            var left = list.Where(i => ResolveSide(i, depth) < 0).OrderBy(i => _nodes[i].Order).ToList();
            var right = list.Where(i => ResolveSide(i, depth) > 0).OrderBy(i => _nodes[i].Order).ToList();

            // pack from center outwards: left to the left, right to the right
            PlaceRow(left, y, toRight: false);
            PlaceRow(right, y, toRight: true);
        }

        return;


        int ResolveSide(int i, Dictionary<int, int> depthMap)
        {
            if (i == root) return 0;
            var ni = _nodes[i];
            if (ni.Side == NodeSide.Left) return -1;
            if (ni.Side == NodeSide.Right) return +1;

            // Auto: inherit first ancestor’s explicit side; otherwise alternate by X order
            var pId = ni.ParentId;
            while (pId != null)
            {
                var pi = _idx[pId];
                var side = _nodes[pi].Side;
                if (side == NodeSide.Left) return -1;
                if (side == NodeSide.Right) return +1;
                pId = _nodes[pi].ParentId;
            }

            // fallback: push everything to right by default
            return +1;
        }

        void PlaceRow(List<int> row, float y, bool toRight)
        {
            if (row.Count == 0) return;

            var gap = _options.SiblingGap;
            if (toRight)
            {
                // center → outward to the right
                var startX = _options.RootX + gap;
                for (int i = 0; i < row.Count; i++)
                    _worldPosition[row[i]] = new Vector2(startX + i * gap, y);
            }
            else
            {
                // center → outward to the left
                var startX = _options.RootX - gap;
                for (int i = 0; i < row.Count; i++)
                    _worldPosition[row[i]] = new Vector2(startX - i * gap, y);
            }
        }
    }

    private void UpdateAvailability()
    {
        for (var i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].ParentId == null)
            {
                // if (!_state[i])
                // {
                //     _state[i] = true;
                // }

                continue;
            }

            var p = _idx[_nodes[i].ParentId!];
            var parentAllocated = _state[p];

            if (!parentAllocated && _state[i])
            {
                _state[i] = false;
            }
        }
    }

    private new bool IsVisible(int i)
    {
        if (DebugShowAllNodes) return true;

        if (_nodes[i].ParentId is null) return true;

        var p = _idx[_nodes[i].ParentId!];
        return _state[p];
    }

    private bool IsInside(Vector2 m) => m.X >= GlobalPosition.X && m.Y >= GlobalPosition.Y &&
                                        m.X <= GlobalPosition.X + Options.Size.X &&
                                        m.Y <= GlobalPosition.Y + Options.Size.Y;
}