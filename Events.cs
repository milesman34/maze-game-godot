using System;
using Godot;

// This autoloaded node handles some global events
public partial class Events : Node {
    // Static reference to the Events singleton
    public static Events instance = new Events();

    // Runs when the level ends
    [Signal]
    public delegate void LevelEndEventHandler();
    
    // Constructor
    public Events() {}
}