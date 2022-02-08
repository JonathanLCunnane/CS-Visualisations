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
        string geometricVertices;
        string faces;
        string normals;
        string textureVertices;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileExplorer(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wavefront files (*.obj)|*.obj|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                geometricVertices = "";
                faces = "";
                normals = "";
                textureVertices = "";

                foreach(string line in File.ReadLines(openFileDialog.FileName))
                {
                    string lineTag = line.Substring(0, 2);
                    if (lineTag == "v ")
                    {
                        //"geometric vertices"
                        geometricVertices += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "f ")
                    {
                        //"faces"
                        faces += "  " + line.Remove(0, 2).Replace('/',' ');
                    }
                    else if (lineTag == "vt")
                    {
                        //"texture vertices"
                        textureVertices += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "vn")
                    {
                        //"vertex normals"
                        normals += "  " + line.Remove(0, 2);
                    }
                    else if (lineTag == "vp")
                    {
                        //"parameter space vertices"
                    }

                }
                Console.WriteLine(faces);
                this.investigationObject.Positions = Point3DCollection.Parse(geometricVertices);
                this.investigationObject.TriangleIndices = Int32Collection.Parse(faces);
                this.investigationObject.Normals = Vector3DCollection.Parse(normals);
                this.investigationObject.TextureCoordinates = PointCollection.Parse(textureVertices);
            }
        }
    }
}
