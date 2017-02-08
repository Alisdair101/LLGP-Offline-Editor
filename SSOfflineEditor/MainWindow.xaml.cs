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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Win32;

namespace SSOfflineEditor
{
    public partial class MainWindow : Window
    {
        List<Sphere> spheres = new List<Sphere>();
        Sphere selectedParentSphere;
        Sphere selectedSphere;
        
        bool replaceSphere;

        string appOutputDirectory;
        string xmlOutputDirectory;
        Vector appResolution;
        string appResolutionCommand;
        int appFrameRate;
        int appLength;

        public MainWindow()
        {
            InitializeComponent();
            replaceSphere = false;

            appResolution = new Vector(640.0f, 480.0f);
            string resTxtBoxValue = appResolution.X.ToString() + " x " + appResolution.Y.ToString();
            appResolutionCommand = appResolution.X.ToString() + "x" + appResolution.Y.ToString();
            appResolutionTxtBox.Text = resTxtBoxValue;

            int fpsVal = (int)appFrameRateSlider.Value;
            appFrameRate = fpsVal;
            string fpsTxtBoxValue = fpsVal.ToString() + " FPS";
            appFrameRateTxtBox.Text = fpsTxtBoxValue;

            int lengthVal = (int)appLengthSlider.Value;
            appLength = lengthVal;
            string lengthTxtBoxValue = lengthVal.ToString() + " seconds";
            appLengthTxtBox.Text = lengthTxtBoxValue;

            appOutputDirTxtBox.Text = "./Debug_Application_Output/";
            appOutputDirectory = appOutputDirTxtBox.Text;

            selectedParentSphere = null;

            // Initialise events on sliders
            sphereRadiusSlider.ValueChanged += sphereRadiusTrackBar_Scroll;
            sphereRotSpeedSlider.ValueChanged += sphereRotationSpeedTrackBar_Scroll;
            sphereColourRedSlider.ValueChanged += sphereColourRedTrackBar_Scroll;
            sphereColourGreenSlider.ValueChanged += sphereColourGreenTrackBar_Scroll;
            sphereColourBlueSlider.ValueChanged += sphereColourBlueTrackBar_Scroll;
            appResolutionSlider.ValueChanged += resolutionTrackBar_Scroll;
            appFrameRateSlider.ValueChanged += applicationFrameRateTrackBar_Scroll;
            appLengthSlider.ValueChanged += applicationLengthTrackBar_Scroll;

            // Initialse all Sphere Property tools to not be enabled
            deleteSphereBtn.IsEnabled = false;
            sphereNameTxtBox.IsEnabled = false;
            sphereParentListBox.IsEnabled = false;
            rootSphereCheckBox.IsEnabled = false;
            sphereRadiusSlider.IsEnabled = false;
            sphereRotSpeedSlider.IsEnabled = false;
            sphereColourRedSlider.IsEnabled = false;
            sphereColourGreenSlider.IsEnabled = false;
            sphereColourBlueSlider.IsEnabled = false;
            spherePositionXTxtBox.IsEnabled = false;
            spherePositionYTxtBox.IsEnabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint()
        {
            int childCount = mainCanvas.Children.Count;

            //for(int i = 0; i < childCount; i++)
            //{
            //    try
            //    {
            //        System.Windows.UIElement child = mainCanvas.Children[i];

            //        string childName = child.ToString();

            //        bool contains = childName.Contains("System.Windows.Controls.TabControl");

            //        if (!contains)
            //        {
            //            mainCanvas.Children.RemoveAt(i);
            //        }
            //    }
            //    catch(ArgumentOutOfRangeException e)
            //    {

            //    }
            //}

            mainCanvas.Children.Clear();
            mainCanvas.Children.Add(tabControl);

            if (spheres.Any())
            {
                foreach (Sphere sphere in spheres)
                {
                    if (sphere == selectedSphere)
                    {
                        sphere.DrawSphere(true, mainCanvas);
                    }
                    else
                    {
                        sphere.DrawSphere(false, mainCanvas);
                    }
                }
            }
        }

        private void addSphereBtn_Click(object sender, EventArgs e)
        {
            selectedSphere = new Sphere(sphereSelectionListBox);
            spheres.Add(selectedSphere);

            selectedParentSphere = null;

            sphereSelectionListBox.Items.Insert(0, selectedSphere.GetName());
            sphereSelectionListBox.SelectedItem = selectedSphere.GetName();

            panel1_Paint();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void spherePositionBtn_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSphereName;

            if (sphereSelectionListBox.SelectedItem == null)
            {
                selectedSphereName = null;
            }
            else
            {
                selectedSphereName = sphereSelectionListBox.SelectedItem.ToString();
            }

            if (selectedSphereName == null || selectedSphereName == "")
            {
                deleteSphereBtn.IsEnabled = false;
                sphereNameTxtBox.IsEnabled = false;
                sphereParentListBox.IsEnabled = false;
                sphereRadiusSlider.IsEnabled = false;
                rootSphereCheckBox.IsEnabled = false;
                sphereRotSpeedSlider.IsEnabled = false;
                sphereColourRedSlider.IsEnabled = false;
                sphereColourGreenSlider.IsEnabled = false;
                sphereColourBlueSlider.IsEnabled = false;
                spherePositionXTxtBox.IsEnabled = false;
                spherePositionYTxtBox.IsEnabled = false;
                replaceSphere = true;
            }
            else
            {
                deleteSphereBtn.IsEnabled = true;
                sphereNameTxtBox.IsEnabled = true;
                sphereParentListBox.IsEnabled = true;
                sphereRadiusSlider.IsEnabled = true;
                rootSphereCheckBox.IsEnabled = true;
                sphereRotSpeedSlider.IsEnabled = true;
                sphereColourRedSlider.IsEnabled = true;
                sphereColourGreenSlider.IsEnabled = true;
                sphereColourBlueSlider.IsEnabled = true;
                spherePositionXTxtBox.IsEnabled = true;
                spherePositionYTxtBox.IsEnabled = true;
            }

            if (!replaceSphere)
            {
                selectedSphere = SearchSphereByName(selectedSphereName);

                sphereNameTxtBox.Text = selectedSphere.GetName();

                sphereRadiusSlider.ValueChanged -= sphereRadiusTrackBar_Scroll;
                sphereRadiusSlider.Value = (int)selectedSphere.GetRadius();
                sphereRadiusSlider.ValueChanged += sphereRadiusTrackBar_Scroll;

                rootSphereCheckBox.IsChecked = selectedSphere.GetRootSphere();

                sphereRotSpeedSlider.ValueChanged -= sphereRotationSpeedTrackBar_Scroll;
                sphereRotSpeedSlider.Value = (int)selectedSphere.GetRotationSpeed();
                sphereRotSpeedSlider.ValueChanged += sphereRotationSpeedTrackBar_Scroll;

                sphereColourRedSlider.ValueChanged -= sphereColourRedTrackBar_Scroll;
                sphereColourRedSlider.Value = (int)selectedSphere.GetColour().X;
                sphereColourRedSlider.ValueChanged += sphereColourRedTrackBar_Scroll;

                sphereColourGreenSlider.ValueChanged -= sphereColourGreenTrackBar_Scroll;
                sphereColourGreenSlider.Value = (int)selectedSphere.GetColour().Y;
                sphereColourGreenSlider.ValueChanged += sphereColourGreenTrackBar_Scroll;

                sphereColourBlueSlider.ValueChanged -= sphereColourBlueTrackBar_Scroll;
                sphereColourBlueSlider.Value = (int)selectedSphere.GetColour().Z;
                sphereColourBlueSlider.ValueChanged += sphereColourBlueTrackBar_Scroll;

                spherePositionXTxtBox.Text = selectedSphere.GetPosition().X.ToString();
                spherePositionYTxtBox.Text = selectedSphere.GetPosition().Y.ToString();

                UpdateParentListBox();

                if (selectedSphere.GetParent() != null)
                {
                    selectedParentSphere = SearchSphereByName(selectedSphere.GetParent().GetName());
                    sphereParentListBox.SelectedItem = selectedSphere.GetParent().GetName();
                }
            }

            replaceSphere = false;

            panel1_Paint();
        }

        private void UpdateParentListBox()
        {
            sphereParentListBox.Items.Clear();

            if (!selectedSphere.GetRootSphere())
            {
                foreach (Sphere sphere in spheres)
                {
                    string sphereName = sphere.GetName();

                    if (selectedSphere.GetName() != sphereName)
                    {
                        sphereParentListBox.Items.Add(sphereName);
                    }
                }
            }
        }

        private Sphere SearchSphereByName(string sphereName)
        {
            Sphere namedSphere = null;

            foreach (Sphere sphere in spheres)
            {
                if (sphereName == sphere.GetName())
                {
                    namedSphere = sphere;
                    break;
                }
            }

            return namedSphere;
        }

        private void sphereNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string originalSphereName = selectedSphere.GetName();
                string selectedListBoxSphere = sphereSelectionListBox.SelectedItem.ToString();

                if (originalSphereName == selectedListBoxSphere)
                {
                    string sphereName = sphereNameTxtBox.Text;

                    if (SearchSphereByName(sphereName) == null)
                    {
                        selectedSphere.SetName(sphereName);

                        int selectedIndex = sphereSelectionListBox.SelectedIndex;

                        replaceSphere = true;
                        sphereSelectionListBox.Items.RemoveAt(selectedIndex);
                        replaceSphere = false;

                        sphereSelectionListBox.Items.Insert(selectedIndex, sphereName);
                        sphereSelectionListBox.SelectedItem = sphereName;
                    }
                    else
                    {
                        MessageBox.Show("A Sphere already has that name.");
                    }
                }

                sphereSelectionListBox.Focus();
            }
        }

