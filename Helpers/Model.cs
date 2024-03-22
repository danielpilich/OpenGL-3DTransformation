using System;
using OpenTK.Graphics.OpenGL4;

namespace Models
{
	//Abstrakcyjna klasa pomocnicza reprezentująca model 3D
	//Wykorzystywana do implementacji klas reprezentujących przykładowe obiekty 3D używane podczas ćwiczeń
	//Zaprojektowana do wspołpracy z przykładowymi programami cieniującymi DemoShaders.
	public abstract class Model
	{
		public int vertexCount; //Liczba wierzchołków modelu
		public float[] vertices; //Tablica wierzchołków
		public float[] normals; //Tablica wektorów normalnych ścian
		public float[] vertexNormals; //Tablica wektorów normalnych wierzchołków
		public float[] texCoords; //Tablica współrzędnych teksturowania
		public float[] colors; //Tablica kolorów

		//Metoda rysująca siatkę modelu przy wykrozystaniu aktywnego programu cieniującego
		//Parametr smooth mówi, czy mają być wykorzystywane wektory normalne ścian (false), czy wierzchołków (true)
		virtual public void drawWire(bool smooth = false)
		{
			GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Line);

			drawSolid(smooth);

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
		}

		//Metoda rysująca wypełniony model 
		//Parametr smooth mówi, czy mają być wykorzystywane wektory normalne ścian (false), czy wierzchołków (true)
		virtual public void drawSolid(bool smooth=true)
		{
			//Aktywuj atrybuty o numerach 0-3. Warto spojrzeć na kod źródłowy programów cieniujących, żeby się zorientować jakie jest znaczenie poszczególnych atrybutów.
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);

			//Kod rysujący. 
			GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, vertices);  //Powiąż dane z tablicy vertices z atrybutem 0
			if (!smooth) GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, normals); //Powiąż dane z tablicy normals (wektory normalne ścian) z atrybutem 1
			else GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, vertexNormals); //Powiąż dane z tablicy vertexNormals (wektory normalne wierzchołków) z atrybutem 1
			GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, texCoords); //Powiąż dane z tablicy texCoords z atrybutem 2
			GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, colors); //Powiąż dane z tablicy colors z atrybutem 3

			GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount); //Rysuj model
				
			


			//Wyłącz atrybuty o numerach 0-3.
			GL.DisableVertexAttribArray(0);
			GL.DisableVertexAttribArray(1);
			GL.DisableVertexAttribArray(2);
			GL.DisableVertexAttribArray(3);
		}
		

	};
}

