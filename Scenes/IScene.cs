using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Sensory.Scenes;

public interface IScene
{
    string Name { get; }
    
    void Initialize();
    void Update(float deltaTime);
    void Draw(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize);
    void Cleanup();
}


