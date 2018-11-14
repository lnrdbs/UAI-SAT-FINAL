using AuthSample.Domain;
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

        public async Task Crear(Valoracion item)
        {
            await repo.Crear(item);
            // voy actualizando en la lista
            repoInmueble = new InmuebleRepository();
            var o = await repoInmueble.Get(item.Id);
            if (item.Voto == 1)
                ++o.VotosPositivos;
            else
                ++o.VotosNegativos;

            var t = await repoInmueble.Modificar(o);
        }

        public async Task<Valoracion> Get(int id, string nickname)
        {
            var item = await this.repo.Get(id, nickname);
           
            return item;
        }
    }
}
