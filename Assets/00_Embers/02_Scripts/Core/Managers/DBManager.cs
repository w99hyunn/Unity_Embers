using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Michsky.UI.Reach.ChapterManager;

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

    public enum CreateCharacterResult
    {
        SUCCESS,
        DUPLICATE,
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

        /// <summary>
        /// 회원가입
        /// </summary>
        /// <param name="username">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <param name="email">이메일</param>
        /// <returns></returns>
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


        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="username">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <returns></returns>
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

        /// <summary>
        /// 유저 정보 업데이트
        /// </summary>
        /// <param name="username">아이디</param>
        /// <param name="newPassword">비밀번호</param>
        /// <param name="newEmail">이메일</param>
        /// <returns></returns>
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

        /// <summary>
        /// 캐릭터 생성
        /// </summary>
        /// <param name="username">로그인 ID</param>
        /// <param name="characterName">캐릭터명</param>
        /// <param name="faction">int값 Hope: 0, Fire: 1</param>
        /// <param name="characterClass">Warrior, Mage, ...</param>
        /// <param name="gender">Male, Female, Other</param>
        /// <param name="mapCode">인트값</param>
        /// <returns></returns>
        public CreateCharacterResult CreateCharacter(string username, string characterName, int faction, int characterClass, int gender, int mapCode)
        {
            try
            {
                // 1. username으로 account_id 가져오기
                string accountQuery = "SELECT account_id FROM account WHERE username = @username";
                int accountId;

                using (MySqlCommand accountCmd = new MySqlCommand(accountQuery, connection))
                {
                    accountCmd.Parameters.AddWithValue("@username", username);

                    object result = accountCmd.ExecuteScalar();
                    if (result == null)
                    {
                        Managers.Log.Log("CreateCharacter Error: Username not found.");
                        return CreateCharacterResult.ERROR;
                    }

                    accountId = Convert.ToInt32(result);
                }

                // 2. 캐릭터 이름 중복 검사
                string nameCheckQuery = "SELECT COUNT(*) FROM `character` WHERE name = @name";
                using (MySqlCommand nameCheckCmd = new MySqlCommand(nameCheckQuery, connection))
                {
                    nameCheckCmd.Parameters.AddWithValue("@name", characterName);

                    int nameCount = Convert.ToInt32(nameCheckCmd.ExecuteScalar());
                    if (nameCount > 0)
                    {
                        return CreateCharacterResult.DUPLICATE;
                    }
                }

                // 3. character 생성 쿼리
                string characterQuery = @"
                                        INSERT INTO `character`
                                        (account_id, name, faction, class, gender, mapCode) 
                                        VALUES 
                                        (@account_id, @name, @faction, @class, @gender, @mapCode)";

                using (MySqlCommand characterCmd = new MySqlCommand(characterQuery, connection))
                {
                    characterCmd.Parameters.AddWithValue("@account_id", accountId);
                    characterCmd.Parameters.AddWithValue("@name", characterName);
                    characterCmd.Parameters.AddWithValue("@faction", faction);
                    characterCmd.Parameters.AddWithValue("@class", characterClass);
                    characterCmd.Parameters.AddWithValue("@gender", gender);
                    characterCmd.Parameters.AddWithValue("@mapCode", mapCode);

                    int rowsAffected = characterCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Managers.Log.Log("Character created successfully. : " + characterName);
                        return CreateCharacterResult.SUCCESS;
                    }
                    else
                    {
                        Managers.Log.Log("Character creation failed.");
                        return CreateCharacterResult.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                Managers.Log.Log("Error creating character: " + ex.Message);
                return CreateCharacterResult.ERROR;
            }
        }

        /// <summary>
        /// 클라이언트가 로그인 했을 때 해당 accountID에 있는 모든 캐릭터 정보를 캐릭터 리스트에 띄울 정보만 가져옴.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<ChapterItem> GetCharactersByUsername(string username)
        {
            List<ChapterItem> characters = new List<ChapterItem>();

            try
            {
                // 1. username으로 account_id 가져오기
                string accountQuery = "SELECT account_id FROM account WHERE username = @username";
                int accountId;

                using (MySqlCommand accountCmd = new MySqlCommand(accountQuery, connection))
                {
                    accountCmd.Parameters.AddWithValue("@username", username);

                    object result = accountCmd.ExecuteScalar();
                    if (result == null)
                    {
                        Debug.LogError("GetCharactersByUsername Error: Username not found.");
                        return characters; // 빈 리스트 반환
                    }

                    accountId = Convert.ToInt32(result);
                }

                // 2. account_id로 캐릭터 목록 가져오기
                string characterQuery = "SELECT name, class, level, attack FROM `character` WHERE account_id = @account_id";

                using (MySqlCommand cmd = new MySqlCommand(characterQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@account_id", accountId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChapterItem character = new ChapterItem
                            {
                                chapterID = reader.GetString("name"),
                                title = reader.GetString("name"),
                                characterClass = reader.GetInt32("class"),
                                description = $"Level {reader.GetInt32("level")} | Attack {reader.GetInt32("attack")}",
                                defaultState = ChapterState.CharacterPlayAndDelete,
                            };
                            characters.Add(character);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("GetCharactersByUsername Error: " + ex.Message);
            }

            return characters;
        }

        /// <summary>
        /// 클라이언트가 캐릭터를 선택했을 때 해당 캐릭터의 모든 정보 불러오기
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public PlayerDataSO FetchPlayerDataFromDB(string username)
        {
            PlayerDataSO playerData = ScriptableObject.CreateInstance<PlayerDataSO>();

            string query = @"
                            SELECT 
                                `name`, `level`, `hp`, `mp`, `exp`, `gold`, `maxhp`, `maxmp`, 
                                `attack`, `class`, `sp`, `gender`, 
                                `current_position_x`, `current_position_y`, `current_position_z`, 
                                `mapCode`
                            FROM `character`
                            WHERE `name` = @username;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        playerData.Username = reader.GetString("name");
                        playerData.Level = reader.GetInt32("level");
                        playerData.Hp = reader.GetInt32("hp");
                        playerData.Mp = reader.GetInt32("mp");
                        playerData.Exp = reader.GetInt32("exp");
                        playerData.Gold = reader.GetInt32("gold");
                        playerData.MaxHp = reader.GetInt32("maxhp");
                        playerData.MaxMp = reader.GetInt32("maxmp");
                        playerData.Attack = reader.GetInt32("attack");
                        playerData.Class = reader.GetString("class");
                        playerData.Sp = reader.GetInt32("sp");
                        playerData.Gender = reader.GetString("gender");

                        float posX = reader.GetFloat("current_position_x");
                        float posY = reader.GetFloat("current_position_y");
                        float posZ = reader.GetFloat("current_position_z");
                        playerData.Position = new Vector3(posX, posY, posZ);

                        playerData.MapCode = reader.GetString("mapCode");
                    }
                }
            }

            return playerData;
        }

        /// <summary>
        /// 클라이언트의 데이터에 변화가 있을 때 해당 캐릭터의 정보 업데이트
        /// </summary>
        /// <param name="username"></param>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        public void UpdateDatabase(string username, string fieldName, string newValue)
        {
            try
            {
                if (fieldName == nameof(PlayerDataSO.Position))
                {
                    // Position 값을 처리하는 로직
                    Vector3 position = JsonUtility.FromJson<Vector3>(newValue);

                    Debug.Log("받은 값 " + position);

                    string query = @"
                                    UPDATE `character` 
                                    SET `current_position_x` = @posX, 
                                        `current_position_y` = @posY, 
                                        `current_position_z` = @posZ 
                                    WHERE `name` = @name;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@posX", position.x);
                        command.Parameters.AddWithValue("@posY", position.y);
                        command.Parameters.AddWithValue("@posZ", position.z);
                        command.Parameters.AddWithValue("@name", username);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Log($"Position updated successfully. Rows affected: {rowsAffected}");
                    }
                }
                else
                {
                    string query = $"UPDATE `character` SET `{fieldName}` = @newValue WHERE `name` = @name;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 파라미터 바인딩
                        command.Parameters.AddWithValue("@newValue", newValue);
                        command.Parameters.AddWithValue("@name", username);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Log($"Database updated successfully. Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating database: {ex.Message}");
            }
        }



    }
}