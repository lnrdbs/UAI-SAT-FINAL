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

        public async Task PostUsuario(string usuario, string clave)
        {
            var c = new Usuario();
            c.Id = Guid.NewGuid().ToString();
            c.Nickname = usuario;
            c.Password = clave;
            var xx = await firebase.Child("usuarios").Child(usuario).PostAsync(c);
        }


    }

    public class InmuebleRepository : Repository<Inmueble>
    {
        private static string tag = "inmuebles";
        public InmuebleRepository() : base(tag)
        {

        }

        public async Task Crear(Inmueble inmueble)
        {
            var xx = await firebase.Child(tag).PostAsync(inmueble);
        }

        public bool Modificar(Inmueble inmueble)
        {
            try
            {
                //borro
                this.Delete(inmueble.Id.ToString());
                //update
                var xx = this.Crear(inmueble);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> Delete(string id)
        {
            /*try
            {
                var xx = firebase.Child(tag).Child("/Id/" + id).DeleteAsync();
                return false;
            }
            catch (Exception)
            {
                return false;
            }*/

            var result = await firebase.Child(tag).OnceAsync<Inmueble>();
            foreach (var item in result)
            {
               if(item.Object.Id.ToString().Equals(id))
                {
                    await firebase
                    .Child(tag)
                    .Child(item?.Key)
                    .DeleteAsync();
                }
            }

            return true;
        }

        public async Task<IList<Inmueble>> Listar()
        {
            var result = await firebase.Child(tag).OnceAsync<Inmueble>();
            IList<Inmueble> list = new List<Inmueble>();
            foreach (var item in result)
            {
                list.Add(new Inmueble()
                {
                    Abierto = item.Object.Abierto,
                    Barrio = item.Object.Barrio,
                    Descripcion = item.Object.Descripcion,
                    Id = item.Object.Id,
                    Imagen = item.Object.Imagen,
                    Titulo = item.Object.Titulo,
                    Valoracion = item.Object.Valoracion
                });
            }

            return list;
        }

        public async Task<Inmueble> Get(int id)
        {
            var result = await firebase.Child(tag).OnceAsync<Inmueble>();
            Inmueble item = new Inmueble();

            foreach (var obj in result)
            {
                if(obj.Object.Id == id)
                {
                    item = new Inmueble()
                    {
                        Abierto = obj.Object.Abierto,
                        Barrio = obj.Object.Barrio,
                        Descripcion = obj.Object.Descripcion,
                        Id = obj.Object.Id,
                        Imagen = obj.Object.Imagen,
                        Titulo = obj.Object.Titulo,
                        Valoracion = obj.Object.Valoracion
                    };
                }
                return obj.Object;
            }
            return null;
        }

        public async Task<FirebaseObject<Inmueble>> GetObject(int id)
        {
            var result = await firebase.Child(tag).OnceAsync<Inmueble>();
            Inmueble item = new Inmueble();

            foreach (var obj in result)
            {
                if (obj.Object.Id == id)
                {
                    return obj;
                }
            }
            return null;
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
