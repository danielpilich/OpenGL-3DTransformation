using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GLFW;
using GlmSharp;

using Shaders;
using Models;
using OpenTK.Mathematics;

namespace PMLabs
{
    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static Torus torus = new Torus();

        static Cube body = new Cube();

        static Cube leftArm1 = new Cube();
        static Cube leftArm2 = new Cube();

        static Cube rightArm1 = new Cube();
        static Cube rightArm2 = new Cube();

        static Cube leftLeg1 = new Cube();
        static Cube leftLeg2 = new Cube();

        static Cube rightLeg1 = new Cube();
        static Cube rightLeg2 = new Cube();

        static Teapot head = new Teapot();

        static float speed_y; //Prędkość obrotu wokół osi Y [rad/s]
        static float speed_x; //Prędkość obrotu wokół osi X [rad/s]
        static float speed_r_arm; //Prędkość obrotu prawej ręki wokół osi Z [rad/s]
        static float speed_l_arm; //Prędkość obrotu lewej ręki wokół osi Z [rad/s]
        static float speed_r_farm; //Prędkość obrotu prawego przedramienia wokół osi Z [rad/s]
        static float speed_l_farm; //Prędkość obrotu lewego przedramienia wokół osi Z [rad/s]

        static KeyCallback kc = KeyProcessor;

        //Obsługa klawiatury - zmiana prędkości obrotu wokół poszczególnych osi w zależności od wciśniętych klawiszy
        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                if (key == Keys.Left) speed_y = 3.14f;
                if (key == Keys.Right) speed_y = -3.14f;
                if (key == Keys.Up) speed_x = -3.14f;
                if (key == Keys.Down) speed_x = 3.14f;
                if (key == Keys.Q) speed_l_arm = 1f;
                if (key == Keys.A) speed_l_arm = -1f;
                if (key == Keys.W) speed_r_arm = -1f;
                if (key == Keys.S) speed_r_arm = 1f;
                if (key == Keys.Z) speed_l_farm = 1f;
                if (key == Keys.X) speed_l_farm = -1f;
                if (key == Keys.C) speed_r_farm = -1f;
                if (key == Keys.V) speed_r_farm = 1f;
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;
                if (key == Keys.Up) speed_x = 0;
                if (key == Keys.Down) speed_x = 0;
                if (key == Keys.Q) speed_l_arm = 0;
                if (key == Keys.A) speed_l_arm = 0;
                if (key == Keys.W) speed_r_arm = 0;
                if (key == Keys.S) speed_r_arm = 0;
                if (key == Keys.Z) speed_l_farm = 0;
                if (key == Keys.X) speed_l_farm = 0;
                if (key == Keys.C) speed_r_farm = 0;
                if (key == Keys.V) speed_r_farm = 0;
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

        public static void DrawScene(Window window, float angle_x, float angle_y, float angle_r_arm, float angle_l_arm, float angle_r_farm, float angle_l_farm)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Calculate the position of the camera relative to the model
            vec3 cameraPosition = new vec3(0.0f, 10.0f, -10.0f);
            vec3 modelPosition = new vec3(0.0f, -2.0f, 0.0f);

            // Calculate the rotated camera position around the model's origin
            float rotatedCameraX = cameraPosition.x * (float)Math.Cos(-angle_y) - cameraPosition.z * (float)Math.Sin(-angle_y);
            float rotatedCameraZ = cameraPosition.x * (float)Math.Sin(-angle_y) + cameraPosition.z * (float)Math.Cos(-angle_y);

            // Update the camera position
            vec3 rotatedCameraPosition = new vec3(rotatedCameraX, cameraPosition.y, rotatedCameraZ);

            // Translate the rotated camera position to keep the same distance from the model
            vec3 translatedCameraPosition = rotatedCameraPosition + modelPosition;

            float maxAngleRArm = MathHelper.DegreesToRadians(0);
            float minAngleRArm = MathHelper.DegreesToRadians(-90);

            float maxAngleLArm = MathHelper.DegreesToRadians(90);
            float minAngleLArm = MathHelper.DegreesToRadians(0);

            float maxAngleLeg = MathHelper.DegreesToRadians(15);
            float minAngleLeg = MathHelper.DegreesToRadians(-15);

            float clampedAngleRArm = Math.Clamp(angle_r_arm, minAngleRArm, maxAngleRArm);
            float clampedAngleLArm = Math.Clamp(angle_l_arm, minAngleLArm, maxAngleLArm);
            float clampedAngleRFArm = Math.Clamp(angle_r_farm, minAngleRArm, maxAngleRArm);
            float clampedAngleLFArm = Math.Clamp(angle_l_farm, minAngleLArm, maxAngleLArm);
            float clampedAngleLeg = Math.Clamp(angle_x, minAngleLeg, maxAngleLeg);

