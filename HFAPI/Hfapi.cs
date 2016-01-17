using System;
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

        public static int PostCount { get; set; }

        public static string Ratio { get; set; }

        public static string Merci { get; set; }

        public static string Reputation { get; set; }

        public static string InscriptionDate { get; set; }

        public static string UsernameChange { get; set; }

        public static string Groupe { get; set; }

        public static List<String> Groupes { get; set; }

        public static int ReferencedMembers { get; set; }

        public static int Credit { get; set; }

        #endregion



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
                    {"quick_password", password}, //Paradoxum123456789
                    {"submit", "Login"},
                    {"quick_remember", "no"}
                };
                var connection = MyWebClient.UploadValues(CompleteUrl, request);
                var myStream = new MemoryStream(connection);
                var myReader = new StreamReader(myStream);
                var xeno = myReader.ReadToEnd();
                Forum = xeno;
                if (myReader.ReadToEnd().Contains("Corrigez les erreurs suivantes avant de continuer :"))
                {
                    throw new Exception("Error : Cannot connect to the forum : Invalid Login or Password");
                }
                GetUserID();
                GetUserPanel();
            }
            catch (Exception e)
            {

                throw new Exception("Error : Cannot connect to the forum : " + e.ToString());
            }
        }

        public static int GetUserID()
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

        public static string GetUserPanel()
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




        public static int Getpostcount()
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

        public static int GetMemberReferedcount()
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

        public static string GetRatio()
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

        public static string GetUsernameChange()
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

        public static string GetMerci()
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

        public static string GetReputation()
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

        public static string GetInscriptionDate()
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
    }
}
