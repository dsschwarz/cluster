using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cluster
{
    class WebService
    {
        const string AbsoluteDomainPattern = "https?://";
        const string RelativeDomainPattern = "/[^/]";
        const string SameProtocolPattern = "//[^/]+.";
        const string StartsWithPattern = "(?=(?:" + AbsoluteDomainPattern + "|" + RelativeDomainPattern + "|" + SameProtocolPattern + "))";
        const string HrefPattern = "href\\s*=\\s*[\"']" + StartsWithPattern + "(?<1>[^\"'?#]*)[\"'?#]";

        public static IEnumerable<string> FetchLinks(string url)
        {
            string responseFromServer = FetchWebPageContents(url);
            var links = ExtractLinks(responseFromServer);
           return ConvertToAbsolute(links, url);
        }

        private static string FetchWebPageContents(string uri)
        {
            // Create a request for the URL. 
            var request = WebRequest.Create(uri);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            var response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            var dataStream = response.GetResponseStream();

            // Early return if stream is null
            if (dataStream == Stream.Null) return "";

            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(dataStream);
            // Read the content.
            var responseFromServer = reader.ReadToEnd();
            // Display the content.
            return responseFromServer;
        }

        private static bool LinkFilter(string link)
        {
            // for now, only accept wiki pages
            const string containsWiki = "/wiki/[^:]+$";
            return Regex.IsMatch(link, containsWiki);
        }

        private static List<string> ExtractLinks(string htmlBody)
        {

            try
            {
                var matches = Regex.Matches(htmlBody, HrefPattern,
                                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                                TimeSpan.FromSeconds(1));

                List<string> links = new List<string>();
                for (var i = 0; i < matches.Count; i++)
                {
                    var link = matches[i].Groups[1].Value;
                    if (LinkFilter(link))
                    {
                        links.Add(link);
                    }
                }
                return links;
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
                throw;
            }
        }

        private static IEnumerable<string> ConvertToAbsolute(IReadOnlyList<string> links, string url)
        {
            var absoluteLinks = new string[links.Count];
            var protocol = Regex.Match(url, "^(https?)").Groups[1].Value;
            var domain = Regex.Match(url, "^https?://([^/]+)(?:$|/)").Groups[1].Value;

            for (var i = 0; i < links.Count; i++)
            {
                string prefix;
                if (Regex.IsMatch(links[i], "^" + AbsoluteDomainPattern))
                {
                    prefix = "";
                }
                else if (Regex.IsMatch(links[i], "^" + RelativeDomainPattern))
                {
                    prefix = protocol + "://" + domain;
                }
                else if (Regex.IsMatch(links[i], "^" + SameProtocolPattern))
                {
                    prefix = protocol + ":";
                }
                else
                {
                    throw new Exception("Invalid link format: " + links[i]);
                }
                absoluteLinks[i] = prefix + links[i];
            }
            return absoluteLinks;
        }
    }
}
