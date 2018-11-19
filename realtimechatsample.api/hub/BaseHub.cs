using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeSample.Firebase;

namespace RealtimeChatSample.Api.hub
{
    public abstract class BaseHub : Hub
    {

        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();



        public static ConnectionMapping<string> GetConnectionMapping()
        {
            return _connections;
        }

        protected void RemoveFromAllrooms()
        {


            foreach (var item in _connections.Groups)
            {

                RemoveFromRoom(item.Key);


            }
        }

        protected void RemoveFromRoom(string room)
        {
         //   string name = Context.QuerreyString["userId"];
            _connections.RemoveFromGroup(room,Context.ConnectionId);

         
            Clients.Group(room).LeaveRoom(Context.ConnectionId);

        }
        
        /*
        protected void AddToRoom(string room,string alias)
        {

            var user = new User();
            user.Alias = alias;
            user.ConnectionId = Context.ConnectionId;
            Clients.Group(room).JoinRoom(user);
            var r = new ChatRoomRepository(room, alias, ShowMessage);
            r.GetAllChatRoom(room);
            // Clients.All.NewRoom(room);
            chatroom = room;
            
        }


        public void ShowMessage(ChatMessage msg)
        {
            if(msg!=null)
                _chats.Add(msg);
            ShowLastMessages();
        }

        private void ShowLastMessages()
        {
            foreach (var item in _chats.Reverse().Take(6).Reverse())
            {
                if (item != null)
                {
                    var dto = new { time = item.Time, User = item.Nikname, Message = item.Message };
                    Clients.Group(chatroom).ReceiveMessage(dto);

                }
            }
        }
        */

        protected void AddToRoom(string room, string alias)
        {
            var user = new User();
            user.Alias = alias;
            user.ConnectionId = Context.ConnectionId;

            if (!_connections.Groups.ContainsKey(room))
            {
                Clients.All.NewRoom(room);
                var r = new ChatRoomRepository(room, alias, ShowMessage);
                r.GetAllChatRoom(room);
            }
            _connections.AddGroup(room, user);

            Clients.Group(room).JoinRoom(user);

            //Clients.Group(room).ReceiveMessage(new { User = "chat", Message = "Ingresó " + alias });
        }

        protected void DelFromRoom(string room, string alias)
        {
            var user = new User();
            user.Alias = alias;
            user.ConnectionId = Context.ConnectionId;

            Clients.Group(room).LeaveRoom(user);

            //Clients.Group(room).ReceiveMessage(new { User = ">> ", Message = alias + " Abandono la sala" });
        }

        public void ShowMessage(string room, ChatMessage msg)
        {
            if (msg != null)  {
                if (_connections.AddMessage(room, msg)) { 
                    var dto = new { User = msg.Nikname, Message = msg.Message };
                    Clients.Group(room).ReceiveMessage(dto);
                }
            }
        }

        public IList GetAllMessagesInRoom(string roomId)
        {
            return _connections.GetAllMesssagesInGroup(roomId);
        }

        public IList GetAllInRoom(string roomId)
        {
            return _connections.GetAllConnectedInGroup(roomId);
        }

        public void GetAllConnectedInRoom(string roomId)
        {

            dynamic c = new ExpandoObject();
            c.room = roomId;
            c.users = _connections.GetAllConnectedInGroup(roomId);


            Clients.Client(Context.ConnectionId).AllConnectedInRoom(c);




        }

        public override Task OnConnected()
        {
            _connections.Add(Context.ConnectionId, Context.ConnectionId);
            var rooms = _connections.Groups;
            //Clients.Client(Context.ConnectionId).AllRooms(rooms);
            //Clienter.AllRooms(rooms);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

           


                _connections.Remove(Context.ConnectionId, Context.ConnectionId);
            RemoveFromAllrooms();


            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {

            if (Context.QueryString.ContainsKey("userId"))

            {
                string name = Context.QueryString["userId"];

                if (!_connections.GetConnections(Context.ConnectionId).Contains(Context.ConnectionId))

                {
                 //   Clients.Client(Context.ConnectionId).AllConnectedUsers(_connections.GetAllConnectedUsers());
                //    Clients.All.ConnectedUser(name);
                    _connections.Add(name, Context.ConnectionId);
                }
            }




            return base.OnReconnected();
        }

    }
}

    

