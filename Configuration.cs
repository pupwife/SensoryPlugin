using Dalamud.Configuration;
using System;

namespace Sensory;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public string CurrentScene { get; set; } = "pendulum";
    public float AnimationSpeed { get; set; } = 1.0f;

    // The below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}

