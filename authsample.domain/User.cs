using System;

namespace AuthSample.Domain
{
    public class User : PersistentEntity
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
    }
}