        private void rootSphereCheckBox_Click(object sender, EventArgs e)
        {
            if ((bool)rootSphereCheckBox.IsChecked && selectedSphere != null)
            {
                sphereParentListBox.IsEnabled = false;
                selectedSphere.SetRootSphere((bool)rootSphereCheckBox.IsChecked);
            }
            else if(selectedSphere != null)
            {
                sphereParentListBox.IsEnabled = true;
                selectedSphere.SetRootSphere((bool)rootSphereCheckBox.IsChecked);
                UpdateParentListBox();
            }
        }

        private void sphereRadiusTrackBar_Scroll(object sender, EventArgs e)
        {
            float newRadius = (float)sphereRadiusSlider.Value;

            if (selectedSphere != null)
            {
                selectedSphere.SetRadius(newRadius);
            }
            panel1_Paint();
        }

        private void sphereRotationSpeedTrackBar_Scroll(object sender, EventArgs e)
        {
            float newRotationSpeed = (float)sphereRotSpeedSlider.Value;
            selectedSphere.SetRotationSpeed(newRotationSpeed);
        }

        private void sphereColourRedTrackBar_Scroll(object sender, EventArgs e)
        {
            float newRedValue = (float)sphereColourRedSlider.Value;
            Vector3D colour = selectedSphere.GetColour();

            Vector3D newColour = new Vector3D(newRedValue, colour.Y, colour.Z);

            selectedSphere.SetColour(newColour);
            panel1_Paint();
        }

