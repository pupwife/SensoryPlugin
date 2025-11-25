using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class WavesScene : IScene
{
    public string Name => "Flowing Waves";
    
    private float _time = 0f;
    private float _speed = 1.0f;
    private int _waveCount = 3;
    private float _amplitude = 50f;
    private Vector4 _primaryColor = PastelColors.SoftPurple;
    private Vector4 _secondaryColor = PastelColors.SoftPink;
    
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
        var centerY = canvasPos.Y + canvasSize.Y / 2f;
        var waveLength = canvasSize.X / 2f;
        
        for (int w = 0; w < _waveCount; w++)
        {
            var color = w % 2 == 0 ? _primaryColor : _secondaryColor;
            var imColor = PastelColors.ToImGuiColor(color);
            var offsetY = (w - (_waveCount - 1) / 2f) * (_amplitude * 2f);
            var phase = (w / (float)_waveCount) * MathF.PI * 2f;
            
            Vector2? prevPoint = null;
            for (int x = 0; x <= canvasSize.X; x += 2)
            {
                var y = centerY + offsetY + MathF.Sin((x / waveLength) * MathF.PI * 2f + _time * 2f + phase) * _amplitude;
                var point = new Vector2(canvasPos.X + x, y);
                
                if (prevPoint.HasValue)
                {
                    drawList.AddLine(prevPoint.Value, point, imColor, 4f);
                }
                
                prevPoint = point;
            }
        }
    }
    
    public void Cleanup() { }
}


