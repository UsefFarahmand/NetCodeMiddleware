using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.CustomMessagingManager;

public class MessageSender : NetworkBehaviour
{

    private List<HandleNamedMessageDelegate> _catched = new();

    private int _lastId = 0;

    public void Register(HandleNamedMessageDelegate action)
    {
        if (!_catched.Contains(action))
        {
            _catched.Add(action);
            NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(nameof(action), action);
        }
    }

    public void Unregister(HandleNamedMessageDelegate action)
    {
        if (_catched.Contains(action))
        {
            _catched.Remove(action);
            NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(nameof(action));
        }
    }

    public void SendToAll(HandleNamedMessageDelegate action, Vector3 data)
    {
        var man = NetworkManager.CustomMessagingManager;
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(data), Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(data);
            man.SendNamedMessageToAll(nameof(action), writer, NetworkDelivery.Reliable);
        }
    }
}
