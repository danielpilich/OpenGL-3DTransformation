using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;

namespace Models
{
    public class Torus:Model
    {

		public Torus() {
			buildTorus(0.75f,0.25f,20,20);
        }

		public Torus(float R, float r, float mainDivs, float tubeDivs) {
			buildTorus(R, r, mainDivs, tubeDivs);
		}

		vec4 generateTorusPoint(float R, float r, float alpha, float beta)
		{
			alpha = glm.Radians(alpha);
			beta = glm.Radians(beta);
			return new vec4((float)( (R + r * Math.Cos(alpha)) * Math.Cos(beta)), (float)((R + r * Math.Cos(alpha)) * Math.Sin(beta)), (float)(r * Math.Sin(alpha)), 1.0f);
		}

		vec4 computeVertexNormal(float alpha, float beta)
		{
			alpha = glm.Radians(alpha);
			beta = glm.Radians(beta);
			return new vec4((float) (Math.Cos(alpha) * Math.Cos(beta)), (float)(Math.Cos(alpha) * Math.Sin(beta)), (float)Math.Sin(alpha), 0.0f);
		}

		vec4 computeFaceNormal(vec4[] face)
		{
			vec3 a = new vec3(face[1] - face[0]);
			vec3 b = new vec3(face[2] - face[0]);

			return (new vec4(vec3.Cross(b, a), 0.0f)).Normalized;
		}

		void generateTorusFace(vec4[] vertices, vec4[] vertexNormals, ref vec4 faceNormal, float R, float r, float alpha, float beta, float step_alpha, float step_beta)
		{

			vertices[0] = generateTorusPoint(R, r, alpha, beta);
			vertices[1] = generateTorusPoint(R, r, alpha + step_alpha, beta);
			vertices[2] = generateTorusPoint(R, r, alpha + step_alpha, beta + step_beta);
			vertices[3] = generateTorusPoint(R, r, alpha, beta + step_beta);

			faceNormal = computeFaceNormal(vertices);

			vertexNormals[0]=computeVertexNormal(alpha, beta);
			vertexNormals[1]=computeVertexNormal(alpha + step_alpha, beta);
			vertexNormals[2]=computeVertexNormal(alpha + step_alpha, beta + step_beta);
			vertexNormals[3]=computeVertexNormal(alpha, beta + step_beta);
		}

		void AddVec4(List<float> target, vec4 value)
        {
			target.Add(value[0]);
			target.Add(value[1]);
			target.Add(value[2]);
			target.Add(value[3]);
		}
		


		void buildTorus(float R, float r, float mainDivs, float tubeDivs)
		{
			vec4[] face= { new vec4(), new vec4(), new vec4(), new vec4() };
			vec4[] faceVertexNormals = { new vec4(), new vec4(), new vec4(), new vec4() };			
			vec4 normal= new vec4();


			List<float> internalVertices=new List<float>();
			List<float> internalFaceNormals = new List<float>();
			List<float> internalVertexNormals = new List<float>();
			List<float> internalColors = new List<float>();


			float mult_alpha = 360.0f / tubeDivs;
			float mult_beta = 360.0f / mainDivs;

			vec4 green = new vec4(0, 1, 0, 1);

			for (int alpha = 0; alpha < Math.Round(tubeDivs); alpha++)
			{
				for (int beta = 0; beta < Math.Round(mainDivs); beta++)
				{

					generateTorusFace( face, faceVertexNormals, ref normal, R, r, alpha * mult_alpha, beta * mult_beta, mult_alpha, mult_beta);

					AddVec4(internalVertices, face[0]);
					AddVec4(internalVertices, face[1]);
					AddVec4(internalVertices, face[2]);

					AddVec4(internalVertices, face[0]);
					AddVec4(internalVertices, face[2]);
					AddVec4(internalVertices, face[3]);

					AddVec4(internalVertexNormals, faceVertexNormals[0]);
					AddVec4(internalVertexNormals, faceVertexNormals[1]);
					AddVec4(internalVertexNormals, faceVertexNormals[2]);

					AddVec4(internalVertexNormals, faceVertexNormals[0]);
					AddVec4(internalVertexNormals, faceVertexNormals[2]);
					AddVec4(internalVertexNormals, faceVertexNormals[3]);

					for (int i = 0; i < 6; i++) AddVec4(internalFaceNormals, normal);
					for (int i = 0; i < 6; i++) AddVec4(internalColors, green);

				}
			}



			vertices = internalVertices.ToArray();
			normals = internalFaceNormals.ToArray();
			vertexNormals = internalVertexNormals.ToArray();
			colors = internalColors.ToArray();			
			vertexCount = internalVertices.Count/4;

			texCoords = new float[vertexCount * 2];
			for (int i = 0; i < vertexCount; i++)
			{
				texCoords[2 * i] = vertexNormals[4 * i];
				texCoords[2 * i + 1] = vertexNormals[4 * i + 1];
			}
		}
		
	}



}
