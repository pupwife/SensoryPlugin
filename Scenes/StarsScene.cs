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
    private const int MaxStars = 15;
    
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
        for (int i = 0; i < MaxStars; i++)
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
            Color = PastelColors.GetRandomColor(_random)
        });
    }
    
    public void Update(float deltaTime)
    {
        for (int i = _stars.Count - 1; i >= 0; i--)
        {
            var star = _stars[i];
            star.Life += deltaTime;
            
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
            
            // Draw glow effect
            var glowColor = new Vector4(star.Color.X, star.Color.Y, star.Color.Z, star.GlowIntensity * 0.5f);
            var glowImColor = PastelColors.ToImGuiColor(glowColor);
            drawList.AddCircleFilled(pos, star.Size * 3f, glowImColor);
            
            // Star center
            var starColor = new Vector4(star.Color.X, star.Color.Y, star.Color.Z, star.GlowIntensity);
            var starImColor = PastelColors.ToImGuiColor(starColor);
            drawList.AddCircleFilled(pos, star.Size, starImColor);
        }
    }
    
    public void Cleanup() { }
}


