using Dalamud.Configuration;
using System;

namespace Sensory;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public string CurrentScene { get; set; } = "breathing circle";
    public float AnimationSpeed { get; set; } = 1.0f;
    public string ColorTheme { get; set; } = "pastel";
    
    // Scene-specific settings
    public int SpiralCount { get; set; } = 1;
    public int WaveCount { get; set; } = 3;
    public int FloatingShapesCount { get; set; } = 30;
    public int StarsCount { get; set; } = 15;

    // The below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}

