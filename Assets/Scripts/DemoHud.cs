using System;
using UnityEngine;

namespace Grafkom.KameraPerspektif
{
    internal sealed class DemoHud : IDisposable
    {
        private static readonly Color Ink = new Color32(255, 246, 215, 255);
        private static readonly Color Muted = new Color32(197, 186, 217, 255);
        private static readonly Color Base = new Color32(21, 15, 36, 255);
        private static readonly Color Panel = new Color32(40, 29, 62, 255);
        private static readonly Color Raised = new Color32(64, 46, 94, 255);
        private static readonly Color Cream = new Color32(255, 221, 132, 255);
        private static readonly Color Lilac = new Color32(197, 168, 255, 255);
        private static readonly string[] PresetNames = { "1 Depan", "2 Serong", "3 Tinggi", "4 Rendah" };
        private static readonly string[] ProbeNames = { "A", "B", "C" };
        private static readonly Vector3 RoadDirection = Vector3.forward;

        private readonly CameraPerspectiveDemo demo;
        private readonly Texture2D[] skin = new Texture2D[5];

        private GUIStyle titleStyle;
        private GUIStyle headingStyle;
        private GUIStyle textStyle;
        private GUIStyle smallStyle;
        private GUIStyle numericStyle;
        private GUIStyle markerStyle;
        private GUIStyle buttonStyle;
        private GUIStyle selectedStyle;
        private GUIStyle trackStyle;
        private GUIStyle thumbStyle;
        private bool stylesReady;

        internal DemoHud(CameraPerspectiveDemo demo)
        {
            this.demo = demo;
        }

        internal void Draw()
        {
            if (!stylesReady)
                CreateStyles();

            ReadKeys(Event.current);
            Matrix4x4 previousMatrix = GUI.matrix;
            Color previousColor = GUI.color;
            GUI.matrix = new DemoLayout(Screen.width, Screen.height).GuiMatrix;
            GUI.color = Color.white;

            DrawViewHeading(false);
            DrawViewHeading(true);
            if (demo.ShowHelp)
            {
                DrawControls(false);
                DrawHelp();
            }
            else
            {
                DrawControls(true);
            }

            GUI.matrix = previousMatrix;
            GUI.color = previousColor;
        }

        private void DrawControls(bool interactive)
        {
            Label(28, 16, 560, 22, "GRAFIKA KOMPUTER   /   KELOMPOK 10", smallStyle, Lilac);
            Label(28, 39, 800, 50, "Kamera dan Perspektif", titleStyle, Ink);

            if (Button(1122, 35, 136, 38, "Petunjuk [I]", false, interactive))
                demo.ShowHelp = true;
            if (Button(1268, 35, 144, 38, "Atur ulang [R]", false, interactive))
                demo.ResetDemo();

            ModeButton(28, 148, "Bandingkan", DemoMode.Compare, interactive);
            ModeButton(185, 142, "Perspektif", DemoMode.Perspective, interactive);
            ModeButton(336, 142, "Ortografis", DemoMode.Orthographic, interactive);
            if (Button(1211, 99, 201, 34,
                demo.ShowMeasurements ? "Pengukuran aktif [M]" : "Pengukuran nonaktif [M]",
                demo.ShowMeasurements, interactive))
            {
                demo.ShowMeasurements = !demo.ShowMeasurements;
            }

            DrawCameraSettings(interactive);
            DrawMeasurements();
            Label(28, 772, 1384, 28, "Tab  Ganti tampilan      1–4  Sudut kamera      R  Atur ulang      I  Petunjuk",
                smallStyle, Muted);
        }

        private void ReadKeys(Event input)
        {
            if (input.type != EventType.KeyDown || input.command || input.control || input.alt)
                return;

            switch (input.keyCode)
            {
                case KeyCode.I:
                    demo.ShowHelp = !demo.ShowHelp;
                    break;
                case KeyCode.Escape:
                    if (!demo.ShowHelp)
                        return;
                    demo.ShowHelp = false;
                    break;
                case KeyCode.R:
                    demo.ResetDemo();
                    break;
                case KeyCode.M:
                    demo.ShowMeasurements = !demo.ShowMeasurements;
                    break;
                case KeyCode.Tab:
                    demo.Mode = (DemoMode)(((int)demo.Mode + 1) % 3);
                    break;
                case KeyCode.Alpha1:
                case KeyCode.Alpha2:
                case KeyCode.Alpha3:
                case KeyCode.Alpha4:
                    demo.ChoosePreset((int)input.keyCode - (int)KeyCode.Alpha1);
                    break;
                default:
                    return;
            }

            input.Use();
        }

