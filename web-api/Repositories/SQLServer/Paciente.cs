using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace web_api.Repositories.SQLServer
{
    public class Paciente
    {
        private readonly SqlConnection conn;
        private readonly SqlCommand cmd;
        private readonly string cacheKey;
        private readonly int defaultCacheTimeInSeconds;

        public Paciente(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
            this.cmd = new SqlCommand
            {
                Connection = this.conn
            };
            this.cacheKey = "paciente";
            this.defaultCacheTimeInSeconds = Configurations.Cache.GetDefaultCacheTimeInSeconds();
        }

        public async Task<List<Models.Paciente>> Select()
        {
            List<Models.Paciente> pacientes = (List<Models.Paciente>)Utils.Cache.Get(this.cacheKey);

            if (pacientes != null)
                return pacientes;

            pacientes = new List<Models.Paciente>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select codigo, nome, datanascimento from paciente;";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Paciente paciente = new Models.Paciente
                            {
                                Codigo = (int)dr["codigo"],
                                Nome = dr["nome"].ToString(),
                                DataNascimento = Convert.ToDateTime(dr["datanascimento"])
                            };

                            pacientes.Add(paciente);
                        }
                    }
                }
            }

            Utils.Cache.Set(this.cacheKey, pacientes, this.defaultCacheTimeInSeconds);

            return pacientes;
        }

        public async Task<Models.Paciente> Select(int codigo)
        {
            Models.Paciente paciente = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select codigo, nome, datanascimento from paciente where codigo = @codigo;";
                    this.cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.Int)).Value = codigo;

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            paciente = new Models.Paciente
                            {
                                Codigo = codigo,
                                Nome = dr["nome"].ToString(),
                                DataNascimento = Convert.ToDateTime(dr["datanascimento"])
                            };

                        }
                    }
                }
            }

            return paciente;
        }               

        public async Task<List<Models.Paciente>> SelectByNome(string nome)
        {
            List<Models.Paciente> pacientes = new List<Models.Paciente>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select codigo, nome, datanascimento from paciente where nome like @nome;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = $"%{nome}%";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Paciente paciente = new Models.Paciente
                            {
                                Codigo = (int)dr["codigo"],
                                Nome = dr["nome"].ToString(),
                                DataNascimento = Convert.ToDateTime(dr["datanascimento"])
                            };

                            pacientes.Add(paciente);
                        }
                    }
                }
            }

            return pacientes;
        }

        public async Task Insert(Models.Paciente paciente)
        {
            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "insert into paciente(nome, datanascimento) values(@nome, @datanascimento); select convert(int,scope_identity());";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = paciente.Nome;
                    this.cmd.Parameters.Add(new SqlParameter("@datanascimento", SqlDbType.Date)).Value = paciente.DataNascimento;
                    paciente.Codigo = (int)await this.cmd.ExecuteScalarAsync();
                }
            }

            Utils.Cache.Remove(this.cacheKey);
        }

        public async Task<bool> Update(Models.Paciente paciente)
        {
            int linhasAfetadas = 0;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "update paciente set nome = @nome, datanascimento = @datanascimento where codigo = @codigo;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = paciente.Nome;
                    this.cmd.Parameters.Add(new SqlParameter("@datanascimento", SqlDbType.Date)).Value = paciente.DataNascimento;
                    this.cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.Int)).Value = paciente.Codigo;

                    linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                }
            }

            Utils.Cache.Remove(this.cacheKey);

            return linhasAfetadas == 1;
        }

        public async Task<bool> Delete(int codigo)
        {
            int linhasAfetadas = 0;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "delete from paciente where codigo = @codigo;";
                    this.cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.Int)).Value = codigo;

                    linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                }
            }

            Utils.Cache.Remove(this.cacheKey);

            return linhasAfetadas == 1;
        }
    }
}