using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class detect : MonoBehaviour
{

    public Camera _camera;
    public Transform Cude;
    int i=0;
    
    private void Start()
    {
        float width = 640;
        float height = 480;
        float fx, fy, cx, cy;
        fx = 572.4114f;
        fy = 573.57043f;
        cx = 325.2611f;
        cy = 242.04899f;
        Matrix4x4 m = PerspectiveOffCenter(fx,fy,cx,cy,width,height);
        StartCoroutine("moveToCam");
        
    }
    static Matrix4x4 PerspectiveOffCenter(float fx,float fy,float cx,float cy,float width,float height)
    {
        
        float x = 2.0F * fx / (width);
        float y = 2.0F * fy / (height);
        float a = 0;
        float b = 0;
        float c = -(10000 + 10) / (1000 - 10);
        float d = -(2.0F * 10000 * 10) / (10000 - 10);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }
    IEnumerator moveToCam()
    {
        for (i = 0; i < 100; i++)
        {
            float temp = Time.time * 100f;

            Random.InitState((int)temp);
            float randX = Random.Range(0f, 360f);
            float randY = Random.Range(0f, 180f);
            float randZ = Random.Range(0f, 360f);
            float x = Cude.position.x + 3 * Mathf.Cos(randX * Mathf.PI / 180);
            float y = Cude.position.y + 3 * Mathf.Sin(randY * Mathf.PI / 180);
            float z = Cude.position.z + 3 * Mathf.Sin(randZ * Mathf.PI / 180);
            Vector3 dir = new Vector3(Cude.transform.position.x - x, Cude.transform.position.y - y, Cude.transform.position.z - z);
            _camera.transform.rotation = Quaternion.LookRotation(dir);
            _camera.transform.position = new Vector3(x, y, z);
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
    private void fibonacci_Sampling(int n_pts,float radius)
    {
        Assert.IsTrue(n_pts % 2 == 1);
        int n_pts_half = n_pts / 2;
        float phi = (Mathf.Sqrt(5.0f) + 1.0f) / 2.0f;
        float phi_inv = phi - 1.0f;
        float ga = 2.0f * Mathf.PI * phi_inv;

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
