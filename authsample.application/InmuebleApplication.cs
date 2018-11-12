using AuthSample.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using RealtimeSample.Firebase;
using System.Threading.Tasks;

namespace AuthSample.Application
{
    public class InmuebleApplication
    {
        private InmuebleRepository repo;

        public InmuebleApplication()
        {
            this.repo = new InmuebleRepository();
        }

        public async Task<IList<Inmueble>> Listar()
        {
            var result = await repo.Listar();

            return result;
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

        public async Task<Inmueble> CerrarValoracion(int id, int positivos, int negativos)
        {
            var item = await this.repo.GetObject(id);

            if (positivos == negativos)
            {

                item.Object.Valoracion = "Valoración[NEUTRO] ";
            }
            if (positivos > negativos)
            {

                item.Object.Valoracion = "Valoración[POSIIVA] ";
            }
            if (positivos < negativos)
            {

                item.Object.Valoracion = "Valoración[NEGATIVA] ";
            }

            item.Object.Valoracion += "Votos Positivos: " + positivos.ToString();
            item.Object.Valoracion += " || Negativos: " + negativos.ToString();
            item.Object.Abierto = "0";
            repo.Modificar(item.Object);
            return item.Object;
        }
    }
}
