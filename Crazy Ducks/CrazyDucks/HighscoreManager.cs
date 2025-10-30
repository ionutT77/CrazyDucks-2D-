using System;
using System.IO;
using System.Text;

namespace CrazyDucks
{
    public class HighscoreData
    {
        public int Highscore { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public static class HighscoreManager
    {
        private static readonly string highscoreFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CrazyDucks",
            "highscore.json"
        );

        /// <summary>
        /// Saves the highscore to a JSON file
        /// </summary>
        public static void SaveHighscore(int highscore)
        {
            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(highscoreFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create simple JSON manually (no external library needed)
                string json = string.Format(
                    "{{\"Highscore\":{0},\"LastUpdated\":\"{1}\"}}",
                    highscore,
                    DateTime.Now.ToString("o") // ISO 8601 format
                );

                // Write to file
                File.WriteAllText(highscoreFilePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Log error but don't crash the game
                System.Diagnostics.Debug.WriteLine($"Error saving highscore: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the highscore from a JSON file
        /// Returns 0 if file doesn't exist or error occurs
        /// </summary>
        public static int LoadHighscore()
        {
            try
            {
                // Check if file exists
                if (!File.Exists(highscoreFilePath))
                {
                    return 0;
                }

                // Read JSON from file
                string json = File.ReadAllText(highscoreFilePath, Encoding.UTF8);

                // Simple JSON parsing (looking for "Highscore":number)
                // Format: {"Highscore":123,"LastUpdated":"2024-01-01T12:00:00"}
                string searchPattern = "\"Highscore\":";
                int startIndex = json.IndexOf(searchPattern);
                
                if (startIndex == -1)
                {
                    return 0;
                }

                startIndex += searchPattern.Length;
                int endIndex = json.IndexOf(',', startIndex);
                if (endIndex == -1)
                {
                    endIndex = json.IndexOf('}', startIndex);
                }

                if (endIndex > startIndex)
                {
                    string highscoreStr = json.Substring(startIndex, endIndex - startIndex).Trim();
                    int highscore;
                    if (int.TryParse(highscoreStr, out highscore))
                    {
                        return highscore;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                // Log error but don't crash the game
                System.Diagnostics.Debug.WriteLine($"Error loading highscore: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Deletes the highscore file (for testing or reset)
        /// </summary>
        public static void ResetHighscore() //not used function, i will if i need a reset highscore feature(even though its unlikely)
        {
            try
            {
                if (File.Exists(highscoreFilePath))
                {
                    File.Delete(highscoreFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting highscore: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the file path where highscore is saved (for debugging)
        /// </summary>
        public static string GetHighscoreFilePath()
        {
            return highscoreFilePath;
        }
    }
}
