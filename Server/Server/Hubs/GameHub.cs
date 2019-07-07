using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        // Players list that save the players connection id and group id
        private static Dictionary<string, string> players = new Dictionary<string, string>();

        /// <summary>
        /// Do something when player join in the game
        /// </summary>
        public override Task OnConnected()
        {
            // Add player to the players list and set the group id to -1
            players.Add(Context.ConnectionId, "-1");

            return base.OnConnected();
        }

        /// <summary>
        /// Search for an alone opponent to connect them together
        /// </summary>
        public void SearchOpponent()
        {
            // Find a player that isn't in any group
            string aloneOpponentId = players.FirstOrDefault(user => (user.Value == "-1" && user.Key != Context.ConnectionId)).Key;

            if(aloneOpponentId != null) // If player found
            {
                // Set groupId to both of players information in players list
                string groupId = Context.ConnectionId + aloneOpponentId;
                players[Context.ConnectionId] = groupId;
                players[aloneOpponentId] = groupId;

                // Add both of players to the group
                Groups.Add(Context.ConnectionId, groupId);
                Groups.Add(aloneOpponentId, groupId);

                // Send Success message to these players
                Clients.Clients(new List<string>() { Context.ConnectionId, aloneOpponentId }).JoinToOppenent(true);
            }
            else // If player not found
            {
                // Send Fail message to the caller player
                Clients.Caller.JoinToOpponent(false);
            }
        }
    }
}