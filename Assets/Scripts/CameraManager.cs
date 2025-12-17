using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Threading;

public class CameraManager : MonoBehaviour
{
    readonly VideoCapture facecam = new();
    readonly Mat mat = new();
    [SerializeField] RawImage canvasimage;

    Texture2D webcamTexture;

    void Awake()
    {
        if (!facecam.Open(0, VideoCaptureAPIs.DSHOW))
        {
            bool opened = false;
            for (int i = 0; i < 10; i++)
            {
                if (facecam.Open(i))
                {
                    Debug.Log($"Opened webcam at index {i}");
                    opened = true;
                    break;
                }
            }
            if (!opened) Debug.LogWarning("Failed to open any webcam.");
        }
        else
        {
            Debug.Log("Opened default webcam (DSHOW).");
        }
        ThreadStart threadDelegate = new(CamThread);
        Thread newThread = new(threadDelegate);
        newThread.Start();
    }

    void CamThread()
    {
        while (true)
        {
            if (facecam == null || !facecam.IsOpened()) break;

            var frame = new Mat();
            facecam.Read(frame);
            if (frame.Empty()) break;

            lock (mat) { Cv2.CvtColor(frame, mat, ColorConversionCodes.BGR2RGBA); }
        }
    }

    void FixedUpdate()
    {
        lock (mat)
        {
            int width = mat.Width;
            int height = mat.Height;
            int channels = mat.Channels(); // should be 4 (RGBA)
            int size = width * height * channels;

            if (webcamTexture == null || webcamTexture.width != width || webcamTexture.height != height)
            {
                if (webcamTexture != null) Destroy(webcamTexture);
                webcamTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                webcamTexture.wrapMode = TextureWrapMode.Clamp;
                canvasimage.texture = webcamTexture;
            }

            var raw = new byte[size];
            var raw2 = new byte[size];
            Marshal.Copy(mat.Data, raw, 0, size);
            int rowSize = width * channels;
            for (int y = 0; y < height; y++)
            {
                int srcIndex = y * rowSize;
                int dstIndex = (height - 1 - y) * rowSize;
                System.Array.Copy(raw, srcIndex, raw2, dstIndex, rowSize);
            }
            webcamTexture.LoadRawTextureData(raw2);
            webcamTexture.Apply(false);
        }
    }

    void OnDisable()
    {
        if (facecam != null && facecam.IsOpened()) facecam.Release();
        mat.Dispose();
        if (webcamTexture != null) Destroy(webcamTexture);
    }
}
