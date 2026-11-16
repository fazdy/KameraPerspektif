using UnityEngine;

namespace Grafkom.KameraPerspektif
{
    // Kedua kamera mengambil gambar dari pose dan bidang kliping yang sama.
    public sealed class CameraRig
    {
        public const float MinimumFieldOfView = 25f;
        public const float MaximumFieldOfView = 85f;
        public const float MinimumDistance = 6f;
        public const float MaximumDistance = 22f;
        public const float MinimumNear = 0.3f;
        public const float MaximumNear = 27f;
        public const float MinimumFar = 4f;
        public const float MaximumFar = 65f;

        private const float DefaultFieldOfView = 55f;
        private const float DefaultDistance = 10f;
        private const float DefaultNear = 0.3f;
        private const float DefaultFar = 60f;
        private const float MinimumClipGap = 0.25f;
        private const float EyeHeight = 2.4f;

        private static readonly Vector3 OrbitCenter = new Vector3(0f, 1.2f, 7f);
        private static readonly Color Backdrop = new Color32(27, 43, 67, 255);

        private float fieldOfView;
        private float distance;
        private float near;
        private float far;
        private int preset;

        public Camera Perspective { get; }
        public Camera Orthographic { get; }

        public float FieldOfView
        {
            get => fieldOfView;
            set => fieldOfView = Mathf.Clamp(value, MinimumFieldOfView, MaximumFieldOfView);
        }

        // Kedalaman positif pusat kubus A sepanjang arah pandang kamera.
        public float Distance
        {
            get => distance;
            set => distance = Mathf.Clamp(value, MinimumDistance, MaximumDistance);
        }

        public float Near
        {
            get => near;
            set => near = Mathf.Clamp(value, MinimumNear, Mathf.Min(MaximumNear, far - MinimumClipGap));
        }

        public float Far
        {
            get => far;
            set => far = Mathf.Clamp(value, Mathf.Max(MinimumFar, near + MinimumClipGap), MaximumFar);
        }

        public CameraRig(Transform parent)
        {
            Perspective = CreateCamera("Kamera Perspektif", parent, false, 0);
            Orthographic = CreateCamera("Kamera Ortografis", parent, true, 1);

            // Setengah tinggi proyeksi perspektif pada bidang acuan (d = 10, FOV = 55°).
            // Nilai ortografis tetap saat FOV atau kedalaman diubah.
            Orthographic.orthographicSize = DefaultDistance * HalfFovTangent(DefaultFieldOfView);

            Reset();
            Apply(new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), false, false);
        }

        public void Reset()
        {
            fieldOfView = DefaultFieldOfView;
            distance = DefaultDistance;
            near = DefaultNear;
            far = DefaultFar;
            preset = 0;
        }

        public void SetPreset(int index)
        {
            preset = Mathf.Clamp(index, 0, 3);
        }

        public void Apply(Rect perspectiveRect, Rect orthographicRect, bool dual, bool orthographicOnly)
        {
            GetPose(out Vector3 position, out Quaternion rotation);
            ApplyPoseAndClipping(Perspective, position, rotation);
            ApplyPoseAndClipping(Orthographic, position, rotation);

            Perspective.fieldOfView = fieldOfView;
            ApplyViewport(Perspective, perspectiveRect, dual || !orthographicOnly);
            ApplyViewport(Orthographic, orthographicRect, dual || orthographicOnly);
        }

        private static float HalfFovTangent(float verticalFieldOfView)
        {
            return Mathf.Tan(verticalFieldOfView * Mathf.Deg2Rad * 0.5f);
        }

        private void GetPose(out Vector3 position, out Quaternion rotation)
        {
            Vector3 nearCubeCenter = DemoWorld.CubeCenter(0);
            if (preset == 0)
            {
                position = new Vector3(0f, EyeHeight, nearCubeCenter.z - distance);
                rotation = Quaternion.identity;
                return;
            }

            Vector3 target = OrbitCenter;
            if (preset == 1)
            {
                // Yaw dan pitch membuat ketiga sumbu memiliki titik hilang yang berbeda.
                rotation = Quaternion.Euler(22f, -17f, 0f);
            }
            else if (preset == 2)
            {
                // Sesuaikan pitch terhadap jarak agar posisi mata tetap dekat tanah.
                float rise = OrbitCenter.y - 0.45f;
                float upwardsPitch = Mathf.Atan2(rise, distance + OrbitCenter.z) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(-upwardsPitch, 0f, 0f);
            }
            else
            {
                // Pitch 0 menjaga garis vertikal tetap sejajar. Sumbu X dan Z memiliki
                // titik hilang berbeda, sehingga mode ini menunjukkan perspektif dua titik.
                rotation = Quaternion.Euler(0f, -25f, 0f);
                target.y = EyeHeight;
            }

            Vector3 forward = rotation * Vector3.forward;
            float targetDepth = distance + Vector3.Dot(target - nearCubeCenter, forward);
            position = target - forward * targetDepth;

            if (preset == 1 || preset == 3)
            {
                // Pusatkan kubus terdekat dan terjauh di sekitar tengah bidang gambar.
                // Pergeseran sejajar bidang gambar tidak mengubah kedalaman kubus A.
                Vector3 right = rotation * Vector3.right;
                Vector3 farCubeCenter = DemoWorld.CubeCenter(2);
                float farCubeDepth = distance + Vector3.Dot(farCubeCenter - nearCubeCenter, forward);
                float nearCubeX = Vector3.Dot(nearCubeCenter - target, right);
                float farCubeX = Vector3.Dot(farCubeCenter - target, right);
                float lateralOffset = (nearCubeX * farCubeDepth + farCubeX * distance)
                    / (farCubeDepth + distance);
                position += right * lateralOffset;
            }
        }

        private static Camera CreateCamera(string name, Transform parent, bool orthographic, int depth)
        {
            var cameraObject = new GameObject(name);
            cameraObject.transform.SetParent(parent, false);

            var camera = cameraObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Backdrop;
            camera.depth = depth;
            camera.orthographic = orthographic;
            camera.useOcclusionCulling = false;
            return camera;
        }

        private void ApplyPoseAndClipping(Camera camera, Vector3 position, Quaternion rotation)
        {
            camera.transform.SetPositionAndRotation(position, rotation);

            // Jika bidang near baru melewati bidang far lama, jauhkan far terlebih dahulu.
            if (camera.farClipPlane < near + MinimumClipGap)
                camera.farClipPlane = far;

            camera.nearClipPlane = near;
            camera.farClipPlane = far;
        }

        private static void ApplyViewport(Camera camera, Rect rect, bool active)
        {
            camera.enabled = active && rect.width > 0f && rect.height > 0f;
            if (!camera.enabled)
                return;

            if (camera.rect != rect)
                camera.rect = rect;

            // Camera.aspect bawaan memakai layar penuh. Pada mode bandingkan, gunakan
            // ukuran panel aktual supaya bentuk objek tidak tertarik atau dipipihkan.
            float aspect = (rect.width * Mathf.Max(Screen.width, 1))
                / (rect.height * Mathf.Max(Screen.height, 1));
            if (!Mathf.Approximately(camera.aspect, aspect))
                camera.aspect = aspect;
        }
    }
}
