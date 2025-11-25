using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Sensory.Scenes;

namespace Sensory.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    public readonly SceneManager sceneManager;
    private System.Diagnostics.Stopwatch? frameTimer;
    private float lastDeltaTime = 0.016f; // Default 60fps

    public MainWindow(Plugin plugin)
        : base("Baby Sensory##SensoryMainWindow", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(800, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        this.sceneManager = new SceneManager();
        
        // Register all scenes
        sceneManager.RegisterScene(new BreathingCircleScene());
        sceneManager.RegisterScene(new FloatingShapesScene());
        sceneManager.RegisterScene(new StarsScene());
        sceneManager.RegisterScene(new SpiralScene());
        sceneManager.RegisterScene(new WavesScene());
        sceneManager.RegisterScene(new ButtonGridScene());
        
        // Initialize first scene from configuration
        var defaultScene = plugin.Configuration.CurrentScene ?? "breathing circle";
        sceneManager.SwitchScene(defaultScene);
        
        frameTimer = System.Diagnostics.Stopwatch.StartNew();
    }

    public void Dispose() 
    {
        frameTimer?.Stop();
    }

    public override void Draw()
    {
        // Calculate delta time
        if (frameTimer != null)
        {
            var elapsed = (float)frameTimer.Elapsed.TotalSeconds;
            lastDeltaTime = elapsed;
            frameTimer.Restart();
        }
        
        // Update scene with animation speed from configuration
        sceneManager.SetSpeed(plugin.Configuration.AnimationSpeed);
        sceneManager.Update(lastDeltaTime);
        
        // Draw scene in a child window
        var canvasSize = ImGui.GetContentRegionAvail();
        if (canvasSize.X < 100) canvasSize.X = 800;
        if (canvasSize.Y < 100) canvasSize.Y = 600;
        
        using (var child = ImRaii.Child("SensoryCanvas", canvasSize, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (child.Success)
            {
                var drawList = ImGui.GetWindowDrawList();
                var canvasAvail = ImGui.GetContentRegionAvail();
                
                // Adjust canvas size to available space
                var adjustedSize = new Vector2(
                    MathF.Max(100, canvasAvail.X),
                    MathF.Max(100, canvasAvail.Y)
                );
                
                // Get the canvas position (relative to the child window)
                var canvasPos = ImGui.GetWindowPos();
                var canvasMin = ImGui.GetWindowContentRegionMin();
                var canvasMax = ImGui.GetWindowContentRegionMax();
                var actualCanvasPos = canvasPos + canvasMin;
                var actualCanvasSize = canvasMax - canvasMin;
                
                // Draw background (pastel mint green)
                var bgColor = Sensory.Utils.PastelColors.ToImGuiColor(
                    new Vector4(0.9f, 0.98f, 0.95f, 1.0f)
                );
                drawList.AddRectFilled(
                    actualCanvasPos,
                    actualCanvasPos + actualCanvasSize,
                    bgColor
                );
                
                // Draw current scene
                // Note: ImGui draw list coordinates are screen-relative
                if (sceneManager.CurrentScene != null)
                {
                    // Push a clip rect to ensure we only draw within the canvas
                    drawList.PushClipRect(actualCanvasPos, actualCanvasPos + actualCanvasSize, true);
                    
                    // Draw scene with canvas position offset
                    sceneManager.CurrentScene.Draw(drawList, actualCanvasPos, actualCanvasSize);
                    
                    drawList.PopClipRect();
                }
                
                // Handle clicks for ButtonGridScene (outside the clip rect)
                if (sceneManager.CurrentScene is ButtonGridScene buttonGridScene)
                {
                    if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        var mousePos = ImGui.GetMousePos();
                        // Check if click is within canvas bounds
                        if (mousePos.X >= actualCanvasPos.X && mousePos.X <= actualCanvasPos.X + actualCanvasSize.X &&
                            mousePos.Y >= actualCanvasPos.Y && mousePos.Y <= actualCanvasPos.Y + actualCanvasSize.Y)
                        {
                            buttonGridScene.HandleClick(mousePos, actualCanvasPos, actualCanvasSize);
                        }
                    }
                }
            }
        }
    }
}

