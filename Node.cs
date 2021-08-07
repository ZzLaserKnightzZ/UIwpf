


namespace realiu
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Media;
    using Dcolor = System.Drawing.Color;

    class Node : INotifyPropertyChanged
    {

        #region private
        private Dcolor _polylineColor, _textColor, _nodeColor, _ringColor;
        private string _text, _polylineName, _points, _startProcessName;
        private double _strokeThinkness, _radius;
        private int _nodeNumber;
        #endregion

        public bool Hiding { get; set; }
        public string StartProcessName { get { return _startProcessName; } set { _startProcessName = value; } }

        public Dcolor PolyLineColor { get { return _polylineColor; } set { _polylineColor = value; NotifyPropertyChanged(nameof(PolyLineColor)); } }
        public SolidColorBrush SoliteLineColor { get { return new SolidColorBrush(Color.FromArgb(PolyLineColor.A, PolyLineColor.R, PolyLineColor.G, PolyLineColor.B)); } }
        public double StrokeThickness { get { return _strokeThinkness; } set { _strokeThinkness = value; NotifyPropertyChanged(nameof(StrokeThickness)); } }

        public Dcolor TextColor { get { return _textColor; } set { _textColor = value; NotifyPropertyChanged(nameof(TextColor)); } }
        public SolidColorBrush SoliteTextColor { get { return new SolidColorBrush(Color.FromArgb(TextColor.A, TextColor.R, TextColor.G, TextColor.B)); } }
        public string Text { get { return _text; } set { _text = value; NotifyPropertyChanged(nameof(Text)); } }

        public Dcolor RingColor { get { return _ringColor; } set { _ringColor = value; NotifyPropertyChanged(nameof(RingColor)); } }
        public SolidColorBrush SoliteRingColor { get { return new SolidColorBrush(Color.FromArgb(RingColor.A, RingColor.R, RingColor.G, RingColor.B)); } }

        public Dcolor NodeColor { get { return _nodeColor; } set { _nodeColor = value; NotifyPropertyChanged(nameof(NodeColor)); } }
        public SolidColorBrush SoliteNodeColor { get { return new SolidColorBrush(Color.FromArgb(NodeColor.A, NodeColor.R, NodeColor.G, NodeColor.B)); } }
        public double Radius { get { return _radius; } set { _radius = value; NotifyPropertyChanged(nameof(Radius)); } }
        public int NodeNumber { get { return _nodeNumber; } set { _nodeNumber = value; NotifyPropertyChanged(nameof(NodeNumber)); } }
        public string PolyLineName { get { return _polylineName; } set { _polylineName = value; NotifyPropertyChanged(nameof(PolyLineName)); } }
        public List<string> PointName = new List<string>();
        public string Points { get { return _points; } set { _points = value; NotifyPropertyChanged(nameof(Points)); } }
        public ObservableCollection<string> Get_PointName
        {
            get
            {
                var obc = new ObservableCollection<string>();
                this.PointName.ForEach((str_name) =>
                {
                    obc.Add(str_name);
                });
                return obc;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

}
