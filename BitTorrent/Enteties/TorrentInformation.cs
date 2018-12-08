using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitTorrent.Enteties
{
 public   class TorrentInformation
    {
        public int _torrentId { get; set; }
        public string _createdBy { get; set; }
        public string _name { get; set; }
        public DateTime _creationDate { get; set; }
        public string _publisherUrl { get; set; }
        public double _partLength { get; set; }
        public double _partCout { get; set; }
        public double _torrentSize { get; set; }
        public string _description { get; set; }
        public  string _downloadPath { get; set; }
        public TorrentInformation(string name,string description,DateTime creationDate,double torrentSize, string downloadPath)
        {
            _name = name;
            _description = description;
            _torrentSize = torrentSize;
            _creationDate = creationDate;
           _downloadPath = downloadPath;
        }
        public double TorrentSize(double torrentsize)
        {
            double result = torrentsize /  Math.Pow(1024, 3);

            return Math.Round(result,2);
        }
    }
}
