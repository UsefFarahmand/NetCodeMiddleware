using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button _Host;
    [SerializeField] private Button _Client;

    private void Start()
    {
        _Host.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        _Client.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
