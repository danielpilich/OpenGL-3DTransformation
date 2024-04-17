using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GLFW;
using GlmSharp;

using Shaders;
using Models;

namespace PMLabs
{
    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC: IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static Cube body = new Cube();
        static Cube leftArm = new Cube();
        static Cube rightArm = new Cube();

        static float speed_y; //Prędkość obrotu wokół osi Y [rad/s]
        static float speed_x; //Prędkość obrotu wokół osi X [rad/s]
        static KeyCallback kc = KeyProcessor;

        //Obsługa klawiatury - zmiana prędkości obrotu wokół poszczególnych osi w zależności od wciśniętych klawiszy
        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                if (key == Keys.Left) speed_y = -3.14f;
                if (key == Keys.Right) speed_y = 3.14f;
                if (key == Keys.Up) speed_x = -3.14f;
                if (key == Keys.Down) speed_x = 3.14f;
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;
                if (key == Keys.Up) speed_x = 0;
                if (key == Keys.Down) speed_x = 0;
            }
        }

        public static void InitOpenGLProgram(Window window)
        {
            // Czyszczenie okna na kolor czarny
            GL.ClearColor(0, 0, 0, 1);

            GL.Enable(EnableCap.DepthTest);

            // Ładowanie programów cieniujących
            DemoShaders.InitShaders("Shaders\\");

            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
        }

        public static void DrawScene(Window window, float angle_x, float angle_y)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);

            mat4 V = mat4.LookAt(
                new vec3(0.0f, 0.0f, -10.0f),
                new vec3(0.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1.0f, 1.0f, 50.0f);

            DemoShaders.spLambert.Use();
            GL.UniformMatrix4(DemoShaders.spLambert.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spLambert.U("V"), 1, false, V.Values1D);

            //mat4 M = mat4.Identity;

            // Korpus robota
            mat4 MBody = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            MBody *= mat4.Scale(new vec3(1.0f, 2.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MBody.Values1D);
            GL.Uniform4(DemoShaders.spConstant.U("color"), 0f, 1f, 1f, 1f);
            body.drawSolid();

            // Lewa reka robota
            mat4 MLeftArm = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            MLeftArm *= mat4.Scale(new vec3(0.5f, 1.5f, 0.5f));
            MLeftArm *= mat4.Translate(new vec3(3.0f, 0.25f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MLeftArm.Values1D);
            GL.Uniform4(DemoShaders.spConstant.U("color"), 1f, 0f, 0f, 1f);
            leftArm.drawSolid();

            // Prawa reka robota
            mat4 MRightArm = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            MRightArm *= mat4.Scale(new vec3(0.5f, 1.5f, 0.5f));
            MRightArm *= mat4.Translate(new vec3(-3.0f, 0.25f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MRightArm.Values1D);
            GL.Uniform4(DemoShaders.spConstant.U("color"), 1f, 0f, 0f, 1f);
            rightArm.drawSolid();

            Glfw.SwapBuffers(window);
        }

        public static void FreeOpenGLProgram(Window window)
        {
            // Możesz dodać odpowiednie czyszczenie zasobów tutaj, jeśli jest to konieczne
        }

        static void Main(string[] args)
        {
            Glfw.Init();

            Window window = Glfw.CreateWindow(500, 500, "Programowanie multimedialne", GLFW.Monitor.None, Window.None);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window);

            Glfw.Time = 0;

            float angle_x = 0;
            float angle_y = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                angle_x += speed_x * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi X zgodnie z prędkością obrotu
                angle_y += speed_y * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi Y zgodnie z prędkością obrotu
                Glfw.Time = 0; //Wyzeruj licznik czasu
                DrawScene(window, angle_x, angle_y);

                Glfw.PollEvents();
            }


            FreeOpenGLProgram(window);

            Glfw.Terminate();
        }
                    

    }
}