            mat4 V = mat4.LookAt(
                translatedCameraPosition,
                modelPosition,
                new vec3(0.0f, 1.0f, 0.0f));
            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1.0f, 1.0f, 50.0f);

            DemoShaders.spLambert.Use();
            GL.UniformMatrix4(DemoShaders.spLambert.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spLambert.U("V"), 1, false, V.Values1D);

            // Donut
            mat4 MTorus = mat4.Translate(new vec3(0.0f, 0.0f, 10.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MTorus.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 0.5f, 0.5f, 0.5f, 1f);
            torus.drawSolid();

            // Korpus robota
            mat4 MBody = mat4.Rotate(angle_y, new vec3(0, 1, 0));
            MBody *= mat4.Scale(new vec3(1.0f, 2.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MBody.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 0f, 1f, 1f, 1f);
            body.drawSolid();

            // Lewa reka robota
            mat4 MLeftArm = mat4.Rotate(angle_y, new vec3(0, 1, 0));
            MLeftArm *= mat4.Translate(new vec3(1.4f, 1.0f, 0.0f));
            MLeftArm *= mat4.Rotate(clampedAngleLArm, new vec3(0, 0, 1)); // Local rotation around Z axis
            MLeftArm *= mat4.Scale(new vec3(0.3f, 0.75f, 0.3f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MLeftArm.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0f, 0f, 1f);
            leftArm1.drawSolid();
            MLeftArm *= mat4.Translate(new vec3(0.0f, -2.0f, 0.0f));
            MLeftArm *= mat4.Scale(new vec3(1.0f / 0.3f, 1.0f / 0.75f, 1.0f / 0.3f));
            MLeftArm *= mat4.Rotate(clampedAngleLFArm, new vec3(0, 0, 1));
            MLeftArm *= mat4.Scale(new vec3(0.3f, 0.75f, 0.3f)); // Scale
            MLeftArm *= mat4.Translate(new vec3(0.0f, -1.5f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MLeftArm.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0.25f, 0f, 1f);
            leftArm2.drawSolid();

            // Prawa reka robota
            mat4 MRightArm = mat4.Rotate(angle_y, new vec3(0, 1, 0)); // Global rotation around Y axis
            MRightArm *= mat4.Translate(new vec3(-1.4f, 1.0f, 0.0f)); // Translate to shoulder joint
            MRightArm *= mat4.Rotate(clampedAngleRArm, new vec3(0, 0, 1)); // Local rotation around Z axis
            MRightArm *= mat4.Scale(new vec3(0.3f, 0.75f, 0.3f)); // Scale
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MRightArm.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0f, 0f, 1f);
            rightArm1.drawSolid();
            MRightArm *= mat4.Translate(new vec3(0.0f, -2.0f, 0.0f)); // Translate to forearm
            MRightArm *= mat4.Scale(new vec3(1.0f / 0.3f, 1.0f / 0.75f, 1.0f / 0.3f));
            MRightArm *= mat4.Rotate(clampedAngleRFArm, new vec3(0, 0, 1));
            MRightArm *= mat4.Scale(new vec3(0.3f, 0.75f, 0.3f)); // Scale
            MRightArm *= mat4.Translate(new vec3(0.0f, -1.5f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MRightArm.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0.25f, 0f, 1f);
            rightArm2.drawSolid();

            // Lewa noga robota
            mat4 MLeftLeg = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(clampedAngleLeg, new vec3(1, 0, 0));
            MLeftLeg *= mat4.Scale(new vec3(0.5f, 0.75f, 0.5f));
            MLeftLeg *= mat4.Translate(new vec3(1.1f, -3.7f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MLeftLeg.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0f, 0f, 1f);
            leftLeg1.drawSolid();
            MLeftLeg *= mat4.Translate(new vec3(0.0f, -2.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MLeftLeg.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0.25f, 0f, 1f);
            leftLeg2.drawSolid();

            // Prawa noga robota
            mat4 MRightLeg = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(-clampedAngleLeg, new vec3(1, 0, 0));
            MRightLeg *= mat4.Scale(new vec3(0.5f, 0.75f, 0.5f));
            MRightLeg *= mat4.Translate(new vec3(-1.1f, -3.7f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MRightLeg.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0f, 0f, 1f);
            rightLeg1.drawSolid();
            MRightLeg *= mat4.Translate(new vec3(0.0f, -2.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MRightLeg.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 1f, 0.25f, 0f, 1f);
            rightLeg2.drawSolid();

            // Głowa robota
            mat4 MHead = mat4.Rotate(angle_y, new vec3(0, 1, 0));
            MHead *= mat4.RotateY(-1.56f);
            MHead *= mat4.Scale(new vec3(2.0f, 2.0f, 2.0f));
            MHead *= mat4.Translate(new vec3(0.0f, 1.38f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spLambert.U("M"), 1, false, MHead.Values1D);
            GL.Uniform4(DemoShaders.spLambert.U("color"), 0.5f, 0.5f, 1f, 1f);
            head.drawSolid();

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
            float angle_r_arm = 0;
            float angle_l_arm = 0;
            float angle_r_farm = 0;
            float angle_l_farm = 0;
            float maxAngle = MathHelper.DegreesToRadians(15);
            float minAngle = MathHelper.DegreesToRadians(-15);

            while (!Glfw.WindowShouldClose(window))
            {
                if (speed_x == 0) angle_x = 0;
                angle_x += speed_x * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi X zgodnie z prędkością obrotu
                angle_y += speed_y * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi Y zgodnie z prędkością obrotu
                angle_r_arm += speed_r_arm * (float)Glfw.Time;
                angle_l_arm += speed_l_arm * (float)Glfw.Time;
                angle_r_farm += speed_r_farm * (float)Glfw.Time;
                angle_l_farm += speed_l_farm * (float)Glfw.Time;
                Glfw.Time = 0; //Wyzeruj licznik czasu
                if (angle_x >= maxAngle || angle_x <= minAngle)
                {
                    speed_x = -speed_x; // Change the direction of rotation if angle_x reaches 15 or -15 degrees
                }
                DrawScene(window, angle_x, angle_y, angle_r_arm, angle_l_arm, angle_r_farm, angle_l_farm);

                Glfw.PollEvents();
            }


            FreeOpenGLProgram(window);

            Glfw.Terminate();
        }


    }
}