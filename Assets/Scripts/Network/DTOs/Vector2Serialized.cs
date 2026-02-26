using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Vector2Serialized
{
    // "x"라는 이름의 JSON 데이터를 받아서 대문자 X 변수에 넣어라!
    [JsonProperty("x")] 
    public float x;

    [JsonProperty("y")]
    public float y;
}
