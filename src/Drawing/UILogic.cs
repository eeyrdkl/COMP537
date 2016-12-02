using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.UI.Input.Inking;

namespace Drawing
{
    public static class UILogic
    {
        //Settings
        public static int TimeoutMS = 1500;
        private static int TranslateX = 300;
        private static int TranslateY = 300;
        private static int PenWidth = 5;
        private static int PenHeight = 5;
        private static Color PenColor = Colors.Black;
        //Variables
        private static MLApp.MLApp m_Matlab = new MLApp.MLApp();
        private static string m_Pwd = "";
        private static BackgroundWorker m_Thread = new BackgroundWorker();
        private static MainWindow m_Window = null;
        public static bool m_RecognitionModeIsHandwriting = false;
        public static InkManager m_InkRecognizer = new InkManager();
        //Function
        public static void InitGestureRecognition(string path, MainWindow window)
        {
            m_Matlab.Visible = 0;
            m_Pwd = path;
            m_Window = window;

            if (m_Window.m_InkCanvas.IsGestureRecognizerAvailable)
            {
                m_Window.m_InkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
                m_Window.m_InkCanvas.Gesture += new InkCanvasGestureEventHandler(InkCanvasGestureHandler);
                m_Window.m_InkCanvas.SetEnabledGestures(new ApplicationGesture[]
                {
                    ApplicationGesture.ChevronUp,
                    ApplicationGesture.ChevronDown,
                    ApplicationGesture.ChevronLeft,
                    ApplicationGesture.ChevronRight,
                    ApplicationGesture.Check,
                });
                m_Window.m_InkCanvas.DefaultDrawingAttributes.Width = PenWidth;
                m_Window.m_InkCanvas.DefaultDrawingAttributes.Height = PenHeight;
                m_Window.m_InkCanvas.DefaultDrawingAttributes.Color = PenColor;
            }

            m_Thread.DoWork += M_Thread_DoWork;
            m_Thread.RunWorkerAsync();          
        }
        private static void InkCanvasGestureHandler(object sender, InkCanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> gestureResults = e.GetGestureRecognitionResults();

            if (gestureResults[0].RecognitionConfidence != RecognitionConfidence.Poor)
            {
                switch (gestureResults[0].ApplicationGesture)
                {
                    case ApplicationGesture.ChevronUp:
                        m_Window.m_Translate.Y += TranslateY;
                        break;
                    case ApplicationGesture.ChevronDown:
                        m_Window.m_Translate.Y -= TranslateY;
                        break;
                    case ApplicationGesture.ChevronLeft:
                        m_Window.m_Translate.X += TranslateX;
                        break;
                    case ApplicationGesture.ChevronRight:
                        m_Window.m_Translate.X -= TranslateX;
                        break;
                    case ApplicationGesture.Check:
                        m_RecognitionModeIsHandwriting = !m_RecognitionModeIsHandwriting;
                        break;
                }
            }
        }
        public static void ClearInkCanvas()
        {
            if (m_Window.m_InkCanvas == null)
                return;

            m_Window.m_InkCanvas.Children.Clear();
            m_Window.m_InkCanvas.Strokes.Clear();

            if (m_Window.m_Suggestions == null)
                return;

            m_Window.m_Suggestions.Visibility = Visibility.Hidden;
        }
        public static void EnterDrawingMode()
        {
            if (m_Window.m_InkCanvas == null)
                return;

            m_Window.m_InkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
        }
        public static void EnterEraserMode()
        {
            if (m_Window.m_InkCanvas == null)
                return;

            m_Window.m_InkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }
        public static void ExportBitmap()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Class Diagram";
            sfd.DefaultExt = ".bmp";
            sfd.Filter = "Bitmap (.bmp)|*.bmp";

            Nullable<bool> result = sfd.ShowDialog();

