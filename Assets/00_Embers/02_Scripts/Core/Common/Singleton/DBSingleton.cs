using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Michsky.UI.Reach.ChapterManager;

namespace NOLDA
{
    public class LoginResponse
    {
        public LoginResult Result { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
    }
    
    public class DBSingleton : MonoBehaviour
    {
        #region # ItemDataBase

        [SerializeField]
        private Dictionary<int, ItemData> itemDataCache = new Dictionary<int, ItemData>();
        
        private void InitializeItemData()
        {
            // 모든 ScriptableObject를 로드
            var allItems = Resources.LoadAll<ItemData>("ItemData");
            foreach (var item in allItems)
            {
                if (!itemDataCache.ContainsKey(item.ID))
                {
                    itemDataCache.Add(item.ID, item);
                }
            }
        }

        public ItemData GetItemDataById(int id)
        {
            itemDataCache.TryGetValue(id, out var itemData);
            return itemData;
        }
        #endregion
        
        
        private MySqlConnection _connection;
        
        private void Start()
        {
            InitializeItemData(); //아이템 데이터베이스
        }

        #region # DB 서버 Open / Close
        public bool ConnectDB()
        {
            bool success = ConnectToDatabase(Singleton.Game.DBServerIP, "embers", Singleton.Game.DBHost, Singleton.Game.DBPw, "3306");
            return success;
        }

        private bool ConnectToDatabase(string server, string database, string uid, string password, string port)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};PORT={port};Allow Zero Datetime=True;Convert Zero Datetime=True;";

            _connection = new MySqlConnection(connectionString);

            try
            {
                _connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CloseDBServer()
        {
            _connection?.Close();
        }

        private void OnApplicationQuit()
        {
            CloseDBServer();
        }
        #endregion

        #region # 비밀번호 암호화 로직
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
        #endregion
        
        #region # 회원가입
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
            string query = "INSERT INTO account (Username, Password_hash, Password_salt, Email) VALUES (@Username, @PasswordHash, @Salt, @Email)";

            MySqlCommand cmd = new MySqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Salt", salt);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                cmd.ExecuteNonQuery();
                DebugUtils.Log($"Sign Up New User : {username}");
                return SignUpResult.SUCCESS;
            }
            catch (Exception ex)
            {
                DebugUtils.LogError($"Sign Up New User Failed : {username} / Exception : {ex}");
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
            string query = "SELECT COUNT(*) FROM account WHERE Username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Username", username);

            try
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Duplicate Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region # 로그인
        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="username">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <returns></returns>
        public LoginResponse Login(string username, string password)
        {
            string query = "SELECT Password_hash, Password_salt, Email, Created_at FROM account WHERE Username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Username", username);

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = reader.GetString("Password_hash");
                        string storedSalt = reader.GetString("Password_salt");

                        string inputPasswordHash = GetHash(password, storedSalt);

                        if (inputPasswordHash == storedPasswordHash)
                        {
                            string email = reader.GetString("Email");
                            DateTime createdAt = reader.GetDateTime("Created_at");
                            string createdAtString = createdAt.ToString("yyyy-MM-dd HH:mm:ss");

                            reader.Close();

                            string updateQuery = "UPDATE account SET Is_online = TRUE WHERE Username = @Username";
                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, _connection);
                            updateCmd.Parameters.AddWithValue("@Username", username);
                            updateCmd.ExecuteNonQuery();

