//note we can make list of connections as if one person open many tap 
//private static ConcurrentDictionary<string, List<string>> AgentConnections
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CallCenterUsingSignalR_2B
{
    public class CallHub : Hub
    {
        //to save agent id and connection id
        private static ConcurrentDictionary<string, string> AgentConnections = new ConcurrentDictionary<string, string>();
        //to save adhent id and 
        private static ConcurrentDictionary<string, string> AgentStatuses = new ConcurrentDictionary<string, string>();

        //Encapsulation to use private
        public static string? GetConnectionId(string agentId)
        {
            return AgentConnections.TryGetValue(agentId, out var connId) ? connId : null;
        }
        public static string? GetAgentStatus(string agentId)
        {
            return AgentStatuses.TryGetValue(agentId, out var status) ? status : null;
        }

        //dah awal lama al connection ytfat7
        public Task RegisterAgent(string agentId)
        {
            AgentConnections[agentId] = Context.ConnectionId;
            AgentStatuses[agentId] = "avaliable";
            return Task.CompletedTask;
        }

        //lama al user y2afl al mokalma aw yft7ha  al status bta3to tt3adl
        public Task UpdateStatus(string agentId, string status)
        {
            AgentStatuses[agentId] = status;
            return Task.CompletedTask;
        }

        //send call to specific user
        public async Task SendCallToAgent(string agentId, string phoneNumber)
        {
            if (AgentConnections.ContainsKey(agentId))
            {
                var connectionId = AgentConnections[agentId];
                await Clients.Client(connectionId).SendAsync("ReceiveCall", phoneNumber);
                AgentStatuses[agentId] = "busy";
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveCall", "Agent not connected");
            }
        }

        public Task DisconnectAgent(string agentId)
        {
            if (AgentConnections.ContainsKey(agentId))
            {
                var connectionId = AgentConnections[agentId];
                AgentConnections.TryRemove(agentId, out var removedConnectionId);
                AgentStatuses.TryRemove(agentId, out var RemovedConnectionId);

            }
            return Task.CompletedTask;

        }
    }
}
