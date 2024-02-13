using Godot;
using System;

/// <summary>
/// Constants class contains some important constants for the game.
/// </summary>
public class Constants {
    /// <summary>
    /// Size of a tile in pixels (default is 32)
    /// </summary>
    public const int TileSize = 32;

    /// <summary>
    /// Size of a tile in pixels, represented as a vector.
    /// </summary>
    public static Vector2 TileVector = new Vector2(32f, 32f);
}