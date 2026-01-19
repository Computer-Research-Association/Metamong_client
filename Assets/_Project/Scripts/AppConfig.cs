using UnityEngine;

[CreateAssetMenu(fileName = "AppConfig", menuName = "Scriptable Objects/AppConfig")]
public class AppConfig : ScriptableObject
{
  [Header("Server URLs")]
  public string apiBaseUrl = "http://localhost:8000/api";
  public string realtimeServerUrl = "ws://localhost:256";

  [Header("Google OAuth")]
  public string googleClientId = "1040965522257-ulcma59lr4dhq7ou3hg0kr2b91e54q6s.apps.googleusercontent.com";

  [Header("Development Setting")]
  public bool isDebugMode = true;
}
