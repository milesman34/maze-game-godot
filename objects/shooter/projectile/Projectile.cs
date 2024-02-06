using Godot;
using System;

/// <summary>
/// The Projectile resource contains information about a projectile to be shot by a Shooter.
/// </summary>
[GlobalClass]
public partial class Projectile : Resource {
    /// <summary>
    /// Texture that the projectile uses.
    /// </summary>
    [Export]
    public Texture2D ProjectileTexture { get; set; }

    /// <summary>
    /// Should the projectile be destroyed after hitting a wall?
    /// </summary>
    [Export]
    public bool CollidesWithWalls { get; set; } = true;

    /// <summary>
    /// Constructs a Projectile object.
    /// </summary>
    /// <param name="texture">Texture to display</param>
    /// <param name="collidesWithWalls">Should it collide with walls?</param>
    public Projectile(Texture2D texture, bool collidesWithWalls) {
        ProjectileTexture = texture;
        CollidesWithWalls = collidesWithWalls;
    }

    /// <summary>
    /// Constructs a Projectile object.
    /// </summary>
    public Projectile() : this(null, false) {}
}
