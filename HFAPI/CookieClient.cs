using System;
using System.Net;

namespace HFAPI
{
    public sealed class CookieClient : WebClient
    {
        private HttpWebRequest _request;

        public CookieContainer Cookies = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            _request = (HttpWebRequest)base.GetWebRequest(address);
            if (_request == null) return null;
            _request.Timeout = 8000;
            _request.ReadWriteTimeout = 30000;
            _request.KeepAlive = false;
            _request.CookieContainer = Cookies;
            _request.Proxy = null;
            return _request;
        }

        public void ClearCookies()
        {
            Cookies = new CookieContainer();
        }
    }
}