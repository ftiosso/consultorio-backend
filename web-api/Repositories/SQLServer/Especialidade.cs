using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace web_api.Repositories.SQLServer
{
    public class Especialidade
    {
        private readonly SqlConnection conn;
        private readonly SqlCommand cmd;
        
        public Especialidade(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
            this.cmd = new SqlCommand
            {
                Connection = this.conn
            };
        }

        public async Task<List<Models.Especialidade>> Select()
        {
            List<Models.Especialidade> especialidades = new List<Models.Especialidade>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome from especialidade;";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Especialidade especialidade = new Models.Especialidade
                            {
                                Id = (int)dr["id"],
                                Nome = dr["nome"].ToString()
                            };

                            especialidades.Add(especialidade);
                        }
                    }
                }
            }

            return especialidades;
        }

        public async Task<Models.Especialidade> Select(int id)
        {
            Models.Especialidade especialidade = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome from especialidade where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            especialidade = new Models.Especialidade
                            {
                                Id = id,
                                Nome = dr["nome"].ToString()
                            };

                        }
                    }
                }
            }

            return especialidade;
        }

        public async Task<List<Models.Especialidade>> SelectByNome(string nome)
        {
            List<Models.Especialidade> especialidades = new List<Models.Especialidade>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome from especialidade where nome like @nome;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = $"%{nome}%";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Especialidade especialidade = new Models.Especialidade
                            {
                                Id = (int)dr["id"],
                                Nome = dr["nome"].ToString()
                            };

                            especialidades.Add(especialidade);
                        }
                    }
                }
            }

            return especialidades;
        }

        public async Task Insert(Models.Especialidade especialidade)
        {
            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "insert into especialidade(nome) values (@nome); select convert(int,scope_identity());";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = especialidade.Nome;                    
                    especialidade.Id = (int)await this.cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task<bool> Update(Models.Especialidade especialidade)
        {
            int linhasAfetadas = 0;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "update especialidade set nome = @nome where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = especialidade.Nome;
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = especialidade.Id;

                    linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                }
            }

            return linhasAfetadas == 1;
        }

        public async Task<bool> Delete(int id)
        {
            int linhasAfetadas = 0;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "delete from especialidade where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

                    linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                }
            }

            return linhasAfetadas == 1;
        }
    }
}