using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace web_api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsuariosController : ApiController
    {
        private readonly Repositories.SQLServer.Usuario repositorioUsuario;
        public UsuariosController()
        {
            this.repositorioUsuario = new Repositories.SQLServer.Usuario
                (Configurations.Database.GetConnectionString());
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await this.repositorioUsuario.Select());
            }            
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
        }


        // GET: api/usuarios/5
        // GET: api/usuarios?id=5
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                Models.Usuario usuario =  await this.repositorioUsuario.Select(id);

                if (usuario is null)
                    return NotFound();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            
        }

        // GET: api/usuarios?nome=zeca
        [HttpGet]
        public async Task<IHttpActionResult> GetByNome(string nome)
        {
            try
            {
                return Ok(await this.repositorioUsuario.SelectByNome(nome));
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }            
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IHttpActionResult> Post(Models.Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                usuario.Senha = Utils.Cryptography.ToMD5(usuario.Senha);

                await this.repositorioUsuario.Insert(usuario);

                return Ok(usuario);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) 
                    return BadRequest("O e-mail informado já existe.");

                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            
        }

        // PUT: api/usuarios/5
        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, Models.Usuario usuario)
        {
            try
            {
                if (!Validations.Requisicao.IdRequisicaoIgualAoIdCorpoRequisicao(id, usuario.Id))
                    return BadRequest("O id da requisição não coincide com o Id do usuário.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await this.repositorioUsuario.Update(usuario))
                    return NotFound();

                return Ok(usuario);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) //TODO - verificar o número da exceção
                    return BadRequest("O e-mail informado já existe.");

                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }
            catch (Exception ex)
            {
                Utils.Logger.WriteException(Configurations.Logger.GetFullPath(), ex);

                return InternalServerError();
            }           
        }

        // DELETE: api/usuarios/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                if (!await this.repositorioUsuario.Delete(id))
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
