using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Grafica_1_25
{
    [JsonObject(MemberSerialization.OptIn)]
    class Face
    {
        [JsonProperty] public Vertex center;
        [JsonProperty] public Dictionary<string, Vertex> ListElement;
        [JsonProperty] public Color color;

        // Nueva propiedad: objeto que maneja todas las transformaciones de esta cara
        public Transformation Transformations { get; set; }

        public Face()
        {
            this.ListElement = new Dictionary<string, Vertex>();
            this.center = new Vertex();
            this.color = Color.Pink;
            // Inicializa las transformaciones con el centro de la cara
            this.Transformations = new Transformation(center);
        }

        public Face(Vertex center, Dictionary<string, Vertex> vertices, Color color)
        {
            this.ListElement = new Dictionary<string, Vertex>();
            this.center = new Vertex(center);
            this.color = color;
            // Cada cara tiene su propio sistema de transformaciones
            this.Transformations = new Transformation(this.center);
            foreach (var vertex in vertices)
                addElement(vertex.Key, new Vertex(vertex.Value));
        }

        public Face(Face face)
        {
            this.center = new Vertex(face.center);
            this.color = face.color;
            this.ListElement = new Dictionary<string, Vertex>();
            this.Transformations = new Transformation(this.center);
            foreach (var vertex in face.ListElement)
                addElement(vertex.Key, new Vertex(vertex.Value));
        }

        public void addElement(string name, Vertex element)
        {
            if (!(element is Vertex vertex)) return;

            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }

            ListElement.Add(name, vertex);
        }

        public void deleteElement(string name)
        {
            if (ListElement.ContainsKey(name))
            {
                ListElement.Remove(name);
            }
        }

        public Vertex getElement(string name)
        {
            return ListElement.ContainsKey(name) ? ListElement[name] : null;
        }

        /// <summary>
        /// Establece un nuevo centro para la cara y actualiza la matriz de centro
        /// El centro es el punto de referencia para rotaciones y escalados
        /// </summary>
        /// <param name="newCenter">Nuevo punto central</param>
        public void setCenter(Vertex newCenter)
        {
            this.center = new Vertex(newCenter);
            // Actualiza la matriz de centro en las transformaciones
            // Esto es crucial para que las rotaciones y escalados ocurran respecto al punto correcto
            this.Transformations.Center = Matrix4.CreateTranslation(this.center.ToVector3());
        }

        /// <summary>
        /// Método de renderizado que aplica transformaciones antes de dibujar
        /// Este es donde se aplican visualmente todas las transformaciones acumuladas
        /// </summary>
        public void Draw()
        {
            if (ListElement.Count < 3) return;  // Una cara necesita al menos 3 vértices

            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            Console.WriteLine($"Centro del cara: {this.center}");

            // Combina todas las transformaciones (rotación, escala, traslación) en una matriz
            this.Transformations.SetTransformation();

            // Ordena los vértices
            var sortedVertices = ListElement.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);

            // Aplica las transformaciones a cada vértice antes de renderizar
            foreach (var vertex in sortedVertices)
            {
                // multiplica cada vértice por la matriz de transformación
                // Esto aplica todas las rotaciones, escalados y traslaciones en un solo paso
                Vertex transformedVertex = vertex * Transformations.TransformationMatrix;
                GL.Vertex3(transformedVertex.x, transformedVertex.y, transformedVertex.z);
            }
            GL.End();
        }

        // ================= NUEVOS MÉTODOS DE TRANSFORMACIÓN =================

        /// <summary>
        /// Aplica rotaciones en los tres ejes
        /// Las rotaciones se acumulan: cada llamada se suma a las rotaciones anteriores
        /// </summary>
        /// <param name="angleX">Ángulo de rotación en eje X (en grados)</param>
        /// <param name="angleY">Ángulo de rotación en eje Y (en grados)</param>
        /// <param name="angleZ">Ángulo de rotación en eje Z (en grados)</param>
        public void Rotate(float angleX, float angleY, float angleZ)
        {
            // OpenTK trabaja en radianes, por eso convertimos de grados
            angleX = MathHelper.DegreesToRadians(angleX);
            angleY = MathHelper.DegreesToRadians(angleY);
            angleZ = MathHelper.DegreesToRadians(angleZ);

            // Multiplica la matriz de rotación actual por las nuevas rotaciones
            // El orden X*Y*Z es importante: determina cómo se combinan las rotaciones
            // *= significa que acumulamos la rotación (no la reemplazamos)
            Transformations.Rotation *= Matrix4.CreateRotationX(angleX) *
                                       Matrix4.CreateRotationY(angleY) *
                                       Matrix4.CreateRotationZ(angleZ);
        }

        /// <summary>
        /// Aplica traslación (movimiento) en las tres coordenadas
        /// Las traslaciones se acumulan: cada llamada se suma al movimiento anterior
        /// </summary>
        /// <param name="x">Desplazamiento en eje X</param>
        /// <param name="y">Desplazamiento en eje Y</param>
        /// <param name="z">Desplazamiento en eje Z</param>
        public void Translate(float x, float y, float z)
        {
            // Multiplica la matriz de traslación actual por la nueva traslación
            // CreateTranslation crea una matriz que representa el movimiento
            Transformations.Translation *= Matrix4.CreateTranslation(x, y, z);
        }

        /// <summary>
        /// Sobrecarga que acepta un Vertex como parámetro de traslación
        /// Útil cuando tenemos la traslación como un objeto Vertex
        /// </summary>
        /// <param name="position">Vértice que representa el desplazamiento</param>
        public void Translate(Vertex position)
        {
            Translate(position.x, position.y, position.z);
        }

        /// <summary>
        /// Aplica escalado en los tres ejes
        /// Los escalados se multiplican: cada llamada se multiplica con el escalado anterior
        /// </summary>
        /// <param name="x">Factor de escala en eje X (1.0 = sin cambio, 2.0 = doble tamaño)</param>
        /// <param name="y">Factor de escala en eje Y</param>
        /// <param name="z">Factor de escala en eje Z</param>
        public void Scale(float x, float y, float z)
        {
            // Multiplica la matriz de escalado actual por el nuevo escalado
            // CreateScale crea una matriz que representa el cambio de tamaño
            Transformations.Scaling *= Matrix4.CreateScale(x, y, z);
        }

        /// <summary>
        /// Sobrecarga que acepta un Vertex como factores de escala
        /// </summary>
        /// <param name="scale">Vértice con los factores de escala para cada eje</param>
        public void Scale(Vertex scale)
        {
            Scale(scale.x, scale.y, scale.z);
        }

        /// <summary>
        /// Obtiene la matriz de centro actual
        /// Útil para operaciones avanzadas o debugging
        /// </summary>
        /// <returns>Matriz 4x4 que representa el centro de la cara</returns>
        public Matrix4 GetCenter()
        {
            return Transformations.Center;
        }
    }
}
