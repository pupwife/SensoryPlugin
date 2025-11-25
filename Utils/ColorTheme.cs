using System;
using System.Numerics;

namespace Sensory.Utils;

public static class ColorTheme
{
    // Convert hex color to Vector4 (0-1 range)
    private static Vector4 HexToVector4(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
        {
            var r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
            var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
            var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
            return new Vector4(r, g, b, 1.0f);
        }
        return new Vector4(1f, 1f, 1f, 1f);
    }
    
    public static Vector4 GetBackgroundColor(string themeName)
    {
        return themeName?.ToLower() switch
        {
            "bright" => HexToVector4("#1a1a2e"),
            "high-contrast" => HexToVector4("#000000"),
            "rainbow" => HexToVector4("#0a0a1a"),
            _ => HexToVector4("#2d2d44") // Default: pastel
        };
    }
    
    public static Vector4[] GetThemeColors(string themeName)
    {
        return themeName?.ToLower() switch
        {
            "bright" => new Vector4[]
            {
                HexToVector4("#FF6B6B"), // Red
                HexToVector4("#4ECDC4"), // Teal
                HexToVector4("#45B7D1"), // Blue
                HexToVector4("#FFA07A"), // Light Salmon
                HexToVector4("#98D8C8"), // Mint
                HexToVector4("#F7DC6F"), // Yellow
                HexToVector4("#BB8FCE")  // Purple
            },
            "high-contrast" => new Vector4[]
            {
                HexToVector4("#FFFFFF"), // White
                HexToVector4("#000000"), // Black
                HexToVector4("#FF0000"), // Red
                HexToVector4("#00FF00"), // Green
                HexToVector4("#0000FF"), // Blue
                HexToVector4("#FFFF00"), // Yellow
                HexToVector4("#FF00FF")  // Magenta
            },
            "rainbow" => new Vector4[]
            {
                HexToVector4("#FF0000"), // Red
                HexToVector4("#FF7F00"), // Orange
                HexToVector4("#FFFF00"), // Yellow
                HexToVector4("#00FF00"), // Green
                HexToVector4("#0000FF"), // Blue
                HexToVector4("#4B0082"), // Indigo
                HexToVector4("#9400D3")  // Violet
            },
            _ => new Vector4[] // Default: pastel
            {
                HexToVector4("#FFB3BA"), // Soft Pink
                HexToVector4("#BAFFC9"), // Mint Green
                HexToVector4("#BAE1FF"), // Sky Blue
                HexToVector4("#FFFFBA"), // Cream Yellow
                HexToVector4("#FFDFBA"), // Light Orange
                HexToVector4("#E0BBE4"), // Lavender
                HexToVector4("#FEC8D8")  // Peach Pink
            }
        };
    }
    
    public static Vector4 GetRandomColor(string themeName, Random random)
    {
        var colors = GetThemeColors(themeName);
        return colors[random.Next(colors.Length)];
    }
    
    public static Vector4 GetColor(string themeName, int index)
    {
        var colors = GetThemeColors(themeName);
        return colors[index % colors.Length];
    }
    
    // Get rainbow color that cycles slowly based on time (for rainbow theme only)
    // time should be in seconds, cycleSpeed controls how fast it cycles (default 0.1 = 10 seconds per full cycle)
    public static Vector4 GetRainbowColor(float time, float cycleSpeed = 0.1f)
    {
        var colors = GetThemeColors("rainbow");
        var cycleTime = time * cycleSpeed;
        var normalizedTime = cycleTime - MathF.Floor(cycleTime); // 0 to 1
        
        // Map normalized time to color index with smooth interpolation
        var colorIndex = normalizedTime * colors.Length;
        var index1 = (int)MathF.Floor(colorIndex) % colors.Length;
        var index2 = (index1 + 1) % colors.Length;
        var t = colorIndex - MathF.Floor(colorIndex);
        
        // Interpolate between two colors
        var color1 = colors[index1];
        var color2 = colors[index2];
        return new Vector4(
            color1.X + (color2.X - color1.X) * t,
            color1.Y + (color2.Y - color1.Y) * t,
            color1.Z + (color2.Z - color1.Z) * t,
            1.0f
        );
    }
}

