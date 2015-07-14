using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cluster
{
    class Link
    {
        private int _weight;
        private readonly string _urlA;
        private readonly string _urlB;

        public Link(string startUrl, string endUrl)
        {
//            if (startUrl == endUrl) Console.WriteLine("Warning: Linking between the same url: " + startUrl);
            // sort alphabetically
            if (string.Compare(startUrl, endUrl) > 0)
            {
                this._urlA = startUrl;
                this._urlB = endUrl;
            }
            else
            {
                this._urlA = endUrl;
                this._urlB = startUrl;
            }
            this._weight = 1;
        }

        public bool Contains(string startUrl, string endUrl)
        {
            return (_urlA == startUrl && _urlB == endUrl) ||
                (_urlA == endUrl && _urlB == startUrl);
        }

        public override int GetHashCode()
        {
            return (_urlA + _urlB).GetHashCode();
        }

        public int GetWeight()
        {
            return _weight;
        }
    }
}
