// Contains all the information for the levels
using System;
using Godot;

[GlobalClass]
public partial class LevelInfo : Resource {
    [Export]
    public LevelResource[] Levels { get; set; }

    public LevelInfo() : this(Array.Empty<LevelResource>()) {}

    public LevelInfo(LevelResource[] levels) {
        Levels = levels;
    }
}