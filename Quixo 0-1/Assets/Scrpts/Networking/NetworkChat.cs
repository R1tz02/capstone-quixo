using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class NetworkChat : NetworkBehaviour
{
    // Subscribe to this event to receive the updated chat log
    public delegate void NetworkChatUpdated(ChatMessage chatMessage);
    public static event NetworkChatUpdated OnNetworkChatUpdated;
    public List<ChatMessage> chatLog = new();

    public struct MaxStorage : IFixedStorage
    {
        public readonly int Capacity => 256;
    }

    [Serializable]
    public struct ChatMessage : INetworkStruct
    {
        public NetworkString<MaxStorage> message;
        public PlayerRef playerRef;
    }

    public void SendChatMessage(string message, PlayerRef playerRef)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            message = new NetworkString<MaxStorage>(message),
            playerRef = playerRef
        };

        RpcSendChatMessage(chatMessage);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendChatMessage(ChatMessage chatMessage)
    {
        chatLog.Add(chatMessage);

        OnNetworkChatUpdated?.Invoke(chatMessage);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSyncChat(ChatMessage[] chatLog)
    {
        this.chatLog = chatLog.ToList();
    }
}
