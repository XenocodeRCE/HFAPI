using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HFAPI
{

    public class User
    {
        #region Sared var

        /// <summary>
        /// L'instance initiale du Forum
        /// </summary>
        public static Uri ForumUri { get; set; }

        /// <summary>
        /// Sert à pointer vers la page de Login
        /// </summary>
        public static string LoginPage { get; set; }

        /// <summary>
        /// La PostKey de MyBB
        /// </summary>
        public static string MyPostKey { get; set; }

        /// <summary>
        /// Le Code Source de la page principale (index)
        /// </summary>
        public static string Forum { get; set; }

        /// <summary>
        /// Le code source de l'UserCP du membre
        /// </summary>
        public static string Userpanel { get; set; }

        /// <summary>
        /// Le Webclient qui permet de garder les Cookies en mémoires
        /// </summary>
        public static CookieClient MyWebClient { get; set; }

        /// <summary>
        /// Le String.Concat du pauvre, pour valider l'instance principale du Forum
        /// </summary>
        public static string CompleteUrl => new Uri(ForumUri, LoginPage).ToString();

        #endregion

        #region User var

        /// <summary>
        /// L'ID qui sert à identifier le membre sur le forum
        /// </summary>
        public static int UserID { get; set; }

        /// <summary>
        /// Le nom d'utilisateur du membre
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// La PostKey de MyBB, utile pour toutes les autres requettes
        /// </summary>
        public static string PostKey { get; set; }
        
        /// <summary>
        /// Le nombre total de posts (messages + sujets)
        /// </summary>
        public static int PostCount { get; set; }

        /// <summary>
        /// Le ratio actuel du membre
        /// </summary>
        public static string Ratio { get; set; }

        /// <summary>
        /// Le nombre total de "Merçi" reçu
        /// </summary>
        public static string Merci { get; set; }

        /// <summary>
        /// Le nombre de réputation positive ou négative
        /// </summary>
        public static string Reputation { get; set; }

        /// <summary>
        /// La date d'inscription au forum
        /// </summary>
        public static string InscriptionDate { get; set; }

        /// <summary>
        /// Le nombre de fois que le membre à changé de pseudo
        /// <remark>Le nombre total de pseudos est donc égale à UsernameChange + 1</remark>
        /// </summary>
        public static string UsernameChange { get; set; }

        /// <summary>
        /// Le titre / nom du groupe d'affichage séléctionné par le membre 
        /// </summary>
        public static string Groupe { get; set; }

        /// <summary>
        /// La liste des groupes dont le membre est membre
        /// </summary>
        public static List<String> Groupes { get; set; }

        /// <summary>
        /// Le nombre de membre qui se sont inscrits en méttant comme Parrain le membre
        /// </summary>
        public static int ReferencedMembers { get; set; }

        /// <summary>
        /// Le nombre de crédit dont dispose le membre
        /// </summary>
        public static string Credit { get; set; }

        /// <summary>
        /// L'adresse URL pointant vers l'Avatar actuel du membre
        /// </summary>
        public static string AvatarURL { get; set; }

        /// <summary>
        /// Le nombre de message / poste / sujets reportés par le membre
        /// </summary>
        public static string ReportedMessage { get; set; }

        /// <summary>
        /// Le taux d'avertissement du membre
        /// <remark>La valeur retournée est une string du nombre sans le pourcentage</remark>
        /// </summary>
        public static string Avertissement { get; set; }

        #endregion

        /// <summary>
        /// Permet de se connecter au forum
        /// </summary>
        /// <param name="username">Votre nom d'utilisateur.</param>
        /// <param name="password">Votre mot de passe.</param>
        public static void Connect(string username, string password)
        {
            try
            {
                ForumUri = new Uri("http://hack-free.net/");
                LoginPage = "member.php";
                MyWebClient = new CookieClient {BaseAddress = ForumUri.ToString()};
                var request = new NameValueCollection
                {
                    {"action", "do_login"},
                    {"url", CompleteUrl},
                    {"quick_login", "1"},
                    {"quick_username", username}, //Prometheus
                    {"quick_password", password}, //
                    {"submit", "Login"},
                    {"quick_remember", "no"}
                };
                var connection = MyWebClient.UploadValues(CompleteUrl, request);
                var myStream = new MemoryStream(connection);
                var myReader = new StreamReader(myStream);
                var xeno = myReader.ReadToEnd();
                Forum = xeno;
                Username = username;
                if (myReader.ReadToEnd().Contains("Corrigez les erreurs suivantes avant de continuer :"))
                {
                    throw new Exception("Error : Cannot connect to the forum : Invalid Login or Password");
                }
                GetData();
            }
            catch (Exception e)
            {

                throw new Exception("Error : Cannot connect to the forum : " + e.ToString());
            }
        }

        /// <summary>
        /// Permet de parier une somme de crédit X contre le système
        /// </summary>
        /// <param name="montant">En int, minimum 50 maximum 10,000</param>
        /// <remark>Renvoie True quand le pari est gagné, False quand ce dernier est perdu</remark>
        public static bool Bet(string montant)
        {
            var betpoints = montant.ToString();

            var response = MyWebClient.UploadValues("http://hack-free.net/newpoints.php", new NameValueCollection()
           {
               { "my_post_key", PostKey },
               { "action", "betting"},
               { "type", "system" },
               { "username", "" },
               { "points", betpoints },
               { "submit", "Affronter" }
           });

            var result = System.Text.Encoding.UTF8.GetString(response);
            //<div><strong>Vous avez un message privé non lu</strong> de <a href="http://hack-free.net/User-84">Prometheus</a> intitulé <a href="private.php?action=read&amp;pmid=13736" style="font-weight: bold;">System bet results</a></div>
            var xeno = 2;
            var src1 = "intitulé <a href=";
            var src2 = " style=";
            var entries2 = Helpers.GetStringInBetween(src1, src2,
                result, false, false);
            entries2 = entries2.Trim('"');
            entries2 = entries2.Replace(">", "");
            entries2 = entries2.Trim('"');
            entries2 = entries2.Replace("amp;", "");
            var readPm = MyWebClient.DownloadString("http://hack-free.net/" + entries2);

            return !readPm.Contains("perdu");
        }

        /// <summary>
        /// Permet d'envoyer un message dans la shoutbox
        /// </summary>
        /// <param name="message">Le message à envoyer</param>
        public static void SendShout(string message)
        {
           MyWebClient.Headers.Add("Referer", "http://hack-free.net/");
           var response = MyWebClient.UploadValues("http://hack-free.net/infernoshout.php?action=newshout", new NameValueCollection()
           {
                { "shout", message},
               { "my_post_key", PostKey }

           });
            var result = System.Text.Encoding.UTF8.GetString(response);
        }

        /// <summary>
        /// Permet de reporter un profile
        /// </summary>
        /// <param name="memberID">l'UserID de la personne à reporter</param>
        /// <param name="reason">La raison du report</param>
        public static void ReportProfile(string memberID, string reason)
        {
            var response = MyWebClient.UploadValues("http://hack-free.net/misc.php", new NameValueCollection()
           {
               { "reason", reason },
               { "submit", "Envoyer mon rapport"},
               { "action", "do_profile_report" },
               { "uid", memberID },
               { "do_uid", UserID.ToString() }
           });

            var result = System.Text.Encoding.UTF8.GetString(response);
        }


        /// <summary>
        /// Permet de récupérer certaines informations d'un membre
        /// </summary>
        /// <param name="pseudo">le pseudonyme du membre</param>
        /// <returns>Ratio, Merçi, Reputation, Message report, Credit, Groupe d'affichage</returns>
        public static List<string> GetMemberinfo(string pseudo)
        {
            try
            {
                var infos = new List<string>();
                var ID = Nickname2ID(pseudo);
                var panel = MyWebClient.DownloadString("http://hack-free.net/User-" + ID);
                Userpanel = panel;
                var ratio = GetRatio();
                var msg = Getpostcount();
                var merci = GetMerci();
                var rep = GetReputation();
                var reportedmessage = GetReportedMessage();
                var credit = GetCredit();
                var maingroupe = GetGroupe();
                infos.Add(ratio);
                infos.Add(msg.ToString());
                infos.Add(merci);
                infos.Add(rep);
                infos.Add(reportedmessage);
                infos.Add(credit);
                infos.Add(maingroupe);
                return infos;
            }
            catch (Exception)
            {

                return null;
            }
           
        }


        /// <summary>
        /// Permet de parier une somme de crédit X contre un utilisateur
        /// </summary>
        /// <param name="username">L'Username de la personne contre laquelle vous pariez</param>
        /// <param name="montant">En int, minimum 50 maximum 10,000</param>
        /// <param name="bet">Choisir le <type cref="BetType"/> de partie : Public, Private, System</param>
        /// <remark>Ne permet pas de savoir si on gagne ou on perd !</remark>
        public static void Bet(string username, string montant, BetType bet)
        {
            var betpoints = montant.ToString();

            var response = MyWebClient.UploadValues("http://hack-free.net/newpoints.php", new NameValueCollection()
           {
               { "my_post_key", PostKey },
               { "action", "betting"},
               { "type", bet.ToString() },
               { "username", username },
               { "points", betpoints },
               { "submit", "Affronter" }
           });

            var result = System.Text.Encoding.UTF8.GetString(response);
        }

        /// <summary>
        /// Permet d'envoyer un message privé à un membre
        /// </summary>
        /// <param name="to">Membre à qui envoyer le message privé</param>
        /// <param name="sujet">Sujet du message privé</param>
        /// <param name="message">Corps, message à envoyer</param>
        /// <param name="signature">inclure ou non votre signature</param>
        /// <param name="savecopie">sauvegarder ou non une copie dans le dossier "Envoyé"</param>
        /// <param name="accuséreception">Recevoir ou non un accusé de reception</param>
        public static void PrivateMessage(string to, string sujet, string message, bool signature, bool savecopie, bool accuséreception)
        {
            var sig = signature ? 1 : 0;
            var savecopy = savecopie ? 1 : 0;
            var readreceipt = accuséreception ? 1 : 0;

            var response = MyWebClient.UploadValues("http://hack-free.net/private.php", new NameValueCollection()
           {        
               { "my_post_key", PostKey },
               { "to", to + ", "},
               { "bcc", "" },
               { "subject", sujet },
               { "message_new", message },
               { "message", message },
               { "options[signature]", sig.ToString() },
               { "options[savecopy]", savecopy.ToString() },
               { "options[readreceipt]", readreceipt.ToString() },
               { "action", "do_send" },
               { "pmid", "" },
               { "do", "" },
               { "submit", "Envoyer le message" },
           });

            var result = System.Text.Encoding.UTF8.GetString(response);
        }

        /// <summary>
        /// Les types de parties
        /// </summary>
        public enum BetType
        {
            Public,
            Private,
            System
        }

        /// <summary>
        /// Permet de récupérer le pseudonyme à partir de l'UserID
        /// </summary>
        /// <returns></returns>
        public static string ID2Nickname(string memberID)
        {
            try
            {
                string nickname;
                var profile = MyWebClient.DownloadString("http://hack-free.net/User-" + memberID);
                //http://hack-free.net/User-1094
                //<span class="active">Profil de `Kysura~_^</span>
                var src1 = "<span class=\"active\">Profil de ";
                var src2 = "</span>";
                var entries2 = Helpers.GetStringInBetween(src1, src2,
                    profile, false, false);
                nickname = entries2;
                return nickname;
            }
            catch (Exception)
            {

                return null; // 404 -> profile delet
            }
            
        }

        /// <summary>
        /// Permet de récupérer l'userID à partir d'un pseudo
        /// </summary>
        /// <param name="pseudo"></param>
        /// <returns></returns>
        public static string Nickname2ID(string pseudo)
        {
            string ID = "";

            var response = MyWebClient.UploadValues("http://hack-free.net/memberlist.php", new NameValueCollection()
           {
               { "username", pseudo },
               { "website", ""},
               { "sort", "lastvisit" },
               { "order", "ascending" },
               { "submit", "Rechercher" }
           });
            var result = System.Text.Encoding.UTF8.GetString(response);
            //<td class="trow1"><a href="http://hack-free.net/User-84">Prometheus</a><br/>
            var src1 = "<td class=\"trow1\"><a href=\"http://hack-free.net/User-";
            var src2 = "><span";
            var entries2 = Helpers.GetStringInBetween(src1, src2,
                result, false, false);
            //84\"
            entries2 = entries2.Trim('"');
            entries2 = entries2.Replace(@"\", "");

            ID = entries2;
            return ID;
        }

        /// <summary>
        /// Permet de récupérer la liste des utilisateurs ayant répondu dernièrement aux sujet
        /// </summary>
        /// <returns>Un array de string comprenant tous les utilisateurs</returns>
        public static List<string> LastReplyByUsers()
        {
            List<string> xx = new List<string>();
            var entries =
                Helpers.GetStringInBetween("<td>Sujet</td><td>Heure&nbsp;</td><td>Auteur</td><td>Derniers</td><td>Forum</td>",
                    "</table></td>", Forum, false, false);
            var x = 0;
            while (x < 15)
            {
                entries = entries.Replace("<tr class=\"trow1 smalltext\" style=\"\">", null);
                entries = entries.Replace("<td>", null);
                entries = entries.Replace(
                    "<img src=\"/images/prostats/ps_minion.gif\" style=\"vertical-align:middle;\" alt=\"\"/>&nbsp;",
                    null);
                entries = entries.Replace(
                    "<img src=\"/images/prostats/ps_minioff.gif\" style=\"vertical-align:middle;\" alt=\"\"/>&nbsp;",
                    null);
                entries = entries.Replace("</tr>", null);
                x++;
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "HFB_users2.txt",entries);
            var file_users = AppDomain.CurrentDomain.BaseDirectory + @"\" + "HFB_users2.txt";
            var sr = new StreamReader(file_users).ReadToEnd();
            var user12delet = Helpers.GetStringInBetween("<strong>", "</strong>", sr, true, true);
            sr.Replace(sr, user12delet);
            //
            var user1 = GetLine(file_users, 8);
            var cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>"+userB+"</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            string user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 15);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 22);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 29);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 36);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 43);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 50);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 57);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 64);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 71);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 78);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 85);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 92);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 99);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            //
            user1 = GetLine(file_users, 106);
            cbdefois = Regex.Matches(user1, "<strong>").Count;
            if (cbdefois == 2)
            {
                string userB = Helpers.GetStringInBetween("<strong>", "</strong>", user1, true, true);
                userB = userB.Replace("<del>", "");
                userB = userB.Replace("</del>", "");
                userB = userB.Replace("<strong>", "");
                userB = userB.Replace("</strong>", "");
                user1 = user1.Replace("<strong>" + userB + "</strong>", null);
                if (!xx.Contains(userB))
                {
                    xx.Add(userB);
                }
            }
            user = Helpers.GetStringInBetween("<strong>", "</strong>", user1, false, false);
            user.Replace("<del>", "");
            user.Replace("</del>", "");
            if (!xx.Contains(user))
            {
                xx.Add(user);
            }
            return xx;
        }

        /// <summary>
        /// Permet de savoir si un membre poste mass de 'Merci du partage'
        /// </summary>
        /// <param name="UserID">le User ID du membre</param>
        /// <returns></returns>
        public static bool IsUserAMerci(string UserID)
        {
            var x = GetMemberinfo(ID2Nickname(UserID));

            if (x != null)
            {
                int memberpost = Convert.ToInt32(x[1]);
                /* 
            Pour 1 page on vérifie 8 merci
            Pour 2 pages 16 
            etc...
            pour X=1 Y=8
            pour X=2 Y=16
            Rapport R {X,Y} = x/0.16 Ry = y'{x/0.16} || x/Ry =0.16
            */
                int wordCount = 0;
                var pagereponses =
                    MyWebClient.DownloadString("http://hack-free.net/search.php?action=finduser&uid=" + UserID);
                pagereponses = pagereponses.ToLower();
                var count = Regex.Matches(pagereponses, "merci").Count;
                int percentComplete = (int) Math.Round((double) (100*count)/memberpost);
                return percentComplete >= 50;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de récupérer le nombre de message d'un membre
        /// </summary>
        /// <param name="username">Le pseudo du membre</param>
        /// <returns></returns>
        public static int GetUserMessageCount(string username)
        {
            var nombremessage = 0;
            var userid = Nickname2ID(username);
            var reply = MyWebClient.DownloadString("http://hack-free.net/search.php?action=finduser&uid=" + userid);
            //<span class="pages">Pages (5) :</span>
            var source1 = "<span class=\"pages\">Pages ";
            var source2 = " :</span>";
            var pagenumber = Helpers.GetStringInBetween(source1, source2, reply, false, false);
            pagenumber = pagenumber.Replace("(","");
            pagenumber = pagenumber.Replace(")", "");
            if (pagenumber != "")
            {
                //<span class="pagination_current">1</span>
                //<a href="search.php?action=results&amp;sid=aa9c3084e675404e26d0057faea4a9c4&amp;sortby=dateline&amp;order=desc&amp;uid=&amp;page=2" class="pagination_page">2</a>
                source1 = ">1</span>";
                source2 = "class=\"pagination_page\">2</a>";
                var url = Helpers.GetStringInBetween(source1, source2, reply, false, false);
                url = url.Replace("<a href=\"", "");
                url = url.Replace("\" ", "");
                url = url.Replace(@"\n", "");
                url = url.Replace("page=2", "page=");
                url = url.Replace("&amp;", "&");
                url = url.Trim();
                //"search.php?action=results&amp;sid=897b4e2d867108d53140c535f4212867&amp;sortby=dateline&amp;order=desc&amp;uid=&amp;page="
                for (int i = 0; i < int.Parse(pagenumber); i++)
                {
                    //1 page complette = 50 messages
                    var page = MyWebClient.DownloadString("http://hack-free.net/" + url + i);
                    if (page.ToLower().Contains("les termes de la recherche."))
                    {
                        break;
                    }
                    else
                    {
                        var xeno = CountStringOccurrences(page, "width=\"2%\">");
                        nombremessage += xeno;
                    }
                }
            }
            else
            {
                var xeno = CountStringOccurrences(reply, "width=\"2%\">");
                nombremessage += xeno;
            }

            return nombremessage;
        }




        #region private method to fetch & scrape data

        /// <summary>
        /// This method returns the percentage-formatted string.
        /// </summary>
        static string GetPercentageString(double ratio)
        {
            return ratio.ToString("0.0%");
        }


        /// <summary>
        /// Count occurrences of strings.
        /// </summary>
        private static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Permet de récupérer une ertaine ligne d'un fichier texte
        /// </summary>
        /// <param name="fileName">Le fichier source</param>
        /// <param name="line">La ligne</param>
        /// <returns></returns>
        private static string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (var i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }

        /// <summary>
        /// Grosse methode bien trash-code qui sert de fourre-tout
        /// </summary>
        private static void GetData()
        {
            GetUserID();
            GetUserPanel();
            GetPostKey(Forum);
            Getpostcount();
            GetMemberReferedcount();
            GetRatio();
            GetUsernameChange();
            GetMerci();
            GetCredit();
            GetAvatarUrl();
            GetReportedMessage();
            GetGroupe();
            GetReputation();
            GetInscriptionDate();
            GetAverto();
        }

        private static int GetUserID()
        {
            try
            {
                var rep = 0;
                var cpanel = MyWebClient.DownloadString("http://hack-free.net/usercp.php");
                Userpanel = cpanel;
                var entries =
                    Helpers.GetStringInBetween("<tr><td class=\"trow1 smalltext\"><a href=\"sellreput.php?uid=",
                        " class=\"usercp_nav_item usercp_nav_myvouches\">Mes Vouches</a></td></tr>", cpanel, false,
                        false);
                if (entries != "")
                {
                    //"84\""
                    entries = entries.Trim('"');
                    entries = entries.Replace(@"\", "");
                }
                else
                {
                    throw new Exception("Error : Cannot fetch userID value !");
                }
                UserID = int.Parse(entries);
                return UserID;
            }
            catch (Exception)
            {

                throw new Exception("Error : Cannot fetch referenced members count !");
            }
        }

        private static  string GetUserPanel()
        {
                try
                {
                    var pagemember = MyWebClient.DownloadString("http://hack-free.net/User-" + UserID);
                    Userpanel = pagemember;
                    return pagemember;
                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch user panel !");
                }
        }

        private static string GetPostKey(string source)
        {
            //var my_post_key = "                        "; var imagepath = "images";</script>
            var Postkeyz = Helpers.GetStringInBetween("var my_post_key = ", "; var imagepath = \"images\";</script>", source, false,
                            false);
            Postkeyz = Postkeyz.Replace("\"", null);
            PostKey = Postkeyz;
            return Postkeyz;
        }

        private static int Getpostcount()
        {
            
                try
                {
                    var postcount = 0;
                    var pagemember = Userpanel;
                    //Total Posts:</span> 230 <span class="smalltext">
                    var entries2 = Helpers.GetStringInBetween("Total Posts:</span> ", " <span class=\"smalltext\">",
                        pagemember, false, false);

                    postcount = int.Parse(entries2);
                    PostCount = postcount;
                    return postcount;
                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch post count !");
                }
        }

        private static  int GetMemberReferedcount()
        {
           
                try
                {
                    var refmember = 0;
                    var pagemember = Userpanel;
                    //<span class="pin">Members Referred:</span> 0
                    //</div>
                    var entries2 = Helpers.GetStringInBetween("<span class=\"pin\">Members Referred:</span> ", "</div>",
                        pagemember, false, false);
                    //"0\n"
                    entries2 = entries2.Trim('\n');
                    entries2 = entries2.Replace(@"\n", "");
                    refmember = int.Parse(entries2);
                    ReferencedMembers = refmember;
                    return refmember;
                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch referenced members count !");
                }
        }

        private static  string GetRatio()
        {
            try
                {
                    var rat = "";
                    var pagemember = Userpanel;
                    //<span class="pinfo">Ratio:<span class="pinfo2"><font color="green">0,750</span></span>
                    var entries2 = Helpers.GetStringInBetween("Ratio:<span class=\"pinfo2\">", "</span></span>",
                        pagemember, false, false);
                    entries2 = entries2.Trim('\n');
                    if (entries2.Contains("green"))
                    {
                        //<font color=\"green\">0,750</font>
                        entries2 = entries2.Replace("<font color=\"green\">", "");
                        entries2 = entries2.Replace("</font>", "");
                        rat = entries2;
                        Ratio = rat;
                        return rat;
                    }
                    else if (entries2.Contains("red"))
                    {
                        //<font color=\"red\">0,750</font>
                        entries2 = entries2.Replace("<font color=\"red\">", "");
                        entries2 = entries2.Replace("</font>", "");
                        rat = entries2;
                        Ratio = rat;
                        return rat;
                    }
                    else if(entries2.Contains("Infini"))
                    {
                        rat = "Infini";
                        Ratio = rat;
                        return rat;
                    }
                    
                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch ratio count !");
                }
            return null;
        }

        private static  string GetUsernameChange()
        {
           
                try
                {
                    var usernamechng = "";
                    var pagemember = Userpanel;
                    //<span class="pinfo">Username Changes: <span class="pinfo2"><a href="#"><a href="misc.php?action=username_history&amp;uhuid=84">2</a></a></span></span>
                    var src1 = "username_history&amp;uhuid=" + UserID;
                    var src2 = "</a></a>";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    //"\">2"
                    entries2 = entries2.Trim('"');
                    entries2 = entries2.Replace(@"\", "");
                    entries2 = entries2.Replace(">", "");
                    usernamechng = entries2;
                    UsernameChange = usernamechng;
                    return usernamechng;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch former username count !");
                }
        }

        private static  string GetMerci()
        {
           
                try
                {
                    var thanks = "";
                    var pagemember = Userpanel;
                    //<div class="prow"><span class="pin">Merci ReÃ§u:</span> 146 </div>
                    var src1 = "Merci ReÃ§u:</span> ";
                    var src2 = " </div>";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    //"\">2"
                    entries2 = entries2.Trim('"');
                    entries2 = entries2.Replace(@"\", "");
                    entries2 = entries2.Replace(">", "");
                    thanks = entries2;
                    Merci = thanks;
                    return thanks;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch 'Merci' count !");
                }
        }

        private static  string GetCredit()
        {
            
                try
                {
                    var cred = "";
                    var pagemember = Userpanel;
                    //<td class="trow2"><a href="http://hack-free.net/newpoints.php">1,383 </a>[<a href="http://hack-free.net/newpoints.php?action=donate&uid=84">Donation</a>]
                    var src1 = "<td class=\"trow2\"><a href=";
                    var src2 = " </a>[<a href=";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    //"http://hack-free.net/newpoints.php">1,383
                    entries2 = entries2.Trim('"');
                    entries2 = entries2.Replace("http://hack-free.net/newpoints.php", "");
                    //\">1,383
                    entries2 = entries2.Replace(">", "");
                    entries2 = entries2.Trim('"');
                    cred = entries2;
                    Credit = cred;
                    return cred;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch credit count !");
                }
        }

        private static  string GetAvatarUrl()
        {
           
                try
                {
                    var avatar = "";
                    var pagemember = Userpanel;
                    //<div class="pavatar"><img src="./uploads/avatars/avatar_84.png?dateline=1452966028" alt="" width="139" height="139"/></div>
                    var src1 = "<div class=\"pavatar\">";
                    var src2 = " alt=";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    //"<img src=\"./uploads/avatars/avatar_84.png?dateline=1452966028\""
                    entries2 = entries2.Trim('"');
                    entries2 = entries2.Replace("<img src=\".", "");
                    entries2 = entries2.Replace("\"", "");
                    entries2 = entries2.Trim('"');
                    entries2 = "http://hack-free.net" + entries2;
                    avatar = entries2;
                    AvatarURL = avatar;
                    return avatar;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch avatar url !");
                }
        }

        private static  string GetReportedMessage()
        {
           
                try
                {
                    var report = "";
                    var pagemember = Userpanel;
                    //<div class="prow"><span class="pin">Messages Signale: </span><a href="usercp.php?action=reports&uid=84">811</a></div>
                    var src1 = "action=reports&uid=" + UserID;
                    var src2 = "</a></div>";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    //\">811
                    entries2 = entries2.Trim('"');
                    entries2 = entries2.Replace(">", "");
                    entries2 = entries2.Trim('"');
                    report = entries2;
                    ReportedMessage = report;
                    return report;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch reported messages count !");
                }
        }

        private static  string GetGroupe()
        {
            
                try
                {
                    var groupe = "";
                    var pagemember = Userpanel;
                    //<div class="pgroupimage">
                    //<img src="/images/userbarsLM/HFHebdo.png" alt="Journalistes" title="Journalistes"/><br/>
                    var src1 = "<div class=\"pgroupimage\">";
                    var src2 = "<br/>";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    entries2 = entries2.Trim('"');
                    var entries3 = Helpers.GetStringInBetween("title=", "/>",
                        entries2, false, false);
                    entries3 = entries3.Replace(">", "");
                    entries3 = entries3.Replace(@"\", "");
                    entries3 = entries3.Trim('"');
                    groupe = entries3;
                    Groupe = groupe;
                    return groupe;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch main group !");
                }
        }

        private static string GetAverto()
        {
           
                try
                {
                    var averto = "";
                    var pagemember = Userpanel;
                    //<span class="pin">Niveau d'avertissement :</span>
                    //<span class="smalltext"><a href="usercp.php"> 0%</a> </span>
                    var src1 = "<a href=\"usercp.php\"> ";
                    var src2 = "</a> </span>";
                    var entries2 = Helpers.GetStringInBetween(src1, src2,
                        pagemember, false, false);
                    entries2 = entries2.Trim('"');
                    averto = entries2;
                    Avertissement = averto;
                    return averto;

                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch avertissement !");
                }
        }

        private static  string GetReputation()
        {

            try
            {
                var rep = "";
                var pagemember = Userpanel;
                //<strong class="reputation_positive">4</strong>
                var entries2 = Helpers.GetStringInBetween("<strong class=\"reputation_positive\">", "</strong>",
                    pagemember, false, false);
                //4
                rep = entries2;
                Reputation = rep;
                return rep;
            }
            catch (Exception)
            {

                throw new Exception("Error : Cannot fetch reputation !");
            }
        }

        private static  string GetInscriptionDate()
        {
           
                try
                {
                    var date = "";
                    var pagemember = Userpanel;
                    //<span class="pinfo">Date d'inscription: <span class="pinfo2">22-05-2015</span></span>
                    var entries2 = Helpers.GetStringInBetween("inscription: <span class=\"pinfo2\">", "</span></span>",
                        pagemember, false, false);
                    //
                    date = entries2;
                    InscriptionDate = date;
                    return date;
                }
                catch (Exception)
                {

                    throw new Exception("Error : Cannot fetch inscription date !");
                }
        }

        #endregion
    }
}
