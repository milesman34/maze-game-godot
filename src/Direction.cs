/// <summary>
/// Enum representing one of the 4 main directions
/// </summary>
public enum Direction {
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public static class DirectionExtensions {
    /// <summary>
    /// Returns the direction represented by the enum in degrees clockwise (with right being 0 degrees).
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static int GetRotation(this Direction direction) {
        switch (direction) {
            case Direction.Right:
                return 0;

            case Direction.Down:
                return 90;

            case Direction.Left:
                return 180;

            case Direction.Up:
                return 270;
        }

        // Default return to prevent a compile error
        return 0;
    }
}