using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class detect : MonoBehaviour
{
    public float radius;
    public Camera _camera;
    public Transform Cude;
    int i=0;
    
    private void Start()
    {
        
        StartCoroutine("moveToCam");
        
    }
    public Vector3 RandomPointOnSphere()
    {
        float u = Random.Range(0f, 1f);
        float v = Random.Range(0f, 1f);

        float theta = 2f * Mathf.PI * u;
        float phi = Mathf.Acos(2f * v - 1);

        Vector3 vec;
        vec.x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        vec.y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
        vec.z = radius * Mathf.Cos(phi);

        return vec;
    }

    IEnumerator moveToCam()
    {
        for (i = 0; i < 100; i++)
        {
            float temp = Time.time * 100f;

            Vector3 pos = RandomPointOnSphere();
            Vector3 dir = new Vector3(Cude.transform.position.x - pos.x, Cude.transform.position.y - pos.y, Cude.transform.position.z - pos.z);
            _camera.transform.rotation = Quaternion.LookRotation(dir);
            _camera.transform.position = pos;
            Capture();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void Capture()
    {
        string path = Application.dataPath + "/capture"+i+".JPG";
        StartCoroutine(CoCapture(path));
    }

    private IEnumerator CoCapture(string path)
    {
        if (path == null)
        {
            yield break;
        }

        // ReadPixels을 하기 위해서 쉬어줌
        yield return new WaitForEndOfFrame();

        Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
        Texture2D texture = Capture(Camera.main, rect);

        byte[] bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(path, bytes);
    }

    private Texture2D Capture(Camera camera, Rect pRect)
    {
        Texture2D capture;
        CameraClearFlags preClearFlags = camera.clearFlags;
        Color preBackgroundColor = camera.backgroundColor;
        {
            camera.clearFlags = CameraClearFlags.SolidColor;

            camera.backgroundColor = Color.black;
            camera.Render();
            Texture2D blackBackgroundCapture = CaptureView(pRect);

            camera.backgroundColor = Color.white;
            camera.Render();
            Texture2D whiteBackgroundCapture = CaptureView(pRect);

            for (int x = 0; x < whiteBackgroundCapture.width; ++x)
            {
                for (int y = 0; y < whiteBackgroundCapture.height; ++y)
                {
                    Color black = blackBackgroundCapture.GetPixel(x, y);
                    Color white = whiteBackgroundCapture.GetPixel(x, y);
                    if (black != Color.clear)
                    {
                        whiteBackgroundCapture.SetPixel(x, y, GetColor(black, white));
                    }
                }
            }
            
            whiteBackgroundCapture.Apply();
            capture = whiteBackgroundCapture;
            Object.DestroyImmediate(blackBackgroundCapture);/*
            blackBackgroundCapture.Apply();
            capture = blackBackgroundCapture;
            Object.DestroyImmediate(whiteBackgroundCapture);*/

        }
        camera.backgroundColor = preBackgroundColor;
        camera.clearFlags = preClearFlags;
        return capture;
    }
    private Color GetColor(Color black, Color white)
    {
        float alpha = GetAlpha(black.r, white.r);
        return new Color(
            black.r / alpha,
            black.g / alpha,
            black.b / alpha,
            alpha);
    }

    private float GetAlpha(float black, float white)
    {
        return 1 + black - white;
    }

    private Texture2D CaptureView(Rect rect)
    {
        Texture2D captureView = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
        captureView.ReadPixels(rect, 0, 0, false);
        return captureView;
    }
}
