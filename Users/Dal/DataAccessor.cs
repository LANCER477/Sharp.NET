using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SharpKnP321.Users.Dal;
using Dapper;
using SharpKnP321.Users.Dal.Entities;

namespace SharpKnP321.Users.Dal
{
    internal class DataAccessor
    {
        private SqlConnection connection;
        
        public DataAccessor()
        {
            String settingsFilename = "appsettings.json";
            if (!File.Exists(settingsFilename))
            {
                Console.WriteLine("Не знайдено файл конфігурації. Прочитайте Readme");
                return;
            }
            var settings = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(settingsFilename)
            );

            String userDb;
            try
            {
                var csSection = settings.GetProperty("ConnectionStrings");
                userDb = csSection.GetProperty("UserDB").GetString()!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка визначення конфігурації: {ex.Message}");
            }
            
            connection = new(userDb);
            
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка визначення конфігурації: {ex.Message}");
            }
        }

        public void SignUp(UserData userData)
        {
            if(userData.UserId == default)
            {
                userData.UserId = Guid.NewGuid();
            }
            userData.UserEmailCode = "";

            {
                userData.UserEmailCode = Random.Shared.Next(100000, 1000000).ToString();
            }
        }

        // --- НАЧАЛО ДЗ: Проверка активного токена при аутентификации ---
        // Д.З. При автентифікації здійснювати перевірку чи є в користувача 
        // активний токен. У такому разі подовжувати його дію, замість 
        // генерування нового. За відсутності - генерувати новий.
        public string CreateOrUpdateToken(Guid accessId)
        {
            var existingTokenId = connection.QueryFirstOrDefault<Guid?>(
                "SELECT TokenId FROM AccessToken WHERE AccessId = @accessId AND TokenExp > CURRENT_TIMESTAMP",
                new { accessId }
            );

            if (existingTokenId != null && existingTokenId != Guid.Empty)
            {
                DateTime newExpiration = DateTime.Now.AddHours(1);
                
                connection.Execute(
                    "UPDATE AccessToken SET TokenExp = @newExp WHERE TokenId = @tokenId",
                    new { newExp = newExpiration, tokenId = existingTokenId }
                );
                
                return existingTokenId.ToString();
            }
            else
            {
                Guid newTokenId = Guid.NewGuid();
                DateTime now = DateTime.Now;
                DateTime expiration = now.AddHours(1);

                connection.Execute(
                    "INSERT INTO AccessToken (TokenId, AccessId, TokenIat, TokenExp) VALUES (@tokenId, @accessId, @iat, @exp)",
                    new { tokenId = newTokenId, accessId = accessId, iat = now, exp = expiration }
                );
                
                return newTokenId.ToString();
            }
        }
       

        public void Install(bool isHard = false)
        {
            if (isHard)
            {
                connection.Execute("DROP TABLE IF EXISTS UserData");
            }
            connection.Execute(@"CREATE TABLE UserData (
        UserId          UNIQUEIDENTIFIER PRIMARY KEY,
        UserName        NVARCHAR(128)    NOT NULL,
        UserEmail       NVARCHAR(256)    NOT NULL,
        UserEmailCode   VARCHAR(16)          NULL,
        UserDelAt       DATETIME2            NULL
    )");

            if (isHard)
            {
                connection.Execute("DROP TABLE IF EXISTS UserAccess");
            }

            connection.Execute(@"CREATE TABLE UserAccess (
        AccessId        UNIQUEIDENTIFIER PRIMARY KEY,
        UserId          UNIQUEIDENTIFIER NOT NULL,
        RoleId          UNIQUEIDENTIFIER     NULL,
        AccessLogin     NVARCHAR(64)     NOT NULL,
        AccessSalt      CHAR(16)         NOT NULL,
        AccessDk        CHAR(32)             NULL
    )");
            if (isHard)
            {
                connection.Execute("DROP TABLE IF EXISTS AccessToken");
            }

            connection.Execute(@"CREATE TABLE AccessToken (
        TokenId         UNIQUEIDENTIFIER PRIMARY KEY,
        AccessId        UNIQUEIDENTIFIER NOT NULL,
        TokenIat        DATETIME2        NOT NULL,
        TokenExp        DATETIME2            NULL
    )");
        }
    }
}
