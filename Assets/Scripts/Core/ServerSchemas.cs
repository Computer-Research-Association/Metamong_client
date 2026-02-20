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

  #region [Data Models - DTO]

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

    // 위치 정보
    [JsonProperty("last_room_id")] public int? LastRoomId { get; set; }
    [JsonProperty("last_room_x")] public int? LastRoomX { get; set; }
    [JsonProperty("last_room_y")] public int? LastRoomY { get; set; }

    // 헬퍼 프로퍼티 ---------------------------------------------------------

    /// <summary>최초 가입 후 프로필 미완성 상태</summary>
    public bool IsNewUser => Status == UserStatus.NEW;

    /// <summary>활동 가능한 상태인지 여부</summary>
    public bool IsActive => Status == UserStatus.ACTIVE;

    /// <summary>마지막 위치 정보가 있는지 여부</summary>
    public bool HasLastPosition =>
        LastRoomId.HasValue && LastRoomX.HasValue && LastRoomY.HasValue;
  }

  #endregion
}