        private void sphereColourGreenTrackBar_Scroll(object sender, EventArgs e)
        {
            float newGreenValue = (float)sphereColourGreenSlider.Value;
            Vector3D colour = selectedSphere.GetColour();

            Vector3D newColour = new Vector3D(colour.X, newGreenValue, colour.Z);

            selectedSphere.SetColour(newColour);
            panel1_Paint();
        }

        private void sphereColourBlueTrackBar_Scroll(object sender, EventArgs e)
        {
            float newBlueValue = (float)sphereColourBlueSlider.Value;
            Vector3D colour = selectedSphere.GetColour();

            Vector3D newColour = new Vector3D(colour.X, colour.Y, newBlueValue);

            selectedSphere.SetColour(newColour);
            panel1_Paint();
        }

        private void spherePositionXTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string stringPositionX = spherePositionXTxtBox.Text;

                if(stringPositionX != "")
                {
                    float floatPositionX = float.Parse(stringPositionX);

                    Vector position = selectedSphere.GetPosition();

                    selectedSphere.SetPosition(new Vector(floatPositionX, position.Y));

                    sphereSelectionListBox.Focus();
                    panel1_Paint();
                }
            }
        }

        private void spherePositionYTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string stringPositionY = spherePositionYTxtBox.Text;

                if(stringPositionY != "")
                {
                    float floatPositionY = float.Parse(stringPositionY);

                    Vector position = selectedSphere.GetPosition();

                    selectedSphere.SetPosition(new Vector(position.X, floatPositionY));

                    sphereSelectionListBox.Focus();
                    panel1_Paint();
                }
            }
        }

        private void sphereParentListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(sphereParentListBox.SelectedItem != null)
            {
                string sphereParentName = sphereParentListBox.SelectedItem.ToString();
                Sphere parentSphere = SearchSphereByName(sphereParentName);

                parentSphere.AddChild(selectedSphere);

                if (selectedParentSphere != null)
                {
                    selectedParentSphere.RemoveChild(selectedSphere);
                }

                selectedSphere.SetParent(parentSphere);

                selectedParentSphere = parentSphere;
            }
        }

        private void resolutionTrackBar_Scroll(object sender, EventArgs e)
        {
            int resVal = (int)appResolutionSlider.Value;

            switch (resVal)
            {
                case 0:
                    appResolution = new Vector(640.0f, 480.0f);
                    break;

                case 1:
                    appResolution = new Vector(1280.0f, 800.0f);
                    break;

                case 2:
                    appResolution = new Vector(1920.0f, 1080.0f);
                    break;

                default:
                    appResolution = new Vector(640.0f, 480.0f);
                    break;
            }

            string resTxtBoxValue = appResolution.X.ToString() + " x " + appResolution.Y.ToString();
            appResolutionCommand = appResolution.X.ToString() + "x" + appResolution.Y.ToString();
            appResolutionTxtBox.Text = resTxtBoxValue;
        }

        private void applicationFrameRateTrackBar_Scroll(object sender, EventArgs e)
        {
            int fpsVal = (int)appFrameRateSlider.Value;
            appFrameRate = fpsVal;
            string fpsTxtBoxValue = fpsVal.ToString() + " FPS";

            appFrameRateTxtBox.Text = fpsTxtBoxValue;
        }

        private void applicationLengthTrackBar_Scroll(object sender, EventArgs e)
        {
            int lengthVal = (int)appLengthSlider.Value;
            appLength = lengthVal;
            string lengthTxtBoxValue = lengthVal.ToString() + " seconds";

            appLengthTxtBox.Text = lengthTxtBoxValue;
        }

        private void appOutputDirTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                appOutputDirectory = appOutputDirTxtBox.Text;
                sphereSelectionListBox.Focus();
            }
        }

        private void exportXMLFileBtn_Click(object sender, EventArgs e)
        {
            bool optionsValid = ValidateOptions();

            if (optionsValid)
            {
                SaveXMLToFile();
            }
            else
            {
                MessageBox.Show("Please create at least one Root Sphere.");
            }
        }

        private bool ValidateOptions()
        {
            bool optionsValid = false;

            foreach (Sphere sphere in spheres)
            {
                bool rootSphere = sphere.GetRootSphere();

                if (rootSphere)
                {
                    return rootSphere;
                }
            }

            return optionsValid;
        }

        private void SaveXMLToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "XMLOutput";
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "eXtensible Markup Language (.xml)|*.xml";

            Nullable<bool> result = saveFileDialog.ShowDialog();

            if(result == true)
            {
                string fileName = saveFileDialog.FileName;

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                {
                    writer.WriteStartElement("SolarSystem");
                    writer.WriteStartElement("ApplicationProperties");
                    writer.WriteElementString("appLength", appLength.ToString());
                    writer.WriteElementString("appFrameRate", appFrameRate.ToString());
                    writer.WriteElementString("appResolutionX", appResolution.X.ToString());
                    writer.WriteElementString("appResolutionY", appResolution.Y.ToString());
                    writer.WriteElementString("appResolutionCommand", appResolutionCommand.ToString());
                    writer.WriteElementString("appOutputDirectory", appOutputDirectory.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Spheres");

                    foreach (Sphere sphere in spheres)
                    {
                        writer.WriteStartElement("sphereProp");
                        writer.WriteElementString("name", sphere.GetName());
                        writer.WriteElementString("positionX", ((int)sphere.GetPosition().X).ToString());
                        writer.WriteElementString("positionY", ((int)sphere.GetPosition().Y).ToString());
                        writer.WriteElementString("colourX", ((int)sphere.GetColour().X).ToString());
                        writer.WriteElementString("colourY", ((int)sphere.GetColour().Y).ToString());
                        writer.WriteElementString("colourZ", ((int)sphere.GetColour().Z).ToString());
                        writer.WriteElementString("radius", ((int)sphere.GetRadius()).ToString());
                        writer.WriteElementString("rotationSpeed", ((int)sphere.GetRotationSpeed()).ToString());
                        writer.WriteElementString("rootSphere", sphere.GetRootSphere().ToString().ToLower());

                        if (!sphere.GetRootSphere() && (sphere.GetParent().GetName() != "" || sphere.GetParent().GetName() != null))
                        {
                            writer.WriteElementString("parent", sphere.GetParent().GetName());
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();

                    MessageBox.Show("Solar System XML has been file successfuly exported.");
                }
            }
        }

        private void deleteSphereBtn_Click(object sender, EventArgs e)
        {
            if (spheres.Count != 0)
            {
                Sphere selectedSphereParent = selectedSphere.GetParent();
                List<Sphere> selectedSphereChildren = selectedSphere.GetChildren();

                if (selectedSphereParent != null)
                {
                    selectedSphereParent.RemoveChild(selectedSphere);

                    if (selectedSphereChildren.Count != 0)
                    {
                        foreach (Sphere sphere in selectedSphereChildren)
                        {
                            sphere.SetParent(selectedSphereParent);
                            selectedSphereParent.AddChild(sphere);
                        }

                        string deleteMessage = "Due to the deletion of Sphere: " + selectedSphere.GetName() + ", its parent: " + selectedSphereParent.GetName() + " has inherited the following chidlren.\n";

                        int sphereCount = 1;

                        foreach (Sphere sphere in selectedSphereChildren)
                        {
                            deleteMessage += "\n" + sphereCount + ". " + sphere.GetName();
                            sphereCount++;
                        }

                        sphereSelectionListBox.SelectionChanged -= listBox1_SelectedIndexChanged;
                        sphereSelectionListBox.Items.Remove(selectedSphere.GetName());
                        spheres.Remove(selectedSphere);
                        sphereSelectionListBox.SelectionChanged += listBox1_SelectedIndexChanged;

                        int parentIndex = sphereSelectionListBox.Items.IndexOf(selectedSphereParent.GetName());
                        sphereSelectionListBox.SelectedItem = parentIndex;

                        MessageBox.Show(deleteMessage);
                    }
                    else
                    {
                        spheres.Remove(selectedSphere);
                        sphereSelectionListBox.Items.Remove(selectedSphere.GetName());
                        MessageBox.Show("Sphere: " + selectedSphere.GetName() + " has been successfully deleted.");
                        sphereSelectionListBox.UnselectAll();
                    }
                }
                else
                {
                    spheres.Remove(selectedSphere);
                    sphereSelectionListBox.Items.Remove(selectedSphere.GetName());
                    MessageBox.Show("Sphere: " + selectedSphere.GetName() + " has been successfully deleted.");
                    sphereSelectionListBox.UnselectAll();
                }
            }

            panel1_Paint();
        }

        private void setupParentSpheres()
        {
            Sphere parentSphere;

            foreach(Sphere sphere in spheres)
            {
                bool rootSphere = sphere.GetRootSphere();

                if(!rootSphere)
                {
                    string parentName = sphere.GetParentName();
                    parentSphere = SearchSphereByName(parentName);
                    sphere.SetParent(parentSphere);
                    parentSphere.AddChild(sphere);
                }
            }
        }

        private void importXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "XMLInput";
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "eXtensible Markup Language (.xml)|*.xml";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                //XmlReaderSettings xmlReaderSetting = new XmlReaderSettings();
                spheres.Clear();

                using (XmlReader reader = XmlReader.Create(openFileDialog.FileName))
                {
                    reader.MoveToContent();

                    while(reader.Read())
                    {
                        if(reader.NodeType == XmlNodeType.Element)
                        {
                            XElement element = XNode.ReadFrom(reader) as XElement;
                            XName elementSectionName = element.Name;

                            if(!elementSectionName.ToString().Contains("Spheres"))
                            {
                                int childElementCount = element.Descendants().Count();

                                for (int i = 0; i < childElementCount; i++)
                                {
                                    XElement childElement = element.Descendants().ElementAt(i);
                                    XName elementName = childElement.Name;

                                    switch (elementName.ToString())
                                    {
                                        case "appLength":
                                            appLengthSlider.Value = Int32.Parse(childElement.Value);
                                            break;

                                        case "appFrameRate":
                                            appFrameRateSlider.Value = Int32.Parse(childElement.Value);
                                            break;

                                        case "appResolutionCommand":
                                            string appResSetting = childElement.Value;

                                            switch (appResSetting)
                                            {
                                                case "640x480":
                                                    appResolutionSlider.Value = 0;
                                                    break;

                                                case "1280x800":
                                                    appResolutionSlider.Value = 1;
                                                    break;

                                                case "1920x1080":
                                                    appResolutionSlider.Value = 2;

                                                    break;
                                            }

                                            appResolutionTxtBox.Text = appResSetting;
                                            break;

                                        case "appOutputDirectory":
                                            appOutputDirTxtBox.Text = childElement.Value;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                int childElementCount = element.Descendants().Count();

                                XElement childElement;

                                for (int i = 0; i < childElementCount; i++)
                                {
                                    Sphere newSphere = new Sphere(sphereSelectionListBox);

                                    // Load in Sphere properties
                                    // Sphere Name
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    newSphere.SetName(childElement.Value);

                                    // Sphere Position X
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    float posX = Int32.Parse(childElement.Value);

                                    // Sphere Position Y
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    float posY = Int32.Parse(childElement.Value);

                                    // Set Sphere Position
                                    newSphere.SetPosition(new Vector(posX, posY));

                                    // Sphere Colour R
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    int colourR = Int32.Parse(childElement.Value);

                                    // Sphere Colour G
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    int colourG = Int32.Parse(childElement.Value);

                                    // Sphere Colour B
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    int colourB = Int32.Parse(childElement.Value);

                                    // Set Sphere Colour
                                    newSphere.SetColour(new Vector3D(colourR, colourG, colourB));

                                    // Sphere Radius
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    newSphere.SetRadius(Int32.Parse(childElement.Value));

                                    // Sphere Rotation Speed
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    newSphere.SetRotationSpeed(Int32.Parse(childElement.Value));

                                    // Sphere Root Sphere
                                    i++;
                                    childElement = element.Descendants().ElementAt(i);
                                    string rootSphereString = childElement.Value;
                                    bool rootSphere = false;

                                    if (rootSphereString == "true")
                                    {
                                        rootSphere = true;
                                    }
                                    else if(rootSphereString == "false")
                                    {
                                        rootSphere = false;
                                    }

                                    newSphere.SetRootSphere(rootSphere);

                                    // Sphre Parent (if root sphere)
                                    if(!rootSphere)
                                    {
                                        i++;
                                        childElement = element.Descendants().ElementAt(i);
                                        string parentName = childElement.Value;
                                        newSphere.SetParentName(parentName);
                                    }

                                    spheres.Add(newSphere);
                                }

                                setupParentSpheres();

                                foreach(Sphere sphere in spheres)
                                {
                                    sphereSelectionListBox.Items.Add(sphere.GetName());
                                }

                                panel1_Paint();
                            }
                        }
                    }
                }
            }
        }
    }
}
