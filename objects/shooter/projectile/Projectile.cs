using Godot;
using System;

// Contains information about a projectile to be shot by the Shooter
[GlobalClass]
public partial class Projectile : Resource
{
    [Export]
    public Texture2D ProjectileTexture { get; set; }

    // Should it disappear when hitting a wall?
    [Export]
    public bool CollidesWithWalls { get; set; } = true;

    // Constructors for the resource
    public Projectile(Texture2D texture, bool collidesWithWalls) {
        ProjectileTexture = texture;
        CollidesWithWalls = collidesWithWalls;
    }

    public Projectile() : this(null, false) {}
}
