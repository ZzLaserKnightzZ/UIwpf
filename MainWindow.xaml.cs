using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace realiu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        bool LockElement = false;
        UIElement element;
        object elementSelected = false;
        Point uI;
        NodePrototype mynode;
        static ResourceComputer resource;
        MyBaseColor BaseColor = new MyBaseColor();
        string fileSerialize = string.Empty;

        public MainWindow()
        {

            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            try
            {
                LiftSide.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                canvas.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                canvas.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

                resource = new ResourceComputer();
                this.gMyCPU.DataContext = resource;
                resource.StartUpdate();
                this.Node_Manager.Visibility = Visibility.Hidden;
                this.Color_Selecter.DataContext = BaseColor;

                fileSerialize = GetAppSettings("FileLocation");

                if (File.Exists(fileSerialize))
                {
                    mynode = new NodePrototype(fileSerialize);
                    mynode.canvas = canvas;
                    mynode.LoadNode();

                    //var pc = new PointCollection();
                    //pc.Add(new Point(500, 300));
                    //pc.Add(new Point(200, 500));
                    //pc.Add(new Point(600, 600));
                    //pc.Add(new Point(100, 80));
                    //pc.Add(new Point(980, 850));
                    //pc.Add(new Point(100, 80));
                    //var cc = new Color();
                    //cc.A = 50;
                    //cc.R = 30;
                    //cc.G = 150;
                    //cc.B = 240;
                    //Console.WriteLine(pc.ToString());
                    //Node node1 = new Node();
                    //node1.PolyLineColor = System.Drawing.Color.FromArgb(200, 10, 200, 225);
                    //node1.RingColor = System.Drawing.Color.FromArgb(200, 150, 200, 22);
                    //node1.NodeColor = System.Drawing.Color.FromArgb(200, 190, 20, 205);
                    //node1.Radius = 50.00;
                    //node1.PolyLineName = "polylinenamegrtre";
                    //node1.StrokeThickness = 0.2f;
                    //node1.Text = "hi threrefg";
                    //node1.Points = pc.ToString();

                    //Node node2 = new Node();
                    //node2.PolyLineColor = System.Drawing.Color.FromArgb(200, 100, 200, 22);
                    //node2.RingColor = System.Drawing.Color.FromArgb(100, 150, 20, 22);
                    //node2.NodeColor = System.Drawing.Color.FromArgb(250, 10, 20, 205);
                    //node2.Radius = 50.00;
                    //node2.StrokeThickness = 0.1f;
                    //node2.PolyLineName = "polfdfe";
                    //node2.Text = "hi eye";
                    //node2.Points = pc.ToString();

                    //mynode.AddNode(node1);
                    //mynode.AddNode(node2);

                    //mynode.ExtendNode("polfdfe", new Point(500 , 500));
                    //mynode.HideLineNode("polylinenamegrtre");
                    //mynode.RemoveAT("polylinenamegrtre" , 1);
                    //mynode.RemoveAT("polfdfe", 2);
                    //mynode.ShowAllNode("polfdfe");
                    AssignEvent();
                    Load_Node_TextBox();
                    Init_CPU_Location();
                }
                else
                {
                    mynode = new NodePrototype("");
                    mynode.canvas = canvas;
                }
            }
            finally
            {

            }


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Process[] p = Process.GetProcessesByName("KillerProcess", Environment.MachineName);
            if (p.Length > 0)
                p[0].Kill();
            Process cmd = new Process();
            cmd.StartInfo.FileName = @"C:\Users\aanu_\source\repos\KillerProcess\KillerProcess\bin\Debug\netcoreapp3.1\KillerProcess.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
        }

        void Init_CPU_Location()
        {
            if (!string.IsNullOrEmpty(GetAppSettings("CPUPOINT")))
            {
                Point pointcpu = Point.Parse(GetAppSettings("CPUPOINT"));
                Canvas.SetLeft(gMyCPU, pointcpu.X);
                Canvas.SetTop(gMyCPU, pointcpu.Y);
                gMyCPU.Width = Convert.ToDouble(GetAppSettings("CPUWIDTH"));
                gMyCPU.Height = Convert.ToDouble(GetAppSettings("CPUHEIGHT"));
            }
        }

        void Save_CPU_Location()
        {
            double x = Canvas.GetLeft(gMyCPU);
            double y = Canvas.GetTop(gMyCPU);
            SetAppSettings("CPUPOINT", x + "," + y);
            SetAppSettings("CPUWIDTH", gMyCPU.Width.ToString("0.000"));
            SetAppSettings("CPUHEIGHT", gMyCPU.Height.ToString("0.000"));
        }

        string GetAppSettings(string key)
        {

            try
            {
                //for read
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    //Console.WriteLine("AppSettings is empty.");
                    return string.Empty;
                }
                else
                {
                    //var setting = appSettings.AllKeys.Single(k => k==key);
                    return (string)appSettings[key];

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        void SetAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                //Console.WriteLine("Error writing app settings");
            }
        }

        private void Canvas_Move_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (element != null)
            {
                var new_point = e.GetPosition(this.canvas) - uI;
                Canvas.SetLeft(element, new_point.X);
                Canvas.SetTop(element, new_point.Y);
                canvas.CaptureMouse();
                e.Handled = true;
            }

        }

        private void Canvas_PassElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("is control  >>>>>>>>>>>>>>>>>>" + (elementSelected is ContentControl));
            if ((elementSelected is ContentControl))
            {
                LockElement = false;
                elementSelected = null;
            }
            else
            {
                if (LockElement)
                {
                    //Console.WriteLine("pass handle working");
                    element = null;
                    canvas.ReleaseMouseCapture();
                    e.Handled = true;
                }

            }
        }

        private void Canvas_SelectElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("is control  >>>>>>>>>>>>>>>>>>" + (sender is Control) + " control is " + sender.GetType());
            if (LockElement)
            {
                elementSelected = sender;
                element = (UIElement)sender;
                uI = e.GetPosition(element);
                e.Handled = true;
            }

        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveCanvasToFile(this, canvas, 96, saveFileDialog.FileName);
            }

        }

        private void AssignEvent()
        {
            //foreach
            mynode.nodeLists.ForEach((node) =>
            {
                if (node.PointName.Count > 0)
                    node.PointName.ForEach((name) =>
                    {
                        var elipse = canvas.FindName(name) as Grid;//(Ellipse)canvas.FindName("hello0");
                                                                   // Debug.WriteLine("gridename"+elipse.Name);
                        elipse.AddHandler(Grid.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Canvas_SelectElement_PreviewMouseLeftButtonDown));
                    });
            });

        }

        #region propery change
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion 

        #region save window or canvas
        public static void SaveCanvasToFile(Window window, Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(window.Width, window.Height);
            canvas.Measure(size);
            //canvas.Arrange(new Rect(size));

            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width
                (int)window.Height, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);

            SaveRTBAsPNGBMP(rtb, filename);
        }

        public static void SaveWindowToFile(Window window, int dpi, string filename)
        {
            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width
                (int)window.Width, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(window);

            SaveRTBAsPNGBMP(rtb, filename);
        }

        private static void SaveRTBAsPNGBMP(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }
        #endregion save

        private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            LockElement = !LockElement;
            Save_CPU_Location();
        }

        private void Close_App(object sender, MouseButtonEventArgs e) //Button_CloseApp
        {
            mynode.StopHandleItem();
            resource.StopUpdate();
            Application.Current.Shutdown();
        }

        private void Rectangle_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            mynode.StopHandleItem();
        }


        private void Rectangle_MouseLeftButtonDown_6(object sender, MouseButtonEventArgs e)
        {
            this.gMyCPU.Width = this.gMyCPU.Width + 5;
            this.gMyCPU.Height = this.gMyCPU.Width + 5;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.gMyCPU.Width = this.gMyCPU.Width + 5;
            this.gMyCPU.Height = this.gMyCPU.Width + 5;
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.gMyCPU.Width = this.gMyCPU.Width - 5;
            this.gMyCPU.Height = this.gMyCPU.Width - 5;
        }

        private void TextBlock_StopHandle(object sender, MouseButtonEventArgs e)
        {
            mynode.StopHandleItem();
        }

        private void TextBlock_StartHandle(object sender, MouseButtonEventArgs e)
        {
            mynode.RunHandleItem();
        }

        private void Show_Tools(object sender, MouseButtonEventArgs e)
        {
            Rect_btn_show.Visibility = Visibility.Hidden;
            Rect_btn_hide.Visibility = Visibility.Visible;
        }

        private void Hide_Tools(object sender, MouseButtonEventArgs e)
        {
            Rect_btn_show.Visibility = Visibility.Visible;
            Rect_btn_hide.Visibility = Visibility.Hidden;
        }

        private void cmb_data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Node node = mynode.GetNode(Cmb_PolylineName.SelectedItem + "");
            PolyLine_Name.Text = node.PolyLineName;
            LineThinker.Text = node.StrokeThickness.ToString("0.00");
            Strings.Text = node.Text;
            Radius.Text = node.Radius.ToString();
            Ring_Thinker.Text = node.StrokeThickness.ToString("0.00");
            Radial_Color.Text = node.RingColor.ToArgb().ToString();
            Circle_Color.Text = node.NodeColor.ToArgb().ToString();
            Add_Point.Text = "";
            Line_Color.Text = node.PolyLineColor.ToArgb().ToString();
            Program_Name.Text = node.StartProcessName;
            Remove_Point.Text = "";

        }

        private void Btn_Save_Circle_Color_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            var thisBtn = sender as Button;
            mynode.StopHandleItem();
            switch (thisBtn.Content + "")
            {
                case "Circle Color":
                    thisBtn.Background = Color_Selecter.Background;
                    if (ISLockColor.IsChecked != true) mynode.ChangeColorInnerNode(Cmb_PolylineName.SelectedItem + "", thisBtn.Background);
                    break;  //add selectname here
                case "Radial Color":
                    thisBtn.Background = Color_Selecter.Background;
                    if (ISLockColor.IsChecked != true) mynode.ChangeColorOuterNode(Cmb_PolylineName.SelectedItem + "", thisBtn.Background);
                    break;
                case "Line Color":
                    thisBtn.Background = Color_Selecter.Background;
                    if (ISLockColor.IsChecked != true) mynode.ChangeColorPolyLine(Cmb_PolylineName.SelectedItem + "", thisBtn.Background);
                    break;
                case "String Color":
                    thisBtn.Background = Color_Selecter.Background;
                    if (ISLockColor.IsChecked != true) mynode.ChangeColorString(Cmb_PolylineName.SelectedItem + "", thisBtn.Background);
                    break;

            }
            mynode.RunHandleItem();

        }

        private void Remove_Node_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.StopHandleItem();
            int node_number = 0;
            int.TryParse(Remove_Point.Text, out node_number);

            if (node_number != 0)
            {
                mynode.RemoveAT(Cmb_PolylineName.SelectedItem + "", node_number);
            }

        }

        private void Add_Node_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            try
            {
                mynode.StopHandleItem();
                if (!string.IsNullOrEmpty(Add_Point.Text))
                {
                    string[] point_xy = Add_Point.Text.Split(',');
                    if (point_xy.Length < 2) return;
                    Point point = new Point(Convert.ToInt32(point_xy[0]), Convert.ToInt32(point_xy[1]));
                    mynode.ExtendNode(Cmb_PolylineName.SelectedItem + "", point);
                }
                mynode.StopHandleItem();
            }
            finally { mynode.StopHandleItem(); }
            AssignEvent();
        }

        private void Delete_Node_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.StopHandleItem();
            mynode.RemoveAllNode(Cmb_PolylineName.SelectedItem + "");
            mynode.RunHandleItem();
            Load_Node_TextBox();
        }

        private void Hide_Node_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.HideNode(Cmb_PolylineName.SelectedItem + "");
        }

        private void Show_Node_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.ShowAllNode(Cmb_PolylineName.SelectedItem + "");
        }

        private void Ring_Thinker_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            double thinker = 0;
            double.TryParse(Ring_Thinker.Text, out thinker);
            if (thinker != 0)
                mynode.ResizeRingThinker(Cmb_PolylineName.SelectedItem + "", thinker);
        }

        private void Radius_Change_Click(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            double radius = 0;
            double.TryParse(Radius.Text, out radius);
            if (radius != 0)
                mynode.ResizeNode(Cmb_PolylineName.SelectedItem + "", radius);
        }

        private void Change_String(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.EditeString(Cmb_PolylineName.SelectedItem + "", Strings.Text);
        }

        private void Edite_Line_Thinker(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            double Line_thinkness = 0;
            double.TryParse(LineThinker.Text, out Line_thinkness);
            mynode.ResizePolyLine(Cmb_PolylineName.SelectedItem + "", Line_thinkness);
        }

        private void Clear_Form_Click(object sender, RoutedEventArgs e)
        {
            this.Add_Point.Text = "";
            this.Circle_Color.Text = "";
            this.Cmb_PolylineName.SelectedIndex = -1;
            this.ISLockColor.IsChecked = false;
            this.PolyLine_Name.Text = "";
            this.Radial_Color.Text = "";
            this.Radius.Text = "";
            this.Remove_Point.Text = "";
            this.Ring_Thinker.Text = "";
            this.Strings.Text = "";
            this.Program_Name.Text = "";
        }

        private void New_Node_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Node node = new Node();
                node.Points = this.Add_Point.Text;
                node.NodeColor = System.Drawing.Color.FromArgb(Convert.ToInt32(this.Circle_Color.Text));
                node.PolyLineColor = System.Drawing.Color.FromArgb(Convert.ToInt32(this.Line_Color.Text));
                node.RingColor = System.Drawing.Color.FromArgb(Convert.ToInt32(this.Radial_Color.Text));
                node.Text = this.Strings.Text;
                node.Radius = Convert.ToDouble(this.Radius.Text);
                node.PolyLineName = this.PolyLine_Name.Text;
                node.StrokeThickness = Convert.ToDouble(this.Ring_Thinker.Text);
                node.StartProcessName = Program_Name.Text;
                mynode.AssignEvent_Node_Click(node.PolyLineName, node.StartProcessName);
                mynode.AddNode(node);
                AssignEvent();
            }
            finally
            {
                Load_Node_TextBox();
            }

        }

        private void Load_Node_TextBox()
        {
            Cmb_PolylineName.Items.Clear();
            mynode.nodeLists.ForEach((node) =>
            {
                Cmb_PolylineName.Items.Add(node.PolyLineName);
            });
        }

        private void Set_Strart_ProgramName(object sender, RoutedEventArgs e)
        {
            if (Cmb_PolylineName.SelectedIndex == -1) return;
            mynode.AssignEvent_Node_Click(Cmb_PolylineName.SelectedItem + "", Program_Name.Text);
        }

        private void Hide_NodeManager(object sender, MouseButtonEventArgs e)
        {
            if (LockElement)
            {
                Node_Manager.Visibility = Visibility.Visible;
            }
            else
            {
                Node_Manager.Visibility = Visibility.Hidden;
            }
        }

        private void Serailize_Node(object sender, MouseButtonEventArgs e)
        {
            Save_CPU_Location();
            if (!string.IsNullOrEmpty(fileSerialize))
            {
              mynode.SerializeObject();
            }  
        }

        private void RegisterStartup(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.IO.Path.Combine(Environment.CurrentDirectory, "realiu.exe"));
                }
                MessageBox.Show("sucess", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }

        private void UnRegisterStartup(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.DeleteValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, false);
                }
                MessageBox.Show("sucess", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }

        private void Brow_file(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "select your empty text";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            bool? dialogResult = openFileDialog.ShowDialog();
            switch (dialogResult)
            {
                case true:
                    SetAppSettings("FileLocation", openFileDialog.FileName);
                    mynode.ConfigObjectSerialize = openFileDialog.FileName;
                    mynode.LoadNode();
                    break;
                case false:
                    MessageBox.Show("error", "invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default: break;
            }
        }

        private void Unloack_UI(object sender, RoutedEventArgs e)
        {
            bool? cked = ((CheckBox)sender).IsChecked;
            if (cked == true)
            {
                LockElement = true;
            }
            else
            {
                LockElement = false;
            }

        }

        private void Add_Filter_program(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = fillter_name.Text;
                Task.Run(() => Ipc_simple("fillterprogram", "Add:" + text+"<"));
                //t1.Start();
                fillter_name.Text = string.Empty;
                Grid gr = new Grid();
                gr.Height = 30;
                
                ColumnDefinition col1 = new ColumnDefinition();
                ColumnDefinition col2 = new ColumnDefinition();

                GridLength length1 = new GridLength(0.9, GridUnitType.Star);
                GridLength length2 = new GridLength(50, GridUnitType.Pixel);

                col1.Width = length1;
                col2.Width = length2;

                TextBlock tb = new TextBlock();
                Button bt = new Button();
                tb.Text = text;
                tb.FontSize = 18;
                bt.Content = "X";
                bt.FontSize = 18;
                gr.ColumnDefinitions.Add(col1);
                gr.ColumnDefinitions.Add(col2);

                Grid.SetColumn(tb, 0);
                Grid.SetColumn(bt, 1);
                gr.Name = text;
                bt.Click += Bt_Click;
                bt.Tag = add_control.Children.Count; //grid number

                gr.Children.Add(tb);
                gr.Children.Add(bt);
                add_control.Children.Add(gr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                int number = Convert.ToInt32(btn.Tag);
                var grid = add_control.Children[number] as Grid;
                var textBlock = grid.Children[0] as TextBlock;
                var text = textBlock.Text;
                add_control.Children.RemoveAt(number);
                Task.Run(() => Ipc_simple("fillterprogram", "Remove:" + text + "<"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Ipc_simple(string servername, string sendmsg)
        {
            try
            {
                var pipeClient =
                                new NamedPipeClientStream(".", servername,
                                PipeDirection.InOut, PipeOptions.None,
                                TokenImpersonationLevel.Impersonation);

                pipeClient.Connect(timeout: 3000);
                if (sendmsg.Length > 32000) throw new Exception("arg out of rage");
                int count = sendmsg.Length;
                byte[] ar = new byte[count];
                ar = Encoding.UTF8.GetBytes(sendmsg);
                pipeClient.Write(ar, 0, count);
                pipeClient.Flush();
                pipeClient.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

}
