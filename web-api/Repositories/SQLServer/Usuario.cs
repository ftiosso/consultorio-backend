using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace web_api.Repositories.SQLServer
{
    public class Usuario
    {
        private readonly SqlConnection conn;
        private readonly SqlCommand cmd;
        
        public Usuario(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
            this.cmd = new SqlCommand
            {
                Connection = this.conn
            };            
        }

        public async Task<List<Models.Usuario>> Select()
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome, email, senha from usuario;";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Usuario usuario = new Models.Usuario();
                            usuario.Id = (int)dr["id"];                            
                            usuario.Nome = (string)dr["nome"];
                            usuario.Email = dr["email"].ToString();
                            usuario.Senha = dr["senha"].ToString();

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return usuarios;
        }

        public async Task<Models.Usuario> Select(int id)
        {
            Models.Usuario usuario = null;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select nome, email, senha from usuario where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            usuario = new Models.Usuario();
                            usuario.Id = id;                            
                            usuario.Nome = dr["nome"].ToString();
                            usuario.Email = dr["email"].ToString();
                            usuario.Senha = dr["senha"].ToString();
                        }
                    }
                }
            }

            return usuario;
        }

        public async Task<List<Models.Usuario>> SelectByNome(string nome)
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "select id, nome, email, senha from usuario where nome like @nome;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = $"%{nome}%";

                    using (SqlDataReader dr = await this.cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            Models.Usuario usuario = new Models.Usuario();
                            usuario.Id = (int) dr["id"]; ;
                            usuario.Nome = dr["nome"].ToString();
                            usuario.Email = dr["email"].ToString();
                            usuario.Senha = dr["senha"].ToString();

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return usuarios;
        }

        public async Task Insert(Models.Usuario usuario)
        {
            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "insert into usuario(nome,email,senha) values(@nome,@email,@senha); select convert(int,scope_identity());";                    
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = usuario.Nome;
                    this.cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar)).Value = usuario.Email;
                    this.cmd.Parameters.Add(new SqlParameter("@senha", SqlDbType.VarChar)).Value = usuario.Senha;
                    usuario.Id = (int)await this.cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task<bool> Update(Models.Usuario usuario)
        {
            int linhasAfetadas = 0;

            using (this.conn)
            {
                await this.conn.OpenAsync();

                using (this.cmd)
                {
                    this.cmd.CommandText = "update usuario set nome = @nome, email = @email, senha = @senha where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar)).Value = usuario.Nome;
                    this.cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar)).Value = usuario.Email;
                    this.cmd.Parameters.Add(new SqlParameter("@senha", SqlDbType.VarChar)).Value = usuario.Senha;
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = usuario.Id;

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
                    this.cmd.CommandText = "delete from usuario where id = @id;";
                    this.cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

                    linhasAfetadas = await this.cmd.ExecuteNonQueryAsync();
                }
            }

            return linhasAfetadas == 1;
        }
    }
}