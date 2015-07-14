using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cluster
{
    class Node
    {
        private readonly string url;

        public Node(string url)
        {
            this.url = url;
        }

        public string GetUrl()
        {
            return url;
        }
    }
}
