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
using System.Net;
using System.Reflection;
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

                  //  Task.Factory.StartNew(() => DoDownload());
                 

                        tasks[j] = Task.Factory.StartNew(() =>DoDownload());
                        label1.Text =  tasks[j].Id.ToString();
                        j++;


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
            string[] returns = new string[] { _torrent.Name, _torrent.Size.ToString(), _torrent.Size.ToString(),"0", "0", "0" };
            ListViewItem item = new ListViewItem();
            foreach(var items in returns)
            {
                item.SubItems.Add(items);
            }

           ///listView1.Invoke((Action<>) ( this.Invoke(=>UpdateListView(item) );

             tasks[j] = Task.Factory.StartNew((() => listView1.Items.Add(item)));

            //tasks[j] = new Task( () =>
            //     {

            //       listView1.Items.Add(item);

            //   });
            //this.Invoke(new Action(() =>
            //     {

            //         listView1.Items.Add(item);

            //     }));

            while (_manager.State != TorrentState.Stopped || Convert.ToInt16(_manager.Progress) != 100)
            {
                UpdateMessage();
            //tasks[j] => };
            //{ 
            //    label1.Text = tasks[j].Id.ToString();
            //    listView1.Items[index].SubItems[4].Text = (_manager.Monitor.DownloadSpeed / 1024).ToString() + " KB/S";
            //    listView1.Items[index].SubItems[5].Text = (_manager.Monitor.UploadSpeed).ToString() + " KB/S";
            //    Type type = listView1.GetType();
            //    PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            //    propertyInfo.SetValue(listView1, true, null);
            //}
            //  );
           // Invoke(new Action(() =>
            //{

            //    label1.Text = tasks[j].Id.ToString();
            //    listView1.Items[index].SubItems[4].Text = (_manager.Monitor.DownloadSpeed / 1024).ToString() + " KB/S";
            //    listView1.Items[index].SubItems[5].Text = (_manager.Monitor.UploadSpeed).ToString() + " KB/S";
            //    Type type = listView1.GetType();
            //    PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            //    propertyInfo.SetValue(listView1, true, null);

            //}));

            
            }
          
        }

        public delegate void UpdateMessageDelegate();

        public void UpdateListView()
        {
            Invoke(new UpdateMessageDelegate(UpdateListView), new object[] { });

        }
        public void UpdateMessage()
        {
            Invoke(new UpdateMessageDelegate(UpdateGroupBox), new object[] { });

        }
        public void UpdateListView(ListViewItem item)
        {
            listView1.Items.Add(item);
        }


        static object locker = new object();
        public void UpdateGroupBox()
        {
            Invoke(new Action(() =>
            {
                lock (locker)
                {
                    label1.Text = tasks[j].Id.ToString();
                    listView1.Items[index].SubItems[4].Text = (_manager.Monitor.DownloadSpeed / 1024).ToString() + " KB/S";
                    listView1.Items[index].SubItems[5].Text = (_manager.Monitor.UploadSpeed).ToString() + " KB/S";
                    Type type = listView1.GetType();
                    PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                    propertyInfo.SetValue(listView1, true, null);
                }

            }));


        }


        private void GerarTorrent()
        {
            MagnetLinkForm magnetLink = new MagnetLinkForm();
            magnetLink.ShowDialog();
           
           
            string magnet = string.Format("magnet:?xt=urn:sha1:{0}", hash);
            MagnetLink ml = new MagnetLink(magnet);
            GetPath();
    
           // _manager = new TorrentManager(ml, _dowlPath, _torrentDef, "test.torrent");
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
