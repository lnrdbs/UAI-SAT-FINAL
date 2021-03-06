﻿using AuthSample.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using RealtimeSample.Firebase;
using System.Threading.Tasks;

namespace AuthSample.Application
{
    public class ValoracionApplication
    {
        private ValoracionRepository repo;
        private InmuebleRepository repoInmueble;
        public ValoracionApplication()
        {
            this.repo = new ValoracionRepository();
        }

        public async Task<IList<Valoracion>> Listar(int id)
        {
            var result = await repo.Listar(id);

            return result;
        }

        public async Task<Inmueble> Crear(Valoracion item)
        {
            Inmueble obj = new Inmueble();
            repoInmueble = new InmuebleRepository();
            var o = await repoInmueble.Get(item.Id);

            var x = await this.Get(item.Id, item.Nickname);
            if(x==null)
            {   
            // voy actualizando en la lista
                if(o!=null)
                {
                    await repo.Crear(item);

                    if (item.Voto == 1)
                        ++o.VotosPositivos;
                    else
                        ++o.VotosNegativos;
                    obj = await repoInmueble.Modificar(o);
                }
            }
            else { throw new Exception("Voto Duplicado"); }
            return obj;
        }

        public async Task<Valoracion> Get(int id, string nickname)
        {
            var item = await this.repo.Get(id, nickname);
           
            return item;
        }
    }
}
