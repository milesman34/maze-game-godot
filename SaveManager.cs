using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Autoloaded node that manages the save files (highscores).
/// </summary>
public partial class SaveManager : Node {
	// Map level names to their stats
	private Dictionary<string, LevelStats> levelStats;

	// Reference to the saves folder
	private DirAccess savesDir;

	// Did the player attain a new personal best in the latest run for this stat?
	private bool newBestScore = false;

	/// <summary>
	/// Did the player attain a new personal best score on the latest attempt?
	/// </summary>
	public bool NewBestScore { 
		get {
			return newBestScore;
		}
	}

	private bool newBestDeaths = false;

	/// <summary>
	/// Did the player attain a new personal fewest deaths on the latest attempt?
	/// </summary>
	public bool NewBestDeaths {
		get {
			return newBestDeaths;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Creates the saves folder if it exists
		savesDir = DirAccess.Open("user://");

		// Create the directory if it doesn't exist
		if (!savesDir.DirExists("levelstats")) {
			savesDir.MakeDirRecursive("levelstats");
		}

		levelStats = new Dictionary<string, LevelStats>();

		// Load any save files
		LoadSaves();

		Events.Instance.LevelEnd += OnLevelEnd;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	/// <summary>
	/// Returns if the SaveManager has a save file for the current level.
	/// </summary>
	/// <param name="levelName">Name of the level</param>
	/// <returns></returns>
	public bool HasSaveForLevel(string levelName) {
		return levelStats.ContainsKey(levelName);
	}

	/// <summary>
	/// Returns the stats for the current level.
	/// </summary>
	/// <param name="levelName"></param>
	/// <returns></returns>
	public LevelStats GetStatsForLevel(string levelName) {
		return levelStats[levelName];
	}

	/// <summary>
	/// Returns the best score attained for the given level.
	/// </summary>
	/// <param name="levelName">Name of the level</param>
	/// <returns></returns>
	public int GetBestScore(string levelName) {
		return levelStats[levelName].BestScore;
	}

	/// <summary>
	/// Returns the fewest deaths for the given level.
	/// </summary>
	/// <param name="levelName">Name of the level</param>
	/// <returns></returns>
	public int GetFewestDeaths(string levelName) {
		return levelStats[levelName].BestDeaths;
	}

	// Saves to the level save file with the new stats
	private void SaveToSaveFile(string levelName, int score, int deaths) {
		var path = string.Format("user://levelstats/{0}.txt", levelName);

		var saveFile = FileAccess.Open(path, FileAccess.ModeFlags.Write);

		saveFile.StoreString(score.ToString());
		saveFile.StoreString("\n");
		saveFile.StoreString(deaths.ToString());

		saveFile.Close();
	}

	private void OnLevelEnd(string levelName, int score, int deaths) {
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
