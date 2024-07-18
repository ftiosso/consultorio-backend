using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace web_api.Repositories.SQLServer
{
    public class Medico
    {
        private readonly SqlConnection conn;
        private readonly SqlCommand cmd;
        private readonly string cacheKey;
        private readonly int defaultCacheTimeInSeconds;

        public Medico(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
            this.cmd = new SqlCommand
            {
                Connection = this.conn
            };
            this.cacheKey = "medico";
            this.defaultCacheTimeInSeconds = Configurations.Cache.GetDefaultCacheTimeInSeconds();
        }

        public async Task<List<Models.Medico>> Select()
        {
            List<Models.Medico> medicos = (List<Models.Medico>)Utils.Cache.Get(this.cacheKey);

            if (medicos != null)
                return medicos;

            medicos = new List<Models.Medico>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, crm, nome from medico;";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Medico medico = new Models.Medico();
                            medico.Id = (int)dr["id"];
                            medico.CRM = dr["crm"].ToString();
                            medico.Nome = (string)dr["nome"];

                            medicos.Add(medico);
                        }
                    }
                }
            }

            Utils.Cache.Set(this.cacheKey, medicos, this.defaultCacheTimeInSeconds);

            return medicos;
        }

        public async Task<Models.Medico> Select(int id)
        {
            Models.Medico medico = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select crm, nome from medico where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

                    using (SqlDataReader drMedico = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await drMedico.ReadAsync())
                        {
                            medico = new Models.Medico();
                            medico.Id = id;
                            medico.CRM = drMedico["crm"].ToString();
                            medico.Nome = drMedico["nome"].ToString();
                        }
                    }

                    if (medico is Models.Medico)
                    {
                        this.cmd.Parameters.Clear();
                        this.cmd.CommandText = "select e.id, e.nome from especialidade e inner join medicoespecialidade me on e.id = me.idespecialidade where me.idmedico = @idmedico;";
                        this.cmd.Parameters.Add(new SqlParameter("@idmedico", SqlDbType.Int)).Value = id;

                        using (SqlDataReader drMedicoEspecialidade = await this.cmd.ExecuteReaderAsync())
                        {
                            while (await drMedicoEspecialidade.ReadAsync())
                            {
                                Models.Especialidade especialidade = new Models.Especialidade();
                                especialidade.Id = (int) drMedicoEspecialidade["id"];
                                especialidade.Nome = drMedicoEspecialidade["nome"].ToString();
                                medico.Especialidades.Add(especialidade);
                            }
                        }
                    }
                }
            }

            return medico;
        }

        public async Task<Models.Medico> SelectByCRM(string CRM)
        {
            Models.Medico medico = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, crm, nome from medico where crm = @crm;";
                    this.cmd.Parameters.Add(new SqlParameter("@crm", SqlDbType.VarChar)).Value = CRM;

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            medico = new Models.Medico();
                            medico.Id = (int)dr["id"];
                            medico.CRM = dr["crm"].ToString();
                            medico.Nome = dr["nome"].ToString();
                        }
                    }
                }
            }

            return medico;
        }

        public async Task<List<Models.Medico>> SelectByNome(string nome)
        {
            List<Models.Medico> medicos = new List<Models.Medico>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, crm, nome from medico where nome like @nome;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = $"%{nome}%";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Medico medico = new Models.Medico();
                            medico.Id = (int)dr["id"];
                            medico.CRM = dr["crm"].ToString();
                            medico.Nome = (string)dr["nome"];

                            medicos.Add(medico);
                        }
                    }
                }
            }

            return medicos;
        }

        public async Task Insert(Models.Medico medico)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (this.conn)
                {                
                    await this.conn.OpenAsync();

                    using (this.cmd)
                    {
                        this.cmd.CommandText = "insert into medico(crm, nome) values(@crm, @nome); select convert(int,scope_identity());";
                        this.cmd.Parameters.Add(new SqlParameter("@crm", SqlDbType.VarChar)).Value = medico.CRM;
                        this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = medico.Nome;
                        medico.Id = (int)await this.cmd.ExecuteScalarAsync();

                        this.cmd.Parameters.Clear();

                        this.cmd.CommandText = "insert into medicoespecialidade(idmedico, idespecialidade) values(@idmedico, @idespecialidade);";
                        this.cmd.Parameters.Add(new SqlParameter("@idmedico", SqlDbType.Int));
                        this.cmd.Parameters.Add(new SqlParameter("@idespecialidade", SqlDbType.Int));
                        
                        this.cmd.Parameters["@idmedico"].Value = medico.Id;

                        foreach (Models.Especialidade especialidade in medico.Especialidades)
                        {
                            this.cmd.Parameters["@idespecialidade"].Value = especialidade.Id;
                            await this.cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                
                scope.Complete();
            }

            Utils.Cache.Remove(this.cacheKey);
        }

        public async Task<bool> Update(Models.Medico medico)
        {
            int linhasAfetadas = 0;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (this.conn)
                {
                    await this.conn.OpenAsync();

                    using (this.cmd)
                    {                        
                        this.cmd.CommandText = "update medico set crm = @crm, nome = @nome where id = @id;";
                        
                        this.cmd.Parameters.Add(new SqlParameter("@crm", SqlDbType.VarChar)).Value = medico.CRM;
                        this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = medico.Nome;
                        this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = medico.Id;
                        
                        linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();

                        if (linhasAfetadas == 1)
                        {
                            this.cmd.Parameters.Clear();

                            this.cmd.CommandText = "delete from medicoespecialidade where idmedico = @idmedico;";
                            this.cmd.Parameters.Add(new SqlParameter("@idmedico", SqlDbType.Int)).Value = medico.Id;
                            await this.cmd.ExecuteNonQueryAsync();

                            this.cmd.Parameters.Clear();

                            this.cmd.CommandText = "insert into medicoespecialidade(idmedico, idespecialidade) values(@idmedico, @idespecialidade);";
                            this.cmd.Parameters.Add(new SqlParameter("@idmedico", SqlDbType.Int));
                            this.cmd.Parameters.Add(new SqlParameter("@idespecialidade", SqlDbType.Int));

                            this.cmd.Parameters["@idmedico"].Value = medico.Id;

                            foreach (Models.Especialidade especialidade in medico.Especialidades)
                            {
                                this.cmd.Parameters["@idespecialidade"].Value = especialidade.Id;
                                await this.cmd.ExecuteNonQueryAsync();
                            }
                        }                        
                    }
                }

                scope.Complete();
            }

            Utils.Cache.Remove(this.cacheKey);

            return linhasAfetadas == 1;
        }

        public async Task<bool> Delete(int id)
        {
            int linhasAfetadas = 0;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (this.conn)
                {
                    await this.conn.OpenAsync();

                    using (this.cmd)
                    {
                        this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                        this.cmd.Parameters["@id"].Value = id;

                        this.cmd.CommandText = "delete from medicoespecialidade where idmedico = @id;";
                        await this.cmd.ExecuteNonQueryAsync();

                        this.cmd.CommandText = "delete from medico where id = @id;";
                        linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                    }
                }

                scope.Complete();
            }

            Utils.Cache.Remove(this.cacheKey);

            return linhasAfetadas == 1;
        }
    }
}