using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeSample.Firebase;
using AuthSample.Application;

namespace RealtimeChatSample.Api.hub
{
    public class ChatHub : BaseHub
    {
        public void SendMessage(string msg, string room, string user)
        {
            //ENVIO UN MENSAJE
            var dto = new {User=user, Message = msg };
            var r = new ChatRoomRepository(room, user, ShowMessage);
            r.PostMessage(msg);
            Clients.Group(room).ReceiveMessage(dto);
        }

        public void VotoMessage(int index1, int id, string room)
        {
            var a = new InmuebleApplication();
            Inmueble item = a.Get(id).Result;
            var dto = new { index = index1, id = item.Id , pos = item.VotosPositivos, neg = item.VotosNegativos };
            Clients.Group(room).ReceiveVoteMessage(dto);
        }

        public void AllRooms(string p1="", string p2="")
        {
            //OBTENGO LA LISTA DE SALAS EN EL SERVER
            var rooms = GetConnectionMapping().Groups.Select(i => i.Key);
            Clients.Caller.GetAllRooms(rooms);
        }
        public void JoinRoom(string roomName, string alias)
        {
            //DEJO DE RECIBIR NOTOFICACIONES DE LA SALA ANTERIOR
            RemoveFromAllrooms();

            //ME UNO A LA NUEVA SALA
            AddToRoom(roomName,alias);
            Groups.Add(Context.ConnectionId, roomName);

            //OBTENGO TODOS LOS USUARIOS QUE HAY EN LA SALA
            GetAllConnectedInRoom(roomName);
            AllRooms();
        }

        public void LeaveRoom(string roomName, string alias)
        {

            //ME UNO A LA NUEVA SALA
            DelFromRoom(roomName, alias);
            Groups.Remove(Context.ConnectionId, roomName);

            //OBTENGO TODOS LOS USUARIOS QUE HAY EN LA SALA
            GetAllConnectedInRoom(roomName);
            AllRooms();
        }

        public void SayWhoIsTyping(string roomName, string alias)
        {
            Clients.OthersInGroup(roomName).sayWhoIsTyping(alias);
        }

        public void ShowNewPublish(string roomName, string alias, string titulo, int id) {
            Clients.All(roomName).showNewPublish(alias, titulo, id);
        }

        public void ShowPublishClosed(string roomName, string alias, string titulo, int id) {
            Clients.All(roomName).showPublishClosed(alias, titulo, id);
        }
    }

}
