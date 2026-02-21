using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Metamong.Core
{
  #region [Enums]

  [JsonConverter(typeof(StringEnumConverter))]
  public enum AuthProvider
  {
    GOOGLE, NAVER, KAKAO, LOCAL
  }

  [JsonConverter(typeof(StringEnumConverter))]
  public enum RC
  {
    UNASSIGNED, Torrey, JangGiRyeo, Kuyper, SonYangWon, Philadelphos, Carmichael
  }

  [JsonConverter(typeof(StringEnumConverter))]
  public enum UserStatus
  {
    ACTIVE, SUSPENDED, DELETED, NEW, GUEST
  }

  [JsonConverter(typeof(StringEnumConverter))]
  public enum MBTI    // 추가
  {
    ISTJ, ISFJ, INFJ, INTJ,
    ISTP, ISFP, INFP, INTP,
    ESTP, ESFP, ENFP, ENTP,
    ESTJ, ESFJ, ENFJ, ENTJ
  }

  #endregion

  #region [Response DTOs]


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

    // 추가 프로필 필드
    [JsonProperty("student_id")] public string StudentId { get; set; }
    [JsonProperty("major")] public string Major { get; set; }
    [JsonProperty("phone_number")] public string PhoneNumber { get; set; }
    [JsonProperty("instagram_id")] public string InstagramId { get; set; }
    [JsonProperty("mbti")] public MBTI? Mbti { get; set; }

    // [JsonProperty("last_room_id")] public int? LastRoomId { get; set; }
    // [JsonProperty("last_room_x")] public int? LastRoomX { get; set; }
    // [JsonProperty("last_room_y")] public int? LastRoomY { get; set; }

    // ---- 헬퍼 프로퍼티 ----------------------------------------------

    /// <summary>최초 가입, RC/프로필 미설정 상태</summary>
    public bool IsNewUser => Status == UserStatus.NEW;

    /// <summary>정상 활동 가능 상태</summary>
    public bool IsActive => Status == UserStatus.ACTIVE;

    /// <summary>RC가 아직 배정되지 않은 상태</summary>
    public bool IsRcUnassigned => Rc == RC.UNASSIGNED;
  }

  #endregion


  #region [Request DTOs]

  /// <summary>
  /// PATCH /api/users/me/rc 요청 스키마
  /// FastAPI RCUpdate와 1:1 대응
  /// </summary>
  public class RCUpdateRequest
  {
    [JsonProperty("rc")]
    public RC Rc { get; set; }

    public RCUpdateRequest(RC rc)
    {
      Rc = rc;
    }
  }

  /// <summary>
  /// PATCH /api/users/me/initialize 요청 스키마
  /// FastAPI InitializeUserInfo와 1:1 대응
  /// NEW 유저 최초 프로필 설정 시 사용
  /// </summary>
  public class InitializeUserRequest
  {
    [JsonProperty("rc")]
    public RC Rc { get; set; }                   // 필수

    [JsonProperty("student_id")]
    public string StudentId { get; set; }         // Optional

    [JsonProperty("major")]
    public string Major { get; set; }             // Optional

    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }       // Optional

    [JsonProperty("instagram_id")]
    public string InstagramId { get; set; }       // Optional

    [JsonProperty("mbti")]
    public MBTI? Mbti { get; set; }               // Optional

    public InitializeUserRequest(RC rc)
    {
      Rc = rc;
    }
  }

  #endregion
}
