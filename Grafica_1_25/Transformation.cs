using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafica_1_25
{
    /// <summary>
    /// Clase que maneja todas las transformaciones geométricas (rotación, escala, traslación)
    /// usando matrices 4x4 de OpenTK. Permite combinar múltiples transformaciones de forma eficiente.
    /// </summary>
    class Transformation
    {
        // Matriz de rotación: almacena todas las rotaciones acumuladas en los ejes X, Y, Z
        public Matrix4 Rotation { get; set; }

        // Matriz de escalado: controla el tamaño del objeto en cada eje
        public Matrix4 Scaling { get; set; }

        // Matriz de traslación: controla la posición del objeto en el espacio 3D
        public Matrix4 Translation { get; set; }

        // Matriz del centro: define el punto de referencia para las transformaciones
        // Es crucial para rotaciones y escalados que deben ocurrir respecto a un punto específico
        public Matrix4 Center { get; set; }

        // Matriz final de transformación: combina todas las matrices anteriores en una sola
        // Esto es más eficiente que aplicar cada transformación por separado
        public Matrix4 TransformationMatrix { get; set; }

        /// <summary>
        /// Constructor por defecto: inicializa todas las matrices como identidad
        /// La matriz identidad no produce ningún cambio (equivale a multiplicar por 1)
        /// </summary>
        public Transformation()
        {
            Rotation = Matrix4.Identity;        // Sin rotación inicial
            Scaling = Matrix4.Identity;         // Escala 1:1 (tamaño original)
            Translation = Matrix4.Identity;     // Sin movimiento inicial
            Center = Matrix4.CreateTranslation(Vector3.Zero);  // Centro en origen (0,0,0)
            TransformationMatrix = Matrix4.Identity;  // Sin transformación inicial
        }

        /// <summary>
        /// Constructor que recibe un punto de centro específico
        /// El centro es importante porque las rotaciones y escalados ocurren respecto a este punto
        /// </summary>
        /// <param name="center">Punto que actuará como centro de transformaciones</param>
        public Transformation(Vertex center)
        {
            Rotation = Matrix4.Identity;
            Scaling = Matrix4.Identity;
            Translation = Matrix4.Identity;
            // Convierte el Vertex a Vector3 y crea la matriz de traslación del centro
            Center = Matrix4.CreateTranslation(center.ToVector3());
            TransformationMatrix = Matrix4.Identity;
        }

        /// <summary>
        /// Combina todas las matrices de transformación en una sola matriz final
        /// El orden de multiplicación es CRÍTICO en transformaciones 3D
        /// </summary>
        /// <param name="self">Si true, aplica transformaciones respecto al objeto mismo.
        /// Si false, aplica transformaciones respecto al mundo</param>
        public void SetTransformation(bool self = true)
        {
            if (self)
            {
                // Orden para transformaciones locales: R * S * C * T
                // 1. Primero rotación (R) - rota el objeto
                // 2. Luego escalado (S) - cambia el tamaño
                // 3. Después centro (C) - establece el punto de referencia
                // 4. Finalmente traslación (T) - mueve el objeto
                TransformationMatrix = Rotation * Scaling * Center * Translation;
            }
            else
            {
                // Orden para transformaciones globales: C * T * R * S
                // Este orden es útil cuando queremos que las transformaciones
                // se apliquen respecto al sistema de coordenadas mundial
                TransformationMatrix = Center * Translation * Rotation * Scaling;
            }
        }

        /// <summary>
        /// Configuración especial de transformación para escalado
        /// Usado cuando necesitamos un control específico del orden para escalado
        /// </summary>
        public void SetScaleTransformation()
        {
            // Orden específico para escalado: T * R * S
            // Esto asegura que el escalado se aplique correctamente
            // sin afectar la posición final del objeto
            TransformationMatrix = Translation * Rotation * Scaling;
        }
    }
}
