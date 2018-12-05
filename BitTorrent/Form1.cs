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
        static string _torrentPath;
        
        public Form1()
        {
            InitializeComponent();
           
            AddLVItem("A", "Ziggy",40.7,40.7,66.7);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
          
            AddGroupBox();
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
                    TorrentInformation tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size);

                    SaveForm save = new SaveForm(tf);
                    save.Show();

                }

            }
        }

        private void AddGroupBox()
        {
            UpdateItemValue("A", "Ziggy", 40.7, 40.7, 80);
            //List<Label> labels = new List<Label>();

            //GroupBox GroupBoxADD = new GroupBox();
            //GroupBoxADD.Name = "GroupBox1";
            //GroupBoxADD.Location = new Point(12, 180);
            //GroupBoxADD.Size = new Size(740,49);
            //GroupBoxADD.BackColor = Color.White;
            //GroupBoxADD.Font = new Font("Calibri", 8);
            //GroupBoxADD.FlatStyle = FlatStyle.Popup;
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Location = new Point(12, 180);
            toolStrip.Size = new Size(740,49);
            toolStrip.BackColor = Color.White;
            toolStrip.Font = new Font("Calibri", 12);


            //Label label1 = new Label();
            //label1.Location = new Point( 14, 18);
            //label1.Text = "2";
            //labels.Add(label1);

            //Label label2 = new Label();
            //label1.Location = new Point(40, 16);
            //label1.Text = "WatchDogs2";
            //labels.Add(label2);

            //Label label3 = new Label();
            //label1.Location = new Point(262, 18);
            //label1.Text = "40,4 GB";
            //labels.Add(label3);

            //Label label4 = new Label();
            //label1.Location = new Point(363, 18);
            //label1.Text = "40,4 GB";
            //labels.Add(label4);

           // ProgressBar progressBar = new ProgressBar();
            //progressBar.Location =  new Point(464, 18);
            //progressBar.ForeColor = Color.AntiqueWhite;



            //toolStrip.Controls.Add



            //toolStrip.Container.Add(label1);
            //toolStrip.Container.Add(label2);
            //toolStrip.Container.Add(label3);
            //toolStrip.Container.Add(label4);
            // toolStrip.Container.Add(progressBar);



            /////???
           // ToolStripLabel label1 = new ToolStripLabel();
           // ToolStripLabel label2 = new ToolStripLabel();
           // ToolStripLabel label3 = new ToolStripLabel();
           // ToolStripLabel label4 = new ToolStripLabel();
           //// ToolStripProgressBar progressBar = new ToolStripProgressBar();

           // progressBar.ForeColor = Color.AntiqueWhite;
           // progressBar.Width =  200;


           // label1.Text = "2";  
           // label2.Text = "WatchDogs2";
           // label3.Text = "45,6GB";
           // label4.Text = "45,6GB";







           // toolStrip.Items.Add(label1);
           // toolStrip.Items.Add(label2);
           // toolStrip.Items.Add(label3);
           // toolStrip.Items.Add(label4);

           //toolStrip.Items.Add(progressBar);
           // groupBox1.Controls.Add(toolStrip);

          
      


            //Bitmap progressBarBitmap = new Bitmap(
            // imageList1.ImageSize.Width,
            // imageList1.ImageSize.Height);
            //imageList1.Images.Add(progressBarBitmap);
           
            //progressBar.MinimumSize = this.imageList1.ImageSize;
            //progressBar.MaximumSize = this.imageList1.ImageSize;
            //progressBar.Size = this.imageList1.ImageSize;

            // this.Controls.Add(toolStrip);

            //GroupBox1.Controls.Add(new object[] { labels });


            //this.Controls.Add(GroupBoxADD);

        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
        private void AddLVItem(string key, string name, double size, double remaning ,double  value)
        {
            ListViewItem lvi = new ListViewItem();
            ProgressBar pb = new ProgressBar();

            lvi.SubItems[0].Text = key;
            lvi.SubItems.Add(name);
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(remaning.ToString());
            lvi.SubItems.Add(value.ToString());            // LV has 3 cols; this wont show
            listView1.Items.Add(lvi);

            Rectangle r = lvi.SubItems[2].Bounds;
            pb.SetBounds(r.X, r.Y, r.Width, r.Height);
            pb.Minimum = 0;
            pb.Maximum = 100;
            pb.Value = Convert.ToInt32(value);
            pb.Name = key;                   // use the key as the name
            listView1.Controls.Add(pb);
        }

        private void UpdateItemValue(string key, string name, double size, double remaning, double value)
        {
            ListViewItem lvi;
            ProgressBar pb;

            // find the LVI based on the "key" in 
            lvi = listView1.Items.Cast<ListViewItem>().FirstOrDefault(q => q.SubItems[0].Text == key);
            if (lvi != null)
                lvi.SubItems[4].Text = value.ToString();

            pb = listView1.Controls.OfType<ProgressBar>().FirstOrDefault(q => q.Name == key);
            if (pb != null)
                pb.Value = Convert.ToInt32(value);
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
