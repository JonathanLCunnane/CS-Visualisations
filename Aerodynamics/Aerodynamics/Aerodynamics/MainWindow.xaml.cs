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

namespace Aerodynamics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point3D> geometricVertices;
        List<Point3D> faces;
        //string normals;
        //string textureVertices;
        //Model3DGroup model3DGroup = new Model3DGroup();
        //MeshGeometry3D mesh = new MeshGeometry3D();
        Viewport3D viewport;
        GeometryModel3D model = new GeometryModel3D();
        List<int> facePoints;
        ModelVisual3D mv3d = new ModelVisual3D();
        int maxRotationSpeed = 4;
        float distanceToCentre = 10;
        Point3D centre = new Point3D(0, 0, 0);
        Point previousMousePoint;
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            this.MouseWheel += new MouseWheelEventHandler(MainWindow_MouseWheel);
            this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
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
                        double speed = 0.05;
                        vu.Normalize();
                        vf.Normalize();
                        TranslateTransform3D tt3d = new TranslateTransform3D(Vector3D.Multiply(vu, delta.Y * speed) + Vector3D.Multiply(Vector3D.CrossProduct(vu, vf), delta.X * speed));
                        centre = tt3d.Transform(centre);
                        mainCam.Position = tt3d.Transform(mainCam.Position);
                    }
                    else
                    {
                        RotateCam(mainCam, new Vector3D(0,1,0), -delta.X, centre);
                        RotateCam(mainCam, Vector3D.CrossProduct(vf,vu), -delta.Y, centre);
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

        private void OpenFileExplorer(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wavefront files (*.obj)|*.obj|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                geometricVertices = new List<Point3D>();
                faces = new List<Point3D>();
                facePoints = new List<int>();
                //normals = "";
                //textureVertices = "";
                viewport = this.scene;
                viewport.Children.Remove(mv3d);

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
                        foreach(string s in line.Remove(0, 2).Split(' '))
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
                foreach(int x in facePoints)
                {
                    faces.Add(geometricVertices[x-1]);
                }
                model = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D() {Positions = new Point3DCollection(faces)},
                    Material = new DiffuseMaterial(Brushes.LightGray),
                };

                //DirectionalLight directLight = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1));
                //PerspectiveCamera cam = new PerspectiveCamera(new Point3D(5, 5, 5), new Vector3D(-1, -1, -1), new Vector3D(1, 1, 1), 60);
                mv3d = new ModelVisual3D() { Content = model };
                viewport.Children.Add(mv3d);                
            }
        }

        void ResetViewpoint(object sender, RoutedEventArgs e)
        {

        }
        void RotateCam(PerspectiveCamera cam, Vector3D axis, double degrees, Point3D rotateAround)
        {
            RotateTransform3D rot = new RotateTransform3D(new AxisAngleRotation3D(axis, degrees), rotateAround);
            cam.Position = rot.Transform(cam.Position);
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
    }
}
