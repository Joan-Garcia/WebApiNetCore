using System.Data;
using Dapper;
using Dapper.Transaction;
using MySqlConnector;
using WebApi.DataTransfer;
using WebApi.Helpers.Exceptions;
using WebApi.Models;

namespace WebApi.DataAcesss;

public class UserDao {
    private readonly Database _database;

    public UserDao(Database database) {
        _database = database;
    }

    public async Task<IEnumerable<User>> GetUsers(IDbTransaction? transaction = null) {

        string query = @"
            SELECT
	            bu.id_personal AS IdPersonal,
	            CONCAT_WS(' ', p.nombre, p.apepaterno, p.apematerno) Name,
                bu.password AS Password,
                bu.fecha_alta AS FechaAlta,
                bu.ultimo_acceso AS UltimoAcceso,
                bu.estatus AS Estatus
            FROM innovacion.boxsh_usuarios bu
            INNER JOIN personal.personal p ON p.idpersonal = bu.id_personal;
        ";


        try {
            if (transaction is null) {
                using var conn = _database.GetConnection();
                return await conn.QueryAsync<User>(query);
            } else {
                return await transaction.QueryAsync<User>(query);
            }
            
        } catch (Exception e) {
            throw new MysqlException(query, e);
        }

    }

    public async Task<User> GetUser(int IdPersonal, IDbTransaction? transaction = null) {

        string query = @"
            SELECT
	            bu.id_personal AS IdPersonal,
	            CONCAT_WS(' ', p.nombre, p.apepaterno, p.apematerno) Name,
                bu.password AS Password,
                bu.fecha_alta AS FechaAlta,
                bu.ultimo_acceso AS UltimoAcceso,
                bu.estatus AS Estatus
            FROM innovacion.boxsh_usuarios bu
            INNER JOIN personal.personal p ON p.idpersonal = bu.id_personal
            WHERE bu.id_personal = @IdPersonal;
        ";

        try {
            if (transaction is null) {
                using var conn = _database.GetConnection();
                return await conn.QueryFirstOrDefaultAsync<User>(query, new { IdPersonal });
            } else {
                Console.WriteLine("Transaction");
                return await transaction.QueryFirstOrDefaultAsync<User>(query, new { IdPersonal });
            }

        } catch (Exception e) {
            throw new MysqlException(query, e);
        }
    }

    public async Task<int> CreateUser(UserDto user, IDbTransaction? transaction = null) {

        string query = @"
            INSERT INTO innovacion.boxsh_usuarios (
                id_personal,
                password,
                fecha_alta,
                estatus
            ) VALUES (
                @IdPersonal,
                @Password,
                NOW(),
                @Estatus
            );
        ";

        try {
            if (transaction is null) {
                using var conn = _database.GetConnection();
                return await conn.ExecuteAsync(query, user);
            } else {
                return await transaction.ExecuteAsync(query, user);
            }

        } catch (Exception e) {
            throw new MysqlException(query, e);
        }
    }

    public async Task<int> UpdateUser(UserDto user, IDbTransaction? transaction = null) {

        string query = @"
            UPDATE innovacion.boxsh_usuarios SET
                password = @Password,
                estatus = @Estatus
            WHERE id_personal = @IdPersonal;
        ";

        try {
            if (transaction is null) {
                using var conn = _database.GetConnection();
                return await conn.ExecuteAsync(query, user);
            } else {
                return await transaction.ExecuteAsync(query, user);
            }

        } catch (Exception e) {
            throw new MysqlException(query, e);
        }
    }

    public async Task<int> DeleteUser(int IdPersonal, IDbTransaction? transaction = null) {

        string query = @"
            DELETE FROM innovacion.boxsh_usuarios
            WHERE id_personal = @IdPersonal;
        ";

        try {
            if (transaction is null) {
                using var conn = _database.GetConnection();
                return await conn.ExecuteAsync(query, new { IdPersonal });
            } else {
                return await transaction.ExecuteAsync(query, new { IdPersonal });
            }

        } catch (Exception e) {
            throw new MysqlException(query, e);
        }
    }
}