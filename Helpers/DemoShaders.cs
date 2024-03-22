using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaders
{
    public class DemoShaders
    {
        public static ShaderProgram spConstant;
        public static ShaderProgram spColored;
        public static ShaderProgram spLambert;
        public static ShaderProgram spTextured;
        public static ShaderProgram spLambertTextured;

        public static void InitShaders(string dir)
        {
            spTextured = new ShaderProgram(dir + "v_textured.glsl", dir + "f_textured.glsl");

            spConstant = new ShaderProgram(dir + "v_constant.glsl", dir + "f_constant.glsl");
            spColored = new ShaderProgram(dir + "v_colored.glsl", dir + "f_colored.glsl");
            spLambert = new ShaderProgram(dir + "v_lambert.glsl", dir + "f_lambert.glsl");
            spLambertTextured = new ShaderProgram(dir + "v_lamberttextured.glsl", dir + "f_lamberttextured.glsl");
        }
    }
}
