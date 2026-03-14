using Newtonsoft.Json;

namespace Metamong.Core
{
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
}
