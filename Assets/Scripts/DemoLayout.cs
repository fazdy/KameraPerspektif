using UnityEngine;

namespace Grafkom.KameraPerspektif
{
    // Satu layout dipakai oleh kamera (Y dari bawah) dan IMGUI (Y dari atas).
    internal readonly struct DemoLayout
    {
        internal const float Width = 1440f;
        internal const float Height = 810f;
        private readonly float screenWidth;
        private readonly float screenHeight;
        private readonly float scale;
        private readonly Vector2 origin;

        internal DemoLayout(float width, float height)
        {
            screenWidth = Mathf.Max(1f, width);
            screenHeight = Mathf.Max(1f, height);
            scale = Mathf.Min(screenWidth / Width, screenHeight / Height);
            origin = new Vector2((screenWidth - Width * scale) * 0.5f, (screenHeight - Height * scale) * 0.5f);
        }

        internal Matrix4x4 GuiMatrix => Matrix4x4.TRS(new Vector3(origin.x, origin.y, 0), Quaternion.identity,
            new Vector3(scale, scale, 1));

        internal static Rect Viewport(DemoMode mode, bool orthographic)
        {
            if (mode != DemoMode.Compare)
                return new Rect(28, 185, 1384, 386);

            return new Rect(orthographic ? 730 : 28, 185, 682, 386);
        }

        internal Rect CameraRect(DemoMode mode, bool orthographic)
        {
            Rect rect = Viewport(mode, orthographic);
            return new Rect(
                (origin.x + rect.x * scale) / screenWidth,
                1f - (origin.y + rect.yMax * scale) / screenHeight,
                rect.width * scale / screenWidth,
                rect.height * scale / screenHeight);
        }
    }
}
