using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeSample.Firebase;

namespace RealtimeChatSample.Api.hub
{
    public class ConnectionMapping<T>
    {
        //MANTENGO LAS CONEXIONES EXISTENTES
        private readonly Dictionary<T, List<string>> _connections = new Dictionary<T, List<string>>();

        //MANTENGO LA CONEXIONES POR GRUPO / USUARIO
        private readonly Dictionary<T, List<User>> _groups = new Dictionary<T, List<User>>();

        //Mensajes por Chats
        private readonly Dictionary<T, List<ChatMessage>> _chats = new Dictionary<T, List<ChatMessage>>();



        public Dictionary<T, List<ChatMessage>> Chats { get { return _chats; } }

        public Dictionary<T, List<User>> Groups { get { return _groups; } }

        public bool AddMessage(T groupKey, ChatMessage msg)
        {
            lock (_chats)
            {
                List<ChatMessage> messages;
                if (!_chats.TryGetValue(groupKey, out messages))
                {
                    //SI NO EXISTE EL GRUPO, LO CREO.
                    messages = new List<ChatMessage>();
                    _chats.Add(groupKey, messages);
                }
                if (messages.Where(m => m.Nikname == msg.Nikname && m.Time == msg.Time && m.Message == msg.Message).Count() == 0)
                {
                    messages.Add(msg);
                    return true;
                }
            }

            return false;
        }

        public IList GetAllMesssagesInGroup(T groupKey)
        {

            //OBTENGO Mensajes 
            List<ChatMessage> messages;
            IList conn = new List<ChatMessage>();
            if (_chats.TryGetValue(groupKey, out messages))
            {

                lock (_chats)
                {
                    foreach (var item in messages)
                    {
                        conn.Add(item);
                    }

                }
                return conn;

            }
            return null;
        }
        
        public void RemoveFromGroup(T groupKey,string id)
        {

            lock (_groups)
            {
                List<User> connections;
                if (!_groups.TryGetValue(groupKey, out connections))
                {
                    return;
                }

                lock (connections)
                {
           
                    connections.RemoveAll(o => o.ConnectionId == id);
                    if (connections.Count == 0)
                    {
                        _connections.Remove(groupKey);
                    }
                    _chats.Remove(groupKey);
                }
            }
        }

        //AGREGAR UN  USUARIO A UN GRUPO
        public void AddGroup(T groupKey, User user)
        {
            lock (_groups)
            {
                List<User> connections;
                if (!_groups.TryGetValue(groupKey, out connections))
                {
                    //SI NO EXISTE EL GRUPO, LO CREO.
                    connections = new List<User>();
                    connections.Add(user);
                    _groups.Add(groupKey, connections);
                }
                if (!connections.Contains(user))
                {
                    connections.Add(user);
                }


            }
        }

        public IList GetAllConnectedInGroup(T groupKey)
        { 

            //OBTENGO LA LISTA 
            List<User> connections;
            IList conn = new List<User>();
            if (_groups.TryGetValue(groupKey, out connections))
            {

                lock (_groups)
                {

                    foreach (var item in connections)
                    {

                        conn.Add(item);



                    }

                }

                return conn;

            }
            return null;





        }

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public IList GetAllConnectedUsers()
        {
            IList conn = new List<string>();
            lock (_connections)
            {

                foreach (var item in _connections)
                {

                    conn.Add(item.Key);



                }

            }

            return conn;
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                List<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new List<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            List<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                List<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }

    }
}
