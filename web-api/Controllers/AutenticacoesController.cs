using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace web_api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AutenticacoesController : ApiController
    {
        private readonly Repositories.SQLServer.Autenticacao repositorioAutenticacao;

        public AutenticacoesController()
        {
            this.repositorioAutenticacao = new Repositories.SQLServer.Autenticacao(Configurations.Database.GetConnectionString());
        }

        [HttpPost]
        public async Task<IHttpActionResult> Autenticar(Models.Login login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                login.Senha = Utils.Cryptography.ToMD5(login.Senha);

                Models.Usuario usuario = await this.repositorioAutenticacao.Autenticar(login);

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
    }
}
