using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ionic.Zip;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace DoCopy           
{
    public static class Copy
    {
        private static string stringJson = File.ReadAllText(AppContext.BaseDirectory + "config.json");
        private static dynamic json = JsonConvert.DeserializeObject(stringJson);

        public static void DoCopy()
        {
            //Prepare data
            List<string> directories = json.directories.ToObject<List<string>>();
            string nameOfBackup = json.name;
            string pathOfBackup = json.backupPath;
            string date = DateTime.Now.ToShortDateString();
            string time = DateTime.Now.ToShortTimeString();
            bool setDate = (bool)json.setDate;
            bool setTime = (bool)json.setTime;

            //Short validation of the file
            if (json == null || json.backupPath == "" || json.backupPath == null || json.name == "" || json.name == null || json.directories == null)
                File.WriteAllText(AppContext.BaseDirectory + "error_" + date + "-" + time.Split(":")[0] + "." + time.Split(":")[1] + ".txt", "Json file is broken. Delete it and try to configure program again.");
            else
            {
                nameOfBackup += setDate ? "_" + date : "";
                nameOfBackup += setTime ? "_" + time.Split(":")[0] + "." + time.Split(":")[1] : "";
                nameOfBackup += ".zip";
                MakeBackup(directories, pathOfBackup, nameOfBackup);
            }
        }

        //Makebackup with sending file to the server
        private static void MakeBackup(List<string> directories, string zipDir, string zipName)
        {
            string zipFileName = zipDir + @"\" + zipName;

            if (File.Exists(zipFileName))
                File.Delete(zipFileName);

            using (ZipFile zip = new ZipFile(zipFileName))
            {
                if((string)json.password != "")
                    zip.Password = json.password;

                foreach (string path in directories)
                    zip.AddDirectory(path, Path.GetFileName(path));
                
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                zip.Save();               
                try
                {
                    if((bool)json.serverOn)
                    {
                        string serverIp = json.serverIp;
                        string serverUser = json.serverUser;
                        string serverPassword = json.serverPassword;

                        using (SftpClient client = new SftpClient(serverIp, serverUser, serverPassword))
                        {
                            client.Connect();
                            client.ChangeDirectory((string)json.serverPath);
                            using (FileStream fs = new FileStream(zipFileName, FileMode.Open))
                            {
                                client.BufferSize = 4 * 1024;
                                client.UploadFile(fs, Path.GetFileName(zipFileName));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    File.WriteAllText(AppContext.BaseDirectory + "/error.log", e.ToString());
                }
            }
        }
    }
}