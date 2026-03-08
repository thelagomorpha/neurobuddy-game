using System;
using System.IO;
using UnityEngine;

public static class EnvLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        // In Editor: Application.dataPath = <project>/Assets → go up one level for project root
        // In builds: Application.dataPath = <build>/<name>_Data → go up one level for build root
        string envPath = Path.Combine(Application.dataPath, "..", ".env");
        if (!File.Exists(envPath)) return;

        foreach (string line in File.ReadAllLines(envPath))
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

            int idx = trimmed.IndexOf('=');
            if (idx < 0) continue;

            string key = trimmed.Substring(0, idx).Trim();
            string value = trimmed.Substring(idx + 1).Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
