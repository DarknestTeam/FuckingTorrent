using BitTorrent.Enteties;
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
                    
                    

                    

                    _listener = new Top10Listener(10);

                    //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи
                    Task.Factory.StartNew(() => DoDownload());

                    AddGroupBox(_torrent.Name, _torrent.Size, 23, _manager.Monitor.DownloadSpeed, 1, 1);//Error


                }
            }
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
            int _port;
            _port = 31337;

           
            EngineSettings _engineSettings = new EngineSettings();
            TorrentSettings _torrentDef = new TorrentSettings(5, 100, 0, 0); //слот отдачи, количество одновременных подключений, макс скорость загрузки, макс скорость отдачи

            _engineSettings.SavePath = _dowlPath;
            _engineSettings.ListenPort = _port;

            _engine = new ClientEngine(_engineSettings);

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

            //Информация о торренте
            tf = new TorrentInformation(_torrent.Name, _torrent.Comment, _torrent.CreationDate, _torrent.Size,_dowlPath);

          SaveForm  save = new SaveForm(tf);
            save.ShowDialog();
            //Console.WriteLine("Created by: {0}", _torrent.CreatedBy);
            //Console.WriteLine("Creation date: {0}", _torrent.CreationDate);
            //Console.WriteLine("Comment: {0}", _torrent.Comment);
            //Console.WriteLine("Publish URL: {0}", _torrent.PublisherUrl);
            //Console.WriteLine("Size: {0}", _torrent.Size);
            //Console.WriteLine("Piece length: {0}", _torrent.PieceLength);
            //Console.WriteLine("Piece count: {0}", _torrent.Pieces.Count);
            //Console.WriteLine("");
            //Console.WriteLine("Press any key for continue...");



            //if (_fastResume.ContainsKey(_torrent.InfoHash))
            //    _manager = new TorrentManager(_torrent, _downloadPath, _torrentDef, new FastResume((BEncodedDictionary)_fastResume[_torrent.InfoHash])); // для уже запущенной закачки
            //else
            _manager = new TorrentManager(_torrent, _dowlPath, _torrentDef); //для новой закачки

            _engine.Register(_manager);


            _manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e)
            {
                lock (_listener)
                    _listener.WriteLine("Last status: " + e.OldState.ToString() + " Current status: " + e.NewState.ToString());
            };

            foreach (TrackerTier ttier in _manager.TrackerManager.TrackerTiers)
            {
                //foreach (MonoTorrent.Client.Tracker.Tracker tr in ttier.Trackers)
                //{
                //    tr.AnnounceComplete += delegate(object sender, AnnounceResponseEventArgs e)
                //    {
                //        _listener.WriteLine(string.Format("{0}: {1}", e.Successful, e.Tracker.ToString()));
                //    };
                //}
            }


            _manager.Start();
            
            int i = 0;
            bool _running = true;

            StringBuilder _stringBuilder = new StringBuilder(1024);
            while (_running)
            {

                if ((i++) % 10 == 0)
                {
                    if (_manager.State == TorrentState.Stopped)
                    {
                        _running = false;
                        exit();
                    }

                    _stringBuilder.Remove(0, _stringBuilder.Length);
                    
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

                    //Console.Clear();
                    //Console.WriteLine(_stringBuilder.ToString());

                }

               
            }
        }
        public void AddGroupBox( string Name, double   Size, double Remaning,  double Download_speed,  double Output,  double Loaded)
        {

            ListViewItem itm = new ListViewItem(new string[] {Name, Size.ToString(), Remaning.ToString(), Download_speed.ToString(), Output.ToString(), Loaded.ToString() });
            listView1.Items.Add(itm);
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {

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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
