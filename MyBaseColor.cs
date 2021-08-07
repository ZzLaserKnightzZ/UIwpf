

namespace realiu
{

    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Media;

    class MyBaseColor : INotifyPropertyChanged
    {
        private int RED, GREEN, BLUE, OPACITY;
        private bool _visible = true;
        public int MaxValue { get { return 255; } }
        public bool Visible { get { return _visible; } set { _visible = value; OnPropertyChanged(nameof(Visible)); } }
        public int MinValue { get { return 0; } }

        public int Red
        {
            get { return RED; }
            set { RED = value; Debug.WriteLine(RED); OnPropertyChanged(nameof(Red)); OnPropertyChanged(nameof(GetColor)); }
        }
        public int Green
        {
            get { return GREEN; }
            set { GREEN = value; Debug.WriteLine(BLUE); OnPropertyChanged(nameof(GREEN)); OnPropertyChanged(nameof(GetColor)); }
        }
        public int Blue
        {
            get { return BLUE; }
            set { BLUE = value; Debug.WriteLine(GREEN); OnPropertyChanged(nameof(BLUE)); OnPropertyChanged(nameof(GetColor)); }
        }
        public int Opacity
        {
            get { return OPACITY; }
            set { OPACITY = value; Debug.WriteLine(GREEN); OnPropertyChanged(nameof(OPACITY)); OnPropertyChanged(nameof(GetColor)); }
        }

        public SolidColorBrush GetColor
        {
            get { return new SolidColorBrush(Color.FromArgb((byte)this.OPACITY, (byte)this.RED, (byte)this.GREEN, (byte)this.BLUE)); }


        }

        #region notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);

                handler(this, e);
            }
        }
        #endregion

    }
}
