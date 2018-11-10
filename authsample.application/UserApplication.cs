using AuthSample.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using RealtimeSample.Firebase;
using System.Threading.Tasks;

namespace AuthSample.Repository
{
    public class UserApplication
    {
        private Usuario User { get; }
        private string NickName { get; set; }
        private string Password { get; set; }

        public bool GetUserId()
        {
            return (this.User != null);
        }

        public async Task<bool> Login(string user, string clave)
        {
            var r = new UsuariosRepository(user, clave, ShowMessage);

           
            bool ok = await r.GetUser(user, clave);
           
            
            return ok;

        }

        public async Task<bool> DuplicatedUser(string user, string clave)
        {
            var r = new UsuariosRepository(user, clave, ShowMessage);


            bool ok = await r.DuplicatedUser(user);


            return ok;

        }

        public void ShowMessage(Usuario user)
        {
            if (user != null)
            {
                if(user.Nickname.Equals(this.NickName) &&
                    user.Password.Equals(this.Password))
                { 
                    this.User.Id = user.Id;
                    this.User.Nickname = user.Nickname;
                    this.User.Password = user.Password;
                }
            }          
        }

        public async void PostUsuario(string usuario, string clave)
        {

            var r = new UsuariosRepository(usuario, clave, ShowMessage);
            await r.PostUsuario(usuario, clave);

        }
    }
}
