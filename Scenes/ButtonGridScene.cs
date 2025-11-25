using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Sensory.Utils;

namespace Sensory.Scenes;

public class ButtonGridScene : IScene
{
    public string Name => "Button Grid";
    
    private readonly List<Button> _buttons = new();
    private readonly Random _random = new();
    private int _gridCols = 8;
    private int _gridRows = 6;
    private Vector2 _canvasSize = new(800f, 600f);
    private float _speed = 1.0f;
    private float _time = 0f;
    
    private class Button
    {
        public Vector2 Position;
        public Vector2 Size;
        public bool Glowing;
        public Vector4 Color;
        public float GlowIntensity;
    }
    
    private Vector4[] _themeColors = Array.Empty<Vector4>();
    private string _currentTheme = "pastel";
    
    public void Initialize()
    {
        UpdateThemeColors();
        _buttons.Clear();
        
        var buttonWidth = _canvasSize.X / _gridCols;
        var buttonHeight = _canvasSize.Y / _gridRows;
        
        for (int row = 0; row < _gridRows; row++)
        {
            for (int col = 0; col < _gridCols; col++)
            {
                var glowing = _random.NextSingle() > 0.5f;
                _buttons.Add(new Button
                {
                    Position = new Vector2(
                        col * buttonWidth + buttonWidth / 2f,
                        row * buttonHeight + buttonHeight / 2f
                    ),
                    Size = new Vector2(buttonWidth * 0.8f, buttonHeight * 0.8f),
                    Glowing = glowing,
                    Color = _themeColors[_random.Next(_themeColors.Length)],
                    GlowIntensity = glowing ? 0.8f + _random.NextSingle() * 0.2f : 0f
                });
            }
        }
    }
    
    private void UpdateThemeColors()
    {
        _themeColors = ColorTheme.GetThemeColors(_currentTheme);
    }
    
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    
    public void SetTheme(string themeName)
    {
        _currentTheme = themeName;
        UpdateThemeColors();
        // Only update colors if not rainbow theme (rainbow will cycle in Draw)
        if (_currentTheme != "rainbow")
        {
            // Update existing buttons' colors
            foreach (var btn in _buttons)
            {
                btn.Color = _themeColors[_random.Next(_themeColors.Length)];
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        _time += deltaTime;
        // Smooth glow intensity changes
        foreach (var btn in _buttons)
        {
            if (btn.Glowing)
            {
                btn.GlowIntensity = MathF.Min(1.0f, btn.GlowIntensity + deltaTime * 2f * _speed);
            }
            else
            {
                btn.GlowIntensity = MathF.Max(0f, btn.GlowIntensity - deltaTime * 2f * _speed);
            }
        }
    }
    
    public void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Update canvas size if it changed
        if (_canvasSize != canvasSize)
        {
            _canvasSize = canvasSize;
            // Reinitialize buttons with new size
            Initialize();
        }
        
        foreach (var btn in _buttons)
        {
            var pos = canvasPos + btn.Position;
            var halfSize = btn.Size / 2f;
            
            // Get color - use rainbow cycling for rainbow theme
            Vector4 buttonColor;
            if (_currentTheme == "rainbow" && btn.Glowing)
            {
                // Each button cycles through rainbow based on its position and time
                var btnTime = _time + btn.Position.X * 0.001f + btn.Position.Y * 0.001f;
                buttonColor = ColorTheme.GetRainbowColor(btnTime, 0.1f);
            }
            else if (btn.Glowing)
            {
                buttonColor = btn.Color;
            }
            else
            {
                buttonColor = new Vector4(0.27f, 0.27f, 0.27f, 1.0f);
            }
            
            // Draw glow effect
            if (btn.Glowing && btn.GlowIntensity > 0)
            {
                var glowSize = btn.GlowIntensity * 15f;
                var glowColor = new Vector4(buttonColor.X, buttonColor.Y, buttonColor.Z, btn.GlowIntensity * 0.3f);
                var glowImColor = PastelColors.ToImGuiColor(glowColor);
                drawList.AddRectFilled(
                    pos - halfSize - new Vector2(glowSize),
                    pos + halfSize + new Vector2(glowSize),
                    glowImColor
                );
            }
            
            // Button body
            var buttonImColor = PastelColors.ToImGuiColor(buttonColor);
            drawList.AddRectFilled(
                pos - halfSize,
                pos + halfSize,
                buttonImColor
            );
            
            // Border
            var borderColor = btn.Glowing 
                ? PastelColors.ToImGuiColor(new Vector4(1f, 1f, 1f, 1f))
                : PastelColors.ToImGuiColor(new Vector4(0.4f, 0.4f, 0.4f, 1f));
            drawList.AddRect(
                pos - halfSize,
                pos + halfSize,
                borderColor,
                0f,
                ImDrawFlags.None,
                2f
            );
        }
    }
    
    public void Cleanup() { }
    
    public void HandleClick(Vector2 clickPos, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Convert click position to canvas-relative coordinates
        var relativePos = clickPos - canvasPos;
        
        // Find clicked button
        Button? clickedBtn = null;
        foreach (var btn in _buttons)
        {
            var halfSize = btn.Size / 2f;
            if (relativePos.X >= btn.Position.X - halfSize.X && 
                relativePos.X <= btn.Position.X + halfSize.X &&
                relativePos.Y >= btn.Position.Y - halfSize.Y && 
                relativePos.Y <= btn.Position.Y + halfSize.Y)
            {
                clickedBtn = btn;
                break;
            }
        }
        
        if (clickedBtn != null)
        {
            // Toggle clicked button
            clickedBtn.Glowing = !clickedBtn.Glowing;
            
            // Randomly toggle other buttons (3-5 random buttons)
            var numToToggle = 3 + _random.Next(3);
            var otherButtons = new List<Button>(_buttons);
            otherButtons.Remove(clickedBtn);
            
            for (int i = 0; i < numToToggle && otherButtons.Count > 0; i++)
            {
                var randomIndex = _random.Next(otherButtons.Count);
                var randomBtn = otherButtons[randomIndex];
                otherButtons.RemoveAt(randomIndex);
                randomBtn.Glowing = _random.NextSingle() > 0.3f; // 70% chance to turn on
            }
            
            // Randomly change colors (30% chance per button)
            foreach (var btn in _buttons)
            {
                if (_random.NextSingle() > 0.7f)
                {
                    btn.Color = _themeColors[_random.Next(_themeColors.Length)];
                }
            }
        }
    }
}

