using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class SpiralScene : IScene
{
    public string Name => "Infinite Spiral";
    
    private float _rotation = 0f;
    private float _speed = 1.0f;
    private int _spiralCount = 1;
    private Vector4 _primaryColor = PastelColors.SoftPurple;
    private Vector4 _secondaryColor = PastelColors.SoftPink;
    
    public void Initialize()
    {
        _rotation = 0f;
    }
    
    public void Update(float deltaTime)
    {
        _rotation += deltaTime * _speed * 0.5f;
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        var centerX = canvasPos.X + canvasSize.X / 2f;
        var centerY = canvasPos.Y + canvasSize.Y / 2f;
        var maxRadius = MathF.Min(canvasSize.X, canvasSize.Y) * 0.4f;
        
        for (int s = 0; s < _spiralCount; s++)
        {
            var spiralRotation = _rotation + (s / (float)_spiralCount) * MathF.PI * 2f;
            var color = s % 2 == 0 ? _primaryColor : _secondaryColor;
            var imColor = PastelColors.ToImGuiColor(color);
            
            const int turns = 8;
            const int points = 360 * turns;
            
            for (int i = 0; i < points; i += 2)
            {
                var angle = (i / 180f) * MathF.PI + spiralRotation;
                var radius = (i / (float)points) * maxRadius;
                var x = centerX + MathF.Cos(angle) * radius;
                var y = centerY + MathF.Sin(angle) * radius;
                
                if (i == 0)
                {
                    // Start point
                }
                else
                {
                    var prevAngle = ((i - 2) / 180f) * MathF.PI + spiralRotation;
                    var prevRadius = ((i - 2) / (float)points) * maxRadius;
                    var prevX = centerX + MathF.Cos(prevAngle) * prevRadius;
                    var prevY = centerY + MathF.Sin(prevAngle) * prevRadius;
                    
                    drawList.AddLine(
                        new Vector2(prevX, prevY),
                        new Vector2(x, y),
                        imColor,
                        3f
                    );
                }
            }
        }
    }
    
    public void Cleanup() { }
}


