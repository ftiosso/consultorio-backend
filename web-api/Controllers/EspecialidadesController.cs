using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace web_api.Controllers
{
    public class EspecialidadesController : ApiController
    {
        private readonly Repositories.SQLServer.Especialidade repositorioEspecialidade;

        public EspecialidadesController()
        {
            this.repositorioEspecialidade = new Repositories.SQLServer.Especialidade(Configurations.Database.GetConnectionString());
        }

        // GET: api/Especialidades
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await this.repositorioEspecialidade.Select());
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // GET: api/Especialidades/5
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                Models.Especialidade especialidade = await this.repositorioEspecialidade.Select(id);

                if (especialidade is null)
                    return NotFound();

                return Ok(especialidade);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // GET: api/Especialidades?nome=dipirona
        [HttpGet]
        public async Task<IHttpActionResult> GetByNome(string nome)
        {
            try
            {
                return Ok(await this.repositorioEspecialidade.SelectByNome(nome));
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // POST: api/Especialidades
        [HttpPost]
        public async Task<IHttpActionResult> Post(Models.Especialidade especialidade)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await this.repositorioEspecialidade.Insert(especialidade);

                return Content(HttpStatusCode.Created, especialidade);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // PUT: api/Especialidades/5
        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, Models.Especialidade especialidade)
        {
            try
            {
                if (!Validations.Requisicao.IdRequisicaoIgualAoIdCorpoRequisicao(id, especialidade.Id))
                    return BadRequest("O id da requisição não coincide com o id da especialidade.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await this.repositorioEspecialidade.Update(especialidade))
                    return NotFound();

                return Ok(especialidade);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }

        // DELETE: api/Especialidades/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                if (!await this.repositorioEspecialidade.Delete(id))
                    return NotFound();

                return Ok();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    return BadRequest("A especialidade está relacionada com algum médico e não pode ser excluída.");

                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }
    }
}
