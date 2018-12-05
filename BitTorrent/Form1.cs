using BitTorrent.Enteties;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitTorrent
{
    public partial class Form1 : Form
    {
        Torrent _torrent;
        TorrentInformation tf;
        static string _torrentPath;
        
        public Form1()
        {
            InitializeComponent();
           

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
          
            
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.FilterIndex = 1;
                openFile.Filter = "Torrent files(*.torrent)|*.torrent| All files(*.*) | *.*";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string sFileName = openFile.FileName;
                    _torrentPath = sFileName;
                    try
                    {
                        _torrent = Torrent.Load(_torrentPath);

                    }
                    catch
                    {

                    }
                   tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size);

                    SaveForm save = new SaveForm(tf);
                    save.Show();

                    AddGroupBox(tf._name, tf.TorrentSize(tf._torrentSize), 10, 10.4, 0.2, 10);


                }

            }

        }

        private void AddGroupBox( string Name, double   Size, double Remaning,  double Download_speed,  double Output,  double Loaded)
        {

            ListViewItem itm = new ListViewItem(new string[] { Name, Size.ToString(), Remaning.ToString(), Download_speed.ToString(), Output.ToString(), Loaded.ToString() });
            listView1.Items.Add(itm);
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
        
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.FilterIndex = 1;
                openFile.Filter = "Torrent files(*.torrent) | *.torrent | All files(*.*) | *.*";
                if (openFile.ShowDialog() == DialogResult.OK)
                {

                }
            }
            string path = @"C:\apache\hta.txt";
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.Delete();
                // альтернатива с помощью класса File
                // File.Delete(path);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
