using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedDTOs;


namespace WebBankDataAccessLayer
{
    public class ClientData
    {
        static string _connectionString = "Server=localhost;Database=WebBankSystem;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        public static List<ClientDTO> GetAllClients()
        {
            var clientsList = new List<ClientDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("sp_GetAllClients", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientsList.Add(new ClientDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ClientID")),
                                reader.GetString(reader.GetOrdinal("FirstName")),
                                reader.GetString(reader.GetOrdinal("LastName")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("PinCode")),
                                reader.GetDecimal(reader.GetOrdinal("Balance")),
                                reader.GetString(reader.GetOrdinal("AccountNumber"))
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DAL Error: " + ex.Message);
                }
            }

            return clientsList;
        }
    }
}
