using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedDTOs;


namespace WebBankDataAccessLayer
{
    public class ClientData
    {
        static string _connectionString = "Server=localhost;Database=WebBankSystem;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        public static bool IsAccountNumberExist(string accountNumber)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsAccountNumberExist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            isFound = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

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
                               reader.IsDBNull(reader.GetOrdinal("Email")) ? "Unknown" : reader.GetString(reader.GetOrdinal("Email")),
                               reader.GetDecimal(reader.GetOrdinal("Balance")),      
                               reader.GetString(reader.GetOrdinal("AccountNumber")),
                               reader.GetString(reader.GetOrdinal("PinCode"))
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

        public static int AddNewClient(ClientDTO newClientDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_AddNewClient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FirstName", newClientDTO.FirstName);
                command.Parameters.AddWithValue("@LastName", newClientDTO.LastName);
                command.Parameters.AddWithValue("@Email", newClientDTO.Email);
                command.Parameters.AddWithValue("@PinCode", newClientDTO.PinCode);
                command.Parameters.AddWithValue("@Balance", newClientDTO.Balance);
                command.Parameters.AddWithValue("@AccountNumber", newClientDTO.AccountNumber);

                var outputIdParam = new SqlParameter("@NewClientID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DAL Add Error: " + ex.Message);

                    return -1;
                }
            }
        }

        public static ClientDTO GetClientByID(int clientID)
        {
            ClientDTO clientDTO = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetClientByID", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClientID", clientID);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientDTO = new ClientDTO(                      
                        reader.GetInt32(reader.GetOrdinal("ClientID")),
                        reader.GetString(reader.GetOrdinal("FirstName")),
                        reader.GetString(reader.GetOrdinal("LastName")),
                        reader.IsDBNull(reader.GetOrdinal("Email")) ? "Unknown" : reader.GetString(reader.GetOrdinal("Email")),
                        reader.GetDecimal(reader.GetOrdinal("Balance")),     
                        reader.GetString(reader.GetOrdinal("AccountNumber")), 
                        reader.GetString(reader.GetOrdinal("PinCode"))
                    );
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DAL GetByID Error: " + ex.Message);
                }
            }

            return clientDTO;
        }

        public static ClientDTO GetClientByAccountNumber(string accountNumber)
        {
            ClientDTO clientDTO = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetClientByAccountNumber", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientDTO = new ClientDTO(
                                 reader.GetInt32(reader.GetOrdinal("ClientID")),
                                 reader.GetString(reader.GetOrdinal("FirstName")),
                                 reader.GetString(reader.GetOrdinal("LastName")),
                                 reader.IsDBNull(reader.GetOrdinal("Email")) ? "Unknown" : reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetDecimal(reader.GetOrdinal("Balance")),     
                                 reader.GetString(reader.GetOrdinal("AccountNumber")),
                                 reader.GetString(reader.GetOrdinal("PinCode"))
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DAL GetByAccNum Error: " + ex.Message);
                }
            }
            return clientDTO;
        }

        public static bool UpdateClient(ClientUpdateDTO clientDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_UpdateClient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClientID", clientDTO.ClientID);
                command.Parameters.AddWithValue("@FirstName", clientDTO.FirstName);
                command.Parameters.AddWithValue("@LastName", clientDTO.LastName);
                command.Parameters.AddWithValue("@Email", clientDTO.Email);
                command.Parameters.AddWithValue("@Balance", clientDTO.Balance);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true; 
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DAL UpdateClient Error: " + ex.Message);
                    return false; 
                }
            }
        }
    }
}
