using UnityEngine;

namespace Grafkom.KameraPerspektif
{
    internal enum DemoMode
    {
        Compare, Perspective, Orthographic
    }

    public sealed class CameraPerspectiveDemo : MonoBehaviour
    {
        // Urutan tombol: depan, serong, tinggi, rendah.
        private static readonly int[] RigPresets = { 0, 3, 1, 2 };

        internal CameraRig Rig { get; private set; }
        internal DemoMode Mode { get; set; }
        internal int Preset { get; private set; }
        internal bool ShowMeasurements { get; set; } = true;
        internal bool ShowHelp { get; set; }

        private GameObject stage;
        private DemoWorld world;
        private DemoHud hud;

        private void Awake()
        {
            stage = new GameObject("Laboratorium Proyeksi");
            stage.transform.SetParent(transform, false);
            world = new DemoWorld(stage.transform);
            Rig = new CameraRig(stage.transform);

            // Kamera ini membersihkan area yang berada di luar kedua viewport.
            Camera backdrop = new GameObject("Latar HUD").AddComponent<Camera>();
            backdrop.transform.SetParent(stage.transform, false);
            backdrop.depth = -100;
            backdrop.cullingMask = 0;
            backdrop.clearFlags = CameraClearFlags.SolidColor;
            backdrop.backgroundColor = new Color32(21, 15, 36, 255);
            backdrop.allowHDR = false;
            backdrop.allowMSAA = false;
            backdrop.gameObject.AddComponent<AudioListener>();

            hud = new DemoHud(this);
            UpdateCameras();
        }

        private void LateUpdate()
        {
            UpdateCameras();
        }

        private void OnGUI()
        {
            hud?.Draw();
        }

        internal void ChoosePreset(int index)
        {
            Rig.SetPreset(RigPresets[index]);
            Preset = index;
        }

        internal void ResetDemo()
        {
            Rig.Reset();
            Mode = DemoMode.Compare;
            Preset = 0;
            ShowHelp = false;
            ShowMeasurements = true;
        }

        private void UpdateCameras()
        {
            var layout = new DemoLayout(Screen.width, Screen.height);
            Rig.Apply(
                layout.CameraRect(Mode, false), layout.CameraRect(Mode, true),
                Mode == DemoMode.Compare, Mode == DemoMode.Orthographic);
        }

        private void OnDestroy()
        {
            hud?.Dispose();
            world?.Dispose();
            if (stage != null)
                Destroy(stage);
        }
    }
}
