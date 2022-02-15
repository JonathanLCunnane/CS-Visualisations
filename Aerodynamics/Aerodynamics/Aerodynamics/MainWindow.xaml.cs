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
            this.scene.KeyDown += Scene_KeyDown;
        }

        private void Scene_KeyDown(object sender, KeyEventArgs e)
        {
            Quaternion quaternion = new Quaternion();
            Quaternion q =
                e.Key == Key.Left ? new Quaternion(new Vector3D(0, 1, 0), -1) :
                e.Key == Key.Right ? new Quaternion(new Vector3D(0, 1, 0), 1) :
                e.Key == Key.Up ? new Quaternion(new Vector3D(1, 0, 0), -1) :
                e.Key == Key.Down ? new Quaternion(new Vector3D(1, 0, 0), 1) :
                Quaternion.Identity;
            quaternion = q * quaternion;
            model.Transform = new RotateTransform3D(new QuaternionRotation3D(quaternion));
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
                        //geometricVertices += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "f ")
                    {
                        //"faces"
                        Console.WriteLine(line);
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
                //Console.WriteLine(normals);
                //mesh.Positions = Point3DCollection.Parse(geometricVertices);
                //mesh.TriangleIndices = Int32Collection.Parse(faces);
                //this.investigationObject.Normals = Vector3DCollection.Parse(normals);
                //mesh.TextureCoordinates = PointCollection.Parse(textureVertices);
                //this.investigationObject = mesh;
                
                //for(int i = 0; i < facePoints.Count/3; i++)
                //{
                //    faces.Add(new Point3D( facePoints[i * 3], facePoints[i * 3 + 1], facePoints[i * 3 + 2]));
                //}
                Console.WriteLine(facePoints.Count);
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
