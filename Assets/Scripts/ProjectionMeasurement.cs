using UnityEngine;

namespace Grafkom.KameraPerspektif
{
    internal enum ProbeVisibility
    {
        Full, Partial, BeyondClipPlane, OutsideViewport
    }

    internal readonly struct ProbeReading
    {
        internal readonly float Depth;
        internal readonly float ProjectedWidth;
        internal readonly ProbeVisibility Visibility;

        internal ProbeReading(float depth, ProbeVisibility visibility, float width = 0f)
        {
            Depth = depth;
            ProjectedWidth = width;
            Visibility = visibility;
        }
    }

    internal static class ProjectionMeasurement
    {
        internal static ProbeReading ReadCube(Camera camera, Vector3 center, float size)
        {
            float depth = camera.WorldToViewportPoint(center).z;
            Vector3 cameraForward = camera.transform.forward;
            float half = size * 0.5f;
            float depthRadius = half * (Mathf.Abs(cameraForward.x) +
                Mathf.Abs(cameraForward.y) + Mathf.Abs(cameraForward.z));

            if (depth + depthRadius < camera.nearClipPlane || depth - depthRadius > camera.farClipPlane)
                return new ProbeReading(depth, ProbeVisibility.BeyondClipPlane);

            if (depth - depthRadius < camera.nearClipPlane || depth + depthRadius > camera.farClipPlane)
                return new ProbeReading(depth, ProbeVisibility.Partial);

            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            for (int corner = 0; corner < 8; corner++)
            {
                // Kubus dunia tidak berotasi sehingga kedelapan sudutnya berada pada AABB ini.
                var offset = new Vector3(
                    (corner & 1) == 0 ? -half : half,
                    (corner & 2) == 0 ? -half : half,
                    (corner & 4) == 0 ? -half : half);
                Vector3 projected = camera.WorldToViewportPoint(center + offset);
                var point = new Vector2(projected.x, projected.y);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            if (max.x < 0f || min.x > 1f || max.y < 0f || min.y > 1f)
                return new ProbeReading(depth, ProbeVisibility.OutsideViewport);

            if (min.x < 0f || max.x > 1f || min.y < 0f || max.y > 1f)
                return new ProbeReading(depth, ProbeVisibility.Partial);

            // Lebar proyeksi kubus utuh. Pengukuran geometri ini tidak menghitung oklusi.
            return new ProbeReading(depth, ProbeVisibility.Full, (max.x - min.x) * camera.pixelRect.width);
        }
    }
}
