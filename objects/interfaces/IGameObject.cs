// IGameObject represents any game object that can be placed in a level
public interface IGameObject {
    // Attaches the signals to the level itself
    public void AttachSignals(Level level);
}