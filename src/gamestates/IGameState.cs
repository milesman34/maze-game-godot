using Godot;

/// <summary>
/// Interface that represents a scene for a state in the game.
/// </summary>
public interface IGameState {
    // Attach necessary signals to the main scene
    /// <summary>
    /// Attaches necessary signals to the main scene
    /// </summary>
    /// <param name="main">Reference to the Main scene object</param>
    public void AttachSignals(Main main);
}