using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
	{
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {

        }

		public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmuebles (Direccion, Estado, TipoDeUso, TipoDeInmueble, CantDeAmbientes, Precio, IdPropietario) " +
					$"VALUES (@Direccion, @Estado, @TipoDeUso, @TipoDeInmueble, @CantDeAmbientes, @Precio, @IdPropietario);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@Estado", entidad.Estado);
					command.Parameters.AddWithValue("@TipoDeUso", entidad.TipoDeUso);
					command.Parameters.AddWithValue("@TipoDeInmueble", entidad.TipoDeInmueble);
					command.Parameters.AddWithValue("@CantDeAmbientes", entidad.CantDeAmbientes);
					command.Parameters.AddWithValue("@Precio", entidad.Precio);
					command.Parameters.AddWithValue("@IdPropietario", entidad.IdPropietario);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.IdInmueble = res;
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
				string sql = $"DELETE FROM Inmuebles WHERE IdInmueble = {id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmuebles SET " +
					"Direccion=@Direccion, Estado=@Estado, TipoDeUso=@TipoDeUso, TipoDeInmueble=@TipoDeInmueble, CantDeAmbientes=@CantDeAmbientes, Precio=@Precio, IdPropietario=@IdPropietario " +
					"WHERE IdInmueble = @IdInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@Estado", entidad.Estado);
					command.Parameters.AddWithValue("@TipoDeUso", entidad.TipoDeUso);
					command.Parameters.AddWithValue("@TipoDeInmueble", entidad.TipoDeInmueble);
					command.Parameters.AddWithValue("@CantDeAmbientes", entidad.CantDeAmbientes);
					command.Parameters.AddWithValue("@Precio", entidad.Precio);
					command.Parameters.AddWithValue("@IdPropietario", entidad.IdPropietario);
					command.Parameters.AddWithValue("@IdInmueble", entidad.IdInmueble);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				
				string sql = $"SELECT IdInmueble, Direccion, Estado, TipoDeUso, TipoDeInmueble, CantDeAmbientes, Precio, p.IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario" ;

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Estado = reader.GetString(2),
							TipoDeUso = reader.GetString(3),
							TipoDeInmueble = reader.GetString(4),
							CantDeAmbientes = reader.GetInt32(5),
							Precio = reader.GetInt32(6),
							IdPropietario = reader.GetInt32(7),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdInmueble, Direccion, Estado, TipoDeUso, TipoDeInmueble, CantDeAmbientes, Precio, p.IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario" +
					$" WHERE IdInmueble=@IdInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdInmueble", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Estado = reader.GetString(2),
							TipoDeUso = reader.GetString(3),
							TipoDeInmueble = reader.GetString(4),
							CantDeAmbientes = reader.GetInt32(5),
							Precio = reader.GetInt32(6),
							IdPropietario = reader.GetInt32(7),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}


		public IList<Inmueble> ObtenerTodosDonde(int IdPropietario, string Estado)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string where="";
				if(IdPropietario != 0)
                {
					where = "WHERE i.IdPropietario=" + IdPropietario;
					where = !String.IsNullOrEmpty(Estado) ? where + " And i.Estado='" + Estado+"'" : where;
				}
                else if (!String.IsNullOrEmpty(Estado))
                {
					where = "WHERE i.Estado='" + Estado+"'";
					where = IdPropietario != 0 ? where + " And i.IdPropietario=" + IdPropietario : where;
				}


				string sql = "SELECT i.IdInmueble, Direccion, i.Estado, TipoDeUso, TipoDeInmueble, CantDeAmbientes, Precio, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario " + where;

				using (SqlCommand command = new SqlCommand(sql, connection))
				{

					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Estado = reader.GetString(2),
							TipoDeUso = reader.GetString(3),
							TipoDeInmueble = reader.GetString(4),
							CantDeAmbientes = reader.GetInt32(5),
							Precio = reader.GetInt32(6),
							IdPropietario = reader.GetInt32(7),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodosDisponiblesPorFechas(string fechaDeInicio, string fechaDeFinalizacion)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.IdInmueble, Direccion, i.Estado, TipoDeUso, TipoDeInmueble, CantDeAmbientes, Precio, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario INNER JOIN Contratos c ON i.IdInmueble = c.IdInmueble" +
					$" WHERE (c.FechaDeInicio < @fechaDeInicio AND c.FechaDeFinalizacion < @fechaDeInicio)  OR (c.FechaDeInicio > @fechaDeFinalizacion AND c.FechaDeFinalizacion > @fechaDeFinalizacion) AND i.Estado='Disponible'";

				
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@fechaDeInicio", SqlDbType.Date).Value = fechaDeInicio;
					command.Parameters.Add("@fechaDeFinalizacion", SqlDbType.Date).Value = fechaDeFinalizacion;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Estado = reader.GetString(2),
							TipoDeUso = reader.GetString(3),
							TipoDeInmueble = reader.GetString(4),
							CantDeAmbientes = reader.GetInt32(5),
							Precio = reader.GetInt32(6),
							IdPropietario = reader.GetInt32(7),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

	}
}
