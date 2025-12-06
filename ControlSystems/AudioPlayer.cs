using System;
using Speaker;

namespace RobotProject.ControlSystems;

public class AudioPlayer
{
  private readonly Dictionary<string, WavSpeaker> _audioLibrary;
  private readonly HashSet<string> _currentlyPlaying;

  private readonly Dictionary<string, int> _repeatCounts = new(); // Tracks remaining play counts

  public AudioPlayer(string audioDirectory)
  {
    _audioLibrary = new Dictionary<string, WavSpeaker>();
    _currentlyPlaying = new HashSet<string>();

    // Preload all WAV files in the specified directory
    foreach (var file in Directory.GetFiles(audioDirectory, "*.wav"))
    {
      var id = Path.GetFileNameWithoutExtension(file);
      _audioLibrary[id] = new WavSpeaker(file, negativeSampleRate: false);
    }
  }



  public async Task PlaySoundAsync(string id, int repeatTimesToPlay = 1)
  {
    if (repeatTimesToPlay <= 0)
    {
      Console.WriteLine("Invalid repeatTimesToPlay value. It must be greater than 0.");
      return;
    }

    // If the sound is not already tracked, add it with the repeat count
    if (!_repeatCounts.ContainsKey(id))
    {
      _repeatCounts[id] = repeatTimesToPlay;
    }

    // Ensure the sound is in the library and not already playing
    if (_audioLibrary.TryGetValue(id, out var speaker))
    {
      if (_currentlyPlaying.Contains(id)) return; // Prevent replay in the same update call

      // Mark as playing and decrement the repeat counter
      _currentlyPlaying.Add(id);
      _repeatCounts[id]--;

      await speaker.PlayAsync(); // Play the sound

      // Remove from currently playing after playback
      _currentlyPlaying.Remove(id);

      // If all repetitions are complete, clean up
      if (_repeatCounts[id] <= 0)
      {
        _repeatCounts.Remove(id);
      }
    }
    else
    {
      Console.WriteLine($"Audio with ID '{id}' not found.");
    }
  }


  /// <summary>
  /// Stops playing the audio associated with the given ID.
  /// </summary>
  /// <param name="id">The ID of the audio file to stop.</param>
  public void StopPlaying(string id)
  {
    if (_audioLibrary.TryGetValue(id, out var speaker))
    {
      speaker.Stop();
    }
  }
}
