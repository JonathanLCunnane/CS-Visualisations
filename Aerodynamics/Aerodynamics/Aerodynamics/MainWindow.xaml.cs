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
        Model3DGroup model3DGroup = new Model3DGroup();
        MeshGeometry3D mesh = new MeshGeometry3D();
        Viewport3D viewport;
        GeometryModel3D model = new GeometryModel3D();
        List<int> facePoints;
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
                mainCam.Position = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -10), new Point3D(0, 0, 0)).Transform(mainCam.Position);
                mainCam.LookDirection = new Vector3D(-mainCam.Position.X, -mainCam.Position.Y, -mainCam.Position.Z);
            }
            else if(e.Key == Key.Left)
            {
                mainCam.Position = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 10), new Point3D(0, 0, 0)).Transform(mainCam.Position);
                mainCam.LookDirection = new Vector3D(-mainCam.Position.X, -mainCam.Position.Y, -mainCam.Position.Z);
            }
            else if(e.Key == Key.Up)
            {
                mainCam.Position = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 1), -10), new Point3D(0, 0, 0)).Transform(mainCam.Position);
                mainCam.LookDirection = new Vector3D(-mainCam.Position.X, -mainCam.Position.Y, -mainCam.Position.Z);
            }
            else if(e.Key == Key.Down)
            {
                mainCam.Position = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 1), 10), new Point3D(0, 0, 0)).Transform(mainCam.Position);
                mainCam.UpDirection = mainCam.UpDirection
                mainCam.LookDirection = new Vector3D(-mainCam.Position.X, -mainCam.Position.Y, -mainCam.Position.Z);
                
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

                foreach(string line in File.ReadLines(openFileDialog.FileName))
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
                viewport = this.scene;
                viewport.Children.Add(new ModelVisual3D() { Content = model });                
            }
        }
    }
}
