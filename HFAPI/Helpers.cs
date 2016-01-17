using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFAPI
{
    class Helpers
    {

        /// <summary>
        /// Permet d'obtenir une chaine spécifique entre deux chaines
        /// </summary>
        /// <param name="strBegin">Le debut ou la chaine avant la cible</param>
        /// <param name="strEnd">La fin ou la chaine après la cible</param>
        /// <param name="strSource">La source, la String dans laquelle chercher</param>
        /// <param name="includeBegin">Si oui, le résutat comportera le début</param>
        /// <param name="includeEnd">Si oui, le résutat comportera la fin</param>
        /// <returns></returns>
        public static string GetStringInBetween(string strBegin, string strEnd, string strSource, bool includeBegin,
            bool includeEnd)
        {
            string[] result = { string.Empty, string.Empty };
            int iIndexOfBegin = strSource.IndexOf(strBegin);

            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired 
                if (includeBegin)
                    iIndexOfBegin -= strBegin.Length;

                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);

                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    // include the End string if desired 
                    if (includeEnd)
                        iEnd += strEnd.Length;
                    result[0] = strSource.Substring(0, iEnd);
                    // advance beyond this segment 
                    if (iEnd + strEnd.Length < strSource.Length)
                        result[1] = strSource.Substring(iEnd + strEnd.Length);
                }
            }
            else
                // stay where we are 
                result[1] = strSource;
            return result[0];
        }
    }
}
