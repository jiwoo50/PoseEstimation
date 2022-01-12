using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Camera targetCamera;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(getCameraInstinct());
    }
    Matrix4x4 getCameraInstinct()
    {
        float fx = targetCamera.focalLength / targetCamera.sensorSize.x * targetCamera.pixelWidth;
        Debug.Log(targetCamera.sensorSize.x );
        Debug.Log(targetCamera.pixelWidth);
        float fy = targetCamera.focalLength / targetCamera.sensorSize.y * targetCamera.pixelHeight;
        Debug.Log(targetCamera.sensorSize.y);
        Debug.Log(targetCamera.pixelHeight);
        float cx = targetCamera.pixelWidth / 2f;
        float cy = targetCamera.pixelHeight / 2f;

        Vector4 v1 = new Vector4(fx, 0, 0, 0);
        Vector4 v2 = new Vector4(0, fy, 0, 0);
        Vector4 v3 = new Vector4(cx, cy, 1, 0);
        Vector4 v4 = new Vector4(0, 0, 0, 0);

        Matrix4x4 K = new Matrix4x4(v1, v2, v3, v4);
        return K;

        //testblock
        //Matrix4x4 mat = targetCamera.worldToCameraMatrix;

        //Vector3 testPts1 = obb_data.objs[0].transform.position;
        //Vector3 testPts2 = targetCamera.WorldToScreenPoint(obb_data.objs[0].transform.position);

        //Vector4 tmp = mat * new Vector4(testPts1.x, testPts1.y, testPts1.z, 1f);

        //Vector3 res = K.MultiplyPoint3x4(tmp);
        //res.x /= res.z;
        //res.y /= res.z;
        //res.z *= -1;

        //Debug.Log(K);
        //Debug.Log(res + ", tz = " + res.z);
        //Debug.Log(testPts2 + ", depth = " + testPts2.z);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
