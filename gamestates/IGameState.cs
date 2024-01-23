// Interface that represents a scene for a state in the game
using Godot;

public interface IGameState {
    // Call the QueueFree method
    public void QueueFree() {
        ((Node) this).QueueFree();
    }

    // Attach necessary signals to the main scene
    public void AttachSignals(Main main);
}