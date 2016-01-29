using System;
using System.IO;
using System.Threading.Tasks;
using HFAPI;

namespace HFAPI_Console
{
    class Program
    {
        const string listreporteduser = @"C:\Users\User\Source\HFAPI\HFAPI_Console\bin\Debug\data\user_report.txt";

        static void Main(string[] args)
        {

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("-----------------------Hack Free Bot--------------------------");
            Console.WriteLine("-------------------------Modo 2.0-----------------------------" + Environment.NewLine);

            User.Connect("Prometheus", "");
        Label_01:
            Console.WriteLine("1. Vérifier les «Merci du partage»");
            Console.WriteLine("2. Bruteforce les profiles à la recherhe de «Merci du partage»");
            Console.WriteLine("3. Parier contre un utilisateur");
            Console.WriteLine("4. Parier contre le système");
            Console.WriteLine("5. Exit");

            var choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    Log(logtype.info, "Début de la tâche de vérification !");
                var newreplies = User.LastReplyByUsers();
                foreach (var user in newreplies)
                {
                    choice1(user);
                }
                Log(logtype.info, "Fin de la tâche de vérification !");
                Console.ReadKey();       
                break;




                case "2":
                Console.WriteLine("[¤] Début de la tâche de vérification !");
                for (int i = 4; i < 4410; i++)
                {
                    var urluser = "http://hack-free.net/User-" + i;
                    var user = User.ID2Nickname(i.ToString());
                    if (user != null)
                    {
                        if (User.IsUserAMerci(i.ToString()))
                        {
                            if (!IsAlready(user, listreporteduser))
                            {
                                User.ReportProfile(i.ToString(),
                                    "Merci du partage : http://hack-free.net/search.php?action=finduser&uid=" + i);
                                Console.WriteLine("[!] {" + user + "} vient d'être reporté pour Merci du Partage");
                                using (StreamWriter w = File.AppendText(listreporteduser))
                                {
                                    Log(user, w);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[*] " + user + " est clean.");
                        }
                    }
                    
                   
                }
                Console.WriteLine("[¤] Fin de la tâche de vérification !");
                Console.ReadKey();
                break;




                case "3":
                Console.WriteLine("[¤] Début de la séssion de pari !");
                var username = Console.ReadLine();
                Console.WriteLine("[!] Vous avez décidé de parier contre " + username);
                Console.WriteLine("[? Combie voulez-vous parier ?");
                var somme = Console.ReadLine();
                Console.WriteLine("[?] Êtes vous sûr de vouloir parier " + somme + " crédits contre " + username +" ? [Y/N]");
                var sûr = Console.ReadLine();
                if (sûr.Equals("Y"))
                {
                    User.Bet(username, somme, User.BetType.Public);
                    Console.WriteLine("[!] Vous avez pariez " + somme + " crédits contre " + username);
                    Console.WriteLine("[¤] Fin de la séssion de pari !");
                    break;
                }
                else if (sûr.Equals("N"))
                {
                    Console.WriteLine("[¤] Fin de la séssion de pari !");
                    break;
                }
                break;
                case "4":
                Console.WriteLine("[¤] Début de la séssion de pari !");
                Console.WriteLine("[!] Vous avez décidé de parier contre le système");
                Console.WriteLine("[? Combie voulez-vous parier ?");
                var somme2 = Console.ReadLine();
                Console.WriteLine("[?] Êtes vous sûr de vouloir parier " + somme2 + " crédits ? [Y/N]");
                var sûr2 = Console.ReadLine();
                if (sûr2.Equals("Y") || sûr2.Equals("y"))
                {
                    Console.WriteLine("[!] Vous avez pariez " + somme2 + " crédits contre le système");
                    var win = User.Bet(somme2);
                    if (win)
                    {
                        Console.WriteLine("[¤] Pari gagné !");
                    }
                    else
                    {
                        Console.WriteLine("[¤] Pari perdu !");
                    }
                    Console.WriteLine("[¤] Fin de la séssion de pari !");
                    break;
                }
                else if (sûr2.Equals("N") || sûr2.Equals("n"))
                {
                    Console.WriteLine("[¤] Fin de la séssion de pari !");
                    break;
                }
                break;
                case "5":
                //Environment.Exit(0);
                    var xeno = User.GetUserMessageCount("wako94");
                break;
            }
            Console.ReadKey();
            goto Label_01;
        }

        static void choice1(string user)
        {
            if (!user.Contains("<del>"))
            {
                var membre = User.Nickname2ID(user);
                if (User.IsUserAMerci(membre))
                {
                    var x = User.GetMemberinfo(user);
                    int memberpost = Convert.ToInt32(x[1]);
                    if (memberpost < 15)
                    {
                        User.PrivateMessage(user, "[Avertissement] Vos messages", "Notre systeme automatique vous averti de ne plus répondre par 'merci du partage' sous peine de bannissement.", true, true, true);

                        Log(logtype.warning, "{" + user + "} vient d'être averti par message privé");
                    }
                    else
                    {
                        if (!IsAlready(user, listreporteduser))
                        {
                            User.ReportProfile(membre,
                                "Merci du partage : http://hack-free.net/search.php?action=finduser&uid=" + membre);
                            User.PrivateMessage(user, "[Avertissement] Vos messages", "Vous avez été reporté par notre bot à cause de vos abus de 'merci du partage'. Merci de ne plus recommencer, sinon votre compte sera bannis. Si vous voulez répondre à un message, merci de bien vouloir y répondre convenablement, et éditer vos messages pour laisser un retour positif ou negatif sur le sujet auquel vous avez répondu.", true, true, true);
                            Log(logtype.danger, "{" + user + "} vient d'être reporté pour Merci du Partage");
                            using (StreamWriter w = File.AppendText(listreporteduser))
                            {
                                Log(user, w);
                            }
                        }
                        else
                        {
                            Log(logtype.info, "{" + user + "} a déjà été reporté pour Merci du Partage");
                        }
                    }

                }
                else
                {
                    Log(logtype.success, "{" + user + "} est clean.");
                }
            }
        }

        static bool IsAlready(string arg, string source)
        {
            using (StreamReader sr = new StreamReader(source))
            {
                string contents = sr.ReadToEnd();
                if (contents.Contains(arg))
                {
                    return true;
                }
            }
            return false;
        }

        public static void WriteToFile(string Str, string Filename)
        {
            File.WriteAllText(Filename, Str);
            return;
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write(logMessage + Environment.NewLine);
        }

        static void Log(logtype log, string message)
        {
            switch (log)
            {
                case logtype.danger:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[X]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                break;
                case logtype.info:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[|]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                break;
                case logtype.success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[V]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                break;
                case logtype.warning:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("[!]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                break;
            }
        }

        enum logtype
        {
            info,
            warning,
            success, 
            danger, 

        }

    }
}
