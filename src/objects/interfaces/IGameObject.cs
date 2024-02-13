/// <summary>
/// IGameObject represents any game object which can be placed in a level and requires handling its signals.
/// </summary>
public interface IGameObject {
    /// <summary>
    /// Attaches any important signals to the level itself, and performs any other necessary actions.
    /// </summary>
    /// <param name="level">Reference to the current Level</param>
    public void AttachSignals(Level level);
}