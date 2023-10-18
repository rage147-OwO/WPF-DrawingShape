using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp3
{
    internal class VIewModel : INotifyPropertyChanged
    {
        #region Properties_for_Binding

        private double _pX;
        public double PX
        {
            get { return _pX; }
            set
            {
                if (_pX != value)
                {
                    _pX = value;
                    OnPropertyChanged(nameof(PX));
                }
            }
        }
        private double _pY;
        public double PY
        {
            get { return _pY; }
            set
            {
                if (_pY != value)
                {
                    _pY = value;
                    OnPropertyChanged(nameof(PY));
                }
            }
        }
        #endregion



        //for MouseMove
        private double EllipseWidthHalf;
        private double EllipseHeightHalf;

        //for MouseClick
        private Canvas canvas;
        private List<ellipseData> ellipseData = new List<ellipseData>();


        public VIewModel(Ellipse ellipse, Canvas canvas)
        {
            EllipseWidthHalf = ellipse.Width / 2;
            EllipseHeightHalf = ellipse.Height / 2;


            Debug.WriteLine("VIewModel is created" + EllipseWidthHalf);
            this.canvas = canvas;
        }

        //MouseMoveHandler, 마우스 커서 따라 이동
        public void MouseMoveHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition((IInputElement)sender);
            PX = p.X - EllipseWidthHalf;
            PY = p.Y - EllipseHeightHalf;

            //Debug.WriteLine("MouseMoveHandler is called in ViewModel"+ PX+""+PY);
        }




        //MouseClickHandler, 마우스 Down시
        public void MouseDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition((IInputElement)sender);
            if (ellipseData.Count == 0 || ellipseData[ellipseData.Count - 1].isDrawing == false)
            {
                ellipseData.Add(new ellipseData(p.X, p.Y, canvas));
            }
            else
            {
                ellipseData[ellipseData.Count - 1].createEllipse(p.X, p.Y);
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void
            OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Debug.WriteLine(propertyName + " was set");
        }
    }


}