        private void ModeButton(float x, float width, string text, DemoMode mode, bool interactive)
        {
            if (Button(x, 99, width, 34, text, demo.Mode == mode, interactive))
                demo.Mode = mode;
        }

        private void DrawViewHeading(bool orthographic)
        {
            if (!CameraVisible(orthographic))
                return;

            Rect viewport = DemoLayout.Viewport(demo.Mode, orthographic);
            Color accent = orthographic ? Lilac : Cream;
            Fill(new Rect(viewport.x, 148, viewport.width, 37), Panel);
            Fill(new Rect(viewport.x, 148, 4, 37), accent);
            Label(viewport.x + 15, 148, 215, 37,
                orthographic ? "Ortografis" : "Perspektif", headingStyle, Ink);
            string note = orthographic ? "Ukuran tetap terhadap kedalaman" : "Makin jauh, makin kecil";
            Label(viewport.x + 230, 148, viewport.width - 245, 37, note, smallStyle, accent);

            if (Event.current.type != EventType.Repaint)
                return;

            Camera camera = orthographic ? demo.Rig.Orthographic : demo.Rig.Perspective;
            if (!orthographic)
                DrawVanishingPoint(camera, viewport);

            if (!demo.ShowMeasurements)
                return;

            for (int i = 0; i < ProbeNames.Length; ++i)
            {
                Vector3 center = DemoWorld.CubeCenter(i);
                if (ProjectionMeasurement.ReadCube(camera, center, DemoWorld.SampleSide).Visibility == ProbeVisibility.Full)
                    DrawTag(camera, viewport, center + Vector3.up * 1.24f, ProbeNames[i], DemoWorld.CubeColor(i), 25);
            }
        }

        private static Vector2 ToGuiPoint(Rect viewport, Vector3 point)
        {
            return new Vector2(viewport.x + point.x * viewport.width, viewport.y + (1f - point.y) * viewport.height);
        }

        private void DrawVanishingPoint(Camera camera, Rect viewport)
        {
            if (Vector3.Dot(camera.transform.forward, RoadDirection) < 0.02f)
                return;

            // Proyeksi arah +Z pada jarak sangat besar mendekati titik hilangnya.
            Vector3 point = camera.transform.position + RoadDirection * 10000f;
            DrawTag(camera, viewport, point, "Titik hilang +Z", Cream, 116);
        }

        private void DrawTag(Camera camera, Rect viewport, Vector3 world, string text, Color color, float width)
        {
            Vector3 point = camera.WorldToViewportPoint(world);
            Vector2 gui = ToGuiPoint(viewport, point);
            Rect rect = new Rect(gui.x - width / 2f, gui.y - 12, width, 24);
            if (point.z <= 0f || rect.xMin < viewport.xMin || rect.xMax > viewport.xMax ||
                rect.yMin < viewport.yMin || rect.yMax > viewport.yMax)
                return;

            Fill(rect, Base);
            Fill(new Rect(rect.x, rect.yMax - 2, rect.width, 2), color);
            GUI.Label(rect, text, markerStyle);
        }

        private bool CameraVisible(bool orthographic)
        {
            return demo.Mode == DemoMode.Compare ||
                (orthographic ? demo.Mode == DemoMode.Orthographic : demo.Mode == DemoMode.Perspective);
        }

