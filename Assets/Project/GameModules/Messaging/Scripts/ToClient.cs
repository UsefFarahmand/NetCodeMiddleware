using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ToClient : NetworkBehaviour
{
    private NetworkManager singleton => NetworkManager.Singleton;
    [SerializeField] private ulong _clientId;

    public override void OnNetworkSpawn()
    {
        singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(GetCliecnt), GetCliecnt);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        singleton.CustomMessagingManager?.UnregisterNamedMessageHandler(nameof(GetCliecnt));
    }

    public override void OnNetworkDespawn()
    {
        if (!singleton.isActiveAndEnabled) return;
    }

    [ContextMenu("Send")]
    public void Send()
    {
        if (!NetworkManager.IsServer)
        {
            Debug.LogWarning("This client is not server and couldn't send message to other clients");
            return;
        }

        string sendPos = $"Hi client {_clientId}";

        var man = singleton.CustomMessagingManager;
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(sendPos), Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(sendPos);

            man.SendNamedMessage(nameof(GetCliecnt), _clientId, writer, NetworkDelivery.Reliable);
        }
    }

    private void GetCliecnt(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out string recData);
        Debug.Log($"server send message to client id {_clientId}\n" +
            $"Sender client id is {senderClientId} ()\n" +
            recData.ToString());
    }
}
