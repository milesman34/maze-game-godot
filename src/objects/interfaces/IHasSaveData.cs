/// <summary>
/// IHasSaveData represents any object which must manage its own save data to deal with the player's death and/or reaching checkpoints.
/// </summary>
public interface IHasSaveData {
    /// <summary>
    /// Saves the current state when a checkpoint is reached.
    /// </summary>
    public void SaveCurrentState();

    /// <summary>
    /// Reloads the current state when the player dies.
    /// </summary>
    public void ReloadCurrentState();
}