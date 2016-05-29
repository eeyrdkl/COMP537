using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Drawing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UILogic.InitGestureRecognition(System.Environment.CurrentDirectory, this);
        }
        private void ClearInkCanvasButton_Click(object sender, RoutedEventArgs e)
        {
            UILogic.ClearInkCanvas();
        }
        private void DrawingModeButton_Click(object sender, RoutedEventArgs e)
        {
            UILogic.EnterDrawingMode();
        }
        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            UILogic.EnterEraserMode();
        }
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            UILogic.ExportBitmap();
        }
        public StrokeCollection sketch = new StrokeCollection();
        public DateTime time = DateTime.MinValue;
        private void m_InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            if (UILogic.m_RecognitionModeIsHandwriting)
                return;

            sketch.Add(e.Stroke);
            time = DateTime.Now;
        }
        public void recognize()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)m_InkCanvas.ActualWidth, (int)m_InkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(m_InkCanvas);
                dc.DrawRectangle(vb, null, new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(

                    m_InkCanvas.ActualWidth, m_InkCanvas.ActualHeight)));

                dc.Close();
                rtb.Render(dv);
            }

            int x = (int)(sketch.GetBounds().TopLeft.X);
            int y = (int)Math.Min(sketch.GetBounds().TopLeft.Y, sketch.GetBounds().TopRight.Y);
            int width = (int)(sketch.GetBounds().TopRight.X - sketch.GetBounds().TopLeft.X);
            int height = (int)(sketch.GetBounds().BottomLeft.Y - sketch.GetBounds().TopLeft.Y);
            Int32Rect rect = new Int32Rect(x, y, width, height);

            BitmapSource bs = new CroppedBitmap(rtb, rect);
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bs));

            FileStream fs = File.Open("Data\\Test.bmp", FileMode.Create);
            encoder.Save(fs);
            fs.Close();

            int pClass;
            double[] probs = UILogic.RecognizeSketch("Data\\Test.bmp", out pClass);

            UILogic.FillSuggestions(probs, m_B1, m_B2, m_B3);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_InkCanvas.Children.RemoveAt(m_InkCanvas.Children.Count - 1);

            Border brd = new Border();
            brd.Width = UILogic.lastwidth;
            brd.Height = UILogic.lastheight;
            brd.Margin = UILogic.lastposition;
            Image im = new Image();
            brd.Child = im;
            im.Source = m_B1.Source;
            im.Stretch = Stretch.Fill;
            m_InkCanvas.Children.Add(brd);

            m_Suggestions.Visibility = Visibility.Hidden;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            m_InkCanvas.Children.RemoveAt(m_InkCanvas.Children.Count - 1);
            Border brd = new Border();
            brd.Width = UILogic.lastwidth;
            brd.Height = UILogic.lastheight;
            brd.Margin = UILogic.lastposition;
            Image im = new Image();
            brd.Child = im;
            im.Source = m_B2.Source;
            im.Stretch = Stretch.Fill;
            m_InkCanvas.Children.Add(brd);

            m_Suggestions.Visibility = Visibility.Hidden;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            m_InkCanvas.Children.RemoveAt(m_InkCanvas.Children.Count - 1);
            Border brd = new Border();
            brd.Width = UILogic.lastwidth;
            brd.Height = UILogic.lastheight;
            brd.Margin = UILogic.lastposition;
            Image im = new Image();
            brd.Child = im;
            im.Source = m_B3.Source;
            im.Stretch = Stretch.Fill;
            m_InkCanvas.Children.Add(brd);

            m_Suggestions.Visibility = Visibility.Hidden;
        }
    }
}
