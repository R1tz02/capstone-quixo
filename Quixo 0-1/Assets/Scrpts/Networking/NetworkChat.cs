using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkChat : NetworkBehaviour
{
    // Subscribe to this event to receive the updated chat log
    public delegate void NetworkChatUpdated(string message, PlayerRef sendingPlayerRef, PlayerRef localPlayerRef, PlayerRef hostPlayerRef);
    public static event NetworkChatUpdated OnNetworkChatUpdated;

    NetworkingManager networkingManager;

    public void Start()
    {
        networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendChatMessage(string message, PlayerRef sendingPlayerRef)
    {
        PlayerRef hostsPlayerRef;
        if (networkingManager._runner.IsServer)
        {
            hostsPlayerRef = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef;
        }
        else
        {
            PlayerRef localPlayerRef = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef;
            var otherPlayer = networkingManager._players.FirstOrDefault(p => !p.Key.Equals(localPlayerRef));

            if (otherPlayer.Key != null)
            {
                hostsPlayerRef = otherPlayer.Key;
            }
            else
            {
                throw new Exception("Could not find host player for chat message");
            }
        }

        OnNetworkChatUpdated?.Invoke(message, sendingPlayerRef, networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef, hostsPlayerRef);
    }
}
