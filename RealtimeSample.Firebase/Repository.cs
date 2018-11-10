using Firebase.Database;
using Firebase.Database.Query;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtimeSample.Firebase
{
    public abstract class Repository<T> 
    {

        protected string _name;
        protected FirebaseClient firebase;
        public Repository(string name)
        {

            var cfg = new FirebaseOptions();
            
            
                firebase = new FirebaseClient("https://uai-chat.firebaseio.com"); 
            
            _name = name;

        }
    }

    public class UsuariosRepository : Repository<Usuario>
    {

        public bool IsUserOk { get { return this.isOk; }  }
        private bool isOk = false;
        public delegate void ShowMessage(Usuario msg);
        IDisposable observable;
        string fullname;
        public UsuariosRepository(string user, string clave, ShowMessage mydelegatee) : base("usuarios")
        {
            fullname = _name + "/" + user;
            observable = firebase
                .Child(fullname)
                .AsObservable<Usuario>()
                .Subscribe(d =>
                    mydelegatee.Invoke(d.Object)
                    );
            
        }


        public async Task<bool> GetUser(string user, string clave)
        {
            fullname = _name + "/" + user;
            var xx = await firebase.Child(fullname).OnceAsync<Usuario>();
            
            foreach(var usuario in xx)
            {
                if(usuario.Object.Nickname.Equals(user) &&
                    usuario.Object.Password.Equals(clave))
                {
                    this.isOk = true;
                    break;
                }
            }

            return this.isOk;
        }

        public async Task<bool> DuplicatedUser(string user)
        {
            fullname = _name + "/" + user;
            var xx = await firebase.Child(fullname).OnceAsync<Usuario>();

            foreach (var usuario in xx)
            {
                if (usuario.Object.Nickname.Equals(user))
                {
                    this.isOk = true;
                    break;
                }
            }

            return this.isOk;
        }

        public async 


        Task PostUsuario(string usuario, string clave)
        {
            var c = new Usuario();
            c.Id = Guid.NewGuid().ToString();
            c.Nickname = usuario;
            c.Password = clave;
            var xx = await firebase.Child("usuarios").Child(usuario).PostAsync(c);
        }


    }

    public class ChatRoomRepository : Repository<ChatMessage>
    {

        public delegate void ShowMessage(string room, ChatMessage msg);
        IDisposable observable;
        string fullroom;
        string _room;
        string _nick;
        public ChatRoomRepository(string room, string nick,  ShowMessage mydelegatee) : base("chat")
        {
            _nick = nick;
            //_chats = new List<ChatMessage>();
            fullroom = _name + "/" + room;
            _room = room;
            observable = firebase
            .Child(fullroom)
            .AsObservable<ChatMessage>()
            .Subscribe(d =>
                mydelegatee.Invoke(room, d.Object)
                );
        }

        public void GetAllChatRoom(string room)
        {
            var xx = firebase.Child(_name).Child(room);
            var xy = xx.OrderBy("chat/" + room + "/Time").LimitToLast(50).OnceAsync<ChatMessage>();
           
        }

     

        public async void PostMessage(string msg)
        {
            var c = new ChatMessage();
            c.Message = msg;
            c.Nikname = _nick;
            var xx = await firebase.Child(_name).Child(_room).PostAsync(c);
        }


      
    
    }
}
