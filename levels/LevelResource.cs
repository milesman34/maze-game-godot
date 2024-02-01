using Godot;

// LevelResource contains information about a level and links to its scene
[GlobalClass]
public partial class LevelResource : Resource {
    [Export]
    public PackedScene LevelScene { get; set; }

    [Export]
    public string LevelName { get; set; }

    // Constructors for the resource
    // A parameterless one is required
    public LevelResource() : this(null, "") {}

    public LevelResource(PackedScene scene, string name) {
        LevelScene = scene;
        LevelName = name;
    }
}