                            DebugUtils.Log($"User Login : {username}");
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
                DebugUtils.Log($"User Login Error: {username}, Exception : {ex}");
                return new LoginResponse
                {
                    Result = LoginResult.ERROR
                };
            }
        }
        #endregion

        #region # 유저 정보 업데이트
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
                           (passwordHash != null ? ", Password_hash = @PasswordHash, Password_salt = @Salt" : "") +
                           " WHERE Username = @Username";

            MySqlCommand cmd = new MySqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", newEmail);

            // 비밀번호가 있으면 추가
            if (passwordHash != null)
            {
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@Salt", salt);
            }

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    DebugUtils.Log($"User Info Updated : {username}");
                    return true;
                }
                else
                {
                    DebugUtils.LogError($"User not found or no changes made : {username}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DebugUtils.LogError($"Error Updating User Info : {username} / Exception : {ex}");
                return false;
            }
        }
        #endregion

        #region # 캐릭터 생성
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
        public CreateCharacterResult CreateCharacter(string username, string characterName, Faction faction, Class characterClass, Gender gender, int mapCode)
        {
            try
            {
                // 1. username으로 Account_id 가져오기
                string accountQuery = "SELECT Account_id FROM account WHERE Username = @Username";
                int accountId;

                using (MySqlCommand accountCmd = new MySqlCommand(accountQuery, _connection))
                {
                    accountCmd.Parameters.AddWithValue("@Username", username);

                    object result = accountCmd.ExecuteScalar();
                    if (result == null)
                    {
                        DebugUtils.Log("CreateCharacter Error: Username not found.");
                        return CreateCharacterResult.ERROR;
                    }

                    accountId = Convert.ToInt32(result);
                }

                // 2. 캐릭터 이름 중복 검사
                string nameCheckQuery = "SELECT COUNT(*) FROM `character` WHERE Name = @Name";
                using (MySqlCommand nameCheckCmd = new MySqlCommand(nameCheckQuery, _connection))
                {
                    nameCheckCmd.Parameters.AddWithValue("@Name", characterName);

                    int nameCount = Convert.ToInt32(nameCheckCmd.ExecuteScalar());
                    if (nameCount > 0)
                    {
                        return CreateCharacterResult.DUPLICATE;
                    }
                }

                // 3. character 생성 쿼리
                string characterQuery = @"
                                        INSERT INTO `character`
                                        (Account_id, Name, Faction, Class, Gender, MapCode) 
                                        VALUES 
                                        (@Account_id, @Name, @Faction, @Class, @Gender, @MapCode)";
                
                using (MySqlCommand characterCmd = new MySqlCommand(characterQuery, _connection))
                {
                    characterCmd.Parameters.AddWithValue("@Account_id", accountId);
                    characterCmd.Parameters.AddWithValue("@Name", characterName);
                    characterCmd.Parameters.AddWithValue("@Faction", faction.ToString());
                    characterCmd.Parameters.AddWithValue("@Class", characterClass.ToString());
                    characterCmd.Parameters.AddWithValue("@Gender", gender.ToString());
                    characterCmd.Parameters.AddWithValue("@MapCode", mapCode);

                    int rowsAffected = characterCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        DebugUtils.Log("Character created successfully. : " + characterName);
                        return CreateCharacterResult.SUCCESS;
                    }
                    else
                    {
                        DebugUtils.Log("Character creation failed.");
                        return CreateCharacterResult.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Log("Error creating character: " + ex.Message);
                return CreateCharacterResult.ERROR;
            }
        }
        #endregion

        #region # 캐릭터 리스트 && 선택(데이터 불러오기)
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
                // 1. username으로 Account_id 가져오기
                string accountQuery = "SELECT Account_id FROM account WHERE Username = @Username";
                int accountId;

                using (MySqlCommand accountCmd = new MySqlCommand(accountQuery, _connection))
                {
                    accountCmd.Parameters.AddWithValue("@Username", username);

                    object result = accountCmd.ExecuteScalar();
                    if (result == null)
                    {
                        Debug.LogError("GetCharactersByUsername Error: Username not found.");
                        return characters; // 빈 리스트 반환
                    }

                    accountId = Convert.ToInt32(result);
                }

                // 2. Account_id로 캐릭터 목록 가져오기
                string characterQuery = "SELECT Name, Class, Level, Attack FROM `character` WHERE Account_id = @Account_id";

                using (MySqlCommand cmd = new MySqlCommand(characterQuery, _connection))
                {
                    cmd.Parameters.AddWithValue("@Account_id", accountId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChapterItem character = new ChapterItem
                            {
                                chapterID = reader.GetString("Name"),
                                title = reader.GetString("Name"),
                                description = $"Level {reader.GetInt32("Level")} | Attack {reader.GetInt32("Attack")}",
                                defaultState = ChapterState.CharacterPlayAndDelete,
                            };
                            
                            string classString = reader.GetString("Class");
                            if (Enum.TryParse(classString, true, out Class characterClass))
                            {
                                character.characterClass = characterClass;
                            }

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

            string characterQuery = @"
        SELECT 
            `Character_id`, `Name`, `Level`, `Hp`, `Mp`, `Hxp`, `Gold`, `MaxHp`, `MaxMp`, 
            `Attack`, `Armor`, `Class`, `Faction`, `Sp`, `Gender`, 
            `Current_position_x`, `Current_position_y`, `Current_position_z`, 
            `MapCode`, `InventorySpace`
        FROM `character`
        WHERE `Name` = @Name;";

            using (MySqlCommand command = new MySqlCommand(characterQuery, _connection))
            {
                command.Parameters.AddWithValue("@name", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 데이터 set 이벤트 중지
                        playerData.suppressEvents = true;

                        int characterId = reader.GetInt32("Character_id");
                        playerData.Username = reader.GetString("Name");
                        playerData.Level = reader.GetInt32("Level");
                        playerData.MaxHp = reader.GetInt32("MaxHp");
                        playerData.Hp = reader.GetInt32("Hp");
                        playerData.MaxMp = reader.GetInt32("MaxMp");
                        playerData.Mp = reader.GetInt32("Mp");
                        playerData.Hxp = reader.GetInt32("Hxp");
                        playerData.Gold = reader.GetInt32("Gold");
                        playerData.Attack = reader.GetInt32("Attack");
                        string classString = reader.GetString("Class");
                        if (Enum.TryParse(classString, out Class characterClass))
                        {
                            playerData.Class = characterClass;
                        }
                        string factionString = reader.GetString("Faction");
                        if (Enum.TryParse(factionString, out Faction characterFaction))
                        {
                            playerData.Faction = characterFaction;
                        }
                        playerData.Sp = reader.GetInt32("Sp");
                        string genderString = reader.GetString("Gender");
                        if (Enum.TryParse(genderString, out Gender characterGender))
                        {
                            playerData.Gender = characterGender;
                        }

                        float posX = reader.GetFloat("Current_position_x");
                        float posY = reader.GetFloat("Current_position_y");
                        float posZ = reader.GetFloat("Current_position_z");
                        playerData.Position = new Vector3(posX, posY, posZ);
                        playerData.MapCode = reader.GetString("MapCode");
                        playerData.InventorySpace = reader.GetInt32("InventorySpace");
                        
                        reader.Close();
                        
                        // 인벤토리 데이터 가져오기
                        string inventoryQuery = @"
                            SELECT `Item_id`, `Amount`, `Position`
                            FROM `inventory`
                            WHERE `Character_id` = @CharacterId;";

                        command.CommandText = inventoryQuery;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@CharacterId", characterId);

                        using (MySqlDataReader inventoryReader = command.ExecuteReader())
                        {
                            playerData.Items = new Item[playerData.InventorySpace]; // 인벤토리 용량

                            while (inventoryReader.Read())
                            {
                                int itemId = inventoryReader.GetInt32("Item_id");
                                int amount = inventoryReader.GetInt32("Amount");
                                int position = inventoryReader.GetInt32("Position");

                                // ItemDatabase에서 ItemData 가져오기
                                ItemData itemData = GetItemDataById(itemId);
                                if (itemData != null)
                                {
                                    Item item = null;

                                    // 아이템 종류에 따라 처리
                                    if (itemData is ArmorItemData armorData)
                                    {
                                        //Debug.Log("방어구 " + itemData.Name);
                                        item = new ArmorItem(armorData);
                                    }
                                    else if (itemData is WeaponItemData weaponData)
                                    {
                                        //Debug.Log("무기 " + itemData.Name);
                                        item = new WeaponItem(weaponData);
                                    }
                                    else if (itemData is PortionItemData portionData)
                                    {
                                        //Debug.Log("포션 " + itemData.Name);
                                        item = new PortionItem(portionData, amount);
                                    }
                                    else
                                    {
                                        item = itemData.CreateItem();
                                    }

                                    // Position에 따라 배열에 저장
                                    if (position >= 0 && position < playerData.Items.Length)
                                    {
                                        playerData.Items[position] = item;
                                    }
                                }
                            }
                        }

                        // 데이터 set 이벤트 재개
                        playerData.suppressEvents = false;
                    }
                }
            }

            return playerData;
        }
        #endregion

        #region # 캐릭터 삭제
        /// <summary>
        /// 캐릭터 삭제
        /// </summary>
        /// <param name="username"></param>
        public bool DeleteCharacter(string username)
        {
            try
            {
                string query = "DELETE FROM `character` WHERE `Name` = @Name;";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Name", username);

                    int rowsAffected = command.ExecuteNonQuery();
                    DebugUtils.Log($"Character deleted successfully. Rows affected: {rowsAffected}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                DebugUtils.LogError($"Error deleting character: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region # 캐릭터 업데이트 자동 업데이트 로직
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
                    
                    string query = @"
                                    UPDATE `character` 
                                    SET `Current_position_x` = @PosX, 
                                        `Current_position_y` = @PosY, 
                                        `Current_position_z` = @PosZ 
                                    WHERE `Name` = @Name;";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@PosX", position.x);
                        command.Parameters.AddWithValue("@PosY", (position.y + 1f));
                        command.Parameters.AddWithValue("@PosZ", position.z);
                        command.Parameters.AddWithValue("@Name", username);

                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    string query = $"UPDATE `character` SET `{fieldName}` = @NewValue WHERE `Name` = @Name;";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        // 파라미터 바인딩
                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@Name", username);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating database: {ex.Message}");
            }
        }
        #endregion

        #region # 캐릭터 인벤토리 자동 업데이트 로직

        public void HandleSlotUpdateInDB(string characterName, int index, int itemId, int amount)
        {
            string query;

            // 슬롯 비우기
            if (itemId == -1)
            {
                query = @"
            DELETE FROM `inventory`
            WHERE `Character_id` = (
                SELECT `Character_id` FROM `character` WHERE `Name` = @CharacterName
            ) AND `Position` = @Position;";
            }
            else
            {
                // 슬롯 업데이트 또는 삽입
                query = @"
            INSERT INTO `inventory` (`Character_id`, `Item_id`, `Amount`, `Position`)
            VALUES (
                (SELECT `Character_id` FROM `character` WHERE `Name` = @CharacterName),
                @ItemId, @Amount, @Position
            )
            ON DUPLICATE KEY UPDATE
                `Item_id` = VALUES(`Item_id`),
                `Amount` = VALUES(`Amount`);";
            }

            using (var command = new MySqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@CharacterName", characterName);
                command.Parameters.AddWithValue("@ItemId", itemId);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Position", index);

                command.ExecuteNonQuery();
            }
        }


        #endregion
    }
}