using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkChat : NetworkBehaviour
{
    // Subscribe to this event to receive the updated chat log
    public delegate void NetworkChatUpdated(string message, PlayerRef sendingPlayerRef, PlayerRef localPlayerRef);
    public static event NetworkChatUpdated OnNetworkChatUpdated;

    NetworkingManager networkingManager;

    public void Start()
    {
        networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendChatMessage(string message, PlayerRef sendingPlayerRef)
    {
        OnNetworkChatUpdated?.Invoke(message, sendingPlayerRef, networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef);
    }
}
