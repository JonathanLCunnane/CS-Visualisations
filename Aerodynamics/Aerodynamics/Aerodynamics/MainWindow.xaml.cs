using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace Aerodynamics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point3D> geometricVertices;
        List<Point3D> faces;

        Viewport3D viewport;

        GeometryModel3D model = new GeometryModel3D();
        List<int> facePoints;
        ModelVisual3D mv3d = new ModelVisual3D();

        int maxRotationSpeed = 4;
        float distanceToCentre = 10;
        Point3D centre = new Point3D(0, 0, 0);
        Point previousMousePoint;

        DispatcherTimer dt;

        bool playing = false;
        List<double> currentDensity;
        List<double> previousDensity;
        List<Vector3D> currentVelocity;
        List<Vector3D> previousVelocity;
        List<bool> occupiedCells;
        
        Vector3D simDimensions = new Vector3D(0, 0, 0);
        int simResolution = 100;

        Regex numericRegex = new Regex("[0-9]");

        public MainWindow()
        {
            InitializeComponent();
            viewport = this.scene;

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            this.MouseWheel += new MouseWheelEventHandler(MainWindow_MouseWheel);
            this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);
            this.MouseDown += new MouseButtonEventHandler(MainWindow_MouseDown);

            dt = new DispatcherTimer();
            dt.Tick += new EventHandler(SimTick);
            dt.Interval = TimeSpan.FromMilliseconds(10);

            this.xLightDirection.Text = this.light.Direction.X.ToString();
            this.yLightDirection.Text = this.light.Direction.Y.ToString();
            this.zLightDirection.Text = this.light.Direction.Z.ToString();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //Key camera controls.
            if(e.Key == Key.Right || e.Key == Key.NumPad6)
            {
                RotateCam(mainCam, mainCam.UpDirection, -1, centre);
            }
            else if(e.Key == Key.Left || e.Key == Key.NumPad4)
            {
                RotateCam(mainCam, mainCam.UpDirection, 1, centre);
            }
            else if(e.Key == Key.Up || e.Key == Key.NumPad8)
            {
                RotateCam(mainCam, Vector3D.CrossProduct(mainCam.UpDirection, mainCam.LookDirection), -1, centre);
            }
            else if(e.Key == Key.Down || e.Key == Key.NumPad2)
            {
                RotateCam(mainCam, Vector3D.CrossProduct(mainCam.UpDirection, mainCam.LookDirection), 1, centre);
            }
            else if (e.Key == Key.NumPad9)
            {
                RotateCam(mainCam, mainCam.LookDirection, -1, centre);
            }
            else if (e.Key == Key.NumPad7)
            {
                RotateCam(mainCam, mainCam.LookDirection, 1, centre);
            }

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Mouse wheel camera controls.
            distanceToCentre += -e.Delta * 0.01f;
            if (distanceToCentre < 2)
            {
                distanceToCentre = 2;
            }
            else if(distanceToCentre > 30)
            {
                distanceToCentre = 30;
            }
            CameraPositionScroll(distanceToCentre, centre, mainCam);
            
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            //Mouse button and movement camera controls.
            if(e.RightButton == MouseButtonState.Pressed)
            {
                if(previousMousePoint != new Point())
                {
                    Vector delta = e.GetPosition(this) - previousMousePoint;
                    if (delta.Y > maxRotationSpeed)
                    {
                        delta.Y = maxRotationSpeed;
                    }
                    else if (delta.Y < -maxRotationSpeed)
                    {
                        delta.Y = -maxRotationSpeed;
                    }
                    if (delta.X > maxRotationSpeed)
                    {
                        delta.X = maxRotationSpeed;
                    }
                    else if (delta.X < -maxRotationSpeed)
                    {
                        delta.X = -maxRotationSpeed;
                    }
                    Vector3D vu = mainCam.UpDirection;
                    Vector3D vf = mainCam.LookDirection;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        //Panning
                        double speed = 0.05;
                        vu.Normalize();
                        vf.Normalize();
                        TranslateTransform3D tt3d = new TranslateTransform3D(Vector3D.Multiply(vu, delta.Y * speed) + Vector3D.Multiply(Vector3D.CrossProduct(vu, vf), delta.X * speed));
                        centre = tt3d.Transform(centre);
                        mainCam.Position = tt3d.Transform(mainCam.Position);
                    }
                    else
                    {
                        //Rotation
                        RotateCam(mainCam, Vector3D.CrossProduct(vf,vu), -delta.Y, centre);
                        RotateCam(mainCam, new Vector3D(0,1,0), -delta.X, centre);
                        VerticalRotationLimiter(mainCam, 80);
                    }
                }
                previousMousePoint = e.GetPosition(this);
                
            }
            else if (previousMousePoint != new Point())
            {
                previousMousePoint = new Point();
            }
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if((Math.Abs(Mouse.GetPosition(this.ParamListBox).X - 150) > 150 || Math.Abs(Mouse.GetPosition(this.ParamListBox).Y - 35) > 35) && this.ParamListBox.Visibility == Visibility.Visible)
            {
                this.ParamListBox.Visibility = Visibility.Collapsed;
            }
            if((Math.Abs(Mouse.GetPosition(this.LightPositionBox).X - 150) > 150 || Math.Abs(Mouse.GetPosition(this.LightPositionBox).Y - 19) > 19) && this.LightPositionBox.Visibility == Visibility.Visible)
            {
                this.LightPositionBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenFileExplorer(object sender, RoutedEventArgs e)
        {
            this.ParamListBox.Visibility = Visibility.Collapsed;
            this.LightPositionBox.Visibility = Visibility.Collapsed;

            //File opening
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wavefront files (*.obj)|*.obj|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                geometricVertices = new List<Point3D>();
                faces = new List<Point3D>();
                facePoints = new List<int>();
                //normals = "";
                //textureVertices = "";
                viewport.Children.Remove(mv3d);

                //Opening .obj file format
                foreach (string line in File.ReadLines(openFileDialog.FileName))
                {
                    string lineTag = line.Substring(0, 2);
                    if (lineTag == "v ")
                    {
                        //"geometric vertices"

                        string[] lineArray = line.Remove(0, 2).Split(' ');
                        geometricVertices.Add(new Point3D(double.Parse(lineArray[0]), double.Parse(lineArray[1]), double.Parse(lineArray[2])));
                    }
                    else if (lineTag == "f ")
                    {
                        //"faces"
                        List<int> list = new List<int>();
                        foreach (string s in line.Remove(0, 2).Split(' '))
                        {
                            list.Add(int.Parse(s.Split('/')[0]));
                        }
                        for (int i = 0; i <= list.Count - 3; i++)
                        {
                            facePoints.Add(list[0]);
                            facePoints.Add(list[i + 1]);
                            facePoints.Add(list[i + 2]);
                        }

                    }
                    else if (lineTag == "vt")
                    {
                        //"texture vertices"
                        //textureVertices += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "vn")
                    {
                        //"vertex normals"
                        //normals += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "vp")
                    {
                        //"parameter space vertices"
                    }

                }
                foreach (int x in facePoints)
                {
                    faces.Add(geometricVertices[x - 1]);
                }
                model = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D() { Positions = new Point3DCollection(faces) },
                    Material = new DiffuseMaterial(Brushes.LightGray),
                };

                //DirectionalLight directLight = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1));
                //PerspectiveCamera cam = new PerspectiveCamera(new Point3D(5, 5, 5), new Vector3D(-1, -1, -1), new Vector3D(1, 1, 1), 60);
                mv3d = new ModelVisual3D() { Content = model };
                viewport.Children.Add(mv3d);

                Vector3D maxBound = new Vector3D(0, 0, 0);
                Vector3D minBound = new Vector3D(0, 0, 0);
                foreach(Point3D point in geometricVertices)
                {
                    if(point.X > maxBound.X)
                    {
                        maxBound.X = point.X;
                    }
                    if(point.Y > maxBound.Y)
                    {
                        maxBound.Y = point.Y;
                    }
                    if(point.Z > maxBound.Z)
                    {
                        maxBound.Z = point.Z;
                    }
                    if(point.X < minBound.X)
                    {
                        minBound.X = point.X;
                    }
                    if(point.Y < minBound.Y)
                    {
                        minBound.Y = point.Y;
                    }
                    if(point.Z < minBound.Z)
                    {
                        minBound.Z = point.Z;
                    }
                }
                viewport.Children[viewport.Children.IndexOf(mv3d)].Transform = new TranslateTransform3D(Vector3D.Multiply(maxBound+minBound, -0.5));
                simDimensions = new Vector3D(Math.Round(maxBound.X - minBound.X) + 3, Math.Round(maxBound.Y - minBound.Y) + 3, Math.Round(maxBound.Z - minBound.Z) + 3);
                simResolution = 100;
                this.xDimension.Text = simDimensions.X.ToString();
                this.yDimension.Text = simDimensions.Y.ToString();
                this.zDimension.Text = simDimensions.Z.ToString();
                this.Resolution.Text = simResolution.ToString();
            }
        }

        void ResetViewpoint(object sender, RoutedEventArgs e)
        {
            this.ParamListBox.Visibility = Visibility.Collapsed;
            this.LightPositionBox.Visibility = Visibility.Collapsed;

            mainCam.Position = new Point3D(10, 0, 0);
            mainCam.LookDirection = new Vector3D(-1, 0, 0);
            mainCam.UpDirection = new Vector3D(0, 1, 0);
            centre = new Point3D(0, 0, 0);
            distanceToCentre = 10;
        }

        void PausePlaySim(object sender, RoutedEventArgs e)
        {
            this.ParamListBox.Visibility = Visibility.Collapsed;
            this.LightPositionBox.Visibility = Visibility.Collapsed;

            Image image = (Image)this.PlayStopSim.Content;
            if (playing)
            {
                image.Source = new BitmapImage( new Uri(@"\Icons\StartWithoutDebug\StartWithoutDebug_16x.png", UriKind.Relative));
                image.ToolTip = "Play Sim";
                dt.Stop();
            }
            else
            {
                image.Source = new BitmapImage(new Uri(@"\Icons\Pause\Pause_16x.png", UriKind.Relative));
                image.ToolTip = "Stop Sim";
                SimBegin();
                dt.Start();
            }
            playing = !playing;
        }

        void PositionLightBox(object sender, RoutedEventArgs e)
        {
            this.ParamListBox.Visibility = Visibility.Collapsed;

            if (this.LightPositionBox.Visibility == Visibility.Collapsed)
            {
                this.LightPositionBox.Visibility = Visibility.Visible;
            }
            else
            {
                this.LightPositionBox.Visibility = Visibility.Collapsed;
            }
        }

        void ParamBox(object sender, RoutedEventArgs e)
        {
            this.LightPositionBox.Visibility = Visibility.Collapsed;

            if (this.ParamListBox.Visibility == Visibility.Collapsed)
            {
                this.ParamListBox.Visibility = Visibility.Visible;
            }
            else
            {
                this.ParamListBox.Visibility = Visibility.Collapsed;
            }
        }

        void NumericInputSanitisation(object sender, TextCompositionEventArgs e)
        {
            TextBox tb= (TextBox)sender;
            if(numericRegex.IsMatch(tb.Text + e.Text) || ((tb.Text+e.Text).Substring(0,1) == "-" && ((tb.Text+e.Text).Length == 1 || numericRegex.IsMatch((tb.Text + e.Text).Substring(1)))))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        void SetXLightDirection(object sender, TextChangedEventArgs e)
        {
            if(this.xLightDirection.Text.Length > 0 && this.xLightDirection.Text != "-")
            {
                this.light.Direction = new Vector3D(int.Parse(this.xLightDirection.Text), this.light.Direction.Y, this.light.Direction.Z);
            }
        }
        void SetYLightDirection(object sender, TextChangedEventArgs e)
        {
            if(this.yLightDirection.Text.Length > 0 && this.yLightDirection.Text != "-")
            {
                this.light.Direction = new Vector3D(this.light.Direction.X, int.Parse(this.yLightDirection.Text), this.light.Direction.Z);
            }
        }
        void SetZLightDirection(object sender, TextChangedEventArgs e)
        {
            if(this.zLightDirection.Text.Length > 0 && this.zLightDirection.Text != "-")
            {
                this.light.Direction = new Vector3D(this.light.Direction.X, this.light.Direction.Y, int.Parse(this.zLightDirection.Text));
            }
        }

        void SetSimXDimension(object sender, TextChangedEventArgs e)
        {
            if(this.xDimension.Text.Length > 0 && this.xDimension.Text != "")
            {
                int value = int.Parse(this.xDimension.Text);
                if (value > simDimensions.X)
                {
                    simDimensions.X = value;
                }
                
            }
        }
        void SetSimYDimension(object sender, TextChangedEventArgs e)
        {
            
            if (this.yDimension.Text.Length > 0 && this.yDimension.Text != "")
            {
                int value = int.Parse(this.yDimension.Text);
                if (value > simDimensions.Y)
                {
                    simDimensions.Y = value;
                }
            }
        }
        void SetSimZDimension(object sender, TextChangedEventArgs e)
        {
            if (this.zDimension.Text.Length > 0 && this.zDimension.Text != "")
            {
                int value = int.Parse(this.zDimension.Text);
                if (value > simDimensions.Z)
                {
                    simDimensions.Z = value;
                }
            }
        }

        void SetSimResolution(object sender, TextChangedEventArgs e)
        {
           
            if (this.Resolution.Text.Length > 0 && this.Resolution.Text != "")
            {
                int value = int.Parse(this.Resolution.Text);
                if (value > 0)
                {
                    simResolution = value;
                }
            }
        }

        int IndexReturn(int x, int y, int z, Vector3D dimensions)
        {
            return (int)(x + (dimensions.Y + 2) * simResolution * y + (dimensions.Z + 2) * simResolution * z );
        }

        void RotateCam(PerspectiveCamera cam, Vector3D axis, double degrees, Point3D rotateAround)
        {
            RotateTransform3D rot = new RotateTransform3D(new AxisAngleRotation3D(axis, degrees), rotateAround);
            cam.Position = rot.Transform(cam.Position);
            rot.CenterX = 0;
            rot.CenterY = 0;
            rot.CenterZ = 0;
            Point3D p = rot.Transform(new Point3D(cam.UpDirection.X, cam.UpDirection.Y, cam.UpDirection.Z));
            cam.UpDirection = new Vector3D(p.X, p.Y, p.Z);
            cam.LookDirection = new Vector3D(centre.X-cam.Position.X, centre.Y-mainCam.Position.Y, centre.Z-mainCam.Position.Z);
        }
    
        void CameraPositionScroll(float distToCentre, Point3D centrePos, PerspectiveCamera cam)
        {
            Vector3D l = -cam.LookDirection;
            l.Normalize();
            cam.Position = new TranslateTransform3D(Vector3D.Multiply(l, distToCentre)).Transform(centrePos);
        }

        void VerticalRotationLimiter(PerspectiveCamera c, float limitingValue)
        {
            double degrees = 180 / Math.PI * Math.Atan( c.LookDirection.Y / Math.Pow(Math.Pow(c.LookDirection.X, 2) + Math.Pow(c.LookDirection.Z, 2), 0.5));
            if ( degrees < -limitingValue)
            {
                RotateCam(c, Vector3D.CrossProduct(c.LookDirection, c.UpDirection), -(degrees + limitingValue), centre);
            }
            else if (degrees > limitingValue)
            {
                RotateCam(c, Vector3D.CrossProduct(c.LookDirection, c.UpDirection), -(degrees - limitingValue), centre);
            }
        }

        void SimBegin()
        {
            currentDensity = new List<double>((int)((simDimensions.X + 2) * (simDimensions.Y + 2) * (simDimensions.Z + 2) * Math.Pow(simResolution, 3)));
            currentVelocity = new List<Vector3D>((int)((simDimensions.X + 2) * (simDimensions.Y + 2) * (simDimensions.Z + 2) * Math.Pow(simResolution, 3)));
            occupiedCells = new List<bool>((int)((simDimensions.X + 2) * (simDimensions.Y + 2) * (simDimensions.Z + 2) * Math.Pow(simResolution, 3)));
            MeshGeometry3D mg3d = (MeshGeometry3D)((GeometryModel3D)mv3d.Content).Geometry;
        }
        void SimTick(object sender, EventArgs e)
        {
            
        }
    }
}

