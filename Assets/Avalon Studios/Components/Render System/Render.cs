using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Utils.CustomRandom;
using AvalonStudios.Additions.Utils.InputsManager;

using System.IO;
using UnityEngine;

namespace AvalonStudios.Additions.Components.RenderSystem
{
    public class Render : MonoBehaviour
    {
        public enum Format { JPG, PNG, RAW }

        [StyledHeader("General")]

        [SerializeField, Tooltip("Image width.")]
        private int width = 1920;

        [SerializeField, Tooltip("Image height.")]
        private int height = 1080;

        [SerializeField, Tooltip("Optimize render system for many screenshots.")]
        private bool optimizeForManyScreenshots = true;

        [StyledHeader("Setup")]

        [SerializeField, Tooltip("Input key to take the screenshot.")]
        private KeyInputManager screenshotInput = null;

        [SerializeField, Tooltip("Time between screenshot.")]
        private float timeBtwScreenshots = .5f;

        [SerializeField, Tooltip("Image format.")]
        private Format format = Format.JPG;

        [SerializeField, Tooltip("Folder to store.")]
        private string folder = "";

        private Rect rect;
        private RenderTexture renderTexture;
        private Texture2D screenshot;
        private new Camera camera;
        private string formatName;
        private float lastTimeScreen;

        private void Awake()
        {
            lastTimeScreen = -timeBtwScreenshots;
            camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (screenshotInput.Execute() && Cooldown())
                Screenshot();
        }

        private bool Cooldown()
        {
            bool result;
            if (Time.time >= lastTimeScreen + timeBtwScreenshots)
            {
                result = true;
                lastTimeScreen = Time.time;
            }
            else
                result = true;

            return result;
        }

        private void Screenshot()
        {
            if (renderTexture == null)
            {
                rect = new Rect(0, 0, width, height);
                renderTexture = new RenderTexture(width, height, 24);
                screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            }

            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(rect, 0, 0);
            screenshot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;

            byte[] fileHeader = null;
            byte[] fileData = null;

            switch (format)
            {
                case Format.JPG:
                    fileData = screenshot.EncodeToJPG();
                    formatName = "JPG";
                    break;
                case Format.PNG:
                    fileData = screenshot.EncodeToPNG();
                    formatName = "PNG";
                    break;
                case Format.RAW:
                    fileData = screenshot.GetRawTextureData();
                    formatName = "RAW";
                    break;
                default:
                    Debug.LogError("The action could not be performed.");
                    break;
            }

            string filename = UniqueFileName((int)rect.width, (int)rect.height, formatName);

            FileStream fileStream = File.Create(filename);
            if (fileHeader != null)
                fileStream.Write(fileHeader, 0, fileHeader.Length);

            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();

            if (!optimizeForManyScreenshots)
            {
                Destroy(renderTexture);
                renderTexture = null;
                screenshot = null;
            }
        }

        private string UniqueFileName(int width, int height, string format)
        {
            if (folder.Equals("") && folder.Length == 0)
            {
                folder = Application.persistentDataPath;
                if (Application.isEditor)
                {
                    string path = folder + "/..";
                    folder = Path.GetFullPath(path);
                }

                folder += "/screenshots";

                Directory.CreateDirectory(folder);
            }

            string randomName = CustomRandom.RandomString(6);
            string filename = $"{folder}/screen_{width}x{height}_{randomName}.{format}";

            while (File.Exists(filename))
            {
                randomName = CustomRandom.RandomString(6);
                filename = $"{folder}/screen_{width}x{height}_{randomName}.{format}";
            }

            return filename;
        }
    }
}
