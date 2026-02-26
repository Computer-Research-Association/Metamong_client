using UnityEngine;
using Colyseus.Schema;
public class Player : Schema
{
    [Type(0, "number")] public float x = 0;
    [Type(1, "number")] public float y = 0;
    [Type(2, "number")] public float speed = 0;
}
