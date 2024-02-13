using Godot;

/// <summary>
/// LevelResources contain information about a level and a link to its scene
/// </summary>
[GlobalClass]
public partial class LevelResource : Resource {
    /// <summary>
    /// Reference to the PackedScene used to instantiate the level.
    /// </summary>
    [Export]
    public PackedScene LevelScene { get; set; }

    /// <summary>
    /// The name of the level.
    /// </summary>
    [Export]
    public string LevelName { get; set; }

    /// <summary>
    /// Constructs a LevelResource object.
    /// </summary>
    public LevelResource() : this(null, "") {}

    /// <summary>
    /// Constructs a LevelResource object.
    /// </summary>
    /// <param name="scene">The scene used to instantiate the level</param>
    /// <param name="name">Name of the level</param>
    public LevelResource(PackedScene scene, string name) {
        LevelScene = scene;
        LevelName = name;
    }
}