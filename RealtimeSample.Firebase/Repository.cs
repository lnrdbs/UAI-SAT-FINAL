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
            var a = await this.Get(inmueble.Id);
            if (a != null)
                throw new Exception("Registro con Id: " + inmueble.Id + " ya existe!");

                var xx = await firebase.Child(tag).PostAsync(inmueble);


        }

        public async Task<bool> Modificar(Inmueble inmueble)
        {
            try
            {
                //borro
                var t = await this.Delete(inmueble.Id.ToString());
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
                    Valoracion = item.Object.Valoracion,
                    VotosNegativos = item.Object.VotosNegativos,
                    VotosPositivos = item.Object.VotosPositivos
                });
            }
            
            return await Task.Run(() => new List<Inmueble>(list));
           
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
                    return item;
                }                
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

    public class ValoracionRepository : Repository<Valoracion>
    {
        private static string tag = "valoracion";
        public ValoracionRepository() : base(tag)
        {

        }

        public async Task Crear(Valoracion item)
        {
            var a = await this.Get(item.Id, item.Nickname);
            if (a != null)
                throw new Exception("Registro con Id: " + item.Id + " ya existe!");

            var xx = await firebase.Child(tag).PostAsync(item);


        }

        public async Task<bool> Modificar(Valoracion item)
        {
            try
            {
                //borro
                var t = await this.Delete(item.Id.ToString(), item.Nickname);
                //update
                var xx = this.Crear(item);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> Delete(string id, string nickname)
        {


            var result = await firebase.Child(tag).OnceAsync<Valoracion>();
            foreach (var item in result)
            {
                if (item.Object.Id.ToString().Equals(id) &&
                    item.Object.Nickname.ToString().Equals(nickname))
                {
                    await firebase
                    .Child(tag)
                    .Child(item?.Key)
                    .DeleteAsync();
                }
            }

            return true;
        }

        public async Task<IList<Valoracion>> Listar(int id)
        {
            var result = await firebase.Child(tag).OnceAsync<Valoracion>();
            IList<Valoracion> list = new List<Valoracion>();
            foreach (var item in result)
            {
                if(item.Object.Id == id)
                { 
                    list.Add(new Valoracion()
                    {
                        Id = item.Object.Id,
                        Nickname = item.Object.Nickname,
                        Voto = item.Object.Voto
                    });
                }
            }

            return list;
        }

        public async Task<Valoracion> Get(int id, string nickname)
        {
            var result = await firebase.Child(tag).OnceAsync<Valoracion>();
            Valoracion item = new Valoracion();

            foreach (var obj in result)
            {
                if (obj.Object.Id == id && obj.Object.Nickname == nickname)
                {
                    item = new Valoracion()
                    {
                        Id = obj.Object.Id,
                        Nickname = obj.Object.Nickname,
                        Voto = obj.Object.Voto
                    };
                    return item;
                }
            }
            return null;
        }

        public async Task<FirebaseObject<Valoracion>> GetObject(int id, string nickname)
        {
            var result = await firebase.Child(tag).OnceAsync<Valoracion>();
            Valoracion item = new Valoracion();

            foreach (var obj in result)
            {
                if (obj.Object.Id == id && obj.Object.Nickname == nickname)
                {
                    return obj;
                }
            }
            return null;
        }

    }
}
