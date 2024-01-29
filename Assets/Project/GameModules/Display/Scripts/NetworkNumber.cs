using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkNumber : NetworkBehaviour
{
    [SerializeField] private TMP_Text m_Text;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_Text.SetText(OwnerClientId.ToString());
    }
}
