using Newtonsoft.Json;

namespace Metamong.Core
{
  /// <summary>
  /// GET /api/users/me 응답 스키마
  /// FastAPI UserResponse와 1:1 대응
  /// </summary>
  public class UserData
  {
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("nickname")] public string Nickname { get; set; }
    [JsonProperty("real_name")] public string RealName { get; set; }
    [JsonProperty("auth_provider")] public AuthProvider AuthProvider { get; set; }
    [JsonProperty("rc")] public RC Rc { get; set; }
    [JsonProperty("status")] public UserStatus Status { get; set; }

    [JsonProperty("student_id")] public string StudentId { get; set; }
    [JsonProperty("major")] public string Major { get; set; }
    [JsonProperty("phone_number")] public string PhoneNumber { get; set; }
    [JsonProperty("instagram_id")] public string InstagramId { get; set; }
    [JsonProperty("mbti")] public MBTI? Mbti { get; set; }

    // ---- 헬퍼 프로퍼티 ----------------------------------------------

    /// <summary>최초 가입, RC/프로필 미설정 상태</summary>
    public bool IsNewUser => Status == UserStatus.NEW;

    /// <summary>정상 활동 가능 상태</summary>
    public bool IsActive => Status == UserStatus.ACTIVE;

    /// <summary>RC가 아직 배정되지 않은 상태</summary>
    public bool IsRcUnassigned => Rc == RC.UNASSIGNED;
  }

  /// <summary>POST /api/auth/dev-login 응답 스키마 (에디터 전용)</summary>
  public class TokenResponse
  {
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("token_type")]   public string TokenType   { get; set; }
    [JsonProperty("user_id")]      public int    UserId      { get; set; }
    [JsonProperty("email")]        public string Email       { get; set; }
    [JsonProperty("nickname")]     public string Nickname    { get; set; }
  }
}
