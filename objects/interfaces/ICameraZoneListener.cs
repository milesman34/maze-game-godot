// ICameraZoneListener represents objects which must know their own camera zone
public interface ICameraZoneListener {
    // Sets the listener's camera zone ID
    public void SetCameraZoneID(int ID);

    // Runs when a camera zone is registered
    public void OnRegisterCameraZone(CameraZone zone);
}