using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Sensory.Scenes;

namespace Sensory.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly Configuration configuration;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("Sensory Configuration###SensoryConfigWindow")
    {
        // Remove NoResize flag to make window resizable
        Flags = ImGuiWindowFlags.None;

        Size = new Vector2(400, 300);
        SizeCondition = ImGuiCond.FirstUseEver;

        this.plugin = plugin;
        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Sensory Plugin Settings");
        ImGui.Spacing();
        
        // Scene selector
        ImGui.TextUnformatted("Scene:");
        var sceneManager = plugin.MainWindow.SceneManager;
        var currentScene = sceneManager.CurrentSceneName;
        var currentSceneName = sceneManager.CurrentScene?.Name ?? "Unknown";
        
        if (ImGui.BeginCombo("##SceneSelector", currentSceneName))
        {
            foreach (var sceneName in sceneManager.SceneNames)
            {
                var displayName = sceneManager.GetSceneDisplayName(sceneName);
                var isSelected = sceneName == currentScene;
                
                if (ImGui.Selectable(displayName, isSelected))
                {
                    sceneManager.SwitchScene(sceneName);
                    configuration.CurrentScene = sceneName;
                    configuration.Save();
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        
        // Animation Speed
        var speed = configuration.AnimationSpeed;
        if (ImGui.SliderFloat("Animation Speed", ref speed, 0.1f, 3.0f, "%.1fx"))
        {
            configuration.AnimationSpeed = speed;
            configuration.Save();
        }
        
        ImGui.Spacing();
        
        // Movable Config Window option
        var movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }
        
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        
        // Help text
        ImGui.TextWrapped("Use /sensory to open the main visual window.");
        ImGui.TextWrapped("Use /sensoryconfig to open this settings window.");
    }
}

