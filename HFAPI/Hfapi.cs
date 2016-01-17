﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace HFAPI
{

    public class User
    {
        #region Sared var

        public static Uri ForumUri { get; set; }

        public static string LoginPage { get; set; }

        public static string MyPostKey { get; set; }

        public static string Forum { get; set; }

        public static string Userpanel { get; set; }

        public static CookieClient MyWebClient { get; set; }

        public static string CompleteUrl => new Uri(ForumUri, LoginPage).ToString();

        #endregion

        #region User var

        public static int UserID { get; set; }

        public static string Username { get; set; }

        public static string PostKey { get; set; }

        public static int PostCount { get; set; }

        public static string Ratio { get; set; }

        public static string Merci { get; set; }

        public static string Reputation { get; set; }

        public static string InscriptionDate { get; set; }

        public static string UsernameChange { get; set; }

        public static string Groupe { get; set; }

        public static List<String> Groupes { get; set; }

        public static int ReferencedMembers { get; set; }

        public static string Credit { get; set; }

        public static string AvatarURL { get; set; }

        public static string ReportedMessage { get; set; }

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
                    {"quick_password", password},
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
        /// Permet de parier une somme de crédit X contre un utilisateur
        /// </summary>
        /// <param name="montant">En int, minimum 50 maximum 10,000</param>
        /// <param name="bet">Choisir le type de partie : Public, Private, System</param>
        public static void Bet(int montant, BetType bet )
        {
            
        }
        /// <summary>
        /// Permet de parier une somme de crédit X contre un utilisateur
        /// </summary>
        /// <param name="username">L'Username de la personne contre laquelle vous pariez</param>
        /// <param name="montant">En int, minimum 50 maximum 10,000</param>
        /// <param name="bet">Choisir le <type cref="BetType"/> de partie : Public, Private, System</param>
        public static void Bet(string username, int montant, BetType bet)
        {

        }

        /// <summary>
        /// Les types de parties
        /// </summary>
        /// <remarks>If you must access this list, you can only call <c>_NoLock()</c> methods
		/// </remarks>
        public enum BetType
        {
            Public,
            Private,
            System
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


        #region privvate method to fetch & scrape data

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
            if (PostCount != 0)
            {
                return PostCount;
            }
            else
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
        }

        private static  int GetMemberReferedcount()
        {
            if (ReferencedMembers != 0)
            {
                return ReferencedMembers;
            }
            else
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
        }

        private static  string GetRatio()
        {
            if (Ratio != null)
            {
                return Ratio;
            }
            else
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
            }
            return null;
        }

        private static  string GetUsernameChange()
        {
            if (UsernameChange != null)
            {
                return UsernameChange;
            }
            else
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
        }

        private static  string GetMerci()
        {
            if (Merci != null)
            {
                return Merci;
            }
            else
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
        }

        private static  string GetCredit()
        {
            if (Credit != null)
            {
                return Credit;
            }
            else
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
        }

        private static  string GetAvatarUrl()
        {
            if (AvatarURL != null)
            {
                return AvatarURL;
            }
            else
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
        }

        private static  string GetReportedMessage()
        {
            if (ReportedMessage != null)
            {
                return ReportedMessage;
            }
            else
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
        }

        private static  string GetGroupe()
        {
            if (Groupe != null)
            {
                return Groupe;
            }
            else
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
        }

        private static string GetAverto()
        {
            if (Avertissement != null)
            {
                return Avertissement;
            }
            else
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
        }

        private static  string GetReputation()
        {
            if (Reputation != null)
            {
                return Reputation;
            }
            else
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
        }

        private static  string GetInscriptionDate()
        {
            if (InscriptionDate != null)
            {
                return InscriptionDate;
            }
            else
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
        }

        #endregion
    }
}
