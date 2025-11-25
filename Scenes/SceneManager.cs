using System;
using System.Collections.Generic;
using System.Linq;
using Sensory.Scenes;

namespace Sensory;

public class SceneManager
{
    private readonly Dictionary<string, IScene> _scenes = new();
    private string _currentSceneName = "pendulum";
    
    public string CurrentSceneName => _currentSceneName;
    public IScene? CurrentScene => _scenes.TryGetValue(_currentSceneName, out var scene) ? scene : null;
    
    public IEnumerable<string> SceneNames => _scenes.Keys;
    
    public void RegisterScene(IScene scene)
    {
        _scenes[scene.Name.ToLower()] = scene;
    }
    
    public void SwitchScene(string sceneName)
    {
        var lowerName = sceneName.ToLower();
        if (!_scenes.ContainsKey(lowerName))
            return;
            
        if (_scenes.TryGetValue(_currentSceneName, out var oldScene))
        {
            oldScene.Cleanup();
        }
        
        _currentSceneName = lowerName;
        if (_scenes.TryGetValue(_currentSceneName, out var newScene))
        {
            newScene.Initialize();
        }
    }
    
    public void Update(float deltaTime)
    {
        if (CurrentScene != null)
        {
            CurrentScene.Update(deltaTime);
        }
    }
}


