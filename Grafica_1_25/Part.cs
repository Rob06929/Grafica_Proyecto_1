using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Grafica_1_25;
using Newtonsoft.Json;
using OpenTK;

namespace Grafica_1_25
{
    [JsonObject(MemberSerialization.OptIn)]
    class Part 
    {
        [JsonProperty] public Vertex center;
        [JsonProperty] public Dictionary<string, Face> ListElement;
        [JsonProperty] public Color color;

        // Cada parte tiene su propio sistema de transformaciones
        // Esto permite transformar partes independientemente dentro de un objeto
        public Transformation Transformations { get; set; }

        public Part()
        {
            this.ListElement = new Dictionary<string, Face>();
            this.center = new Vertex();
            this.color = Color.Pink;
            Transformations = new Transformation(center);
        }

        public Part(Vertex center, Dictionary<string, Face> faces, Color color)
        {
            this.ListElement = new Dictionary<string, Face>();
            this.center = new Vertex(center);
            this.color = color;
            this.Transformations = new Transformation(this.center);
            foreach (var face in faces)
                addElement(face.Key, new Face(face.Value));
        }

        public Part(Part part)
        {
            this.center = new Vertex(part.center);
            this.color = part.color;
            this.ListElement = new Dictionary<string, Face>();
            this.Transformations = new Transformation(this.center);
            foreach (var face in part.ListElement)
                addElement(face.Key, new Face(face.Value));
        }

        public void addElement(string name, Face element)
        {
            if (!(element is Face face)) return;

            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }
            face.setCenter(new Vertex(this.center.x,
                         this.center.y,
                         this.center.z));
            ListElement.Add(name, face);
        }

        public void deleteElement(string name)
        {
            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }
        }

        public Face getElement(string name)
        {
            return ListElement.ContainsKey(name) ? ListElement[name] : null;
        }

        public void setCenter(Vertex newCenter)
        {
            // Calcula el desplazamiento necesario
            float deltaX = newCenter.x - this.center.x;
            float deltaY = newCenter.y - this.center.y;
            float deltaZ = newCenter.z - this.center.z;

            this.center = new Vertex(newCenter);
            this.Transformations.Center = Matrix4.CreateTranslation(this.center.ToVector3());

            // Actualiza el centro de todas las partes manteniendo sus posiciones relativas
            foreach (var face in ListElement.Values)
            {
                face.setCenter(new Vertex(face.center.x + deltaX,
                                         face.center.y + deltaY,
                                         face.center.z + deltaZ));
            }
        }

        /// <summary>
        /// Dibuja todas las caras que componen esta parte
        /// Las transformaciones de la parte se aplicarán a través de las caras individuales
        /// </summary>
        public void Draw()
        {
            foreach (var face in ListElement.Values)
            {
                Console.WriteLine($"Centro del parte: {this.center}");

                face.Draw();
            }
        }

        // ================= MÉTODOS DE TRANSFORMACIÓN PARA PARTES =================

        /// <summary>
        /// Rota toda la parte aplicando la rotación a todas sus caras
        /// Esto crea un efecto donde toda la parte rota como una unidad
        /// </summary>
        /// <param name="angleX">Rotación en eje X (grados)</param>
        /// <param name="angleY">Rotación en eje Y (grados)</param>
        /// <param name="angleZ">Rotación en eje Z (grados)</param>
        public void Rotate(float angleX, float angleY, float angleZ)
        {
            // Aplica la misma rotación a todas las caras de la parte
            // Esto mantiene la integridad geométrica de la parte
            foreach (var face in ListElement.Values)
            {
                face.Rotate(angleX, angleY, angleZ);
            }
        }

        /// <summary>
        /// Traslada toda la parte moviendo todas sus caras
        /// </summary>
        /// <param name="x">Desplazamiento en X</param>
        /// <param name="y">Desplazamiento en Y</param>
        /// <param name="z">Desplazamiento en Z</param>
        public void Translate(float x, float y, float z)
        {
            // Aplica la misma traslación a todas las caras
            foreach (var face in ListElement.Values)
            {
                face.Translate(x, y, z);
            }
        }

        public void Translate(Vertex position)
        {
            foreach (var face in ListElement.Values)
            {
                face.Translate(position);
            }
        }

        /// <summary>
        /// Escala toda la parte modificando el tamaño de todas sus caras
        /// </summary>
        /// <param name="x">Factor de escala en X</param>
        /// <param name="y">Factor de escala en Y</param>
        /// <param name="z">Factor de escala en Z</param>
        public void Scale(float x, float y, float z)
        {
            foreach (var face in ListElement.Values)
            {
                face.Scale(x, y, z);
            }
        }

        /// <summary>
        /// Versión especial de escalado que sincroniza las transformaciones
        /// Esta versión es importante para mantener consistencia en escalados complejos
        /// </summary>
        /// <param name="scale">Factores de escala como Vertex</param>
        public void Scale(Vertex scale)
        {
            bool isLoaded = false;
            foreach (var face in ListElement.Values)
            {
                face.Scale(scale);
                // Configura la transformación especial para escalado
                Transformations.SetScaleTransformation();

                // Sincroniza la matriz de escalado con la primera cara
                // Esto asegura consistencia en el escalado de toda la parte
                if (!isLoaded)
                {
                    Transformations.Scaling = face.Transformations.Scaling;
                    isLoaded = true;
                }
            }
        }

        public Matrix4 GetCenter()
        {
            return Transformations.Center;
        }
    }
}
