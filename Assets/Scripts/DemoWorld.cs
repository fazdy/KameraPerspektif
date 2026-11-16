using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Grafkom.KameraPerspektif
{
    /// <summary>
    /// Builds the fixed scene shared by the perspective and orthographic cameras.
    /// The three marked cubes have identical geometry and different positions.
    /// </summary>
    public sealed class DemoWorld : IDisposable
    {
        public const float SampleSide = 2f;
        private static readonly Vector3[] CubeCenters =
        {
            new Vector3(-2.6f, 1f, 0f),
            new Vector3(0f, 1f, 7f),
            new Vector3(2.6f, 1f, 14f)
        };

        private static readonly Color32[] CubeColors =
        {
            new Color32(40, 187, 224, 255),
            new Color32(246, 184, 62, 255),
            new Color32(246, 107, 130, 255)
        };

        private static readonly Vector3[] FaceNormals =
        {
            Vector3.back, Vector3.right, Vector3.forward, Vector3.left, Vector3.up, Vector3.down
        };

        private static readonly Vector3[] FaceRight =
        {
            Vector3.left, Vector3.back, Vector3.right, Vector3.forward, Vector3.left, Vector3.left
        };

        private readonly List<Object> generatedAssets = new List<Object>();
        private readonly GameObject root;
        private readonly Mesh unitCube;
        private readonly Shader shader;
        private bool disposed;

        public DemoWorld(Transform parent)
        {
            // Resources keeps the scene's lit shader available in both the editor and the player.
            shader = Resources.Load<Shader>("WorldSurface");
            if (shader == null || !shader.isSupported)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null || !shader.isSupported)
            {
                shader = Shader.Find("Legacy Shaders/Diffuse");
            }

            if (shader == null || !shader.isSupported)
            {
                throw new InvalidOperationException("Shader Built-in Standard atau Diffuse tidak tersedia.");
            }

            root = new GameObject("Lingkungan demo");
            root.transform.SetParent(parent, false);
            unitCube = Own(BuildUnitCube());
            BuildGround();
            BuildArchitecture();
            BuildSample("A", CubeCenter(0), CubeColor(0), new[]
            {
                "01110", "11011", "11011", "11111", "11011", "11011", "11011"
            });
            BuildSample("B", CubeCenter(1), CubeColor(1), new[]
            {
                "11110", "11011", "11011", "11110", "11011", "11011", "11110"
            });
            BuildSample("C", CubeCenter(2), CubeColor(2), new[]
            {
                "01111", "11000", "11000", "11000", "11000", "11000", "01111"
            });
            BuildLighting();
        }

        public static Vector3 CubeCenter(int index) => CubeCenters[index];

        public static Color CubeColor(int index) => CubeColors[index];

        private void BuildGround()
        {
            Transform floor = CreateGroup("Pelataran dan garis panduan");
            Material ground = MakeMaterial("Slate", new Color32(42, 55, 77, 255));
            Material asphalt = MakeMaterial("Aspal", new Color32(36, 45, 65, 255));
            Material paving = MakeMaterial("Jalur pejalan kaki", new Color32(63, 67, 97, 255));
            Material seam = MakeMaterial("Sambungan lantai", new Color32(45, 51, 75, 255));
            Material boundary = MakeMaterial("Garis tepi", new Color32(153, 178, 220, 255));
            Material centerLine = MakeMaterial("Garis lintasan", new Color32(133, 107, 185, 255));

            AddBox(floor, "Tanah", new Vector3(0f, -0.65f, 16f),
                new Vector3(112f, 1f, 112f), ground);
            AddBox(floor, "Lintasan utama", new Vector3(0f, -0.05f, 18f),
                new Vector3(10.8f, 0.1f, 90f), asphalt);

            for (int side = -1; side <= 1; side += 2)
            {
                AddBox(floor, "Trotoar", new Vector3(side * 7.3f, -0.05f, 18f),
                    new Vector3(3.7f, 0.15f, 90f), paving);
                AddBox(floor, "Tepi lintasan", new Vector3(side * 5.12f, 0.005f, 18f),
                    new Vector3(0.07f, 0.01f, 90f), boundary);
                AddBox(floor, "Tepi luar trotoar", new Vector3(side * 9.1f, 0.005f, 18f),
                    new Vector3(0.12f, 0.06f, 90f), seam);

                for (int step = -7; step <= 15; step++)
                {
                    float z = step * 4f;
                    AddBox(floor, "Sambungan trotoar", new Vector3(side * 7.3f, 0.027f, z),
                        new Vector3(3.65f, 0.004f, 0.055f), seam);
                    AddBox(floor, "Penanda jarak", new Vector3(side * 4.95f, 0.006f, z),
                        new Vector3(0.36f, 0.012f, 0.06f), boundary);
                    AddBox(floor, "Garis putus", new Vector3(side * 1.8f, 0.006f, z),
                        new Vector3(0.04f, 0.012f, 1.8f), centerLine);
                }
            }
        }

        private void BuildArchitecture()
        {
            Transform architecture = CreateGroup("Bangunan pelataran");
            Material facade = MakeMaterial("Fasad", new Color32(71, 78, 115, 255));
            Material recess = MakeMaterial("Kaca gelap", new Color32(35, 55, 79, 255));
            Material trim = MakeMaterial("Lis beton", new Color32(109, 122, 156, 255));
            Material window = MakeMaterial("Kaca berwarna", new Color32(92, 143, 177, 255));
            Material warmWindow = MakeMaterial("Kaca hangat", new Color32(167, 137, 119, 255));
            Material plant = MakeMaterial("Daun", new Color32(60, 112, 120, 255));

            for (int side = -1; side <= 1; side += 2)
            {
                for (int index = 0; index < 3; index++)
                {
                    float z = 4f + 17f * index + (side == 1 ? 4f : 0f);
                    float height = 3.2f + 1.2f * index + (side == 1 ? 0.7f : 0f);
                    AddBox(architecture, "Massa bangunan", new Vector3(side * 15.1f, height / 2f, z),
                        new Vector3(5f, height, 9.5f), facade, castShadow: true);
                    AddBox(architecture, "Tepi atap", new Vector3(side * 15.1f, height, z),
                        new Vector3(5.25f, 0.18f, 9.7f), trim);
                    AddBox(architecture, "Panel jendela", new Vector3(side * 12.56f, height * 0.57f, z),
                        new Vector3(0.10f, height * 0.45f, 8.05f), recess);

                    for (int bay = -2; bay <= 2; bay++)
                    {
                        Material glazing = (bay + index + side) % 3 == 0 ? warmWindow : window;
                        AddBox(architecture, "Kisi fasad",
                            new Vector3(side * 12.48f, height * 0.57f, z + 1.45f * bay),
                            new Vector3(0.08f, height * 0.36f, 1.13f), glazing);
                    }

                    AddPlanter(architecture, new Vector3(side * 7.9f, 0f, z + 5.1f), trim, plant);
                }
            }

            AddBox(architecture, "Ambang gerbang jauh", new Vector3(0f, 4.5f, 49f),
                new Vector3(12.6f, 0.8f, 1.2f), facade, castShadow: true);

            for (int side = -1; side <= 1; side += 2)
            {
                AddBox(architecture, "Pilar gerbang jauh", new Vector3(side * 5.7f, 2.2f, 49f),
                    new Vector3(1f, 4.4f, 1.2f), facade, castShadow: true);
                AddBox(architecture, "Aksen gerbang", new Vector3(side * 5.7f, 2.52f, 48.36f),
                    new Vector3(0.18f, 2.6f, 0.10f), window);
            }
        }

        private void AddPlanter(Transform parent, Vector3 center, Material baseMaterial, Material leaves)
        {
            AddBox(parent, "Bak tanaman", center + Vector3.up * 0.17f,
                new Vector3(1.1f, 0.26f, 1.25f), baseMaterial);
            Transform canopy = AddBox(parent, "Tanaman", center + Vector3.up * 0.68f,
                new Vector3(0.72f, 0.72f, 0.72f), leaves);
            canopy.localRotation = Quaternion.Euler(14f, 34f, 12f);
        }

        private void BuildSample(string label, Vector3 center, Color color, string[] glyph)
        {
            Transform station = CreateGroup("Stasiun " + label);
            station.localPosition = new Vector3(center.x, 0f, center.z);
            Material surface = MakeMaterial("Tekstur kubus " + label, Color.white);
            surface.mainTexture = Own(BuildSampleTexture(label, color, glyph));
            Material marker = MakeMaterial("Penanda " + label, color);
            AddBox(station, "Kubus " + label + " (2 × 2 × 2)", Vector3.up * center.y,
                Vector3.one * SampleSide, surface, castShadow: true);

            // Separate marks show each station without changing the cube's dimensions.
            for (int side = -1; side <= 1; side += 2)
            {
                for (int corner = -1; corner <= 1; corner += 2)
                {
                    AddBox(station, "Sudut penanda", new Vector3(side * 1.46f, 0.008f, corner * 1.28f),
                        new Vector3(0.06f, 0.016f, 0.42f), marker);
                    AddBox(station, "Sudut penanda", new Vector3(side * 1.28f, 0.008f, corner * 1.46f),
                        new Vector3(0.42f, 0.016f, 0.06f), marker);
                }
            }
        }

        private static Texture2D BuildSampleTexture(string label, Color baseColor, string[] glyph)
        {
            const int size = 128;
            const int glyphScale = 8;
            int glyphLeft = (size - glyph[0].Length * glyphScale) / 2;
            int glyphBottom = (size - glyph.Length * glyphScale) / 2;
            Color lightColor = Color.Lerp(baseColor, Color.white, 0.78f);
            Color32[] pixels = new Color32[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int edge = Mathf.Min(Mathf.Min(x, size - 1 - x), Mathf.Min(y, size - 1 - y));
                    bool grid = x % 16 == 0 || y % 16 == 0;
                    float shade = ((x / 16 + y / 16) % 2 == 0) ? 0.89f : 0.95f;
                    Color pixel = baseColor * shade;
                    if (grid)
                    {
                        pixel = Color.Lerp(pixel, lightColor, 0.21f);
                    }

                    if (edge < 3 || edge == 6)
                    {
                        pixel = lightColor;
                    }

                    bool insideGlyph = x >= glyphLeft && x < size - glyphLeft &&
                        y >= glyphBottom && y < size - glyphBottom;
                    if (insideGlyph)
                    {
                        int column = (x - glyphLeft) / glyphScale;
                        int row = glyph.Length - 1 - (y - glyphBottom) / glyphScale;
                        if (glyph[row][column] == '1')
                        {
                            pixel = lightColor;
                        }
                    }

                    pixel.a = 1f;
                    pixels[y * size + x] = pixel;
                }
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: true)
            {
                name = "Grid kalibrasi " + label,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Trilinear,
                anisoLevel = 4
            };
            texture.SetPixels32(pixels);
            texture.Apply(updateMipmaps: true, makeNoLongerReadable: true);
            return texture;
        }

        private void BuildLighting()
        {
            AddDirectionalLight("Cahaya utama", new Vector3(48f, -32f, 0f),
                new Color32(255, 239, 217, 255), 1.17f, LightShadows.Soft);
            AddDirectionalLight("Cahaya pengisi", new Vector3(31f, 137f, 0f),
                new Color32(186, 212, 255, 255), 0.56f, LightShadows.None);
        }

        private void AddDirectionalLight(string name, Vector3 angles, Color color,
            float intensity, LightShadows shadows)
        {
            Transform node = CreateGroup(name);
            node.localRotation = Quaternion.Euler(angles);
            var light = node.gameObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = color;
            light.intensity = intensity;
            light.shadows = shadows;
            light.shadowStrength = 0.66f;
        }

        private Transform CreateGroup(string name)
        {
            var node = new GameObject(name);
            node.transform.SetParent(root.transform, false);
            return node.transform;
        }

        private Transform AddBox(Transform parent, string name, Vector3 position, Vector3 size,
            Material material, bool castShadow = false)
        {
            var box = new GameObject(name);
            Transform transform = box.transform;
            transform.SetParent(parent, false);
            transform.localPosition = position;
            transform.localScale = size;
            box.AddComponent<MeshFilter>().sharedMesh = unitCube;
            var renderer = box.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadow ? ShadowCastingMode.On : ShadowCastingMode.Off;
            return transform;
        }

        private Material MakeMaterial(string name, Color color)
        {
            var material = Own(new Material(shader) { name = name, color = color });
            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", 0.16f);
            }

            return material;
        }

        private static Mesh BuildUnitCube()
        {
            var vertices = new Vector3[24];
            var normals = new Vector3[vertices.Length];
            var textureCoordinates = new Vector2[vertices.Length];
            var indices = new int[36];

            for (int face = 0; face < FaceNormals.Length; face++)
            {
                Vector3 normal = FaceNormals[face];
                Vector3 right = FaceRight[face];
                Vector3 up = Vector3.Cross(normal, right);
                int v = face * 4;
                int t = face * 6;

                vertices[v] = (normal - right - up) * 0.5f;
                vertices[v + 1] = (normal + right - up) * 0.5f;
                vertices[v + 2] = (normal + right + up) * 0.5f;
                vertices[v + 3] = (normal - right + up) * 0.5f;
                // FaceRight follows clockwise winding. Reverse U to read the label from outside.
                textureCoordinates[v] = new Vector2(1f, 0f);
                textureCoordinates[v + 1] = new Vector2(0f, 0f);
                textureCoordinates[v + 2] = new Vector2(0f, 1f);
                textureCoordinates[v + 3] = new Vector2(1f, 1f);

                for (int vertex = 0; vertex < 4; vertex++)
                {
                    normals[v + vertex] = normal;
                }

                indices[t] = v;
                indices[t + 1] = v + 1;
                indices[t + 2] = v + 2;
                indices[t + 3] = v;
                indices[t + 4] = v + 2;
                indices[t + 5] = v + 3;
            }

            return new Mesh
            {
                name = "Kubus satuan dengan UV per wajah",
                vertices = vertices,
                normals = normals,
                uv = textureCoordinates,
                triangles = indices
            };
        }

        private T Own<T>(T asset) where T : Object
        {
            generatedAssets.Add(asset);
            return asset;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Release(root);
            for (int i = generatedAssets.Count - 1; i >= 0; i--)
            {
                Release(generatedAssets[i]);
            }

            generatedAssets.Clear();
        }

        private static void Release(Object target)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }
    }
}
