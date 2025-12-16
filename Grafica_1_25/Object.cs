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
    class Object
    {
        [JsonProperty] public Vertex center;
        [JsonProperty] public Dictionary<string, Part> ListElement;
        [JsonProperty] public Color color;

        // Sistema de transformaciones del objeto completo
        // Las transformaciones del objeto se propagan a todas sus partes
        public Transformation Transformations { get; set; }

        public Object()
        {
            this.ListElement = new Dictionary<string, Part>();
            this.center = new Vertex();
            this.color = Color.Pink;
            this.Transformations = new Transformation(center);
        }

        public Object(Vertex center, Dictionary<string, Part> parts, Color color)
        {
            this.ListElement = new Dictionary<string, Part>();
            this.center = new Vertex(center);
            this.color = color;
            this.Transformations = new Transformation(this.center);
            foreach (var part in parts)
                addElement(part.Key, new Part(part.Value));
        }

        public Object(Object obj)
        {
            this.center = new Vertex(obj.center);
            this.color = obj.color;
            this.ListElement = new Dictionary<string, Part>();
            Transformations = new Transformation(this.center);
            foreach (var part in obj.ListElement)
                addElement(part.Key, new Part(part.Value));
        }

        public void addElement(string name, Part element)
        {
            if (!(element is Part part)) return;

            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }

            // Establece la posición de la parte relativa al centro del objeto
            // Esto mantiene la jerarquía: Object -> Part -> Face -> Vertex
            part.setCenter(new Vertex(this.center.x + part.center.x,
                                     this.center.y + part.center.y,
                                     this.center.z + part.center.z));
            ListElement.Add(name, part);
        }

        public void deleteElement(string name)
        {
            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }
        }

        public Part getElement(string name)
        {
            return ListElement.ContainsKey(name) ? ListElement[name] : null;
        }

        /// <summary>
        /// Cambia el centro del objeto y actualiza todas las partes
        /// Esto es importante para mantener las relaciones espaciales correctas
        /// </summary>
        /// <param name="newCenter">Nuevo centro del objeto</param>
        public void setCenter(Vertex newCenter)
        {
            // Calcula el desplazamiento necesario
            float deltaX = newCenter.x - this.center.x;
            float deltaY = newCenter.y - this.center.y;
            float deltaZ = newCenter.z - this.center.z;

            this.center = new Vertex(newCenter);
            this.Transformations.Center = Matrix4.CreateTranslation(this.center.ToVector3());

            // Actualiza el centro de todas las partes manteniendo sus posiciones relativas
            foreach (var part in ListElement.Values)
            {
                part.setCenter(new Vertex(part.center.x + deltaX,
                                         part.center.y + deltaY,
                                         part.center.z + deltaZ));
            }
        }

        public void Draw()
        {
            foreach (var part in ListElement.Values)
            {
                Console.WriteLine($"Centro del objeto: {this.center}");
                part.Draw();
            }
        }

        // ================= MÉTODOS DE TRANSFORMACIÓN PARA OBJETOS =================

        /// <summary>
        /// Rota todo el objeto aplicando la rotación a todas sus partes
        /// Esto permite rotar objetos complejos como una sola unidad
        /// </summary>
        /// <param name="angleX">Rotación en eje X (grados)</param>
        /// <param name="angleY">Rotación en eje Y (grados)</param>
        /// <param name="angleZ">Rotación en eje Z (grados)</param>
        public void Rotate(float angleX, float angleY, float angleZ)
        {
            // Propaga la rotación a todas las partes del objeto
            // Cada parte aplicará la rotación a todas sus caras
            foreach (var part in ListElement.Values)
            {
                part.Rotate(angleX, angleY, angleZ);
            }
        }

        /// <summary>
        /// Traslada todo el objeto moviendo todas sus partes
        /// </summary>
        public void Translate(float x, float y, float z)
        {
            foreach (var part in ListElement.Values)
            {
                part.Translate(x, y, z);
            }
        }

        public void Translate(Vertex position)
        {
            foreach (var part in ListElement.Values)
            {
                part.Translate(position);
            }
        }

        /// <summary>
        /// Escala todo el objeto modificando todas sus partes
        /// </summary>
        public void Scale(float x, float y, float z)
        {
            foreach (var part in ListElement.Values)
            {
                part.Scale(x, y, z);
            }
        }

        public void Scale(Vertex scale)
        {
            foreach (var part in ListElement.Values)
            {
                part.Scale(scale);
            }
        }

        public Matrix4 GetCenter()
        {
            return Transformations.Center;
        }

        public static void SerializeJsonFile(string path,Object obj)
        {
            string textJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, textJson);
        }

        public static Object DeserializeJsonFile(string json)
        {
            string textJson = new StreamReader(json).ReadToEnd();
            return JsonConvert.DeserializeObject<Object>(textJson);
        }
    }
}
