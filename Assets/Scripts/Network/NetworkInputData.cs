using Fusion;
using UnityEngine;


public struct NetworkInputData : INetworkInput
{
    public Vector2 MovementInput;
    public NetworkBool Jumped;
}
