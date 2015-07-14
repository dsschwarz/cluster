using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cluster
{
    class NodeCollection
    {
        HashSet<Link> uniqueLinks = new HashSet<Link>();
        Dictionary<string, Node> nodeLookup = new Dictionary<string, Node>();
        public void AddUrlLink(string startUrl, string endUrl)
        {
            var link = new Link(startUrl, endUrl);
            uniqueLinks.Add(link);
        }

        public Node GetOrCreateNode(string url)
        {
            return NodeExists(url) ? nodeLookup[url] : CreateNode(url);
        }

        public bool NodeExists(string url)
        {
            return nodeLookup.ContainsKey(url);
        }

        public List<Node> GetNodes()
        {
            return nodeLookup.Values.ToList();
        }

        public int GetWeight(string startUrl, string endUrl)
        {
            return (from link in uniqueLinks where link.Contains(startUrl, endUrl) select link.GetWeight()).FirstOrDefault();
        }

        public Node CreateNode(string url)
        {
            var node = new Node(url);
            nodeLookup.Add(url, node);
            return node;
        }
    }
}
