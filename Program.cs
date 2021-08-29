using System;

namespace DoCopy
{
    class Program
    {       
        static void Main(string[] args)
        {
            Console.Title = "DoCopy!";
            if (args.Length != 0 && args[0] == "-copy")
                Copy.DoCopy();
            else
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(SaveOnExit);
                Menu.Load();
                Console.WriteLine("Welcome to DoCopy! Write 'help' to show help menu.");
                ReadCommand();
            }
        }

        private static void ReadCommand()
        {
            Console.Write('>');
            string input = Console.ReadLine();
            ParseCommand(input);
        }

        private static void ParseCommand(string input)
        {
            string[] command = input.Split(' ');
            try
            {
                switch (command[0])
                {
                    case "help":
                        Menu.Help();
                        break;

                    case "show":
                        Menu.Show();
                        break;

                    case "add":
                        Menu.Add(command[1]);
                        break;

                    case "delete":
                        Menu.Delete(command[1]);
                        break;

                    case "edit":
                        Menu.Edit(command[1], command[2]);
                        break;

                    case "name":
                        if(command[1] != "")
                            Menu.Name(command[1]);
                        else
                            Console.WriteLine("Wrong arguments. Try 'help'");
                        break;

                    case "setPassword":
                        Menu.SetPassword();
                        break;

                    case "deletePassword":
                        Menu.DeletePassword();
                        break;

                    case "setPath":
                        Menu.SetPath(command[1]);
                        break;

                    case "showPath":
                        Menu.ShowPath();
                        break;

                    case "setTime":
                        Menu.SetTime();
                        break;

                    case "setDate":
                        Menu.SetDate();
                        break;

                    case "serverOn":
                        Menu.ServerOn();
                        break;

                    case "serverOff":
                        Menu.ServerOff();
                        break;

                    case "setServerIp":
                        if(command[1] != "")
                            Menu.SetServerIp(command[1]);
                        else
                            Console.WriteLine("Wrong arguments. Try 'help'");
                        break;

                    case "setServerPath":
                        Menu.SetServerPath(command[1]);
                        break;

                    case "setServerUser":
                        if(command[1] != "")
                            Menu.SetServerUser(command[1]);
                        else
                            Console.WriteLine("Wrong arguments. Try 'help'");
                        break;

                    case "setServerPassword":
                        Menu.SetServerPassword();
                        break;

                    case "showServerIp":
                        Menu.ShowServerIp();
                        break;

                    case "save":
                        Menu.Save();
                        break;

                    case "showConfig":
                        Menu.ShowConfig();
                        break;

                    case "isGood":
                        Menu.IsGood();
                        break;

                    case "on":
                        Menu.On();
                        break;

                    case "off":
                        Menu.Off();
                        break;

                    default:
                        Console.WriteLine("There is no such a command. Try 'help'.");
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Wrong arguments. Try 'help'");
            }

            ReadCommand();
        }

        private static void SaveOnExit(object sender, EventArgs e)
        {
            Menu.Save();
        }
    }
}