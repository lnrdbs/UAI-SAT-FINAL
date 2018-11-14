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
    public class ValoracionController : Controller
    {
        ValoracionApplication _repo;
        public ValoracionController()
        {
            _repo = new ValoracionApplication();
        }


        [HttpGet]
        public IActionResult Get(int id)
        {
            return Ok(_repo.Listar(id));
        }

        [HttpPut]
        public IActionResult Put(int id, int voto)
        {
            Valoracion item = new Valoracion()
            { Id = id,
                Voto = voto,
                Nickname = this.User.Claims.First(i => i.Type == "name").ToString()
            };
                return Ok(_repo.Crear(item));
        }
    }
}
