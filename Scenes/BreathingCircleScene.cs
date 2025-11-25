using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class BreathingCircleScene : IScene
{
    public string Name => "Breathing Circle";
    
    private float _time = 0f;
    private float _speed = 1.0f;
    private float _minSize = 0.08f; // Relative to canvas (8% of smaller dimension)
    private float _maxSize = 0.3f;  // Relative to canvas (30% of smaller dimension)
    private Vector4 _color;
    private string _currentTheme = "pastel";
    
    public void Initialize()
    {
        _time = 0f;
        UpdateThemeColors();
    }
    
    private void UpdateThemeColors()
    {
        var colors = ColorTheme.GetThemeColors(_currentTheme);
        _color = colors[0]; // Use first color from theme
    }
    
    public void Update(float deltaTime)
    {
        _time += deltaTime * _speed;
    }
    
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    
    public void SetTheme(string themeName)
    {
        _currentTheme = themeName;
        UpdateThemeColors();
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        var centerX = canvasPos.X + canvasSize.X / 2f;
        var centerY = canvasPos.Y + canvasSize.Y / 2f;
        
        // Scale sizes relative to canvas
        var baseSize = MathF.Min(canvasSize.X, canvasSize.Y);
        var minSize = _minSize * baseSize;
        var maxSize = _maxSize * baseSize;
        
        // Breathing animation (sine wave)
        var breathPhase = (MathF.Sin(_time * 2f) + 1f) / 2f; // 0 to 1
        var size = minSize + (maxSize - minSize) * breathPhase;
        
        // Get color - use rainbow cycling for rainbow theme, otherwise use static color
        Vector4 currentColor;
        if (_currentTheme == "rainbow")
        {
            currentColor = ColorTheme.GetRainbowColor(_time, 0.1f);
        }
        else
        {
            currentColor = _color;
        }
        
        // Draw glow effect (multiple circles for glow)
        var glowColor = PastelColors.ToImGuiColor(new Vector4(currentColor.X, currentColor.Y, currentColor.Z, 0.3f));
        drawList.AddCircleFilled(new Vector2(centerX, centerY), size * 1.5f, glowColor);
        
        // Main circle
        var mainColor = PastelColors.ToImGuiColor(currentColor);
        drawList.AddCircleFilled(new Vector2(centerX, centerY), size, mainColor);
        
        // Inner highlight
        var highlightColor = PastelColors.ToImGuiColor(new Vector4(1f, 1f, 1f, 0.3f));
        drawList.AddCircleFilled(
            new Vector2(centerX - size * 0.3f, centerY - size * 0.3f),
            size * 0.4f,
            highlightColor
        );
    }
    
    public void Cleanup() { }
}


