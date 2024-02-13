// Contains all the information for the levels
using System;
using Godot;

/// <summary>
/// Resource which stores the resources for all playable levels.
/// </summary>
[GlobalClass]
public partial class LevelInfo : Resource {
    /// <summary>
    /// The list of all levels represented by their LevelResource objects.
    /// </summary>
    [Export]
    public LevelResource[] Levels { get; set; }

    /// <summary>
    /// Constructs a LevelInfo object.
    /// </summary>
    public LevelInfo() : this(Array.Empty<LevelResource>()) {}

    /// <summary>
    /// Constructs a LevelInfo object.
    /// </summary>
    /// <param name="levels">The array of LevelResource objects to use</param>
    public LevelInfo(LevelResource[] levels) {
        Levels = levels;
    }
}