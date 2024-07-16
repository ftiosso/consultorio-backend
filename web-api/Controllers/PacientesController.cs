using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace web_api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PacientesController : ApiController
    {
        private readonly Repositories.SQLServer.Paciente repositorioPaciente;

        public PacientesController()
        {
            this.repositorioPaciente = new Repositories.SQLServer.Paciente(Configurations.Database.GetConnectionString());
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await this.repositorioPaciente.Select());
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // GET: api/Pacientes/5
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                Models.Paciente Paciente = await this.repositorioPaciente.Select(id);

                if (Paciente is null)
                    return NotFound();

                return Ok(Paciente);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // GET: api/Pacientes?nome=dipirona
        [HttpGet]
        public async Task<IHttpActionResult> GetByNome(string nome)
        {
            try
            {
                return Ok(await this.repositorioPaciente.SelectByNome(nome));
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // POST: api/Pacientes
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Models.Paciente paciente)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await this.repositorioPaciente.Insert(paciente);

                return Content(HttpStatusCode.Created, paciente); //201

                //return Ok(paciente);
                //return Created($"api/pacientes/{paciente.Codigo}", paciente);     
                

            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // PUT: api/Pacientes/5
        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, [FromBody] Models.Paciente paciente)
        {
            try
            {
                if (!Validations.Requisicao.IdRequisicaoIgualAoIdCorpoRequisicao(id, paciente.Codigo))
                    return BadRequest("O código da requisição não coincide com o código do paciente.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await this.repositorioPaciente.Update(paciente))
                    return NotFound();

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // DELETE: api/Pacientes/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                if (!await this.repositorioPaciente.Delete(id))
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }
    }
}



                  
