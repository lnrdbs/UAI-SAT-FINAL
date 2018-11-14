using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeSample.Firebase
{
    public class Inmueble
    {
        public Inmueble()
        {
        }

        public string Barrio { get; set; }
        public string Descripcion { get; set; }
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Abierto { get; set; }
        public string Imagen { get; set; }
        public string Valoracion { get; set; }
        public int VotosPositivos { get; set; }
        public int VotosNegativos { get; set; }
    }
}
