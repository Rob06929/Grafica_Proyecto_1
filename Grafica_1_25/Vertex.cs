using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafica_1_25
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Vertex
    {
        [JsonProperty] public float x { get; set; }
        [JsonProperty] public float y { get; set; }
        [JsonProperty] public float z { get; set; }

        public Vertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vertex() : this(0, 0, 0) { }

        public Vertex(Vertex v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public void Add(Vertex v)
        {
            this.x += v.x;
            this.y += v.y;
            this.z += v.z;
        }

        public void Add(float x, float y, float z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }

        public void SetValue(float value)
        {
            this.x = this.y = this.z = value;
        }

        // ================= NUEVOS MÉTODOS PARA TRANSFORMACIONES =================

        /// <summary>
        /// Operador de multiplicación entre Vertex y Matrix4
        /// Este es el corazón de las transformaciones: permite aplicar cualquier matriz
        /// de transformación (rotación, escala, traslación) a un vértice
        /// </summary>
        /// <param name="vertex">El vértice a transformar</param>
        /// <param name="matrix">La matriz de transformación a aplicar</param>
        /// <returns>Un nuevo vértice transformado</returns>
        public static Vertex operator *(Vertex vertex, Matrix4 matrix)
        {
            // Convierte el vértice 3D a coordenadas homogéneas (4D) agregando w=1
            // Esto es necesario para trabajar con matrices 4x4 en transformaciones 3D
            Vector4 v = new Vector4(vertex.x, vertex.y, vertex.z, 1.0f);

            // Aplica la transformación multiplicando el vector por la matriz
            // Vector4.Transform realiza la multiplicación matriz-vector
            Vector4 result = Vector4.Transform(v, matrix);

            // Convierte de vuelta a coordenadas 3D (descarta la componente w)
            return new Vertex(result.X, result.Y, result.Z);
        }

        /// <summary>
        /// Operador de suma entre vértices
        /// Útil para operaciones de traslación y combinación de transformaciones
        /// </summary>
        /// <param name="a">Primer vértice</param>
        /// <param name="b">Segundo vértice</param>
        /// <returns>Nuevo vértice con la suma de coordenadas</returns>
        public static Vertex operator +(Vertex a, Vertex b)
        {
            return new Vertex(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public override string ToString() => $"({x}|{y}|{z})";
    }
}
