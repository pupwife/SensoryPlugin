using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Sensory.Utils;

public static class PastelColors
{
    // Pastel color palette: mint green, soft pinks/purples, soft light oranges
    public static readonly Vector4 MintGreen = new(0.7f, 0.95f, 0.85f, 1.0f);      // #B3F2D9
    public static readonly Vector4 SoftPink = new(1.0f, 0.7f, 0.73f, 1.0f);       // #FFB3BA
    public static readonly Vector4 SoftPurple = new(0.78f, 0.66f, 1.0f, 1.0f);    // #C8A8FF
    public static readonly Vector4 LightOrange = new(1.0f, 0.87f, 0.73f, 1.0f);  // #FFDFBA
    public static readonly Vector4 Peach = new(1.0f, 0.85f, 0.8f, 1.0f);         // #FFD9CC
    public static readonly Vector4 Lavender = new(0.88f, 0.78f, 1.0f, 1.0f);      // #E0C7FF
    public static readonly Vector4 SkyBlue = new(0.73f, 0.9f, 1.0f, 1.0f);        // #BAE6FF
    public static readonly Vector4 Cream = new(1.0f, 0.98f, 0.9f, 1.0f);          // #FFFAE6
    
    public static readonly Vector4[] ColorPalette = new[]
    {
        MintGreen, SoftPink, SoftPurple, LightOrange, Peach, Lavender, SkyBlue, Cream
    };
    
    public static Vector4 GetColor(int index)
    {
        return ColorPalette[index % ColorPalette.Length];
    }
    
    public static Vector4 GetRandomColor(Random random)
    {
        return ColorPalette[random.Next(ColorPalette.Length)];
    }
    
    // Convert Vector4 to ImGui color (0-1 range)
    public static uint ToImGuiColor(Vector4 color)
    {
        return ImGui.ColorConvertFloat4ToU32(color);
    }
}