        private void DrawCameraSettings(bool interactive)
        {
            CameraRig rig = demo.Rig;
            Fill(new Rect(28, 586, 830, 178), Panel);
            Label(47, 596, 218, 34, "Kontrol kamera", headingStyle, Ink);

            for (int i = 0; i < PresetNames.Length; ++i)
            {
                if (Button(287 + i * 132, 599, 121, 30, PresetNames[i], i == demo.Preset, interactive))
                    demo.ChoosePreset(i);
            }

            rig.FieldOfView = Slider(47, 639, "FOV vertikal", rig.FieldOfView,
                CameraRig.MinimumFieldOfView, CameraRig.MaximumFieldOfView, "°",
                interactive && demo.Mode != DemoMode.Orthographic);
            rig.Distance = Slider(453, 639, "Jarak kamera ke A", rig.Distance,
                CameraRig.MinimumDistance, CameraRig.MaximumDistance, " u", interactive);
            rig.Near = Slider(47, 701, "Near clip", rig.Near,
                CameraRig.MinimumNear, CameraRig.MaximumNear, " u", interactive);
            rig.Far = Slider(453, 701, "Far clip", rig.Far,
                CameraRig.MinimumFar, CameraRig.MaximumFar, " u", interactive);
        }

        private float Slider(float x, float y, string title, float value, float min, float max, string unit, bool interactive)
        {
            Label(x, y, 250, 23, title, textStyle, Ink);
            Label(x + 257, y, 124, 23, value.ToString("0.#") + unit, numericStyle, Cream);
            var rect = new Rect(x, y + 28, 381, 19);

            if (interactive)
            {
                float next = GUI.HorizontalSlider(rect, value, min, max, trackStyle, thumbStyle);
                return Mathf.Approximately(next, value) ? value : Mathf.Round(next * 10f) * 0.1f;
            }

            if (Event.current.type == EventType.Repaint)
            {
                float t = Mathf.InverseLerp(min, max, value);
                Fill(new Rect(x, y + 34, rect.width, 4), Raised);
                Fill(new Rect(x + t * (rect.width - 12), y + 28, 12, 16), Muted);
            }

            return value;
        }

        private void DrawMeasurements()
        {
            Fill(new Rect(876, 586, 536, 178), Panel);
            Label(894, 597, 500, 28, "Kubus uji    •    2 × 2 × 2 unit", headingStyle, Ink);

            if (!demo.ShowMeasurements)
            {
                Label(894, 643, 494, 32, "Tekan M untuk menampilkan pengukuran.", textStyle, Muted);
                return;
            }

            Label(893, 629, 66, 25, "Kubus", smallStyle, Muted);
            Label(966, 629, 106, 25, "Kedalaman", smallStyle, Muted);
            Label(1074, 629, 153, 25, "Perspektif", smallStyle, Cream);
            Label(1233, 629, 153, 25, "Ortografis", smallStyle, Lilac);

            for (int i = 0; i < ProbeNames.Length; ++i)
            {
                float y = 656 + 29 * i;
                Vector3 center = DemoWorld.CubeCenter(i);
                CameraRig rig = demo.Rig;
                ProbeReading p = ProjectionMeasurement.ReadCube(rig.Perspective, center, DemoWorld.SampleSide);
                ProbeReading o = ProjectionMeasurement.ReadCube(rig.Orthographic, center, DemoWorld.SampleSide);
                Fill(new Rect(894, y + 5, 12, 12), DemoWorld.CubeColor(i));
                Label(918, y, 42, 22, ProbeNames[i], textStyle, Ink);
                Label(966, y, 100, 22, p.Depth.ToString("0.#") + " u", textStyle, Ink);
                Label(1074, y, 153, 22, CameraVisible(false) ? Describe(p) : "—", smallStyle, Ink);
                Label(1233, y, 161, 22, CameraVisible(true) ? Describe(o) : "—", smallStyle, Ink);
            }
        }

        private static string Describe(ProbeReading reading)
        {
            switch (reading.Visibility)
            {
                case ProbeVisibility.Full:
                    return reading.ProjectedWidth.ToString("0") + " px";
                case ProbeVisibility.Partial:
                    return "Terpotong";
                case ProbeVisibility.BeyondClipPlane:
                    return "Di luar clip";
                default:
                    return "Di luar layar";
            }
        }

