using Godot;
using System;
using System.Collections.Generic;

// This autoloaded node manages save files (highscores)
public partial class SaveManager : Node
{
	// Map level names to their stats
	public Dictionary<string, LevelStats> levelStats;

	// Reference to the saves folder
	private DirAccess savesDir;

	// Did the player attain a new personal best in the latest run for this stat?
	public bool newBestScore = false;
	public bool newBestDeaths = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Creates the saves folder if it exists
		savesDir = DirAccess.Open("user://");

		// Create the directory if it doesn't exist
		if (!savesDir.DirExists("levelstats")) {
			savesDir.MakeDirRecursive("levelstats");
		}

		levelStats = new Dictionary<string, LevelStats>();

		// Load any save files
		LoadSaves();

		Events.instance.LevelEnd += OnLevelEnd;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Saves to the level save file with the new stats
	public void SaveToSaveFile(string levelName, int score, int deaths) {
		var path = string.Format("user://levelstats/{0}.txt", levelName);

		var saveFile = FileAccess.Open(path, FileAccess.ModeFlags.Write);

		saveFile.StoreString(score.ToString());
		saveFile.StoreString("\n");
		saveFile.StoreString(deaths.ToString());

		saveFile.Close();
	}

	public void OnLevelEnd(string levelName, int score, int deaths) {
		if (!levelStats.ContainsKey(levelName)) {
			levelStats[levelName] = new LevelStats(score, deaths);

			newBestScore = true;
			newBestDeaths = true;
		} else {
			var stats = levelStats[levelName];
			var newScore = Math.Max(score, stats.BestScore);
			var newDeaths = Math.Min(deaths, stats.BestDeaths);

			// Did the player attain a personal best for these on this run?
			newBestScore = score > stats.BestScore;
			newBestDeaths = deaths < stats.BestDeaths;

			levelStats[levelName] = new LevelStats(newScore, newDeaths);
		}

		SaveToSaveFile(levelName, levelStats[levelName].BestScore, levelStats[levelName].BestDeaths);
		GD.Print("saved file");
	}

	// Loads any of the save files into storage
	private void LoadSaves() {
		var levelStatsFolder = DirAccess.Open("user://levelstats");

		foreach (var fileName in levelStatsFolder.GetFiles()) {
			var levelFile = FileAccess.Open(string.Format("user://levelstats/{0}", fileName), FileAccess.ModeFlags.Read);

			var scoreLine = levelFile.GetLine();
			var deathsLine = levelFile.GetLine();

			var score = int.Parse(scoreLine);
			var deaths = int.Parse(deathsLine);

			// Save to the internal data structure
			// Since the level save files are named after the level, I can just remove the file ending with Split
			levelStats[fileName.Split(".txt")[0]] = new LevelStats(score, deaths);
		}
	}
}
