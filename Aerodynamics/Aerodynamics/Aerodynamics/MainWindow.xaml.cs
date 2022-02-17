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
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Right)
            {
                RotateCam(mainCam, mainCam.UpDirection, -1, new Point3D(0, 0, 0));
            }
            else if(e.Key == Key.Left)
            {
                RotateCam(mainCam, mainCam.UpDirection, 1, new Point3D(0, 0, 0));
            }
            else if(e.Key == Key.Up)
            {
                RotateCam(mainCam, Vector3D.CrossProduct(mainCam.UpDirection, mainCam.LookDirection), -1, new Point3D(0, 0, 0));
            }
            else if(e.Key == Key.Down)
            {
                RotateCam(mainCam, Vector3D.CrossProduct(mainCam.UpDirection, mainCam.LookDirection), 1, new Point3D(0, 0, 0));
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {

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
                    Material = new DiffuseMaterial(Brushes.Red),
                };

                //DirectionalLight directLight = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1));
                //PerspectiveCamera cam = new PerspectiveCamera(new Point3D(5, 5, 5), new Vector3D(-1, -1, -1), new Vector3D(1, 1, 1), 60);
                mv3d = new ModelVisual3D() { Content = model };
                viewport.Children.Add(mv3d);                
            }
        }

        void RotateCam(PerspectiveCamera cam, Vector3D axis, float degrees, Point3D rotateAround)
        {
            RotateTransform3D rot = new RotateTransform3D(new AxisAngleRotation3D(axis, degrees), rotateAround);
            cam.Position = rot.Transform(cam.Position);
            Point3D p = rot.Transform(new Point3D(cam.UpDirection.X, cam.UpDirection.Y, cam.UpDirection.Z));
            cam.UpDirection = new Vector3D(p.X, p.Y, p.Z);
            cam.LookDirection = new Vector3D(-cam.Position.X, -mainCam.Position.Y, -mainCam.Position.Z);
        }
    }
}
