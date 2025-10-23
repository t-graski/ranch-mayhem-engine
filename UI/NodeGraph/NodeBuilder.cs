// =========================================================
// Ranch Mayhem — NodeBuilder.cs
// Author: Tobias Graski
// Created: 10/20/2025 01:10
// Project: ranch-mayhem
// 
// Copyright (c) 2025 Ranch Mayhem. All rights reserved.
// =========================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;

namespace ranch_mayhem_engine.UI;

public class NodeBuilder
{
    private string _id;
    private string? _parentId;
    private Texture2D? _icon;
    private UiComponent? _component;
    private int _cost = 1;
    private bool _startAllocated = false;
    private Vector2? _worldPosition;
    private float? _angle;
    private float? _distance;
    private Vector2? _preferredSize;
    private NodeSize _size = NodeSize.Medium;
    private NodeType _type = NodeType.Gray;
    private object? _data;

    public NodeBuilder(string id)
    {
        _id = id;
    }

    public NodeBuilder P(string parentId)
    {
        _parentId = parentId;
        return this;
    }

    public NodeBuilder P(Node node)
    {
        _parentId = node.Id;
        return this;
    }

    public NodeBuilder I(Texture2D icon)
    {
        _icon = icon;
        return this;
    }

    public NodeBuilder I(string id)
    {
        _icon = ContentManager.GetAtlasSprite(id);
        return this;
    }

    public NodeBuilder C(UiComponent component)
    {
        _component = component;
        return this;
    }

    public NodeBuilder Co(int cost)
    {
        _cost = cost;
        return this;
    }

    public NodeBuilder Alloc(bool allocated = true)
    {
        _startAllocated = allocated;
        return this;
    }

    public NodeBuilder At(float x, float y)
    {
        _worldPosition = new Vector2(x, y);
        return this;
    }

    public NodeBuilder Offset(float angle, float distance)
    {
        _angle = angle;
        _distance = distance;
        return this;
    }

    public NodeBuilder NS(NodeSize size = NodeSize.Medium)
    {
        _size = size;
        return this;
    }

    public NodeBuilder NT(NodeType type = NodeType.Gray)
    {
        _type = type;
        return this;
    }

    public NodeBuilder N(float distance = 100f) => Offset(-90, distance);
    public NodeBuilder S(float distance = 100f) => Offset(90, distance);
    public NodeBuilder E(float distance = 100f) => Offset(0, distance);
    public NodeBuilder W(float distance = 100f) => Offset(180, distance);
    public NodeBuilder NE(float distance = 100f) => Offset(-45, distance);
    public NodeBuilder NW(float distance = 100f) => Offset(-135, distance);
    public NodeBuilder SE(float distance = 100f) => Offset(45, distance);
    public NodeBuilder SW(float distance = 100f) => Offset(135, distance);

    public NodeBuilder Size(float width, float height)
    {
        _preferredSize = new Vector2(width, height);
        return this;
    }

    public NodeBuilder Data(object data)
    {
        _data = data;
        return this;
    }

    public Node Build()
    {
        // if (_component == null)
        //     throw new InvalidOperationException($"Node '{_id}' must have a Component");

        return new Node
        {
            Id = _id,
            ParentId = _parentId,
            Component = _component,
            Icon = _icon,
            Cost = _cost,
            StartAllocated = _startAllocated,
            WorldPosition = _worldPosition,
            Angle = _angle,
            Distance = _distance,
            PreferredSize = _preferredSize,
            Data = _data,
            Size = _size,
            Type = _type
        };
    }
}

public static class NodeGraphExtensions
{
    public static NodeBuilder Node(this NodeGraph graph, string id, UiComponent component)
    {
        return new NodeBuilder(id).C(component);
    }
}