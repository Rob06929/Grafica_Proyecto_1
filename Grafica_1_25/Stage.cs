using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafica_1_25
{
    class Stage
    {
        public Vertex center;
        public Dictionary<string, Object> ListElement;
        public Color color;

        // El Stage tiene su propio sistema de transformaciones
        // Esto permite transformar toda la escena como una unidad
        public Transformation Transformations { get; set; }

        public Stage()
        {
            this.ListElement = new Dictionary<string, Object>();
            this.center = new Vertex();
            this.color = Color.Pink;
            this.Transformations = new Transformation(center);
        }

        public Stage(Vertex center, Dictionary<string, Object> objects, Color color)
        {
            this.ListElement = new Dictionary<string, Object>();
            this.center = new Vertex(center);
            this.color = color;
            this.Transformations = new Transformation(this.center);
            foreach (var object_item in objects)
                addElement(object_item.Key, new Object(object_item.Value));
        }

        public Stage(Stage obj)
        {
            this.center = new Vertex(obj.center);
            this.color = obj.color;
            this.ListElement = new Dictionary<string, Object>();
            this.Transformations = new Transformation(this.center);
            foreach (var object_item in obj.ListElement)
                addElement(object_item.Key, new Object(object_item.Value));
        }

        public void addElement(string name, Object element)
        {
            if (!(element is Object object_item)) return;

            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }

            // Posiciona el objeto relativo al centro del stage
            object_item.setCenter(new Vertex(this.center.x + object_item.center.x,
                                 this.center.y + object_item.center.y,
                                 this.center.z + object_item.center.z));
            ListElement.Add(name, object_item);
        }

        public void deleteElement(string name)
        {
            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }
        }

        public Object getElement(string name)
        {
            return ListElement.ContainsKey(name) ? ListElement[name] : null;
        }

        public void setCenter(Vertex newCenter)
        {
            float deltaX = newCenter.x - this.center.x;
            float deltaY = newCenter.y - this.center.y;
            float deltaZ = newCenter.z - this.center.z;

            this.center = new Vertex(newCenter);
            this.Transformations.Center = Matrix4.CreateTranslation(this.center.ToVector3());

            foreach (var object_item in ListElement.Values)
            {
                object_item.setCenter(new Vertex(object_item.center.x + deltaX,
                                     object_item.center.y + deltaY,
                                     object_item.center.z + deltaZ));
            }
        }

        public void Draw()
        {
            foreach (var object_item in ListElement.Values)
            {

                object_item.Draw();
                Console.WriteLine($"Centro de escena: {this.center}");

            }
        }

        // ================= MÉTODOS DE TRANSFORMACIÓN PARA STAGE =================

        /// <summary>
        /// Rota toda la escena aplicando la transformación a todos los objetos
        /// Esto permite efectos como rotar toda la escena alrededor de un punto
        /// Útil para crear animaciones globales o cambios de perspectiva
        /// </summary>
        /// <param name="angleX">Rotación en eje X (grados)</param>
        /// <param name="angleY">Rotación en eje Y (grados)</param>
        /// <param name="angleZ">Rotación en eje Z (grados)</param>
        public void Rotate(float angleX, float angleY, float angleZ)
        {
            // Propaga la rotación a todos los objetos en el stage
            // Cada objeto propagará la rotación a sus partes, y cada parte a sus caras
            // Esto crea una cascada de transformaciones desde el nivel más alto (Stage)
            // hasta el más bajo (Vertex)
            foreach (var object_item in ListElement.Values)
            {
                object_item.Rotate(angleX, angleY, angleZ);
            }
        }

        /// <summary>
        /// Traslada toda la escena moviendo todos los objetos
        /// Útil para efectos como desplazar toda la escena o crear animaciones globales
        /// </summary>
        /// <param name="x">Desplazamiento en eje X</param>
        /// <param name="y">Desplazamiento en eje Y</param>
        /// <param name="z">Desplazamiento en eje Z</param>
        public void Translate(float x, float y, float z)
        {
            // Aplica la misma traslación a todos los objetos del stage
            // Esto mantiene las posiciones relativas entre objetos
            foreach (var object_item in ListElement.Values)
            {
                object_item.Translate(x, y, z);
            }
        }

        /// <summary>
        /// Sobrecarga que acepta un Vertex como parámetro de traslación
        /// </summary>
        /// <param name="position">Vector de desplazamiento</param>
        public void Translate(Vertex position)
        {
            foreach (var object_item in ListElement.Values)
            {
                object_item.Translate(position);
            }
        }

        /// <summary>
        /// Escala toda la escena modificando el tamaño de todos los objetos
        /// Útil para efectos de zoom o para cambiar la escala general de la escena
        /// </summary>
        /// <param name="x">Factor de escala en eje X</param>
        /// <param name="y">Factor de escala en eje Y</param>
        /// <param name="z">Factor de escala en eje Z</param>
        public void Scale(float x, float y, float z)
        {
            // Aplica el mismo factor de escala a todos los objetos
            // Esto crea un efecto de zoom uniforme en toda la escena
            foreach (var object_item in ListElement.Values)
            {
                object_item.Scale(x, y, z);
            }
        }

        /// <summary>
        /// Sobrecarga que acepta un Vertex como factores de escala
        /// </summary>
        /// <param name="scale">Factores de escala para cada eje</param>
        public void Scale(Vertex scale)
        {
            foreach (var object_item in ListElement.Values)
            {
                object_item.Scale(scale);
            }
        }

        /// <summary>
        /// Obtiene la matriz de centro del stage
        /// </summary>
        /// <returns>Matriz 4x4 del centro del stage</returns>
        public Matrix4 GetCenter()
        {
            return Transformations.Center;
        }
    }
}