            if (result != true)
                return;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)m_Window.m_InkCanvas.ActualWidth, (int)m_Window.m_InkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(m_Window.m_InkCanvas);
                dc.DrawRectangle(vb, null, new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(

                    m_Window.m_InkCanvas.ActualWidth, m_Window.m_InkCanvas.ActualHeight)));

                dc.Close();
                rtb.Render(dv);
            }

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            FileStream fs = File.Open(sfd.FileName, FileMode.Create);
            encoder.Save(fs);
            fs.Close();
        }
        public static double[] RecognizeSketch(string filename, out int predictedClass)
        {
            object result = null;
            predictedClass = 0;

            m_Matlab.Execute(@"cd '" + m_Pwd + "\\Data'");
            m_Matlab.Feval("predictClass", 2, out result, filename);

            object[] res = result as object[];

            if (res == null || res.Length < 2)
                return null;

            //Results
            //1=class, 2=imp, 3=inh
            predictedClass = (int)((double)res[0]);
            double[] probs = new double[3];

            double[,] p = (double[,])res[1];
            for (int i=0; i<p.GetLength(1); i++)
            {
                probs[i] = p[0,i];
            }
            return probs;
        }
        private static void M_Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (m_RecognitionModeIsHandwriting)
                    continue;

                if ((DateTime.Now - m_Window.time).TotalMilliseconds >= TimeoutMS && m_Window.time != DateTime.MinValue)
                {
                    m_Window.Dispatcher.Invoke(new Action(() =>
                        {
                            m_Window.recognize();
                            m_Window.sketch.Clear();
                            m_Window.time = DateTime.MinValue;
                        }));
                }
            }
        }
        public static void FillSuggestions(double[] probs, Image b1, Image b2, Image b3)
        {
            m_Window.m_Suggestions.Visibility = System.Windows.Visibility.Visible;

            Dictionary<double, string> map = new Dictionary<double, string>();
            List<double> p = new List<double>();

            map.Add(probs[0], "Class");
            map.Add(probs[1], "Implementation");
            map.Add(probs[2], "Inheritance");

            p.Add(probs[0]);
            p.Add(probs[1]);
            p.Add(probs[2]);

            p.Sort();

            string c1 = "";
            string c2 = "";
            string c3 = "";
            map.TryGetValue(p[2], out c1);
            map.TryGetValue(p[1], out c2);
            map.TryGetValue(p[0], out c3);

            switch (c1)
            {
                case "Class":
                    b1.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Class Icon.png"));
                    break;
                case "Implementation":
                    b1.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Implementation Icon.png"));
                    break;
                case "Inheritance":
                    b1.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Inheritance Icon.png"));
                    break;
            }

            switch (c2)
            {
                case "Class":
                    b2.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Class Icon.png"));
                    break;
                case "Implementation":
                    b2.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Implementation Icon.png"));
                    break;
                case "Inheritance":
                    b2.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Inheritance Icon.png"));
                    break;
            }

            switch (c3)
            {
                case "Class":
                    b3.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Class Icon.png"));
                    break;
                case "Implementation":
                    b3.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Implementation Icon.png"));
                    break;
                case "Inheritance":
                    b3.Source = new BitmapImage(new Uri(@"pack://application:,,,/Icons/Inheritance Icon.png"));
                    break;
            }

            m_Window.m_InkCanvas.Strokes.Erase(m_Window.sketch.GetBounds());

            Border brd = new Border();
            brd.Width = m_Window.sketch.GetBounds().Width;
            brd.Height = m_Window.sketch.GetBounds().Height;
            brd.Margin = new Thickness(m_Window.sketch.GetBounds().Left,
            m_Window.sketch.GetBounds().Top, m_Window.sketch.GetBounds().Right, m_Window.sketch.GetBounds().Bottom);
            Image im = new Image();
            brd.Child = im;
            im.Source = b1.Source;
            im.Stretch = Stretch.Fill;
            m_Window.m_InkCanvas.Children.Add(brd);

            lastposition = brd.Margin;
            lastwidth = brd.Width;
            lastheight = brd.Height;
        }
        public static Thickness lastposition = new Thickness();
        public static double lastwidth = 0;
        public static double lastheight = 0;
    }
}
