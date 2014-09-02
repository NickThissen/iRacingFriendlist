using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace iRacingPmToolPluginLibrary
{
    public class HttpPost
    {
        private const string LOGIN_URL = @"https://members.iracing.com/membersite/Login";
        private const string LOGIN_PARAMETERS = @"username={0}&password={1}&utcoffset=-120&todaysdate=";

        private const string FRIENDS_URL =
            @"http://members.iracing.com/membersite/member/GetDriverStatus?friends=1&studied=1";

        private const string FRIENDSPAGE_URL =
            @"http://members.iracing.com/membersite/member/myracers.jsp";

        private const string DRIVERSEARCH_URL = @"http://members.iracing.com/membersite/member/GetDriverStatus";
        private const string DRIVERSEARCH_PARAMETERS = @"searchTerms={0}";

        public CookieContainer cookieContainer;

        public HttpPost(string username, string password)
        {
            _Username = username;
            _Password = password;
        }

        private readonly string _Username;
        public string Username { get { return _Username; } }

        private readonly string _Password;
        public string Password { get { return _Password; } }

        public LoginResults Login()
        {
            var request = CreateInitialWebRequest(LOGIN_URL);
            request.AllowAutoRedirect = false;
            this.cookieContainer = new CookieContainer();
            request.CookieContainer = this.cookieContainer;
            SetPostContent(request, string.Format(LOGIN_PARAMETERS, this.Username, this.Password.ToString()));

            using (var httpWebResponse = (HttpWebResponse)request.GetResponse())
            {
                var responseStream = httpWebResponse.GetResponseStream();
                if (responseStream == null)
                    return LoginResults.NoResponse;

                using (var streamReader = new StreamReader(responseStream))
                {
                    if (streamReader.ReadToEnd().Contains("The email address or password was invalid.") ||
                        ((NameValueCollection)httpWebResponse.Headers)["Location"] ==
                        "https://members.iracing.com/membersite/failedlogin.jsp")
                        return LoginResults.IncorrectCredentials;

                    return LoginResults.Success;
                }
            }
        }

        private HttpWebRequest CreateInitialWebRequest(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
            return request;
        }

        private HttpWebRequest CreateWebRequest(string uri)
        {
            var request = this.CreateInitialWebRequest(uri);
            request.CookieContainer = cookieContainer;
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-ms-application, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-ms-xbap, application/x-shockwave-flash, */*";
            request.UserAgent = "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            request.Method = "GET";
            request.Timeout = 30000; // 30 sec
            return request;
        }

        private static void SetPostContent(HttpWebRequest req, string postData)
        {
            req.Method = "POST";
            byte[] bytes = new ASCIIEncoding().GetBytes(postData);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = (long)bytes.Length;
            ((WebRequest)req).GetRequestStream().Write(bytes, 0, bytes.Length);
            ((WebRequest)req).GetRequestStream().Close();
        }

        private string GetPageSource(string url)
        {
            var request = this.CreateWebRequest(url);
            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                if (stream == null) throw new Exception("Empty response stream.");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string GetFriends()
        {
            return GetPageSource(FRIENDS_URL);
        }

        public string GetFriendsPage()
        {
            return GetPageSource(FRIENDSPAGE_URL);
        }

        public string DriverSearch(string search)
        {
            var request = this.CreateWebRequest(DRIVERSEARCH_URL);
            SetPostContent(request, string.Format(DRIVERSEARCH_PARAMETERS, search));

            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                if (stream == null) throw new Exception("Empty response stream.");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string DownloadCsv(string url)
        {
            var request = CreateWebRequest(url);
            request.CookieContainer = cookieContainer;
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-ms-application, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-ms-xbap, application/x-shockwave-flash, */*";
            request.UserAgent = "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506; .NET CLR 3.5.21022)";
            request.Method = "GET";

            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                if (stream == null) return string.Empty;

                using (var webReader = new StreamReader(stream))
                {
                    return webReader.ReadToEnd();
                }
            }
        }

        public class AcceptAllCertificatePolicy : ICertificatePolicy
        {
            public bool CheckValidationResult(ServicePoint sPoint, X509Certificate cert, WebRequest wRequest, int certProb)
            {
                return true;
            }
        }

        public enum LoginResults
        {
            Success = 0,
            IncorrectCredentials,
            NoResponse
        }
    }
}