        private void DrawHelp()
        {
            Fill(new Rect(0, 0, DemoLayout.Width, DemoLayout.Height), new Color(0, 0, 0, 0.77f));
            Fill(new Rect(279, 125, 882, 553), Panel);
            Fill(new Rect(279, 125, 882, 5), Cream);
            Label(315, 146, 700, 57, "Petunjuk percobaan", titleStyle, Ink);
            if (Button(983, 160, 138, 39, "Tutup [I]", false, true))
                demo.ShowHelp = false;

            HelpEntry(226, "Bandingkan proyeksi",
                "Bandingkan lebar kubus A, B, dan C. Ketiganya berukuran sama di scene.");
            HelpEntry(314, "Pisahkan FOV dari dolly",
                "Ubah FOV untuk mengubah cakupan. Ubah dolly untuk menggeser posisi kamera.");
            HelpEntry(402, "Temukan titik hilang",
                "Ganti sudut kamera. Amati arah garis jalan dan sisi bangunan pada proyeksi perspektif.");
            HelpEntry(490, "Peragakan clipping",
                "Dari posisi awal, atur Far ke 20 untuk menyembunyikan C atau Near ke 12 untuk A.");
            Label(315, 592, 803, 44, "Lebar diukur saat kubus utuh dalam viewport. Tabel tidak memperhitungkan oklusi.",
                smallStyle, Muted);
        }

        private void HelpEntry(float y, string heading, string body)
        {
            Label(315, y, 800, 26, heading, headingStyle, Cream);
            Label(315, y + 32, 810, 35, body, textStyle, Ink);
        }

        private bool Button(float x, float y, float width, float height, string text, bool selected, bool interactive)
        {
            Rect rect = new Rect(x, y, width, height);
            GUIStyle style = selected ? selectedStyle : buttonStyle;
            if (interactive)
                return GUI.Button(rect, text, style);
            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, new GUIContent(text), false, false, false, false);
            return false;
        }

        private static void Label(float x, float y, float w, float h, string text, GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            GUI.Label(new Rect(x, y, w, h), text, style);
        }

        private static void Fill(Rect rect, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            Color prior = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prior;
        }

        private void CreateStyles()
        {
            titleStyle = MakeLabel(40, FontStyle.Bold);
            headingStyle = MakeLabel(21, FontStyle.Bold);
            textStyle = MakeLabel(17, FontStyle.Normal);
            smallStyle = MakeLabel(14, FontStyle.Normal);
            numericStyle = MakeLabel(17, FontStyle.Bold, TextAnchor.MiddleRight);
            markerStyle = MakeLabel(14, FontStyle.Bold, TextAnchor.MiddleCenter);
            markerStyle.normal.textColor = Ink;

            skin[0] = Solid(Raised);
            skin[1] = Solid(new Color32(85, 65, 115, 255));
            skin[2] = Solid(Cream);
            skin[3] = Solid(new Color32(255, 234, 175, 255));
            skin[4] = Solid(Lilac);
            buttonStyle = MakeButton(skin[0], skin[1], Ink);
            selectedStyle = MakeButton(skin[2], skin[3], Base);

            trackStyle = new GUIStyle { fixedHeight = 5, alignment = TextAnchor.MiddleCenter };
            trackStyle.normal.background = skin[1];
            trackStyle.hover.background = skin[1];
            trackStyle.active.background = skin[1];

            thumbStyle = new GUIStyle { fixedWidth = 15, fixedHeight = 18 };
            thumbStyle.normal.background = skin[4];
            thumbStyle.hover.background = skin[2];
            thumbStyle.active.background = skin[3];
            stylesReady = true;
        }

        private static Texture2D Solid(Color color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply(false, true);
            return texture;
        }

        private static GUIStyle MakeLabel(int size, FontStyle weight, TextAnchor align = TextAnchor.MiddleLeft)
        {
            return new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = weight, alignment = align, clipping = TextClipping.Clip };
        }

        private static GUIStyle MakeButton(Texture2D normal, Texture2D hover, Color textColor)
        {
            var style = new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.background = normal;
            style.hover.background = hover;
            style.active.background = hover;
            style.focused.background = hover;
            style.normal.textColor = textColor;
            style.hover.textColor = textColor;
            style.active.textColor = textColor;
            style.focused.textColor = textColor;
            return style;
        }

        public void Dispose()
        {
            foreach (Texture2D texture in skin)
            {
                if (texture != null)
                    UnityEngine.Object.Destroy(texture);
            }
        }
    }
}
