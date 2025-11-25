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
        for (int i = 0; i < 30; i++)
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
                Size = 15f + _random.NextSingle() * 40f,
                Rotation = _random.NextSingle() * MathF.PI * 2f,
                RotationSpeed = (_random.NextSingle() - 0.5f) * 0.02f,
                Color = PastelColors.GetRandomColor(_random),
                Alpha = 0.7f + _random.NextSingle() * 0.3f
            });
        }
    }
    
    public void Update(float deltaTime)
    {
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
        // Update canvas size if it changed
        if (_canvasSize != canvasSize)
        {
            _canvasSize = canvasSize;
        }
        
        foreach (var shape in _shapes)
        {
            var color = new Vector4(shape.Color.X, shape.Color.Y, shape.Color.Z, shape.Alpha);
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

