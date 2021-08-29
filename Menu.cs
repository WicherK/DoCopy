using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DoCopy
{
    public static class Menu
    {
        private static Config config = new Config();

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void Help()
        {
            Console.WriteLine("Remember to store backups on the other disk (not partition) or other machine!!!");
            Console.WriteLine("Remember it is recommended to protect your backup with password!!!");
            Console.WriteLine("Remember it is recommended to set all settings check if you set all necessary settings with command 'isGood'!!!");
            Console.WriteLine("Remember if you didn't set necessary settings program won't work properly!!!");
            Console.WriteLine("Remember to not change config file manually if you don't know what you do it can cause program crash!!!");
            Console.WriteLine("------------------------Add directories to be backuped-------------------------");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("show - show directories you chose.                                             ");
            Console.WriteLine("add <Directory_To_Backup> - add directory to the config file that will be backuped.");
            Console.WriteLine("delete <Id_Of_The_Directory> - delete directory from the config file.");
            Console.WriteLine("edit <Id_Of_The_Directory> <New_Directory_To_Backup> - edit directory with the given id.");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("------------------------Final backup zip file settings-------------------------");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("name <Copy_Name> - set name of the copy file (default 'Copy').                 ");
            Console.WriteLine("setPassword <Copy_Password> - set password of the copy file.                      ");
            Console.WriteLine("deletePassword - remove password of the copy file.                             ");
            Console.WriteLine("setPath <Name_Of_The_Path> - set path of directory where copies are gonna be save saved.");
            Console.WriteLine("showPath - show path of directory where copies are gonna be save saved.");
            Console.WriteLine("setTime - enable or disable setting time on the name of file.                  ");
            Console.WriteLine("setDate - enable or disable setting date on the name of file.                  ");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("-----------------------------Send backup to server-----------------------------");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("serverOn - turn on sending backups on server                                   ");
            Console.WriteLine("serverOff - turn off sending backups on server                                 ");
            Console.WriteLine("setServerIp <Server_Ip> - set ip of the server where you want to store copy.   ");
            Console.WriteLine("setServerPath <Path_On_Server> - set path where backup will be sent (only directories in your home folder).");
            Console.WriteLine("setServerUser <User> - set user to connect to the server (it is better to use user that is  meant only to store backups).");
            Console.WriteLine("setServerPassword - set password to connect to the server.");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("------------------------------Start doing copies-------------------------------");
            Console.WriteLine("                                                                               ");
            Console.WriteLine("showConfig - show config.                                                      ");
            Console.WriteLine("isGood - check if all necessary settings have been set.                        ");
            Console.WriteLine("on - enable doing copies every time after closing the computer.                ");
            Console.WriteLine("off - disable doing copies every time after closing the computer.              ");
            Console.WriteLine("save - save all (save is doing automatically after closing an application but this is more save way).");
        }

        public static void Show()
        {
            if (config.directories.Count == 0)
                Console.WriteLine("There are no directories set.");
            else
                for(int i = 0; i < config.directories.Count; i++)
                    Console.WriteLine((i + 1).ToString() + ". " + config.directories[i]);
        }

        public static void Add(string path)
        {
            string normalizedPath = UppercaseFirst(Path.GetFullPath(path));
            bool isFound = false;

            if (path.Length > 3)
                if (Directory.Exists(path))
                    if (normalizedPath.Length > path.Length)
                        Console.WriteLine("There is no such a directory.");
                    else
                    {
                        //Check if there is already directory with this path
                        for (int i = 0; i < config.directories.Count; i++)
                        {
                            if (config.directories[i].ToLower() == normalizedPath.ToLower())
                            {
                                isFound = true;
                                Console.WriteLine("You have already added this directory to the list.");
                                break;
                            }
                        }

                        //If not add it 
                        if (!isFound)
                        {
                            config.directories.Add(normalizedPath);
                            Console.WriteLine("Directory has been successfully added to the list. ");
                        }
                    }
                else
                    Console.WriteLine("There is no such a directory.");
            else
                Console.WriteLine("You can't add partition.");
        }

        public static void Delete(string id)
        {
            try
            {
                config.directories.RemoveAt(int.Parse(id) - 1);
                Console.WriteLine("Directory has been successfuly removed from the list.");
            }
            catch
            {
                Console.WriteLine("You chose wrong id. Try again.");
            }
        }

        public static void Edit(string id, string newPath)
        {
            string normalizedPath = UppercaseFirst(Path.GetFullPath(newPath));
            bool isFound = false;

            if (config.directories.Count > 0)
                if (Directory.Exists(newPath))
                    if (normalizedPath.Length > newPath.Length)
                        Console.WriteLine("There is no such a directory.");
                    else
                    {
                        //Check if there is already directory with this path
                        for (int i = 0; i < config.directories.Count; i++)
                        {
                            if (config.directories[i].ToLower() == normalizedPath.ToLower())
                            {
                                isFound = true;
                                Console.WriteLine("You have already added this directory to the list.");
                                break;
                            }
                        }

                        //If not edit it
                        if (!isFound)
                        {
                            config.directories[int.Parse(id) - 1] = normalizedPath;
                            Console.WriteLine("Directory has been successfully edited.");
                        }
                    }
                else
                    Console.WriteLine("There is no such a directory.");
            else
                Console.WriteLine("There are no directories set.");
        }

        public static void Name(string name)
        {
            config.name = name;
            Console.WriteLine("Name has been set successfully.");
        }

        public static void SetPassword()
        {
            string password = ReadPassword();
            if (password.Length < 6 || password == "")
            {
                Console.WriteLine("Password is too short. Try again.");
                SetPassword();
            }
            else
            {
                Console.WriteLine('\n' + "Password has been set successfully.");
                config.password = password;
            }
        }

        public static void DeletePassword()
        {
            config.password = "";
            Console.WriteLine("Password has been removed successfully.");
        }

        public static void SetPath(string path)
        {
            string normalizedPath = UppercaseFirst(Path.GetFullPath(path));

            if (Directory.Exists(path))
                if (normalizedPath.Length > path.Length)
                    Console.WriteLine("There is no such a directory.");
                else
                {
                    config.backupPath = normalizedPath;
                    Console.WriteLine("Path has been added.");
                }    
        }

        public static void ShowPath()
        {
            if (config.backupPath != "")
                Console.WriteLine(config.backupPath);
            else
                Console.WriteLine("Path have not been set yet.");
        }

        public static void SetTime()
        {
            config.setTime = config.setTime ? false : true;
            string status = config.setTime ? "Time on name has been enabled." : "Time on name has been disabled.";
            Console.WriteLine(status);
        }

        public static void SetDate()
        {
            config.setDate = config.setDate ? false : true;
            string status = config.setDate ? "Date on name has been enabled." : "Date on name has been disabled.";
            Console.WriteLine(status);
        }

        public static void ServerOn()
        {
            config.serverOn = true;
            Console.WriteLine("Sending backups on the server is on.");
        }

        public static void ServerOff()
        {
            config.serverOn = false;
            Console.WriteLine("Sending backups on the server is off.");
        }

        public static void SetServerIp(string ip)
        {
            config.serverIp = ip;
            Console.WriteLine("Server IP has been assigned.");
        }

        public static void SetServerPath(string path)
        {
            config.serverPath = path;
            Console.WriteLine("Server path has been set.");
        }

        public static void SetServerUser(string user)
        {
            config.serverUser = user;
            Console.WriteLine("Server user has been set.");
        }

        public static void SetServerPassword()
        {
            string password = ReadPassword();
            Console.WriteLine('\n' + "Server password has been set successfully.");
            config.serverPassword = password;
        }

        public static void ShowServerIp()
        {
            string status = config.serverIp == "" || config.serverIp == null ? "You haven't assigned server ip yet." : "Your server ip is: " + config.serverIp;
            Console.WriteLine(status);
        }

        public static void ShowConfig()
        {
            Console.Clear();
            Console.WriteLine("---------------Paths----------------");
            Console.WriteLine("Directories to backup: ");
            foreach (string path in config.directories)
                Console.WriteLine("-" + path);
            Console.WriteLine("----------Final zip config----------");
            Console.WriteLine("Name of the copy: " + config.name);
            Console.WriteLine(config.password == "" ? "Password of the final zip file: no" : "Password of the final zip file: yes");
            Console.WriteLine("Path of the final zip file: " + config.backupPath);
            Console.WriteLine(config.setTime ? "Time in name of the zip file: yes" : "Time in name of the zip file: no");
            Console.WriteLine(config.setDate ? "Date in name of the zip file: yes" : "Date in name of the zip file: no");
            Console.WriteLine("------------Server config-----------");
            Console.WriteLine(config.serverOn ? "Sending backup to the server: yes" : "Sending backup to the server: no");
            Console.WriteLine("Server ip: " + config.serverIp);
            Console.WriteLine("Server user: " + config.serverUser);
            Console.WriteLine("Server path: " + config.serverPath);
            Console.WriteLine(config.serverPassword == "" ? "Server password: no" : "Server password: yes");
        }

        public static async void Save()
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            await File.WriteAllTextAsync(Directory.GetCurrentDirectory() + "/config.json", json);
            Console.WriteLine("Config has been saved.");
        }

        public static async void Load()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
            {
                string json = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/config.json");
                config = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public static void IsGood()
        {
            bool isGood = true;

            if (config.backupPath == "")
            {
                isGood = false;
                Console.WriteLine("You need to set backup path. Try 'help'.");
            }

            if (config.directories.Count <= 0)
            {
                isGood = false;
                Console.WriteLine("There are no directories to backup. Try 'help'.");
            }

            if (config.serverOn)
            {
                if (config.serverIp == "")
                {
                    isGood = false;
                    Console.WriteLine("Sending files to the server is on but there is no ip set. Try 'help'.");
                }

                if (config.serverUser == "")
                {
                    isGood = false;
                    Console.WriteLine("Sending files to the server is on but there is no user set. Try 'help'.");
                }
                if (config.serverPassword == "")
                {
                    isGood = false;
                    Console.WriteLine("Sending files to the server is on but there is no password for it. Try 'help'.");
                }

                if (config.serverPath == "")
                {
                    isGood = false;
                    Console.WriteLine("Sending files to the server is on but there is no path where should backup be saved. Try 'help'.");
                }
            }

            if (isGood)
                Console.WriteLine("Everything is good and program is ready to do backups. If you haven't done this yet type 'on'.");
        }

        public static void On()
        {
            string batFile = @"C:\WINDOWS\System32\GroupPolicy\Machine\Scripts\Shutdown\startCopy.bat";
            string iniFile = @"C:\WINDOWS\System32\GroupPolicy\Machine\Scripts\scripts.ini";
            string path = Directory.GetCurrentDirectory();
            bool isAlreadyOn = false;

            string[] text =
            {
                "@echo off",
                path + @"\DoCopy.exe -copy",
            };
            File.WriteAllLines(batFile, text);

            string[] scriptsLines = File.ReadAllLines(iniFile);

            int numberOfScriptInFile = 0;
            if(int.TryParse(scriptsLines[scriptsLines.Length - 1][0].ToString(), out _))
                numberOfScriptInFile = int.Parse(scriptsLines[scriptsLines.Length - 1][0].ToString()) + 1;

            //Check if starting our script is on
            foreach (string line in scriptsLines)
                if (line.Contains("startCopy.bat"))
                    isAlreadyOn = true;

            if(!isAlreadyOn)
            { 
                WriteIniFile("Shutdown", numberOfScriptInFile + "CmdLine", "startCopy.bat", iniFile);
                WriteIniFile("Shutdown", numberOfScriptInFile + "Parameters", "", iniFile);
                Console.WriteLine("Making backups is on.");
            }
            else
                Console.WriteLine("Making backups is already on.");
        }

        public static void Off()
        {
            string batFile = @"C:\WINDOWS\System32\GroupPolicy\Machine\Scripts\Shutdown\startCopy.bat";
            string iniFile = @"C:\WINDOWS\System32\GroupPolicy\Machine\Scripts\scripts.ini";

            string[] scriptsLines = File.ReadAllLines(iniFile);
            int numberOfOurSciptLine = 0;
            bool isOn = false;

            if (File.Exists(batFile))
                File.Delete(batFile);

            //Check if starting our script is on
            foreach (string line in scriptsLines)
            {
                if (line.Contains("startCopy.bat"))
                {
                    numberOfOurSciptLine = int.Parse(line[0].ToString());
                    isOn = true;
                }
            }

            if (isOn)
            {
                WriteIniFile("Shutdown", numberOfOurSciptLine + "CmdLine", null, iniFile);
                WriteIniFile("Shutdown", numberOfOurSciptLine + "Parameters", null, iniFile);
                Console.WriteLine("Making backup is off.");
            }
            else
                Console.WriteLine("Making backup is already off.");
        }

        private static void WriteIniFile(string section, string key, string value, string iniFile)
        {
            WritePrivateProfileString(section, key, value, iniFile);
        }

        private static string UppercaseFirst(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        public static string ReadPassword()
        {
            Console.Write("Password typing mode>");
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            return password;
        }
    }
}