/// <summary>
/// ICameraZoneListener represents any object which must know its own camera zone and potentially perform actions based on that.
/// </summary>
public interface ICameraZoneListener {
    /// <summary>
    /// Sets the listener's camera zone ID.
    /// </summary>
    /// <param name="ID">ID representing a camera zone</param>
    public void SetCameraZoneID(int ID);

    /// <summary>
    /// This function runs when a camera zone is entered by the player.
    /// </summary>
    /// <param name="ID">ID representing the camera zone being entered by the player</param>
    public void OnCameraZoneEntered(int ID);

    // Runs when a camera zone is exited
    /// <summary>
    /// This function runs when a camera zone is exited by the player.
    /// </summary>
    /// <param name="ID">ID representing the camera zone being exited by the player</param>
    public void OnCameraZoneExited(int ID);
}