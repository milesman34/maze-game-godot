// Interface that represents a scene for a state in the game
using Godot;

public interface IGameState {
    // Attach necessary signals to the main scene
    public void AttachSignals(Main main);
}