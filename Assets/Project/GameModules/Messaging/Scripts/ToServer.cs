using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ToServer : NetworkBehaviour
{
    private NetworkManager singleton => NetworkManager.Singleton;

    public override void OnNetworkSpawn()
    {
        singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(GetServer), GetServer);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        singleton.CustomMessagingManager?.UnregisterNamedMessageHandler(nameof(GetServer));
    }

    public override void OnNetworkDespawn()
    {
        if (!singleton.isActiveAndEnabled) return;
    }

    [ContextMenu("Send")]
    public void Send()
    {
        Custom data = new()
        {
            _int = 1,
            _float = 0.23f,
            _string = "custom text"
        };

        byte[] serializedData = ToByte(data);


        var man = singleton.CustomMessagingManager;
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serializedData), Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(serializedData);

            man.SendNamedMessage(nameof(GetServer), NetworkManager.ServerClientId, writer, NetworkDelivery.Reliable);
        }
    }

    private void GetServer(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out byte[] recData);
        Custom data = (Custom)ToObject(recData);
        Debug.Log($"send message to server\n" +
            $"Sender client id is {senderClientId}\n" +
            $"Int: {data._int}, Float: {data._float}, String: {data._string}");
    }

    private byte[] ToByte(object data)
    {
        if (data == null) return null;

        BinaryFormatter bf = new();
        MemoryStream ms = new();
        bf.Serialize(ms, data);

        return ms.ToArray();
    }

    private object ToObject(byte[] arrBytes)
    {
        MemoryStream ms = new();
        BinaryFormatter bf = new();

        ms.Write(arrBytes, 0, arrBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        object obj = (object)bf.Deserialize(ms);

        return obj;
    }

    [System.Serializable]
    public class Custom
    {
        public int _int;
        public float _float;
        public string _string;
    }
}
