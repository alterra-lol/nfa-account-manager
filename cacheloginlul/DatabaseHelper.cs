using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace nfa
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string SteamId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public long LastLogin { get; set; }
        public bool InConfig { get; set; }
    }

    public class DataStore
    {
        private readonly string dataPath;
        private List<Account> accounts;
        private int nextId;

        public DataStore()
        {
            string dataDir = AppDomain.CurrentDomain.BaseDirectory;
            dataPath = Path.Combine(dataDir, "accounts.json");
            
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(dataPath))
            {
                try
                {
                    string json = File.ReadAllText(dataPath);
                    accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
                    nextId = accounts.Count > 0 ? accounts.Max(a => a.Id) + 1 : 1;
                }
                catch
                {
                    accounts = new List<Account>();
                    nextId = 1;
                }
            }
            else
            {
                accounts = new List<Account>();
                nextId = 1;
            }
        }

        private void SaveData()
        {
            try
            {
                string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public void AddAccount(string username, string token, string steamid)
        {
            if (accounts.Any(a => a.Username == username && a.SteamId == steamid))
                return;

            accounts.Add(new Account
            {
                Id = nextId++,
                Username = username,
                SteamId = steamid,
                Token = token
            });

            SaveData();
        }

        public void RemoveAccount(int id)
        {
            accounts.RemoveAll(a => a.Id == id);
            SaveData();
        }

        public List<Account> GetAllAccounts()
        {
            return new List<Account>(accounts);
        }

        public Account? GetAccount(string username)
        {
            return accounts.FirstOrDefault(a => a.Username == username);
        }

        public int? GetAccountId(string username)
        {
            return accounts.FirstOrDefault(a => a.Username == username)?.Id;
        }
    }

    public static class TokenParser
    {
        public static string GetSteamIdFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return string.Empty;

                string payload = parts[1];
                
                int padding = payload.Length % 4;
                if (padding > 0)
                {
                    payload += new string('=', 4 - padding);
                }

                byte[] data = Convert.FromBase64String(payload);
                string json = System.Text.Encoding.UTF8.GetString(data);
                
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("sub", out var subElement))
                {
                    return subElement.GetString() ?? string.Empty;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
