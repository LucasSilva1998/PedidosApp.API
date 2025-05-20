using Dapper;
using Microsoft.Data.SqlClient;
using PedidosApp.API.Entities;
namespace PedidosApp.API.Repositories
{
    /// <summary>
    /// Classe de repositório para operações no banco de dados
    /// </summary>
    public class PedidoRepository
    {
        private readonly string _connectionString;
        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task Inserir(Pedido pedido)
        {
            var query = @"
                         INSERT INTO PEDIDO(
                         ID, 
                         DATAHORA, 
                         VALOR, 
                         CLIENTE, 
                         DETALHES)
                         VALUES(
                         @Id, 
                         @DataHora, 
                         @Valor, 
                         @Cliente, 
                         @Detalhes)
                         ";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, pedido);
            }
        }
    }
}