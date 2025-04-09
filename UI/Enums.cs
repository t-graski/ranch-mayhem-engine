using System;

namespace ranch_mayhem_engine.UI;

public enum SizeUnit
{
    Pixels,
    Percent
}

public enum BorderOrientation
{
    Inside,
    Outside
}

[Flags]
public enum BorderPosition
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    All = Top | Bottom | Left | Right
}

public enum LogLevel
{
    Info,
    Warning,
    Error,
    Internal
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}

public enum ButtonState
{
    Normal,
    Disabled
}

public enum AnimationDirection
{
    Top,
    Right
}

[Flags]
public enum UiAnchor
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    CenterX = 1 << 4,
    CenterY = 1 << 5
}