using BitTorrent.Enteties;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Client.Tracker;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
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
        //Имя и путь к файлу, который будет содержать служебную информацию, необходимую для возобновления закачки


        public Form1()
        {
            InitializeComponent();
           

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
                    GetPath();
                    Task.Factory.StartNew(() => DoDownload());




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
        private  void DoDownload()
        {

            EngineSettings _engineSettings = new EngineSettings();
            TorrentSettings _torrentDef = new TorrentSettings(5, 100, 0, 0); //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи
            _engineSettings.AllowedEncryption = ChooseEncryption();
            _engineSettings.GlobalMaxUploadSpeed = 400 * 1024;
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
            tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size,_dowlPath);

          SaveForm  save = new SaveForm(tf);
            save.ShowDialog();
           
            
                _manager = new TorrentManager(_torrent, _dowlPath, _torrentDef); //для новой закачки
            
           
            managers.Add(_manager);
            _engine.Register(_manager);

            PiecePicker picker = new StandardPicker();

            picker = new PriorityPicker(picker);
            _manager.ChangePicker(picker);

            _engine.StartAll();
            








            //formatOutput(_stringBuilder, "Total Download Rate: {0:0.00}kB/sec", _engine.TotalDownloadSpeed / 1024.0);
            //formatOutput(_stringBuilder, "Total Upload Rate:   {0:0.00}kB/sec", _engine.TotalUploadSpeed / 1024.0);
            //formatOutput(_stringBuilder, "Disk Read Rate:      {0:0.00} kB/s", _engine.DiskManager.ReadRate / 1024.0);
            //formatOutput(_stringBuilder, "Disk Write Rate:     {0:0.00} kB/s", _engine.DiskManager.WriteRate / 1024.0);
            //formatOutput(_stringBuilder, "Total Read:         {0:0.00} kB", _engine.DiskManager.TotalRead / 1024.0);
            //formatOutput(_stringBuilder, "Total Written:      {0:0.00} kB", _engine.DiskManager.TotalWritten / 1024.0);
            //formatOutput(_stringBuilder, "Open Connections:    {0}", _engine.ConnectionManager.OpenConnections);


            //formatOutput(_stringBuilder, "Name:            {0}", _manager.Torrent.Name);
            //formatOutput(_stringBuilder, "Progress:           {0:0.00}", _manager.Progress);
            //formatOutput(_stringBuilder, "Download Speed:     {0:0.00} kB/s", _manager.Monitor.DownloadSpeed / 1024.0);
            //formatOutput(_stringBuilder, "Upload Speed:       {0:0.00} kB/s", _manager.Monitor.UploadSpeed / 1024.0);
            //formatOutput(_stringBuilder, "Total Downloaded:   {0:0.00} MB", _manager.Monitor.DataBytesDownloaded / (1024.0 * 1024.0));
            //formatOutput(_stringBuilder, "Total Uploaded:     {0:0.00} MB", _manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0));
            //formatOutput(_stringBuilder, "Tracker Status:     {0}", _manager.TrackerManager.CurrentTracker.State);
            //formatOutput(_stringBuilder, "Warning Message:    {0}", _manager.TrackerManager.CurrentTracker.WarningMessage);
            //formatOutput(_stringBuilder, "Failure Message:    {0}", _manager.TrackerManager.CurrentTracker.FailureMessage);

        }

        public delegate void AddMessageDelegate(string Name, double Size, double Remaning, double Download_speed, double Output, double Loaded);

        public void AddMessage()
        {
           Invoke(new AddMessageDelegate(AddGroupBox), new object[] { _torrent.Name, _torrent.Size, 23, _manager.Monitor.DownloadSpeed, 1, 1 });
           
        }

        public void AddGroupBox( string Name, double   Size, double Remaning,  double Download_speed,  double Output,  double Loaded)
        {

            //ListViewItem itm = new ListViewItem(new string[] {Name, Size.ToString(), Remaning.ToString(), Download_speed.ToString(), Output.ToString(), Loaded.ToString() });
            //listView1.Items.Add(itm);
            //label1.Text = Download_speed.ToString();
        }
      
        private static void exit()
        {
            BEncodedDictionary fastResume = new BEncodedDictionary();

            //WaitHandle handle = _manager.Stop(); ;

            //fastResume.Add(_manager.Torrent.InfoHash, _manager.SaveFastResume().Encode());

            File.WriteAllBytes(_fastResumeFile, fastResume.Encode());

            _engine.Dispose();

            foreach (TraceListener lst in Debug.Listeners)
            {
                lst.Flush();
                lst.Close();
            }

            System.Threading.Thread.Sleep(2000);
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

      

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invoke(new AddMessageDelegate(AddGroupBox), new object[] { Name, Size.ToString(), Remaning.ToString(), Download_speed.ToString(), Output.ToString(), Loaded.ToString() });
           
            timer1.Enabled = false;
        }
        private void GerarTorrent()
        {
            MagnetLinkForm magnetLink = new MagnetLinkForm();
            magnetLink.ShowDialog();
            hash = magnetLink.textBox1.Text;
            EngineSettings _engineSettings = new EngineSettings();
            TorrentSettings _torrentDef = new TorrentSettings(5, 100, 0, 0); //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи
            _engineSettings.AllowedEncryption = ChooseEncryption();
            _engineSettings.GlobalMaxUploadSpeed = 400 * 1024;
            _engineSettings.SavePath = _dowlPath;

            _engine = new ClientEngine(_engineSettings);
            _engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
          

          
            if (hash == null)
            {
                GerarTorrent();
            }
           
            string magnet = string.Format("magnet:?xt=urn:sha1:{0}", hash);
            MagnetLink ml = new MagnetLink(magnet);
            GetPath();
            _manager = new TorrentManager(ml, _dowlPath, _torrentDef, "test.torrent");
            hash = "";
            

            managers.Add(_manager);
            _engine.Register(_manager);
            _listener = new Top10Listener(10);

            _manager.Start();


            _engine.StartAll();
          


        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            GerarTorrent();
            
           
        }
    }
}
