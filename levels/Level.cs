using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class Level : Node2D
{
    // List of rooms in this level
    [Export]
    private Godot.Collections.Array<PackedScene> RoomScenes { get; set; }

    // Starting room and position
    [Export]
    private string StartingRoom { get; set; }

    [Export]
    private Vector2 StartPosition { get; set; }

    // Track the current score
    public int score = 0;

    // Store the instantiated rooms
    private Dictionary<string, Room> Rooms = new Dictionary<string, Room>();

    public override void _Ready()
    {
        // Instantiate a copy of each room
        foreach (var scene in RoomScenes) {
            var instance = scene.Instantiate<Room>();

            instance.AddScore += OnAddScore;

            Rooms[instance.RoomName] = instance;
        }

        // Instantiate the first room
        SwitchToRoom(StartingRoom);
    }

    // Switches to a room if it exists, otherwise instantiating it
    private void SwitchToRoom(string roomName) {
        if (Rooms.ContainsKey(roomName)) {
            var room = Rooms[roomName];
            room.StartPosition = StartPosition;
            AddChild(room);
            room.Load();
        }
    }

    // Adds to the score
    private void OnAddScore(int value) {
        score += value;
        GD.Print(score);
    }
}