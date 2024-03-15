using System.Collections.Generic;
using Fusion;

public class NetworkChat : NetworkBehaviour
{
    public string chatMessage = "";
    public List<KeyValuePair<string, PlayerRef>> chatLog = new();

    // Subscribe to this event to receive the updated chat log
    public delegate void NetworkChatUpdated(string chatLog);
    public static event NetworkChatUpdated OnNetworkChatUpdated;

    public void SendChatMessage(string message, PlayerRef playerRef)
    {
        RpcSendChatMessage(message, playerRef);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendChatMessage(string message, PlayerRef playerRef)
    {
        chatLog.Add(new KeyValuePair<string, PlayerRef>(message, playerRef));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSyncChat(List<KeyValuePair<string, PlayerRef>> hostChatLog)
    {
        chatLog = hostChatLog;
    }
}
