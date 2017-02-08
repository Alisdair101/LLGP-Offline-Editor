using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SSOfflineEditor
{
    class Sphere
    {
        string name;

        Vector position;
        Vector3D colour;
        float radius;
        float rotationSpeed;

        bool rootSphere;
        Sphere parent;
        string parentName;
        List<Sphere> children;

        ListBox sphereSelectionListBox;

        public Sphere(ListBox sphereSelectionListBox)
        {
            position = new Vector(275.0f, 0.0f);
            name = "New Sphere";
            rootSphere = false;
            colour = new Vector3D(255.0f, 0.0f, 255.0f);
            radius = 100.0f;
            rotationSpeed = 10.0f;
            children = new List<Sphere>();
            parent = null;
            this.sphereSelectionListBox = sphereSelectionListBox;
        }

        public void DrawSphere(bool selected, Canvas mainCanvas)
        {
            SolidColorBrush solidBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)colour.X, (byte)colour.Y, (byte)colour.Z));

            Ellipse ellipse = new Ellipse();
            ellipse.Fill = solidBrush;
            ellipse.Width = radius;
            ellipse.Height = radius;
            ellipse.MouseDown += ellipsePressed;

            if (selected)
            {
                ellipse.Stroke = System.Windows.Media.Brushes.Black;
            }

            mainCanvas.Children.Add(ellipse);

            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
        }

        private void ellipsePressed(object sender, EventArgs e)
        {
            sphereSelectionListBox.SelectedItem = name;
        }

        #region Get/Set Functions

        // Get/Set Name
        public string GetName() { return name; }
        public void SetName(string name) { this.name = name; }

        // Get/Set Position
        public Vector GetPosition() { return position; }
        public void SetPosition(Vector position) { this.position = position; }

        // Get/Set Colour
        public Vector3D GetColour() { return colour; }
        public void SetColour(Vector3D colour) { this.colour = colour; }

        // Get/Set Radius
        public float GetRadius() { return radius; }
        public void SetRadius(float radius) { this.radius = radius; }

        // Get/Set Rotation Speed
        public float GetRotationSpeed() { return rotationSpeed; }
        public void SetRotationSpeed(float rotationSpeed) { this.rotationSpeed = rotationSpeed; }

        // Get/Set Parent
        public Sphere GetParent() { return parent; }
        public void SetParent(Sphere parent) { this.parent = parent; }

        // Get/Set Parent Name
        public string GetParentName() { return parentName; }
        public void SetParentName(string parentName) { this.parentName = parentName; }

        // Get/Set/Add Children
        public List<Sphere> GetChildren() { return children; }
        public void SetChildren(List<Sphere> newChildren) { children = newChildren; }
        public void AddChild(Sphere child) { children.Add(child); }
        public void RemoveChild(Sphere childToDelete) { children.Remove(childToDelete); }

        // Get/Set Root Sphere
        public bool GetRootSphere() { return rootSphere; }
        public void SetRootSphere(bool rootSphere) { this.rootSphere = rootSphere; }

        #endregion
    }
}