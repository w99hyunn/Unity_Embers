using MySql.Data.MySqlClient;

using System;

using System.Text;
using UnityEngine;

namespace STARTING
{
    public class LoginResponse
    {
        public LoginResult Result { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
    }

    public enum SignUpResult
    {
        SUCCESS,
        DUPLICATE,
        ERROR
    }

    public enum LoginResult
    {
        SUCCESS,
        PWWRONG,
        IDWRONG,
        ERROR
    }

    public class DBManager : MonoBehaviour
    {
        [Header("DB Server Info")]
        public string DBSERVER_IP = "35.184.161.253";
        public string DBHOST = "orbit-readonly";
        public string DBPW = "orbitreadonlyohmygodpassword!!@";

        private MySqlConnection connection;

        //[Header("클라이언트가 각자 가지고 있는 자신의 정보")]
        ////public GameData clientGameData;
        //public string userName;
        //public int userId;

        public bool ConnectDB()
        {
            bool success = ConnectToDatabase(DBSERVER_IP, "embers", DBHOST, DBPW, "3306");
            return success;
        }

        public bool ConnectToDatabase(string server, string database, string uid, string password, string port)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};PORT={port};Allow Zero Datetime=True;Convert Zero Datetime=True;";

            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public SignUpResult SignUp(string username, string password, string email)
        {
            // username 중복 확인
            if (IsUsernameDuplicate(username))
            {
                return SignUpResult.DUPLICATE;
            }

            var (passwordHash, salt) = GeneratePasswordHashAndSalt(password);
            string query = "INSERT INTO account (username, password_hash, password_salt, email) VALUES (@username, @passwordHash, @salt, @email)";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
            cmd.Parameters.AddWithValue("@salt", salt);
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                cmd.ExecuteNonQuery();
                Managers.Log.Log($"Sign Up New User : {username}");
                return SignUpResult.SUCCESS;
            }
            catch (Exception ex)
            {
                Managers.Log.LogError($"Sign Up New User Failed : {username} / Exception : {ex}");
                return SignUpResult.ERROR;
            }
        }

        /// <summary>
        /// 회원가입 아이디 중복 확인
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool IsUsernameDuplicate(string username)
        {
            string query = "SELECT COUNT(*) FROM account WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);

            try
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Managers.Log.LogError("Duplicate Error: " + ex.Message);
                return false;
            }
        }


        // 로그인 처리
        public LoginResponse Login(string username, string password)
        {
            string query = "SELECT password_hash, password_salt, email, created_at FROM account WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = reader.GetString("password_hash");
                        string storedSalt = reader.GetString("password_salt");

                        string inputPasswordHash = GetHash(password, storedSalt);

                        if (inputPasswordHash == storedPasswordHash)
                        {
                            string email = reader.GetString("email");
                            DateTime createdAt = reader.GetDateTime("created_at");
                            string createdAtString = createdAt.ToString("yyyy-MM-dd HH:mm:ss");

                            reader.Close();

                            string updateQuery = "UPDATE account SET is_online = TRUE WHERE username = @username";
                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                            updateCmd.Parameters.AddWithValue("@username", username);
                            updateCmd.ExecuteNonQuery();

                            Managers.Log.Log($"User Login : {username}");
                            return new LoginResponse
                            {
                                Result = LoginResult.SUCCESS,
                                Email = email,
                                CreatedAt = createdAtString
                            };
                        }
                        else
                        {
                            return new LoginResponse
                            {
                                Result = LoginResult.PWWRONG
                            };
                        }
                    }
                    else
                    {
                        return new LoginResponse
                        {
                            Result = LoginResult.IDWRONG
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Managers.Log.Log($"User Login Error: {username}, Exception : {ex}");
                return new LoginResponse
                {
                    Result = LoginResult.ERROR
                };
            }
        }

        public bool UpdateUserInfo(string username, string newPassword, string newEmail)
        {
            string passwordHash = null;
            string salt = null;

            if (!string.IsNullOrEmpty(newPassword))
            {
                var (newPasswordHash, newSalt) = GeneratePasswordHashAndSalt(newPassword);
                passwordHash = newPasswordHash;
                salt = newSalt;
            }

            string query = "UPDATE account SET email = @email" +
                           (passwordHash != null ? ", password_hash = @passwordHash, password_salt = @salt" : "") +
                           " WHERE username = @username";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@email", newEmail);

            // 비밀번호가 있으면 추가
            if (passwordHash != null)
            {
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@salt", salt);
            }

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Managers.Log.Log($"User Info Updated : {username}");
                    return true;
                }
                else
                {
                    Managers.Log.LogError($"User not found or no changes made : {username}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Managers.Log.LogError($"Error Updating User Info : {username} / Exception : {ex}");
                return false;
            }
        }



        private (string, string) GeneratePasswordHashAndSalt(string password)
        {
            byte[] salt = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            string saltString = Convert.ToBase64String(salt);
            string passwordHash = GetHash(password, saltString);

            return (passwordHash, saltString);
        }

        private string GetHash(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            var pbkdf2 = new System.Security.Cryptography.HMACSHA256();
            pbkdf2.Key = saltBytes;
            byte[] hashBytes = pbkdf2.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hashBytes);
        }


        public void CloseDBServer()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        private void OnApplicationQuit()
        {
            CloseDBServer();
        }
    }
}