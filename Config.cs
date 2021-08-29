using System;
using System.Collections.Generic;

namespace DoCopy
{
    class Config
    {
        public string name = "Copy";
        public string password = ""; //ZIP file password
        public string backupPath = ""; //Local path for backup
        public bool setDate;
        public bool setTime;
        public List<string> directories = new List<string>();
        //Server stuff
        public bool serverOn = false;
        public string serverIp = "";
        public string serverUser = ""; //SFTP username
        public string serverPassword = ""; //SFTP password
        public string serverPath = ""; //SFTP path for backup
    }
}
