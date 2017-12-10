using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace RandomSteamGame
{
    class Program
    {
        private static List<Game> GameConfigs = new List<Game>();

        /// <summary>
        /// Loads a Config XML to setup special game configs for those fuckers with lots of exe's
        /// </summary>
        private static void SetupGameConfigs() {
            XmlSerializer deserializer = new XmlSerializer(typeof(GameConfigs));
            TextReader reader = new StreamReader("GameConfigs.xml");
            GameConfigs Configs = (GameConfigs)deserializer.Deserialize(reader);
            reader.Close();

            GameConfigs = Configs.Games;

            Console.WriteLine("Found {0} Game Config(s)", GameConfigs.Count);
        }

        private static String PickGame(String SteamPath) {
            Console.WriteLine("Looking In: {0}", SteamPath + "/steamapps/common");

            var SubDirs = Directory.GetDirectories(SteamPath + "/steamapps/common");

            Console.WriteLine("Found: {0}", SubDirs.Length);

            foreach (var gameDirs in SubDirs)
            {
                Console.WriteLine("Found: {0}", gameDirs);
            }

            Random rand = new Random();
            int game = rand.Next(0, SubDirs.Length);

            return SubDirs[game];
        }

        private static Game IsSpecialGame(String name) {
            foreach(Game game in GameConfigs)
            {
                if (game.Name == name) return game;
            }

            return null;
        }

        private static void StartGame(String GameName, String FullGamePath, String SteamPath) {
            FullGamePath = FullGamePath.Replace(@"\", "/");

            String TrimmedPath = FullGamePath.Replace(SteamPath, "");

            Console.WriteLine(TrimmedPath);

            Game GameConfig = IsSpecialGame(GameName);

            String GameExeLoc = "";

            if (GameConfig != null) {
                Console.WriteLine("Special Game Config Needed");
                GameExeLoc = FullGamePath + "/" + GameConfig.Exe;
                Console.WriteLine("EXE {0}", GameExeLoc);
            } else {
                Console.WriteLine("No Special Game Config Needed");
                string[] GameExes = Directory.GetFiles(FullGamePath, "*.exe");

                if (GameExes.Length != 0) {
                    GameExeLoc = GameExes[0];
                } else {
                    Console.WriteLine("Unable To Find Game Exe, Perhaps it needs a special game config OR the folder might be empty");

                    Console.ReadKey();
                }
            }

            if (GameExeLoc != "") {
                ProcessStartInfo game = new ProcessStartInfo();
                game.FileName = GameExeLoc;

                Process.Start(game);
            }
        }

        static void Main(string[] args) {
            Console.WriteLine("Setting Up Game Configs");

            SetupGameConfigs();

            Console.WriteLine("Looking For Steam Path");

            string SteamPath = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "Unable To Find Steam Path");

            if (SteamPath != "Unable To Find Steam Path") {
                Console.WriteLine("Found Steam Path: {0}", SteamPath);

                String GamePath = PickGame(SteamPath);

                int NamePos = GamePath.LastIndexOf(@"\") + 1;

                String GameName = GamePath.Substring(NamePos, GamePath.Length - NamePos);

                Console.WriteLine("\nChose: {0}\n", GameName);

                Console.WriteLine("Confirm You Wish To Play This Game? Y/N");

                ConsoleKeyInfo choice = Console.ReadKey();

                if (Char.ToUpper(choice.KeyChar) == 'Y') {
                    Console.WriteLine();

                    StartGame(GameName, GamePath, SteamPath);
                }
            } else {
                Console.WriteLine("Unable To Find Steam Path");

                Console.ReadKey();
            }
        }
    }
}
