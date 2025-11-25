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
    
    private class Button
    {
        public Vector2 Position;
        public Vector2 Size;
        public bool Glowing;
        public Vector4 Color;
        public float GlowIntensity;
    }
    
    private readonly Vector4[] _pastelColors = new Vector4[]
    {
        PastelColors.SoftPink,
        PastelColors.MintGreen,
        PastelColors.SkyBlue,
        PastelColors.Cream,
        PastelColors.LightOrange,
        PastelColors.Lavender,
        PastelColors.Peach,
        PastelColors.SoftPurple,
        new Vector4(0.7f, 0.95f, 0.85f, 1.0f), // Additional mint
        new Vector4(1.0f, 0.85f, 0.8f, 1.0f)  // Additional peach
    };
    
    public void Initialize()
    {
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
                    Color = _pastelColors[_random.Next(_pastelColors.Length)],
                    GlowIntensity = glowing ? 0.8f + _random.NextSingle() * 0.2f : 0f
                });
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        // Smooth glow intensity changes
        foreach (var btn in _buttons)
        {
            if (btn.Glowing)
            {
                btn.GlowIntensity = MathF.Min(1.0f, btn.GlowIntensity + deltaTime * 2f);
            }
            else
            {
                btn.GlowIntensity = MathF.Max(0f, btn.GlowIntensity - deltaTime * 2f);
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
            
            // Draw glow effect
            if (btn.Glowing && btn.GlowIntensity > 0)
            {
                var glowSize = btn.GlowIntensity * 15f;
                var glowColor = new Vector4(btn.Color.X, btn.Color.Y, btn.Color.Z, btn.GlowIntensity * 0.3f);
                var glowImColor = PastelColors.ToImGuiColor(glowColor);
                drawList.AddRectFilled(
                    pos - halfSize - new Vector2(glowSize),
                    pos + halfSize + new Vector2(glowSize),
                    glowImColor
                );
            }
            
            // Button body
            var buttonColor = btn.Glowing ? btn.Color : new Vector4(0.27f, 0.27f, 0.27f, 1.0f);
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
                    btn.Color = _pastelColors[_random.Next(_pastelColors.Length)];
                }
            }
        }
    }
}

