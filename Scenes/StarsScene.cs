using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class StarsScene : IScene
{
    public string Name => "Glowing Stars";
    
    private readonly List<Star> _stars = new();
    private readonly Random _random = new();
    private int _maxStars = 15;
    private float _speed = 1.0f;
    private string _currentTheme = "pastel";
    private float _time = 0f;
    
    private class Star
    {
        public Vector2 Position;
        public float Size;
        public float Life;
        public float MaxLife;
        public float GlowIntensity;
        public Vector4 Color;
    }
    
    public void Initialize()
    {
        _stars.Clear();
        for (int i = 0; i < _maxStars; i++)
        {
            SpawnStar();
        }
    }
    
    private Vector2 _canvasSize = new(800f, 600f);
    
    private void SpawnStar()
    {
        _stars.Add(new Star
        {
            Position = new Vector2(
                _random.NextSingle() * _canvasSize.X,
                _random.NextSingle() * _canvasSize.Y
            ),
            Size = 3f + _random.NextSingle() * 8f,
            Life = 0f,
            MaxLife = 2f + _random.NextSingle() * 3f,
            GlowIntensity = 0f,
            Color = ColorTheme.GetRandomColor(_currentTheme, _random)
        });
    }
    
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
            // Update existing stars' colors
            foreach (var star in _stars)
            {
                star.Color = ColorTheme.GetRandomColor(_currentTheme, _random);
            }
        }
    }
    
    public void SetMaxStars(int count)
    {
        _maxStars = Math.Max(5, Math.Min(50, count));
        // Adjust current stars to match new max
        while (_stars.Count > _maxStars)
        {
            _stars.RemoveAt(_stars.Count - 1);
        }
        while (_stars.Count < _maxStars)
        {
            SpawnStar();
        }
    }
    
    public int GetMaxStars() => _maxStars;
    
    public void Update(float deltaTime)
    {
        _time += deltaTime;
        for (int i = _stars.Count - 1; i >= 0; i--)
        {
            var star = _stars[i];
            star.Life += deltaTime * _speed;
            
            // Fade in
            if (star.Life < 0.5f)
            {
                star.GlowIntensity = star.Life / 0.5f;
            }
            // Fade out
            else if (star.Life > star.MaxLife - 0.5f)
            {
                star.GlowIntensity = (star.MaxLife - star.Life) / 0.5f;
            }
            else
            {
                star.GlowIntensity = 1.0f;
            }
            
            // Remove and respawn when done
            if (star.Life >= star.MaxLife)
            {
                _stars.RemoveAt(i);
                SpawnStar();
            }
        }
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Update canvas size if it changed
        if (_canvasSize != canvasSize)
        {
            _canvasSize = canvasSize;
        }
        
        foreach (var star in _stars)
        {
            var pos = canvasPos + star.Position;
            
            // Get color - use rainbow cycling for rainbow theme
            Vector4 color;
            if (_currentTheme == "rainbow")
            {
                // Each star cycles through rainbow based on its position and time
                var starTime = _time + star.Position.X * 0.001f + star.Position.Y * 0.001f;
                color = ColorTheme.GetRainbowColor(starTime, 0.1f);
            }
            else
            {
                color = star.Color;
            }
            
            // Draw glow effect
            var glowColor = new Vector4(color.X, color.Y, color.Z, star.GlowIntensity * 0.5f);
            var glowImColor = PastelColors.ToImGuiColor(glowColor);
            drawList.AddCircleFilled(pos, star.Size * 3f, glowImColor);
            
            // Star center
            var starColor = new Vector4(color.X, color.Y, color.Z, star.GlowIntensity);
            var starImColor = PastelColors.ToImGuiColor(starColor);
            drawList.AddCircleFilled(pos, star.Size, starImColor);
        }
    }
    
    public void Cleanup() { }
}


