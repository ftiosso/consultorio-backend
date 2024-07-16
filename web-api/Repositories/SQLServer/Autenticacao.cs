using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace web_api.Repositories.SQLServer
{
    public class Autenticacao
    {
        private readonly SqlConnection conn;
        private readonly SqlCommand cmd;

        public Autenticacao(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
            this.cmd = new SqlCommand
            {
                Connection = this.conn
            };
        }

        public async Task<Models.Usuario> Autenticar(Models.Login login)
        {
            Models.Usuario usuario  = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome from usuario where email = @email and senha = @senha;";
                    this.cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar)).Value = login.Email;
                    this.cmd.Parameters.Add(new SqlParameter("@senha", SqlDbType.VarChar)).Value = login.Senha;

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            usuario = new Models.Usuario();
                            usuario.Id = (int)dr["id"];
                            usuario.Nome = dr["nome"].ToString();
                            usuario.Email = login.Email;
                            usuario.Senha = login.Senha;
                        }
                    }
                }
            }

            return usuario;
        }
    }
}