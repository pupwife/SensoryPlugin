using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class PendulumScene : IScene
{
    public string Name => "Pendulum";
    
    private float _time = 0f;
    private float _speed = 1.0f;
    private float _length = 200f;
    private float _amplitude = MathF.PI / 3f; // 60 degrees
    private Vector4 _color = PastelColors.SoftPurple;
    
    public void Initialize()
    {
        _time = 0f;
    }
    
    public void Update(float deltaTime)
    {
        _time += deltaTime * _speed;
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        var centerX = canvasPos.X + canvasSize.X / 2f;
        var topY = canvasPos.Y + 100f;
        
        // Calculate pendulum angle
        var angle = _amplitude * MathF.Sin(_time * 2f);
        
        // Calculate bob position
        var bobX = centerX + MathF.Sin(angle) * _length;
        var bobY = topY + MathF.Cos(angle) * _length;
        
        // Draw string
        var lineColor = PastelColors.ToImGuiColor(new Vector4(0.88f, 0.88f, 0.88f, 1.0f));
        drawList.AddLine(new Vector2(centerX, topY), new Vector2(bobX, bobY), lineColor, 2f);
        
        // Draw watch/circle
        var watchSize = 40f;
        var watchColor = PastelColors.ToImGuiColor(_color);
        var whiteColor = PastelColors.ToImGuiColor(new Vector4(1f, 1f, 1f, 1f));
        
        // Draw watch face
        drawList.AddCircleFilled(new Vector2(bobX, bobY), watchSize, watchColor);
        drawList.AddCircle(new Vector2(bobX, bobY), watchSize, whiteColor, 0, 3f);
        
        // Draw hour markers
        for (int i = 0; i < 12; i++)
        {
            var hourAngle = (i / 12f) * MathF.PI * 2f;
            var startX = bobX + MathF.Cos(hourAngle) * (watchSize - 5f);
            var startY = bobY + MathF.Sin(hourAngle) * (watchSize - 5f);
            var endX = bobX + MathF.Cos(hourAngle) * watchSize;
            var endY = bobY + MathF.Sin(hourAngle) * watchSize;
            drawList.AddLine(new Vector2(startX, startY), new Vector2(endX, endY), whiteColor, 2f);
        }
        
        // Draw hands
        drawList.AddLine(
            new Vector2(bobX, bobY),
            new Vector2(bobX, bobY - watchSize * 0.6f),
            whiteColor,
            3f
        );
        drawList.AddLine(
            new Vector2(bobX, bobY),
            new Vector2(bobX + watchSize * 0.4f, bobY),
            whiteColor,
            2f
        );
    }
    
    public void Cleanup() { }
}

