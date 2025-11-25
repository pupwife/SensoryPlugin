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
    private float _minSize = 50f;
    private float _maxSize = 200f;
    private Vector4 _color = PastelColors.SoftPurple;
    
    public void Initialize()
    {
        _time = 0f;
    }
    
    public void Update(float deltaTime)
    {
        _time += deltaTime * _speed;
    }
    
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        var centerX = canvasPos.X + canvasSize.X / 2f;
        var centerY = canvasPos.Y + canvasSize.Y / 2f;
        
        // Breathing animation (sine wave)
        var breathPhase = (MathF.Sin(_time * 2f) + 1f) / 2f; // 0 to 1
        var size = _minSize + (_maxSize - _minSize) * breathPhase;
        
        // Draw glow effect (multiple circles for glow)
        var glowColor = PastelColors.ToImGuiColor(new Vector4(_color.X, _color.Y, _color.Z, 0.3f));
        drawList.AddCircleFilled(new Vector2(centerX, centerY), size * 1.5f, glowColor);
        
        // Main circle
        var mainColor = PastelColors.ToImGuiColor(_color);
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


