using GlmNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public class Model
    {
        private List<int[]> faces = new List<int[]>();
        private List<int[]> textureIndices = new List<int[]>();
        private List<vec3> vertices = new List<vec3>();
        private List<vec3> textureCoords = new List<vec3>();

        public static Model Parse(string text)
        {
            Model model = new Model();

            foreach (var line in text.Split('\n'))
            {
                var tokens = line.Split(' ').Where(s => s.Length != 0).ToArray();
                var line2 = line.Replace("  ", " ");
                if (line2.StartsWith("v ") && tokens.Length == 4)
                {
                    float x = (float)double.Parse(tokens[1], CultureInfo.InvariantCulture);
                    float y = (float)double.Parse(tokens[2], CultureInfo.InvariantCulture);
                    float z = (float)double.Parse(tokens[3], CultureInfo.InvariantCulture);
                    model.vertices.Add(new vec3(x, y, z));

                }
                else if (line2.StartsWith("vt "))
                {
                    if (tokens.Length == 4)
                    {
                        float x = (float)double.Parse(tokens[1], CultureInfo.InvariantCulture);
                        float y = (float)double.Parse(tokens[2], CultureInfo.InvariantCulture);
                        float z = (float)double.Parse(tokens[3], CultureInfo.InvariantCulture);
                        model.textureCoords.Add(new vec3(x, y, z));
                    }
                }
                else if (line2.StartsWith("f ") && tokens.Length == 4)
                {
                    int[] faces = new int[3];
                    if (Regex.IsMatch(tokens[1], @"\d+/\d+/\d+"))
                    {
                        int[] textureIndices = new int[3];
                        for (int i = 0; i < 3; i++)
                        {
                            TakeIntAndInt(tokens[i + 1], out faces[i], out textureIndices[i]);
                        }
                        model.textureIndices.Add(textureIndices);
                    }
                    else
                    {
                        faces[0] = TakeInt(tokens[1]);
                        faces[1] = TakeInt(tokens[2]);
                        faces[2] = TakeInt(tokens[3]);
                    }
                    model.faces.Add(faces);
                }
                else if (line.StartsWith("f ") && tokens.Length == 5)
                {
                    int[] faces = new int[4];
                    int a = TakeInt(tokens[1]);
                    int b = TakeInt(tokens[2]);
                    int c = TakeInt(tokens[3]);
                    int d = TakeInt(tokens[4]);

                    model.faces.Add(new int[] { a, b, c });
                    model.faces.Add(new int[] { c, d, a });
                }
            }

            return model;
        }

        static int TakeInt(string text)
        {
            Match match = Regex.Match(text, @"(\d+).*"); ;
            return int.Parse(match.Groups[1].Value);
        }

        static void TakeIntAndInt(string text, out int index1, out int index2)
        {
            Match match = Regex.Match(text, @"(\d+)/(\d+).*"); ;
            index1 = int.Parse(match.Groups[1].Value);
            index2 = int.Parse(match.Groups[2].Value);
        }

        public static Model FromFile(string file) => Parse(File.ReadAllText(file));

        public List<int []> Faces
        {
            get { return faces; }
        }

        public vec3 GetVertex(int face, int index)
        {
            int i = faces[face][index];
            return vertices[i - 1];
        }

        public vec3 GetTexture(int face, int index)
        {
            int i = textureIndices[face][index];
            return textureCoords[i - 1];
        }

        public vec3 Vertex(int index)
        {
            return new vec3(0);
        }

        public bool HasTextures() => textureCoords.Count > 0;

    }
}
