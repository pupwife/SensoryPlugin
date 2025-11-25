using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class FloatingShapesScene : IScene
{
    public string Name => "Floating Shapes";
    
    private readonly List<Shape> _shapes = new();
    private readonly Random _random = new();
    private float _speed = 1.0f;
    private int _shapeCount = 30;
    private string _currentTheme = "pastel";
    private float _time = 0f;
    
    private class Shape
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Size;
        public float Rotation;
        public float RotationSpeed;
        public Vector4 Color;
        public float Alpha;
    }
    
    private Vector2 _canvasSize = new(800f, 600f);
    
    public void Initialize()
    {
        _shapes.Clear();
        var baseSize = MathF.Min(_canvasSize.X, _canvasSize.Y);
        var minShapeSize = baseSize * 0.02f; // 2% of smaller dimension
        var maxShapeSize = baseSize * 0.06f; // 6% of smaller dimension
        
        for (int i = 0; i < _shapeCount; i++)
        {
            _shapes.Add(new Shape
            {
                Position = new Vector2(
                    _random.NextSingle() * _canvasSize.X,
                    _random.NextSingle() * _canvasSize.Y
                ),
                Velocity = new Vector2(
                    (_random.NextSingle() - 0.5f) * 2f,
                    (_random.NextSingle() - 0.5f) * 2f
                ),
                Size = minShapeSize + _random.NextSingle() * (maxShapeSize - minShapeSize),
                Rotation = _random.NextSingle() * MathF.PI * 2f,
                RotationSpeed = (_random.NextSingle() - 0.5f) * 0.02f,
                Color = ColorTheme.GetRandomColor(_currentTheme, _random),
                Alpha = 0.7f + _random.NextSingle() * 0.3f
            });
        }
    }
    
    public void SetShapeCount(int count)
    {
        _shapeCount = Math.Max(5, Math.Min(100, count));
        Initialize(); // Reinitialize with new count
    }
    
    public int GetShapeCount() => _shapeCount;
    
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    
    public void SetTheme(string themeName)
    {
        _currentTheme = themeName;
        // Only update colors if not rainbow theme (rainbow will cycle in Draw)
        if (_currentTheme != "rainbow")
        {
            // Update existing shapes' colors
            foreach (var shape in _shapes)
            {
                shape.Color = ColorTheme.GetRandomColor(_currentTheme, _random);
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        _time += deltaTime;
        for (int i = 0; i < _shapes.Count; i++)
        {
            var shape = _shapes[i];
            shape.Position += shape.Velocity * _speed * deltaTime * 60f;
            shape.Rotation += shape.RotationSpeed * _speed * deltaTime * 60f;
            
            // Bounce off edges
            if (shape.Position.X < 0 || shape.Position.X > _canvasSize.X)
                shape.Velocity.X *= -1f;
            if (shape.Position.Y < 0 || shape.Position.Y > _canvasSize.Y)
                shape.Velocity.Y *= -1f;
            
            shape.Position.X = MathF.Max(0, MathF.Min(_canvasSize.X, shape.Position.X));
            shape.Position.Y = MathF.Max(0, MathF.Min(_canvasSize.Y, shape.Position.Y));
        }
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Update canvas size if it changed - rescale shapes if needed
        if (_canvasSize != canvasSize)
        {
            var oldBaseSize = MathF.Min(_canvasSize.X, _canvasSize.Y);
            var newBaseSize = MathF.Min(canvasSize.X, canvasSize.Y);
            var scaleFactor = newBaseSize / oldBaseSize;
            
            // Scale all shapes to new canvas size
            foreach (var shape in _shapes)
            {
                shape.Position.X = (shape.Position.X / _canvasSize.X) * canvasSize.X;
                shape.Position.Y = (shape.Position.Y / _canvasSize.Y) * canvasSize.Y;
                shape.Size *= scaleFactor;
            }
            
            _canvasSize = canvasSize;
        }
        
        foreach (var shape in _shapes)
        {
            // Get color - use rainbow cycling for rainbow theme
            Vector4 color;
            if (_currentTheme == "rainbow")
            {
                // Each shape cycles through rainbow based on its position and time
                var shapeTime = _time + shape.Position.X * 0.001f + shape.Position.Y * 0.001f;
                color = ColorTheme.GetRainbowColor(shapeTime, 0.1f);
                color.W = shape.Alpha; // Preserve alpha
            }
            else
            {
                color = new Vector4(shape.Color.X, shape.Color.Y, shape.Color.Z, shape.Alpha);
            }
            var imColor = PastelColors.ToImGuiColor(color);
            
            var pos = canvasPos + shape.Position;
            
            // Draw circle
            drawList.AddCircleFilled(pos, shape.Size, imColor);
            
            // Draw border
            var borderColor = PastelColors.ToImGuiColor(new Vector4(1f, 1f, 1f, 0.5f));
            drawList.AddCircle(pos, shape.Size, borderColor, 0, 2f);
        }
    }
    
    public void Cleanup() { }
}


