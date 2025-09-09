using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Inventory_Management_iTransition.Hubs
{
	public class CommentHub : Hub
    {
        public Task JoinInventoryGroup(string inventoryId)
        {
            return Groups.Add(Context.ConnectionId, inventoryId);
        }

        public void Send(string inventoryId, string user, string message)
        {
            Clients.Group(inventoryId).addNewMessageToPage(user, message);
        }
    }
}