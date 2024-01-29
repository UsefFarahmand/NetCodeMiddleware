using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ClientNetworkTransform))]
public class NetworkMovement : NetworkBehaviour
{
    [SerializeField, Min(0)] private float _moveSpeed = 3;

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        transform.position += _moveSpeed * Time.deltaTime * moveDir;
    }
}
