using Grafica_1_25;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Grafica_1_25
{
    class Game : GameWindow
    {
        static float scale = 50.0f;
        EventosTeclado ev;
        static double angle = 0;
        Object computer;
        Object mouse;
        Object mouse2;
        Object keyboard;
        Stage escena;

        public Game(int width, int height) : base(width, height)
        {
            ev = new EventosTeclado();
            computer = CreateComputer();
            mouse = CreateMouse();
            mouse2 = CreateMouse();
            keyboard = CreateKeyboard();
            escena = new Stage();
        }

        protected override void OnLoad(EventArgs e)
        {
            Color backgroundColor = Color.Black;
            GL.ClearColor(backgroundColor);
            GL.Enable(EnableCap.DepthTest);

            //Object.SerializeJsonFile("computer.json", computer);
            //Object.SerializeJsonFile("mouse.json", mouse);
            //Object.SerializeJsonFile("keyboard.json", keyboard);

            computer = Object.DeserializeJsonFile("computer.json");
            mouse = Object.DeserializeJsonFile("mouse.json");

            mouse2 = Object.DeserializeJsonFile("mouse.json");

            keyboard = Object.DeserializeJsonFile("keyboard.json");
            //Object.SerializeJsonFile("mouse.json", mouse);
            //escena.addElement("mouse", mouse);
            //escena.setCenter(new Vertex(20, 0, 0));

            escena.addElement("computer", computer);
            escena.addElement("mouse", mouse);

            escena.addElement("mouse2", mouse2);
            escena.addElement("keyboard", keyboard);
            escena.setCenter(new Vertex(0, 0, 0));

            escena.getElement("keyboard").Translate(new Vertex(5, 0, 0));
            escena.getElement("mouse2").Translate(new Vertex(-6, 0, 0));
            escena.getElement("mouse").Scale(1.5f, 1.5f, 1.5f);

            Console.WriteLine($"Centro del stage: {escena.center}");

            // PASO 4: Verificar y corregir la posición final
            //Console.WriteLine($"Centro del stage: {escena.center}");
            //Console.WriteLine($"Centro del mouse: {computer.center}");

            //escena.addElement("keyboard", keyboard);


            base.OnLoad(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnKeyDown(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            angle += 1.0;
            if (angle > 360)
            {
                angle -= 360;
            }
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushMatrix();

            // Dibujar ejes de coordenadas
            DrawAxes();

            //GL.Rotate(angle, 0.1, 0.0, 0.0);

            // Dibujar objetos
            //computer.Draw();
            //mouse.Draw();
            //keyboard.Draw();

            escena.Rotate(0.3f, 0, 0);
            escena.Draw();
            //escena.getElement("computer").setCenter(new Vertex(7, 7, 7));
            //escena.Rotate(5 , 0, 0);


            GL.PopMatrix();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void DrawAxes()
        {
            GL.Begin(PrimitiveType.Lines);
            // Eje X (rojo)
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(10, 0, 0);
            GL.Vertex3(-10, 0, 0);

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(10, -1, 0);
            GL.Vertex3(-10, -1, 0);

            // Eje Y (verde)
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0, 10, 0);
            GL.Vertex3(0, -10, 0);

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0, 10, 0);
            GL.Vertex3(0, -10, 0);
            // Eje Z (azul)
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 5);
            GL.End();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                Width / (float)Height,
                0.1f,
                100.0f
            );
            GL.LoadMatrix(ref perspectiveMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Vector3 eye = new Vector3(0, 0, 20);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.UnitY;
            Matrix4 viewMatrix = Matrix4.LookAt(eye, target, up);
            GL.LoadMatrix(ref viewMatrix);

            base.OnResize(e);
        }

        // ================= CREACIÓN DE OBJETOS =================

        private Object CreateComputer()
        {
            Dictionary<string, Part> computerParts = new Dictionary<string, Part>();

            // Monitor
            computerParts.Add("monitor", CreateMonitor());

            // Torre/CPU
            computerParts.Add("tower", CreateTower());

            return new Object(new Vertex(0, 0, 0), computerParts, Color.Gray);
        }

        private Part CreateMonitor()
        {
            Dictionary<string, Face> monitorFaces = new Dictionary<string, Face>();

            // Dimensiones del monitor
            float screenWidth = 2.5f;
            float screenHeight = 1.5f;
            float frameWidth = 3.0f;
            float frameHeight = 2.0f;
            float depth = 0.3f; // Profundidad del monitor

            // Pantalla (frente) - ligeramente hacia adelante
            var screenVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-screenWidth, screenHeight, 0.1f) },
                { "v2", new Vertex(screenWidth, screenHeight, 0.1f) },
                { "v3", new Vertex(screenWidth, -screenHeight, 0.1f) },
                { "v4", new Vertex(-screenWidth, -screenHeight, 0.1f) }
            };
            monitorFaces.Add("screen", new Face(new Vertex(0, 0, 0), screenVertices, Color.Black));

                    // Marco frontal
             var frameVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-frameWidth, frameHeight, 0.0f) },
                { "v2", new Vertex(frameWidth, frameHeight, 0.0f) },
                { "v3", new Vertex(frameWidth, -frameHeight, 0.0f) },
                { "v4", new Vertex(-frameWidth, -frameHeight, 0.0f) }
            };
           monitorFaces.Add("frame", new Face(new Vertex(0, 0, 0), frameVertices, Color.DarkGray));

                    // Cara posterior
           var backVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-frameWidth, frameHeight, -depth) },
                { "v2", new Vertex(frameWidth, frameHeight, -depth) },
                { "v3", new Vertex(frameWidth, -frameHeight, -depth) },
                { "v4", new Vertex(-frameWidth, -frameHeight, -depth) }
            };
           monitorFaces.Add("back", new Face(new Vertex(0, 0, 0), backVertices, Color.Gray));

                    // Lado derecho
           var rightVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(frameWidth, frameHeight, 0.0f) },
                { "v2", new Vertex(frameWidth, frameHeight, -depth) },
                { "v3", new Vertex(frameWidth, -frameHeight, -depth) },
                { "v4", new Vertex(frameWidth, -frameHeight, 0.0f) }
            };
            monitorFaces.Add("right", new Face(new Vertex(0, 0, 0), rightVertices, Color.DimGray));

                    // Lado izquierdo
                    var leftVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-frameWidth, frameHeight, -depth) },
                { "v2", new Vertex(-frameWidth, frameHeight, 0.0f) },
                { "v3", new Vertex(-frameWidth, -frameHeight, 0.0f) },
                { "v4", new Vertex(-frameWidth, -frameHeight, -depth) }
            };
                    monitorFaces.Add("left", new Face(new Vertex(0, 0, 0), leftVertices, Color.DimGray));

                    // Parte superior
            var topVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-frameWidth, frameHeight, 0.0f) },
                { "v2", new Vertex(-frameWidth, frameHeight, -depth) },
                { "v3", new Vertex(frameWidth, frameHeight, -depth) },
                { "v4", new Vertex(frameWidth, frameHeight, 0.0f) }
            };
                    monitorFaces.Add("top", new Face(new Vertex(0, 0, 0), topVertices, Color.LightGray));

                    // Parte inferior
            var bottomVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-frameWidth, -frameHeight, -depth) },
                { "v2", new Vertex(-frameWidth, -frameHeight, 0.0f) },
                { "v3", new Vertex(frameWidth, -frameHeight, 0.0f) },
                { "v4", new Vertex(frameWidth, -frameHeight, -depth) }
            };
                    monitorFaces.Add("bottom", new Face(new Vertex(0, 0, 0), bottomVertices, Color.LightGray));

                    // Base del monitor (soporte)
            var baseVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-1.0f, -2.0f, 0.5f) },
                { "v2", new Vertex(1.0f, -2.0f, 0.5f) },
                { "v3", new Vertex(1.0f, -2.5f, -0.5f) },
                { "v4", new Vertex(-1.0f, -2.5f, -0.5f) }
            };
                    monitorFaces.Add("base", new Face(new Vertex(0, 0, 0), baseVertices, Color.Silver));

            // Soporte vertical 
            var standVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.2f, -2.0f, -0.1f) },
                { "v2", new Vertex(0.2f, -2.0f, -0.1f) },
                { "v3", new Vertex(0.2f, -2.0f, -0.3f) },
                { "v4", new Vertex(-0.2f, -2.0f, -0.3f) }
            };
            monitorFaces.Add("stand", new Face(new Vertex(0, 0, 0), standVertices, Color.Blue));

            return new Part(new Vertex(0, 0, 0), monitorFaces, Color.Gray);
        }

        private Part CreateTower()
        {
            Dictionary<string, Face> towerFaces = new Dictionary<string, Face>();

            // Aumentamos el tamaño de la fuente de poder
            float width = 1.5f;   // Ancho (X)
            float height = 2.0f;  // Alto (Y) 
            float depth = 1.5f;   // Profundidad (Z)

            // Cara frontal (Z positivo)
            var frontVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-width, height, depth) },
                { "v2", new Vertex(width, height, depth) },
                { "v3", new Vertex(width, -height, depth) },
                { "v4", new Vertex(-width, -height, depth) }
            };
                    towerFaces.Add("front", new Face(new Vertex(0, 0, 0), frontVertices, Color.LightGray));

                    // Cara posterior (Z negativo)
                    var backVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-width, height, -depth) },
                { "v2", new Vertex(width, height, -depth) },
                { "v3", new Vertex(width, -height, -depth) },
                { "v4", new Vertex(-width, -height, -depth) }
            };
                    towerFaces.Add("back", new Face(new Vertex(0, 0, 0), backVertices, Color.DarkGray));

                    // Cara lateral derecha (X positivo)
                    var rightVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(width, height, depth) },
                { "v2", new Vertex(width, height, -depth) },
                { "v3", new Vertex(width, -height, -depth) },
                { "v4", new Vertex(width, -height, depth) }
            };
                    towerFaces.Add("right", new Face(new Vertex(0, 0, 0), rightVertices, Color.Gray));

                    // Cara lateral izquierda (X negativo)
                    var leftVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-width, height, -depth) },
                { "v2", new Vertex(-width, height, depth) },
                { "v3", new Vertex(-width, -height, depth) },
                { "v4", new Vertex(-width, -height, -depth) }
            };
                    towerFaces.Add("left", new Face(new Vertex(0, 0, 0), leftVertices, Color.Gray));

                    // Cara superior (Y positivo)
                    var topVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-width, height, depth) },
                { "v2", new Vertex(-width, height, -depth) },
                { "v3", new Vertex(width, height, -depth) },
                { "v4", new Vertex(width, height, depth) }
            };
                    towerFaces.Add("top", new Face(new Vertex(0, 0, 0), topVertices, Color.Silver));

                    // Cara inferior (Y negativo)
                    var bottomVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-width, -height, -depth) },
                { "v2", new Vertex(-width, -height, depth) },
                { "v3", new Vertex(width, -height, depth) },
                { "v4", new Vertex(width, -height, -depth) }
            };
            towerFaces.Add("bottom", new Face(new Vertex(0, 0, 0), bottomVertices, Color.DimGray));

            return new Part(new Vertex(4, -1, 0), towerFaces, Color.Gray);
        }

        private Object CreateMouse()
        {
            Dictionary<string, Part> mouseParts = new Dictionary<string, Part>();
            Dictionary<string, Face> mouseFaces = new Dictionary<string, Face>();

            // Parte superior del ratón
            var topVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.6f, -3.6f, 0.8f) },
                { "v2", new Vertex(0.6f, -3.6f, 0.8f) },
                { "v3", new Vertex(0.6f, -3.6f, -0.8f) },
                { "v4", new Vertex(-0.6f, -3.6f, -0.8f) }
            };
            mouseFaces.Add("top", new Face(new Vertex(0, 0, 0), topVertices, Color.White));

            // Parte inferior del ratón
            var bottomVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.6f, -3.9f, 0.8f) },
                { "v2", new Vertex(-0.6f, -3.9f, -0.8f) },
                { "v3", new Vertex(0.6f, -3.9f, -0.8f) },
                { "v4", new Vertex(0.6f, -3.9f, 0.8f) }
            };
            mouseFaces.Add("bottom", new Face(new Vertex(0, 0, 0), bottomVertices, Color.Gainsboro));

            // Lados del ratón
            var leftSideVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.6f, -3.6f, 0.8f) },
                { "v2", new Vertex(-0.6f, -3.6f, -0.8f) },
                { "v3", new Vertex(-0.6f, -3.9f, -0.8f) },
                { "v4", new Vertex(-0.6f, -3.9f, 0.8f) }
            };
            mouseFaces.Add("left_side", new Face(new Vertex(0, 0, 0), leftSideVertices, Color.LightGray));

            var rightSideVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(0.6f, -3.6f, 0.8f) },
                { "v2", new Vertex(0.6f, -3.9f, 0.8f) },
                { "v3", new Vertex(0.6f, -3.9f, -0.8f) },
                { "v4", new Vertex(0.6f, -3.6f, -0.8f) }
            };
            mouseFaces.Add("right_side", new Face(new Vertex(0, 0, 0), rightSideVertices, Color.LightGray));

            // Frente y atrás del ratón
            var frontVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.6f, -3.6f, 0.8f) },
                { "v2", new Vertex(0.6f, -3.6f, 0.8f) },
                { "v3", new Vertex(0.6f, -3.9f, 0.8f) },
                { "v4", new Vertex(-0.6f, -3.9f, 0.8f) }
            };
            mouseFaces.Add("front", new Face(new Vertex(0, 0, 0), frontVertices, Color.Silver));

            var backVertices = new Dictionary<string, Vertex>
            {
                { "v1", new Vertex(-0.6f, -3.6f, -0.8f) },
                { "v2", new Vertex(-0.6f, -3.9f, -0.8f) },
                { "v3", new Vertex(0.6f, -3.9f, -0.8f) },
                { "v4", new Vertex(0.6f, -3.6f, -0.8f) }
            };
            mouseFaces.Add("back", new Face(new Vertex(0, 0, 0), backVertices, Color.Silver));

            mouseParts.Add("body", new Part(new Vertex(0, 0, 0), mouseFaces, Color.White));

            return new Object(new Vertex(4, 0, 0.5f), mouseParts, Color.White); // Separado del teclado
        }

        private Object CreateKeyboard()
        {
            Dictionary<string, Part> keyboardParts = new Dictionary<string, Part>();
            Dictionary<string, Face> keyboardFaces = new Dictionary<string, Face>();

            // Dimensiones del teclado
            float keyboardWidth = 4.5f;
            float keyboardDepth = 1.8f;
            float keyboardTopY = -3.7f;
            float keyboardBottomY = -3.9f;

            // === BASE DEL TECLADO ===
            // Parte superior del teclado
            var topVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(-keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v2", new Vertex(keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v3", new Vertex(keyboardWidth, keyboardTopY, -keyboardDepth) },
        { "v4", new Vertex(-keyboardWidth, keyboardTopY, -keyboardDepth) }
    };
            keyboardFaces.Add("top", new Face(new Vertex(0, 0, 0), topVertices, Color.DimGray));

            // Parte inferior del teclado
            var bottomVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(-keyboardWidth, keyboardBottomY, keyboardDepth) },
        { "v2", new Vertex(-keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v3", new Vertex(keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v4", new Vertex(keyboardWidth, keyboardBottomY, keyboardDepth) }
    };
            keyboardFaces.Add("bottom", new Face(new Vertex(0, 0, 0), bottomVertices, Color.Gray));

            // Lados del teclado
            var frontVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(-keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v2", new Vertex(keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v3", new Vertex(keyboardWidth, keyboardBottomY, keyboardDepth) },
        { "v4", new Vertex(-keyboardWidth, keyboardBottomY, keyboardDepth) }
    };
            keyboardFaces.Add("front", new Face(new Vertex(0, 0, 0), frontVertices, Color.DarkSlateGray));

            var backVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(-keyboardWidth, keyboardTopY, -keyboardDepth) },
        { "v2", new Vertex(-keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v3", new Vertex(keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v4", new Vertex(keyboardWidth, keyboardTopY, -keyboardDepth) }
    };
            keyboardFaces.Add("back", new Face(new Vertex(0, 0, 0), backVertices, Color.DarkSlateGray));

            var leftSideVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(-keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v2", new Vertex(-keyboardWidth, keyboardTopY, -keyboardDepth) },
        { "v3", new Vertex(-keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v4", new Vertex(-keyboardWidth, keyboardBottomY, keyboardDepth) }
    };
            keyboardFaces.Add("left_side", new Face(new Vertex(0, 0, 0), leftSideVertices, Color.SlateGray));

            var rightSideVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(keyboardWidth, keyboardTopY, keyboardDepth) },
        { "v2", new Vertex(keyboardWidth, keyboardBottomY, keyboardDepth) },
        { "v3", new Vertex(keyboardWidth, keyboardBottomY, -keyboardDepth) },
        { "v4", new Vertex(keyboardWidth, keyboardTopY, -keyboardDepth) }
    };
            keyboardFaces.Add("right_side", new Face(new Vertex(0, 0, 0), rightSideVertices, Color.SlateGray));

            // Añadir la base como una parte
            keyboardParts.Add("base", new Part(new Vertex(0, 0, 0), keyboardFaces, Color.DimGray));

            // === TECLAS INDIVIDUALES ===

            // Fila 1 (números) - 10 teclas
            for (int i = 0; i < 10; i++)
            {
                float keyX = -3.6f + (i * 0.8f);
                float keyZ = -1.2f;
                keyboardParts.Add($"key_num_{i}", CreateKey(keyX, keyZ, keyboardTopY + 0.05f));
            }

            // Fila 2 (QWERTY) - 10 teclas
            for (int i = 0; i < 10; i++)
            {
                float keyX = -3.6f + (i * 0.8f);
                float keyZ = -0.4f;
                keyboardParts.Add($"key_qwerty_{i}", CreateKey(keyX, keyZ, keyboardTopY + 0.05f));
            }

            // Fila 3 (ASDF) - 9 teclas
            for (int i = 0; i < 9; i++)
            {
                float keyX = -3.2f + (i * 0.8f);
                float keyZ = 0.4f;
                keyboardParts.Add($"key_asdf_{i}", CreateKey(keyX, keyZ, keyboardTopY + 0.05f));
            }

            // Fila 4 (ZXCV) - 7 teclas
            for (int i = 0; i < 7; i++)
            {
                float keyX = -2.4f + (i * 0.8f);
                float keyZ = 1.2f;
                keyboardParts.Add($"key_zxcv_{i}", CreateKey(keyX, keyZ, keyboardTopY + 0.05f));
            }

            // Barra espaciadora (más grande)
            var spacebarFaces = new Dictionary<string, Face>();
            var spacebarVertices = new Dictionary<string, Vertex>
                {
                    { "v1", new Vertex(-2.0f, keyboardTopY - 0.05f, 0.8f) },
                    { "v2", new Vertex(2.0f, keyboardTopY - 0.05f, 0.8f) },
                    { "v3", new Vertex(2.0f, keyboardTopY - 0.05f, 0.4f) },
                    { "v4", new Vertex(-2.0f, keyboardTopY - 0.05f, 0.4f) }
                };
            //spacebarFaces.Add("top", new Face(new Vertex(0, 0, 0), spacebarVertices, Color.Blue));
            //keyboardParts.Add("spacebar", new Part(new Vertex(0, 0.12f, 0), spacebarFaces, Color.Blue));

            return new Object(new Vertex(0, -1, 0), keyboardParts, Color.DimGray);
        }

        // Método auxiliar para crear teclas individuales
        private Part CreateKey(float x, float z, float y)
        {
            Dictionary<string, Face> keyFaces = new Dictionary<string, Face>();

            float keySize = 0.5f;
            float keyHeight = 0.07f;

            // Parte superior de la tecla
            var keyTopVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(x - keySize/2, y, z + keySize/2) },
        { "v2", new Vertex(x + keySize/2, y, z + keySize/2) },
        { "v3", new Vertex(x + keySize/2, y, z - keySize/2) },
        { "v4", new Vertex(x - keySize/2, y, z - keySize/2) }
    };
            keyFaces.Add("top", new Face(new Vertex(0, 0.05f, 0), keyTopVertices, Color.WhiteSmoke));

            // Lados de la tecla (frente)
            var keyFrontVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(x - keySize/2, y, z + keySize/2) },
        { "v2", new Vertex(x + keySize/2, y, z + keySize/2) },
        { "v3", new Vertex(x + keySize/2, y - keyHeight, z + keySize/2) },
        { "v4", new Vertex(x - keySize/2, y - keyHeight, z + keySize/2) }
    };
            keyFaces.Add("front", new Face(new Vertex(0, 0.05f, 0), keyFrontVertices, Color.LightGray));

            // Lado derecho de la tecla
            var keyRightVertices = new Dictionary<string, Vertex>
    {
        { "v1", new Vertex(x + keySize/2, y, z + keySize/2) },
        { "v2", new Vertex(x + keySize/2, y, z - keySize/2) },
        { "v3", new Vertex(x + keySize/2, y - keyHeight, z - keySize/2) },
        { "v4", new Vertex(x + keySize/2, y - keyHeight, z + keySize/2) }
    };
            keyFaces.Add("right", new Face(new Vertex(0, 0.05f, 0), keyRightVertices, Color.LightGray));

            return new Part(new Vertex(0, 0, 0), keyFaces, Color.WhiteSmoke);
        }
    }
}