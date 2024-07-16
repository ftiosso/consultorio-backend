using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace web_api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MedicosController : ApiController
    {
        private readonly Repositories.SQLServer.Medico repositorioMedico;
        public MedicosController()
        {
            this.repositorioMedico = new Repositories.SQLServer.Medico
                (Configurations.Database.GetConnectionString());
        }

        // GET: api/Medicos
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await this.repositorioMedico.Select());
            }            
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }


        // GET: api/Medicos/5
        // GET: api/Medicos?id=5
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                Models.Medico medico =  await this.repositorioMedico.Select(id);

                if (medico is null)
                    return NotFound();

                return Ok(medico);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            
        }

        // GET: api/Medicos?crm=123
        [HttpGet]
        public async Task<IHttpActionResult> GetByCRM(string CRM)
        {
            try
            { 
                Models.Medico medico = await this.repositorioMedico.SelectByCRM(CRM);

                if (medico is null)
                    return NotFound();

                return Ok(medico);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            
        }

        // GET: api/Medicos?nome=zeca
        [HttpGet]
        public async Task<IHttpActionResult> GetByNome(string nome)
        {
            try
            {
                return Ok(await this.repositorioMedico.SelectByNome(nome));
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }            
        }

        // POST: api/Medicos
        [HttpPost]
        public async Task<IHttpActionResult> Post(Models.Medico medico)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await this.repositorioMedico.Insert(medico);

                return Ok(medico);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    return BadRequest("O CRM informado já existe.");

                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            
        }

        // PUT: api/Medicos/5
        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, Models.Medico medico)
        {
            try
            {
                if (!Validations.Requisicao.IdRequisicaoIgualAoIdCorpoRequisicao(id, medico.Id))
                    return BadRequest("O id da requisição não coincide com o Id do médico.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await this.repositorioMedico.Update(medico))
                    return NotFound();

                return Ok(medico);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    return BadRequest("O CRM informado já existe.");

                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }           
        }

        // DELETE: api/Medicos/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                if (!await this.repositorioMedico.Delete(id))
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
