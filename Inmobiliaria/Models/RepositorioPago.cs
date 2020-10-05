using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
	{
		public RepositorioPago (IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Pago entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Pagos (FechaDePago, Importe, IdContrato) " +
					$"VALUES (@FechaDePago, @Importe, @IdContrato);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@FechaDePago", entidad.FechaDePago);
					command.Parameters.AddWithValue("@Importe", entidad.Importe);
					command.Parameters.AddWithValue("@IdContrato", entidad.IdContrato);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.IdPago = res;
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
				string sql = $"DELETE FROM Pagos WHERE IdPago = {id}";
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
		public int Modificacion(Pago entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Pagos SET " +
					"FechaDePago=@FechaDePago, Importe=@Importe, IdContrato=@IdContrato " +
					"WHERE IdPago = @IdPago";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@FechaDePago", entidad.FechaDePago);
					command.Parameters.AddWithValue("@Importe", entidad.Importe);
					command.Parameters.AddWithValue("@IdContrato", entidad.IdContrato);
					command.Parameters.AddWithValue("@IdPago", entidad.IdPago);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Pago> ObtenerTodos()
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT IdPago, Importe, p.FechaDePago, p.IdContrato," +
					" c.IdInquilino, i.Nombre, i.Apellido, n.IdInmueble, n.Direccion" +
					" FROM Pagos p INNER JOIN Contratos c ON p.IdContrato = c.IdContrato INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino " +
					"INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago entidad = new Pago
						{
							IdPago = reader.GetInt32(0),
							Importe = reader.GetInt32(1),
							FechaDePago = reader.GetDateTime(2),
							IdContrato = reader.GetInt32(3),

							Contrato = new Contrato
							{
								IdContrato = reader.GetInt32(3),
								
								IdInquilino = reader.GetInt32(4),
								Inquilino = new Inquilino
								{
									IdInquilino = reader.GetInt32(4),
									Nombre = reader.GetString(5),
									Apellido = reader.GetString(6),
								},
								Inmueble = new Inmueble
								{
									IdInmueble = reader.GetInt32(7),
									Direccion = reader.GetString(8),
								}

							},


						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPorId(int id)
		{
			Pago entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdPago, Importe, FechaDePago, p.IdContrato, c.IdInquilino, i.Nombre, i.Apellido, n.IdInmueble, n.Direccion" +
					$" FROM Pagos p INNER JOIN Contratos c ON p.IdContrato = c.IdContrato INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino " +
					"INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble" +
					$" WHERE IdPago=@IdPago";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdPago", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Pago
						{
							IdPago = reader.GetInt32(0),
							Importe = reader.GetInt32(1),
							FechaDePago = reader.GetDateTime(2),
							IdContrato = reader.GetInt32(3),

							Contrato = new Contrato
							{
								IdContrato = reader.GetInt32(3),
								IdInquilino = reader.GetInt32(4),
								Inquilino = new Inquilino
								{
									IdInquilino = reader.GetInt32(4),
									Nombre = reader.GetString(5),
									Apellido = reader.GetString(6),
								},
								Inmueble = new Inmueble
								{
									IdInmueble = reader.GetInt32(7),
									Direccion = reader.GetString(8),
								}

							}
						};
				}
					connection.Close();
				}
			}
			return entidad;
		}


		public IList<Pago> ObtenerTodosDonde(string where)
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT IdPago, Importe, p.FechaDePago, p.IdContrato," +
					" c.IdInquilino, i.Nombre, i.Apellido, n.IdInmueble, n.Direccion" +
					" FROM Pagos p INNER JOIN Contratos c ON p.IdContrato = c.IdContrato INNER JOIN Inquilinos i ON i.IdInquilino = c.IdInquilino " +
					"INNER JOIN Inmuebles n ON n.IdInmueble = c.IdInmueble" +
					$" WHERE " + where; ;
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago entidad = new Pago
						{
							IdPago = reader.GetInt32(0),
							Importe = reader.GetInt32(1),
							FechaDePago = reader.GetDateTime(2),
							IdContrato = reader.GetInt32(3),

							Contrato = new Contrato
							{
								IdContrato = reader.GetInt32(3),
								IdInquilino = reader.GetInt32(4),
								Inquilino = new Inquilino
								{
									IdInquilino = reader.GetInt32(4),
									Nombre = reader.GetString(5),
									Apellido = reader.GetString(6),
								},
								Inmueble = new Inmueble
								{
									IdInmueble = reader.GetInt32(7),
									Direccion = reader.GetString(8),
								}

							},


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
