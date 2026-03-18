using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Metamong.Core
{
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
  public enum MBTI
  {
    ISTJ, ISFJ, INFJ, INTJ,
    ISTP, ISFP, INFP, INTP,
    ESTP, ESFP, ENFP, ENTP,
    ESTJ, ESFJ, ENFJ, ENTJ
  }
}
