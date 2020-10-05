using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
	{
		public RepositorioContrato (IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Contrato entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Contratos (Detalle, Monto, FechaDeInicio, FechaDeFinalizacion, IdInquilino, IdInmueble) " +
					$"VALUES (@Detalle, @Monto, @FechaDeInicio, @FechaDeFinalizacion, @IdInquilino, @IdInmueble);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Detalle", entidad.Detalle);
					
					command.Parameters.AddWithValue("@Monto", entidad.Monto);
					command.Parameters.AddWithValue("@FechaDeInicio", entidad.FechaDeInicio);
					command.Parameters.AddWithValue("@FechaDeFinalizacion", entidad.FechaDeFinalizacion);
					command.Parameters.AddWithValue("@IdInquilino", entidad.IdInquilino);
					command.Parameters.AddWithValue("@IdInmueble", entidad.IdInmueble);
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
				string sql = $"DELETE FROM Contratos WHERE IdContrato = {id}";
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
		public int Modificacion(Contrato entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Contratos SET " +
					"Detalle=@Detalle, Monto=@Monto, FechaDeInicio=@FechaDeInicio, FechaDeFinalizacion=@FechaDeFinalizacion, IdInquilino=@IdInquilino, IdInmueble=@IdInmueble " +
					"WHERE IdContrato = @IdContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Detalle", entidad.Detalle);
					command.Parameters.AddWithValue("@Monto", entidad.Monto);
					command.Parameters.AddWithValue("@FechaDeInicio", entidad.FechaDeInicio);
					command.Parameters.AddWithValue("@FechaDeFinalizacion", entidad.FechaDeFinalizacion);
					command.Parameters.AddWithValue("@IdInquilino", entidad.IdInquilino);
					command.Parameters.AddWithValue("@IdInmueble", entidad.IdInmueble);
					command.Parameters.AddWithValue("@IdContrato", entidad.IdContrato);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerTodos()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT IdContrato, Detalle, Monto, FechaDeInicio, FechaDeFinalizacion, c.IdInquilino, c.IdInmueble," +
					" i.Nombre, i.Apellido, n.Direccion" +
					" FROM Contratos c INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato entidad = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							Detalle = reader.GetString(1),
							Monto = reader.GetInt32(2),
							FechaDeInicio = reader.GetDateTime(3),
							FechaDeFinalizacion = reader.GetDateTime(4),
							IdInquilino = reader.GetInt32(5),
							IdInmueble = reader.GetInt32(6),
							Inquilino = new Inquilino
							{
								IdInquilino = reader.GetInt32(5),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							Inmueble = new Inmueble
							{
								IdInmueble = reader.GetInt32(6),
								Direccion = reader.GetString(9),
							}

						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Contrato ObtenerPorId(int id)
		{
			Contrato entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdContrato, Detalle, Monto, FechaDeInicio, FechaDeFinalizacion, c.IdInquilino, c.IdInmueble, i.Nombre, i.Apellido, n.Direccion" +
					$" FROM Contratos c INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble" +
					$" WHERE IdContrato=@IdContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdContrato", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							Detalle = reader.GetString(1),
							Monto = reader.GetInt32(2),
							FechaDeInicio = reader.GetDateTime(3),
							FechaDeFinalizacion = reader.GetDateTime(4),
							IdInquilino = reader.GetInt32(5),
							IdInmueble = reader.GetInt32(6),
							Inquilino = new Inquilino
							{
								IdInquilino = reader.GetInt32(5),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							Inmueble = new Inmueble
							{
								IdInmueble = reader.GetInt32(6),
								Direccion = reader.GetString(9),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}



		public IList<Contrato> ObtenerTodosDonde(int IdInmueble, string fechaDeInicio, string fechaDeFinalizacion)
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string where = "";


				if (IdInmueble != 0)
				{
					where = "WHERE n.IdInmueble=" + IdInmueble;
					where = (fechaDeInicio != "0001-01-01") && (fechaDeFinalizacion != "0001-01-01") ? where + " AND FechaDeInicio<= '" + fechaDeInicio + "' AND FechaDeFinalizacion>= '" + fechaDeFinalizacion + "'" : where;
				}
                else if((fechaDeInicio != "0001-01-01") && (fechaDeFinalizacion != "0001-01-01"))
                {
					where = "WHERE FechaDeInicio <= '" + fechaDeInicio + "' AND FechaDeFinalizacion>= '" + fechaDeFinalizacion + "'";
					
				}

				string sql = "SELECT IdContrato, Detalle,  Monto, FechaDeInicio, FechaDeFinalizacion, c.IdInquilino, c.IdInmueble, i.Nombre, i.Apellido, n.Direccion" +
					$" FROM Contratos c INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble "+where;
				using (SqlCommand command = new SqlCommand(sql, connection))
				{

					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato entidad = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							Detalle = reader.GetString(1),
							
							Monto = reader.GetInt32(2),
							FechaDeInicio = reader.GetDateTime(3),
							FechaDeFinalizacion = reader.GetDateTime(4),
							IdInquilino = reader.GetInt32(5),
							IdInmueble = reader.GetInt32(6),
							Inquilino = new Inquilino
							{
								IdInquilino = reader.GetInt32(5),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							Inmueble = new Inmueble
							{
								IdInmueble = reader.GetInt32(6),
								Direccion = reader.GetString(9),
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
