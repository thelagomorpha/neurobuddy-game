using System;

/// <summary>
/// Central configuration values for the NeuroBuddy game client.
/// Set BACKEND_URL in a .env file at the project root to override the default.
/// </summary>
public static class GameConfig
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BACKEND_URL") ?? "http://localhost:8000";
}
