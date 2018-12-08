using BitTorrent.Enteties;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitTorrent
{
    public partial class SaveForm : Form
    {
       
        public SaveForm(TorrentInformation information)
        {
            InitializeComponent();
            label6.Text = information._name;
            label7.Text = information._description;
            label8.Text = TorrentSize(information._torrentSize).ToString("#.##") + " GB";
            label9.Text = information._creationDate.ToString();
            label10.Text = information._downloadPath.ToString();

        }
        public double TorrentSize(double torrentsize) {
            double result = torrentsize/(1*Math.Pow(10,9));
            
            return result;

        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
