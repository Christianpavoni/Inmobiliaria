
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{
        }

		public int Alta(Inquilino e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email, LugarDeTrabajo, NombreGarante, DniGarante, TelefonoGarante, EmailGarante) " +
					$"VALUES (@nombre, @apellido, @dni, @telefono, @email, @LugarDeTrabajo, @NombreGarante, @DniGarante, @TelefonoGarante, @EmailGarante);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@dni", e.Dni);
					command.Parameters.AddWithValue("@telefono", e.Telefono);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@LugarDeTrabajo", e.LugarDeTrabajo);
					command.Parameters.AddWithValue("@NombreGarante", e.NombreGarante);
					command.Parameters.AddWithValue("@DniGarante", e.DniGarante);
					command.Parameters.AddWithValue("@TelefonoGarante", e.TelefonoGarante);
					command.Parameters.AddWithValue("@EmailGarante", e.EmailGarante);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					e.IdInquilino = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inquilinos WHERE IdInquilino = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inquilino e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Inquilinos SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, LugarDeTrabajo=@LugarDeTrabajo, NombreGarante=@NombreGarante, DniGarante=@DniGarante, TelefonoGarante=@TelefonoGarante, EmailGarante=@EmailGarante " +
					$"WHERE IdInquilino = @idInquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@dni", e.Dni);
					command.Parameters.AddWithValue("@telefono", e.Telefono);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@LugarDeTrabajo", e.LugarDeTrabajo);
					command.Parameters.AddWithValue("@NombreGarante", e.NombreGarante);
					command.Parameters.AddWithValue("@DniGarante", e.DniGarante);
					command.Parameters.AddWithValue("@TelefonoGarante", e.TelefonoGarante);
					command.Parameters.AddWithValue("@EmailGarante", e.EmailGarante);
					command.Parameters.AddWithValue("@idInquilino", e.IdInquilino);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, LugarDeTrabajo, NombreGarante, DniGarante, TelefonoGarante, EmailGarante" +
					$" FROM Inquilinos";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							IdInquilino = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
							LugarDeTrabajo = reader.GetString(6),
							NombreGarante = reader.GetString(7),
							DniGarante = reader.GetString(8),
							TelefonoGarante = reader.GetString(9),
							EmailGarante = reader.GetString(10),
							
					};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inquilino ObtenerPorId(int id)
		{
			Inquilino p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, LugarDeTrabajo, NombreGarante, DniGarante, TelefonoGarante, EmailGarante FROM Inquilinos" +
					$" WHERE IdInquilino=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
							LugarDeTrabajo = reader.GetString(6),
							NombreGarante = reader.GetString(7),
							DniGarante = reader.GetString(8),
							TelefonoGarante = reader.GetString(9),
							EmailGarante = reader.GetString(10),
						};
						return p;
					}
					connection.Close();
				}
			}
			return p;
		}
	}
}
