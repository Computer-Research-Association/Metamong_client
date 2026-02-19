using System;
using UnityEngine;

public class AuthManager
{
    // Private 필드
    private string _accessToken;
    private DateTime _tokenExpiry;
    private const string TOKEN_KEY = "access_token";
    private const string EXPIRY_KEY = "token_expiry";
    private bool _initialized = false;

    // Public 프로퍼티
    public string AccessToken
    {
        get
        {
            EnsureInitialized();
            return _accessToken;
        }
        private set
        {
            _accessToken = value;
            if (!string.IsNullOrEmpty(value))
            {
                PlayerPrefs.SetString(TOKEN_KEY, value);
                PlayerPrefs.Save();
            }
        }
    }

    public bool IsLoggedIn
    {
        get
        {
            EnsureInitialized();
            return !string.IsNullOrEmpty(_accessToken) && !IsTokenExpired();
        }
    }

    // Constructor - PlayerPrefs 호출 제거!
    public AuthManager()
    {
        Debug.Log("[AuthManager] Created (lazy initialization)");
    }

    /// <summary>
    /// 지연 초기화 - 처음 접근 시에만 PlayerPrefs 로드
    /// </summary>
    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            LoadToken();
            _initialized = true;
        }
    }

    /// <summary>
    /// 토큰 설정 및 저장
    /// </summary>
    public void SetToken(string token, int expiresInSeconds = 3600)
    {
        EnsureInitialized();

        AccessToken = token;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds);

        PlayerPrefs.SetString(EXPIRY_KEY, _tokenExpiry.ToString("o"));
        PlayerPrefs.Save();

        Debug.Log($"[AuthManager] Token saved (expires in {expiresInSeconds}s)");
    }

    /// <summary>
    /// 토큰 삭제 (로그아웃)
    /// </summary>
    public void ClearToken()
    {
        _accessToken = null;
        _tokenExpiry = DateTime.MinValue;

        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey(EXPIRY_KEY);
        PlayerPrefs.Save();

        Debug.Log("[AuthManager] Token cleared");
    }

    /// <summary>
    /// Authorization 헤더 문자열 반환
    /// </summary>
    public string GetAuthorizationHeader()
    {
        EnsureInitialized();

        if (string.IsNullOrEmpty(_accessToken))
        {
            Debug.LogWarning("[AuthManager] No token available");
            return string.Empty;
        }

        if (IsTokenExpired())
        {
            Debug.LogWarning("[AuthManager] Token is expired");
            return string.Empty;
        }

        return $"Bearer {_accessToken}";
    }

    /// <summary>
    /// 저장된 토큰 로드
    /// </summary>
    private void LoadToken()
    {
        _accessToken = PlayerPrefs.GetString(TOKEN_KEY, "");

        string expiryStr = PlayerPrefs.GetString(EXPIRY_KEY, "");
        if (!string.IsNullOrEmpty(expiryStr))
        {
            if (DateTime.TryParse(expiryStr, out DateTime expiry))
            {
                _tokenExpiry = expiry;
            }
        }

        // 토큰이 만료되었으면 삭제
        if (IsTokenExpired())
        {
            Debug.Log("[AuthManager] Saved token is expired, clearing...");
            ClearToken();
        }
        else if (!string.IsNullOrEmpty(_accessToken))
        {
            TimeSpan remaining = _tokenExpiry - DateTime.UtcNow;
            Debug.Log($"[AuthManager] Token loaded (valid for {remaining.TotalMinutes:F1} minutes)");
        }
        else
        {
            Debug.Log("[AuthManager] No saved token found");
        }
    }

    /// <summary>
    /// 토큰 만료 여부 확인
    /// </summary>
    private bool IsTokenExpired()
    {
        if (_tokenExpiry == DateTime.MinValue)
            return true;

        return DateTime.UtcNow >= _tokenExpiry;
    }

    /// <summary>
    /// 토큰 남은 시간 (초)
    /// </summary>
    public int GetRemainingSeconds()
    {
        EnsureInitialized();

        if (IsTokenExpired())
            return 0;

        TimeSpan remaining = _tokenExpiry - DateTime.UtcNow;
        return (int)remaining.TotalSeconds;
    }

    /// <summary>
    /// 토큰 갱신 필요 여부 (만료 10분 전)
    /// </summary>
    public bool NeedsRefresh(int thresholdSeconds = 600)
    {
        return GetRemainingSeconds() < thresholdSeconds;
    }
}