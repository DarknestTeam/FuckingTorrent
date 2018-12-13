using BitTorrent.Enteties;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Client.Tracker;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BitTorrent
{
    public partial class Form1 : Form
    {
        
       static TorrentInformation tf;
        public static Torrent _torrent;
        static string _dowlPath;
        static string _fastResumeFile;
        static string _torrentPath;
        static ClientEngine _engine;
        static Top10Listener _listener;
        static TorrentManager _manager;
        List<TorrentManager> managers = new List<TorrentManager>();
        string hash = "";
        int index = 0;
        SqlDBConnection sql = new SqlDBConnection();
        //Имя и путь к файлу, который будет содержать служебную информацию, необходимую для возобновления закачки


        public Form1()
        {
            
            InitializeComponent();
           

        }
        int j = 0;
        Task[] tasks = new Task[20];
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {


           using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.FilterIndex = 1;
                openFile.Filter = "Torrent files(*.torrent)|*.torrent| All files(*.*) | *.*";
                if (openFile.ShowDialog() == DialogResult.OK  &&  j<20)
                {
                    string sFileName = openFile.FileName;
                    _torrentPath = sFileName;
                    GetPath();
                   


                    //Thread dowln = new Thread(new ThreadStart(DoDownload));
                    //dowln.Start();
                        Task.Factory.StartNew(() =>DoDownload());
                        //label1.Text =  tasks[j].Id.ToString();
                        //j++;


                    _listener = new Top10Listener(10);
                   

                    //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи


                    //Error


                }
            }
        }
        EncryptionTypes ChooseEncryption()
        {
            EncryptionTypes encryption;
            // This completely disables connections - encrypted connections are not allowed
            // and unencrypted connections are not allowed
            encryption = EncryptionTypes.None;

            // Only unencrypted connections are allowed
            encryption = EncryptionTypes.PlainText;

            // Allow only encrypted connections
            encryption = EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;

            // Allow unencrypted and encrypted connections
            encryption = EncryptionTypes.All;
            encryption = EncryptionTypes.PlainText | EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;

            return encryption;
        }
        public void GetPath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
               _dowlPath = dialog.SelectedPath;

            }
           
        }


        private void DoDownload()
        {

            EngineSettings _engineSettings = new EngineSettings();
            TorrentSettings _torrentDef = new TorrentSettings(); //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи
            _engineSettings.AllowedEncryption = ChooseEncryption();
            _engineSettings.GlobalMaxUploadSpeed = 800 * 1024;
            _engineSettings.SavePath = _dowlPath;

            _engine = new ClientEngine(_engineSettings);
            _engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
            BEncodedDictionary _fastResume;
            _fastResumeFile = _dowlPath + "\temp.data";


            // Читаю или создаю индексный файл. Если файл есть - читаю, если нет - создаю
            try
            {
                _fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(_fastResumeFile)); // чтение индексного файла
            }
            catch
            {
                _fastResume = new BEncodedDictionary(); // создание индексного файла
            }

            // Загрезка торрент файла
            try
            {
                _torrent = Torrent.Load(_torrentPath); // если все ОК

            }
            catch
            {
                //Console.Write("Decoding error");      // если во время загрузки возникли ошибки
                _engine.Dispose();
                return;
            }
            foreach (TorrentFile file in _torrent.Files)
            {
                file.Priority = Priority.Normal;

            }

            //Информация о торренте
            tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size, _dowlPath);

            SaveForm save = new SaveForm(tf);
            save.ShowDialog();


            _manager = new TorrentManager(_torrent, _dowlPath, _torrentDef); //для новой закачки


            managers.Add(_manager);
            _engine.Register(_manager);

            PiecePicker picker = new StandardPicker();

            picker = new PriorityPicker(picker);
            _manager.ChangePicker(picker);

            _manager.Start();
            foreach (TrackerTier ttier in _manager.TrackerManager.TrackerTiers)
            {

            }
            string[] returns = new string[] { _torrent.Name, ToGB(_torrent.Size).ToString() + " GB", (_torrent.Size / 1024/1024/1024).ToString() + "GB", "0", "0", "0" };
            ListViewItem item = new ListViewItem();
            foreach (var items in returns)
            {
                item.SubItems.Add(items);
            }

            ///listView1.Invoke((Action<>) ( this.Invoke(=>UpdateListView(item) );

            //Thread.CurrentThread.Start(  listView1.Items.Add(item));

            //tasks[j] = new Task( () =>
            //     {

            //       listView1.Items.Add(item);

            //   });

            Invoke(new Action(() =>
            {
                listView1.Items.Add(item);
            }));

            //Thread th = new Thread(() => UpdateListView(listView1.Items[index].SubItems[4].Text, listView1.Items[index].SubItems[5].Text, (_manager.Monitor.DownloadSpeed / 1024).ToString() + " KB/S", (_manager.Monitor.UploadSpeed / 1024).ToString() + " KB/S")); ;

            //th.Start();


            while (_manager.State != TorrentState.Stopped || Convert.ToInt16(_manager.Progress) != 100)
            {
                Invoke(new Action(() =>
                {
                Type type = listView1.GetType();

                PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                propertyInfo.SetValue(listView1, true, null);

                listView1.Items[index].SubItems[4].Text = (_manager.Monitor.DownloadSpeed / 1024).ToString() + " KB/S";
                listView1.Items[index].SubItems[5].Text = (_manager.Monitor.UploadSpeed / 1024).ToString() + " KB/S";
                listView1.Items[index].SubItems[3].Text = ToGB(_torrent.Size - _manager.Monitor.DataBytesDownloaded).ToString() + " GB";
                    listView1.Items[index].SubItems[6].Text = ToMB(_manager.Monitor.DataBytesDownloaded).ToString() + " MB";

                }));

                if (_manager.State == TorrentState.Stopped)
                {
                    sql.PutHash(_torrent.InfoHash.ToString(), (_manager.Monitor.DataBytesDownloaded).ToString(),_torrent.Publisher.ToString());
                    break;
                }
            }

            

        }


        public double ToGB(double smth)
        {
            double result =smth / Math.Pow(1024, 3)/8;

            return Math.Round(result, 2);
        }
        public double ToMB (double smth)
        {
            double result = smth / Math.Pow(1024, 2) / 8;

            return Math.Round(result, 2);
        }
        private void GerarTorrent()
        {
            MagnetLinkForm magnetLink = new MagnetLinkForm();
            magnetLink.ShowDialog();
            hash = magnetLink.textBox1.Text;
            //URL stores the magnetlink
            TorrentSettings _torrentDef = new TorrentSettings();
            EngineSettings settings = new EngineSettings();
            settings.AllowedEncryption = EncryptionTypes.All;


            //Create a new engine, give it some settings and use it.
            ClientEngine engine = new ClientEngine(settings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, 6969));

            string magnet = string.Format("magnet:?xt=urn:bith:{0}", hash);
            MagnetLink ml = new MagnetLink(magnet);
            
          //TorrentManager  manager = new TorrentManager(ml, _dowlPath, _torrentDef, "tt.torrent",List<>R);
            managers.Add(_manager);
            _engine.Register(_manager);
            _listener = new Top10Listener(10);
            _manager.Start();
            
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            GetPath();
            GerarTorrent();
            
           
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            _manager.Stop();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() => DoDownload());
            _manager.Start();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            _manager.Stop();
          
        }
    }
}
