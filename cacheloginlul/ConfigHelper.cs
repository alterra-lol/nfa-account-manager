using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using System.Collections.Generic;

namespace nfa
{
    public static class ConfigHelper
    {
        private static string GetAppDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private static string GetBackupPath()
        {
            string backupDir = Path.Combine(GetAppDirectory(), "backup");
            Directory.CreateDirectory(backupDir);
            return backupDir;
        }

        public static string ParseEya(string eya)
        {
            var tokenArr = eya.Split('.');
            if (tokenArr.Length != 3)
            {
                return null;
            }

            string base64 = tokenArr[1];
            int padding = base64.Length % 4;
            if (padding != 0)
            {
                base64 += new string('=', 4 - padding);
            }

            try
            {
                byte[] data = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                return null;
            }
        }

        public static void DoLogin(string accountName, string token)
        {
            if (accountName.Contains('@'))
            {
                accountName = accountName.Split('@')[0];
            }

            string crc32AccountName = ComputeCrc32(accountName) + "1";
            string jsonData = ParseEya(token);
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var jsonDoc = JsonDocument.Parse(jsonData);
            string steamId = jsonDoc.RootElement.GetProperty("sub").GetString();

            string mtbf = GenerateRandomDigits(9);
            string jwt = SteamEncrypt(token, accountName);
            string path = GetSteamInstallPath();
            string localVdfPath = GetLocalVdfPath();

            if (File.Exists(localVdfPath))
            {
                File.Delete(localVdfPath);
            }

            try
            {
                Directory.CreateDirectory(Path.Combine(path, "config"));
            }
            catch { }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam", true))
            {
                key?.SetValue("AutoLoginUser", accountName);
            }

            string config = BuildConfig(mtbf, steamId, accountName);
            string loginUsers = BuildLoginUsers(steamId, accountName);
            string local = BuildLocal(crc32AccountName, jwt);

            RemoveReadonly(Path.Combine(path, "config", "config.vdf"));
            File.WriteAllText(Path.Combine(path, "config", "config.vdf"), config, Encoding.UTF8);

            RemoveReadonly(Path.Combine(path, "config", "loginusers.vdf"));
            File.WriteAllText(Path.Combine(path, "config", "loginusers.vdf"), loginUsers, Encoding.UTF8);

            if (File.Exists(localVdfPath))
            {
                RemoveReadonly(localVdfPath);
                File.Delete(localVdfPath);
            }
            File.WriteAllText(localVdfPath, local, Encoding.UTF8);

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(path, "steam.exe"),
                UseShellExecute = true
            });
        }

        private static int GetPid(string processName)
        {
            var processes = Process.GetProcessesByName(processName.Replace(".exe", ""));
            return processes.Length > 0 ? processes[0].Id : 0;
        }

        private static string ReadRegistryValue(string keyPath, string valueName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                var value = key?.GetValue(valueName);
                return value?.ToString() ?? string.Empty;
            }
        }

        public static string GetSteamInstallPath()
        {
            int steamPid = GetPid("steam.exe");
            string steamPath;

            if (steamPid != 0)
            {
                var process = Process.GetProcessById(steamPid);
                steamPath = process.MainModule.FileName;
                KillSteam();
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                steamPath = ReadRegistryValue(@"Software\Classes\steam\Shell\Open\Command", "");
                steamPath = steamPath.Replace("\"", "");
                if (steamPath.Length > 6)
                    steamPath = steamPath.Substring(0, steamPath.Length - 6);
            }

            if (steamPath.Length > 9)
                steamPath = steamPath.Substring(0, steamPath.Length - 9);

            return steamPath;
        }

        private static string ComputeCrc32(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            uint crc = 0xFFFFFFFF;

            foreach (byte b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
                }
            }

            crc ^= 0xFFFFFFFF;
            return crc.ToString("x8").TrimStart('0');
        }

        private static string SteamEncrypt(string token, string accountName)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(token);
            byte[] entropy = Encoding.UTF8.GetBytes(accountName);

            byte[] encryptedData = ProtectedData.Protect(
                dataToEncrypt,
                entropy,
                DataProtectionScope.CurrentUser
            );

            return BitConverter.ToString(encryptedData).Replace("-", "").ToLower();
        }

        private static string GenerateRandomDigits(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => (char)('0' + random.Next(10))).ToArray());
        }

        private static string BuildLoginUsers(string steamId, string accountName)
        {
            return $@"users
{{
    {steamId}
    {{
        AccountName    ""{accountName}""
        PersonaName    ""alterra.lol""
        RememberPassword    ""1""
        WantsOfflineMode    ""0""
        SkipOfflineModeWarning    ""0""
        AllowAutoLogin    ""1""
        MostRecent    ""1""
        Timestamp    ""{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}""
    }}
}}";
        }

        private static string BuildConfig(string mtbf, string steamId, string accountName)
        {
            return $@"InstallConfigStore
{{
    Software
    {{
        Valve
        {{
            Steam
            {{
                AutoUpdateWindowEnabled    ""0""
                Accounts
                {{
                    {accountName}
                    {{
                        SteamID    ""{steamId}""
                    }}
                }}
                MTBF    ""{mtbf}""
                CellIDServerOverride    ""170""
                Rate    ""30000""
            }}
        }}
    }}
}}";
        }

        private static string BuildLocal(string crc32, string jwt)
        {
            return $@"MachineUserConfigStore
{{
    Software
    {{
        Valve
        {{
            Steam
            {{
                ConnectCache
                {{
                    {crc32}    ""{jwt}""
                }}
            }}
        }}
    }}
}}";
        }
        private static string GetLocalVdfPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataPath, "Steam", "local.vdf");
        }

        private static void RemoveReadonly(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                }
                catch { }
            }
        }

        public static void ResetSteam()
        {
            string path = GetSteamInstallPath();
            
            string[] directories = {
                Path.Combine(path, "userdata"),
                Path.Combine(path, "config")
            };

            foreach (var directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch { }
                }
            }

            string localVdfPath = GetLocalVdfPath();
            if (File.Exists(localVdfPath))
            {
                File.Delete(localVdfPath);
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(path, "steam.exe"),
                UseShellExecute = true
            });
        }

        public static string GetCurrentAccount()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
                {
                    return key?.GetValue("AutoLoginUser")?.ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        public static void KillSteam()
        {
            int steamPid = GetPid("steam.exe");
            if (steamPid != 0)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = "/f /im steam.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false
                })?.WaitForExit();

                Process.Start(new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = "/f /im steamwebhelper.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false
                })?.WaitForExit();

                System.Threading.Thread.Sleep(2000);
            }
        }

        public static void SaveCurrentAccounts()
        {
            try
            {
                string username = GetCurrentAccount();
                string vdfPath = GetLocalVdfPath();
                string path = GetSteamInstallPath();

                string backupPath = GetBackupPath();

                string userFile = Path.Combine(backupPath, "saved_user.txt");
                if (File.Exists(userFile))
                {
                    File.Delete(userFile);
                }

                try
                {
                    File.Copy(Path.Combine(path, "config", "config.vdf"), 
                             Path.Combine(backupPath, "config.vdf"), true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                try
                {
                    File.Copy(Path.Combine(path, "config", "loginusers.vdf"), 
                             Path.Combine(backupPath, "loginusers.vdf"), true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                try
                {
                    File.Copy(vdfPath, Path.Combine(backupPath, "local.vdf"), true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                if (!string.IsNullOrEmpty(username))
                {
                    File.WriteAllText(userFile, username);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void RestoreSavedAccounts()
        {
            KillSteam();
            try
            {
                string vdfPath = GetLocalVdfPath();
                string path = GetSteamInstallPath();

                string backupPath = GetBackupPath();

                try
                {
                    File.Copy(Path.Combine(backupPath, "config.vdf"), 
                             Path.Combine(path, "config", "config.vdf"), true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                try
                {
                    File.Copy(Path.Combine(backupPath, "loginusers.vdf"), 
                             Path.Combine(path, "config", "loginusers.vdf"), true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                try
                {
                    File.Copy(Path.Combine(backupPath, "local.vdf"), vdfPath, true);
                }
                catch (Exception e) { Console.WriteLine(e); }

                StartSteam();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void StartSteam()
        {
            string path = GetSteamInstallPath();
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(path, "steam.exe"),
                UseShellExecute = true
            });
        }
    }
}
