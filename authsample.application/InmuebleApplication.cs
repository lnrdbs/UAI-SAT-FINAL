﻿using AuthSample.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using RealtimeSample.Firebase;
using System.Threading.Tasks;
using System.Linq;

namespace AuthSample.Application
{
    public class InmuebleApplication
    {
        private InmuebleRepository repo;

        public InmuebleApplication()
        {
            this.repo = new InmuebleRepository();
        }

        public IList<Inmueble> Listar()
        {
            /*var result = await repo.Listar();
            return await Task.Run(()=> new List<Inmueble>(result));*/
            IList<Inmueble> list;
            try
            {
                list = repo.Listar().Result;  
            }
            catch (Exception)
            {
                throw;
            }
            return list.OrderByDescending(x=>x.Id).ToList();
        }

        public async Task Crear(Inmueble inmueble)
        {
            await repo.Crear(inmueble);
        }

        public async Task<Inmueble> Get(int id)
        {
            var item = await this.repo.Get(id);
           
            return item;
        }

        public Inmueble GetById(int id)
        {
            var item = this.repo.Get(id);

            return item.Result;
        }

        public async Task<bool> Modificar(Inmueble inmueble)
        {
            try
            {
                
                var t = await this.Modificar(inmueble);
                return t;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Inmueble> CerrarValoracion(int id)
        {
            var item = await this.repo.GetObject(id);

            if (item.Object.VotosPositivos == item.Object.VotosNegativos)
            {

                item.Object.Valoracion = "Valoración[NEUTRO] ";
            }
            if (item.Object.VotosPositivos > item.Object.VotosNegativos)
            {

                item.Object.Valoracion = "Valoración[POSITIVA] ";
            }
            if (item.Object.VotosPositivos < item.Object.VotosNegativos)
            {

                item.Object.Valoracion = "Valoración[NEGATIVA] ";
            }

            item.Object.Valoracion += "Votos Positivos: " + item.Object.VotosPositivos;
            item.Object.Valoracion += " || Negativos: " + item.Object.VotosNegativos;
            item.Object.Abierto = "0";
            var t = await repo.Modificar(item.Object);
            return item.Object;
        }
    }
}
