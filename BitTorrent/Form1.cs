using BitTorrent.Enteties;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Common;
using System;
using System.IO;
using System.Windows.Forms;


namespace BitTorrent
{
    public partial class Form1 : Form
    {
        Torrent _torrent;
        TorrentInformation tf;
        int _port;
        EngineSettings _engineSettings = new EngineSettings();
        TorrentSettings _torrentSettings = new TorrentSettings(10, 100, 50, 50);
        static string _torrentPath;
        SaveForm save;
        static ClientEngine _engine;
        //вспомогательный класс
        static Top10Listener _listener;
        //Менеджер для хранения законченных настроек для очередного torrent-файла
        static TorrentManager _manager;
        static string _fastResumeFile;
        //Имя и путь к файлу, который будет содержать служебную информацию, необходимую для возобновления закачки
        BEncodedDictionary _fastResume;
        public Form1()
        {
            InitializeComponent();
           

        }
        private void StartDowln()
        {
            _port = 5564;
            _engineSettings.SavePath = save.textBox1.Text;
            _engineSettings.ListenPort = _port;
            _engine = new ClientEngine(_engineSettings);
           _fastResumeFile = save.textBox1.Text + "\temp.data";

            try
            {
                _fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(_fastResumeFile));
            }
            catch
            {
                _fastResume = new BEncodedDictionary();
            }

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        public void Register()
        {
           string hash = "";
            if (_fastResume.ContainsKey(_torrent.InfoHash.ToString()))
                _manager = new TorrentManager(_torrent, save.textBox1.Text ,_torrentSettings,_torrent.InfoHash.ToString());
            else
                _manager = new TorrentManager(_torrent, save.textBox1.Text, _torrentSettings);

            _engine.Register(_manager);
            

            
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
                        _engine.Dispose();
                        return;

                    }
                   
                    tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size);

                    save = new SaveForm(tf);
                    save.ShowDialog();

                    AddGroupBox(tf._name, tf.TorrentSize(tf._torrentSize), 10, 10.4, 0.2, 10);
                    StartDowln();
                    Register();
                    int i = 0;
                    bool _running = true;
                    while (_running)
                    {
                       
                        if ((i++) % 10 == 0)
                        {
                            if (_manager.State == TorrentState.Stopped)
                            {
                                _running = false;
                            }
                            label1.Text = _engine.TotalDownloadSpeed.ToString();
                        }

                    }

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
