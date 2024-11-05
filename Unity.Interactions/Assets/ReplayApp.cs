using System.IO;
using Domain;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Video;

public class ReplayApp : MonoBehaviour
{
	public GameObject Ball;
	public GameObject UserHead;
	public GameObject NonDominantFoot;
	public GameObject DominantFoot;
	public GameObject Opponent;
	public int FrameIndex;

	public VideoPlayer VideoPlayer;
	public string DataPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_3"; // Forward slashes

	void Start()
	{
		// Use Directory.GetFiles with forward slashes
		var csvFiles = Directory.GetFiles(DataPath, "*.csv");

		foreach (var csvFile in csvFiles)
		{
			Debug.Log("Found csv file: " + csvFile);

			// Construct paths using Path.Combine to handle separators
			var jsonFile = Path.Combine(Path.GetDirectoryName(csvFile), Path.GetFileNameWithoutExtension(csvFile) + ".json");
			var mp4File = Path.Combine(Path.GetDirectoryName(csvFile), Path.GetFileNameWithoutExtension(csvFile) + ".mp4");

			var jsonFileExists = File.Exists(jsonFile);
			var mp4FileExists = File.Exists(mp4File);

			if (!jsonFileExists || !mp4FileExists)
			{
				Debug.LogError("Missing json or mp4 file for: " + csvFile);
				continue;
			}

			VideoPlayer.url = mp4File;
			VideoPlayer.Prepare();

			var json = File.ReadAllText(jsonFile);

			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			_trial = JsonConvert.DeserializeObject<Trial>(json, jsonSettings);

			var frameEvents = CsvParser.ParseCsv(csvFile);


			Debug.Log(VideoPlayer.timeReference);
		}
	}

	void Update()
	{
		FrameIndex = Mathf.Clamp(FrameIndex, 0, _trial.NumberOfFrames - 1);


		VideoPlayer.frame = FrameIndex;

		if (VideoPlayer.isPlaying)
			VideoPlayer.Pause();
		VideoPlayer.Play();


		// Update object positions based on frame index
		DominantFoot.transform.position = _trial.UserDominantFootPositions[FrameIndex];
		NonDominantFoot.transform.position = _trial.UserNonDominantFootPositions[FrameIndex];
		Opponent.transform.position = _trial.OpponentHipPositions[FrameIndex];
		UserHead.transform.position = _trial.UserHeadPositions[FrameIndex];
	}

	Trial _trial;
	bool _videoReady;
}