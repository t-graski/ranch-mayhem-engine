// =========================================================
// Ranch Mayhem — NodeGraphSettings.cs
// Author: Tobias Graski
// Created: 10/19/2025 22:10
// Project: ranch-mayhem
// 
// Copyright (c) 2025 Ranch Mayhem. All rights reserved.
// =========================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class NodeGraphOptions : UiComponentOptions
{
    public float LevelGap { get; set; } = 160f;
    public float SiblingGap { get; set; } = 220f;
    public float RootX { get; set; } = 0f;
    public float RootY { get; set; } = 0f;
    
    public float ZoomMin { get; set; } = 0.4f;
    public float ZoomMax { get; set; } = 2.2f;
    public float ZoomStep { get; set; } = 0.15f;

    public Texture2D? EdgeTexture { get; set; }
    public Effect? EdgeShader { get; set; }
    public float EdgeThickness { get; set; } = 4f;
    public Color EdgeColor = new Color(170, 170, 170);

    public bool ClipToBounds { get; set; } = true;
    public int ClipPaddingPx { get; set; } = 0;

    public bool EnablePanLimits { get; set; } = true;
    public float PanPadding { get; set; } = 1000f;
}

public sealed class Node
{
    public required string Id;
    public string? ParentId;
    public int Cost = 1;
    public Texture2D? Icon;
    public UiComponent? Component;
    public object? Data;
    public Vector2? PreferredSize;
    public bool StartAllocated;

    public Vector2? WorldPosition;
    public float? Angle;
    public float? Distance;

    public NodeSide Side = NodeSide.Auto;
    public int Order = 0;
}

public enum NodeSide
{
    Auto,
    Left,
    Right
}