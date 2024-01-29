using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ToClients : NetworkBehaviour
{
    private NetworkManager singleton => NetworkManager.Singleton;
    private List<ulong> serverConnections = new();

    public override void OnNetworkSpawn()
    {
        singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(GetCliecnts), GetCliecnts);

        if (singleton.IsServer)
            singleton.OnClientConnectedCallback += ServerRegisterConnections;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        singleton.OnClientConnectedCallback += ServerRegisterConnections;

        singleton.CustomMessagingManager?.UnregisterNamedMessageHandler(nameof(GetCliecnts));
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

        Vector3 sendPos = Vector3.one;

        var man = singleton.CustomMessagingManager;
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(sendPos), Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(sendPos);

            man.SendNamedMessage(nameof(GetCliecnts), serverConnections, writer, NetworkDelivery.Reliable);
        }
    }

    private void ServerRegisterConnections(ulong clientId)
    {
        serverConnections.Add(clientId);
    }

    private void GetCliecnts(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out Vector3 recData);
        Debug.Log($"server send message to all clients\n" +
            $"Sender client id is {senderClientId}\n" +
            recData.ToString());
    }
}
