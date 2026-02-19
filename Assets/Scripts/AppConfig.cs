using UnityEngine;

public class AppConfig : MonoBehaviour
{
  private static AppConfig _instance;
  public static AppConfig Instance
  {
    get
    {
      if (_instance == null)
      {
        GameObject go = new GameObject("AppConfig");
        _instance = go.AddComponent<AppConfig>();
        // _instance = Resources.Load<AppConfig>("Configs/ENV");
        DontDestroyOnLoad(go);
      }
      return _instance;
    }
  }

  private void Awake()
  {
    if (_instance != null && _instance != this)
    {
      Destroy(gameObject);
      return;
    }

    _instance = this;
    DontDestroyOnLoad(gameObject);
  }


  [Header("Server URLs")]
  public string apiBaseUrl = "http://localhost:8000/api";
  public string realtimeServerUrl = "ws://localhost:256";

  [Header("Google OAuth")]
  [Tooltip("클라이언트 시크릿을 공개 저장소에 포함하지 마세요.")]
  public string googleClientId = "1040965522257-ulcma59lr4dhq7ou3hg0kr2b91e54q6s.apps.googleusercontent.com";

  [Header("Development Setting")]
  public bool isDebugMode = true;
}
