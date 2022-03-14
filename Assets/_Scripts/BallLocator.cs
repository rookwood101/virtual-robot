using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;

public class BallLocator : MonoBehaviour
{
    private CameraToTexture cameraTexture;
    void Awake()
    {
        print(CvInvoke.BuildInformation);
        cameraTexture = GetComponent<CameraToTexture>();
        cameraTexture.eventManager.AddListener(CameraToTexture.RenderedToTextureEvent, LocateBallInFrame);
    }

    void LocateBallInFrame(object _frame)
    {
        var frame = (Texture2D)_frame;
        var mat = ConvertTexture(frame);
        CvInvoke.Imshow("frame", mat);
        CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Rgb2Hsv);
        var saturationThreshold = 25; // out of 255
        CvInvoke.GaussianBlur(mat, mat, new Size(11, 11), 0);
        CvInvoke.InRange(mat, new ScalarArray(new MCvScalar(0, 0, 0)), new ScalarArray(new MCvScalar(255, saturationThreshold, 255)), mat);
        CvInvoke.Imshow("threshold", mat);
    }

    private static Mat ConvertTexture(Texture2D texture)
    {
        Color32[] colors = texture.GetPixels32();
        if (colors == null || colors.Length == 0)
            return null;

        int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
        int length = lengthOfColor32 * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            var mat = new Mat(texture.height, texture.width, Emgu.CV.CvEnum.DepthType.Cv8U, 4, ptr, 0);
            mat = mat.Clone();
            CvInvoke.CvtColor(mat, mat, Emgu.CV.CvEnum.ColorConversion.Bgra2Rgb);
            CvInvoke.Flip(mat, mat, Emgu.CV.CvEnum.FlipType.Vertical);
            return mat.Clone();
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }
    }
}
