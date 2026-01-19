using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Metamong.Core
{
  #region [Enums]

  [JsonConverter(typeof(StringEnumConverter))]
  public enum AuthProvider
  {
    GOOGLE, KAKAO, NAVER
  }

  [JsonConverter(typeof(StringEnumConverter))]
  public enum RC
  {
    Torrey, JangGiRyeo, Kuyper, SonYangWon, Philadelphos, Carmichael
  }

  [JsonConverter(typeof(StringEnumConverter))]
  public enum UserStatus
  {
    ACTIVE, INACTIVE, BANNED
  }

  #endregion

  #region [Data Models - DTO]

  public class UserData
  {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Nickname { get; set; }

    [JsonProperty("real_name")]
    public string RealName { get; set; }

    public AuthProvider AuthProvider { get; set; }
    public RC Rc { get; set; }
    public UserStatus Status { get; set; }

    [JsonProperty("last_room_id")]
    public int? LastRoomId { get; set; }
  }

  #endregion
}
