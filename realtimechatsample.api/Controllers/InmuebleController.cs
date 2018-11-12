using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthSample.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeSample.Firebase;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthSample.Api.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    public class InmuebleController : Controller
    {
        InmuebleApplication _repo;
        public InmuebleController()
        {
            _repo = new InmuebleApplication();
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repo.Listar());
        }

        [HttpPut]
        public IActionResult Put([FromBody]Inmueble item)
        {
                return Ok(_repo.Crear(item));
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]Inmueble item)
        {
            return Ok(_repo.CerrarValoracion(item.Id, 10, 5));
        }

    }
}
