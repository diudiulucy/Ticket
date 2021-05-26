using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class CameraTest : MonoBehaviour
{

    public string DeviceName;
    public Vector2 CameraSize;
    public float CameraFPS;  //在计算机图像领域中，“FPS”是词组“Frames Per Second”的缩写。“Frames Per Second”在计算机图像范畴内被翻译为：“每秒传输帧数”。更确切的解释，就是“每秒中填充图像的帧数(帧/秒)“。

    public RawImage rawImage;//相机渲染的UI
    WebCamTexture _webCameraTexture; //接收返回的图片数据 
    public GameObject Plane;//作为显示摄像头的面板
    public Button btnTakePhoto;

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "初始化相机"))
        {
            StartCoroutine("InitCameraCor");//开始协程，意思就是启动一个辅助的线程
        }

        //添加一个按钮来控制摄像机的开和关
        if (GUI.Button(new Rect(100, 250, 100, 100), "ON/OFF"))
        {
            if (_webCameraTexture != null && Plane != null)
            {

                if (_webCameraTexture.isPlaying)
                {
                    Debug.Log("=================停止");
                    StopCamera();
                }

                else
                {
                    Debug.Log("=================播放");
                    PlayCamera();
                }

            }
        }
        if (GUI.Button(new Rect(100, 450, 100, 100), "Quit"))
        {

            Application.Quit();//退出
        }

    }

    public void PlayCamera()
    {
        Plane.GetComponent<MeshRenderer>().enabled = true;
        _webCameraTexture.Play();
    }


    public void StopCamera()
    {
        Plane.GetComponent<MeshRenderer>().enabled = false;
        _webCameraTexture.Stop();
    }

    /// <summary> 
    /// 初始化摄像头
    /// </summary> 
    public IEnumerator InitCameraCor()   //定义一个协程
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("==================================初始化成功");

            // 监控第一次授权，是否获得到设备（因为很可能第一次授权了，但是获得不到设备，这里这样避免）
            // 多次 都没有获得设备，可能就是真没有摄像头，结束获取 camera
            int i = 0;
            while (WebCamTexture.devices.Length <= 0 && i < 300)
            {
                yield return new WaitForEndOfFrame();
                i++;
            }

            if (WebCamTexture.devices.Length <= 0)
            {
                Debug.LogError("没有摄像头设备，请检查");
            }
            else
            {
                WebCamDevice[] devices = WebCamTexture.devices;


                DeviceName = devices[0].name;
                Debug.Log("==================================有设备" + DeviceName);
                _webCameraTexture = new WebCamTexture(DeviceName, (int)Screen.width, (int)Screen.height, (int)60);
                Plane.GetComponent<Renderer>().material.mainTexture = _webCameraTexture;
                Plane.transform.localScale = new Vector3(1, 1, 1);

                if (rawImage != null)
                {
                    rawImage.texture = _webCameraTexture;
                }

                _webCameraTexture.Play();
            }
        }
    }

    void Start()
    {
        addEvent();
    }

    void addEvent()
    {
        btnTakePhoto.onClick.AddListener(() =>
        {
            Debug.Log("点击拍照");
            savePhoto(_webCameraTexture);
        });
    }

    void savePhoto(WebCamTexture webCamTexture)
    {
        Texture2D t2d = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.ARGB32, true);
        t2d.SetPixels(webCamTexture.GetPixels());
        t2d.Apply();
        byte[] imageTytes = t2d.EncodeToJPG();
        File.WriteAllBytes(Application.streamingAssetsPath + "/mys/" + Time.time + ".jpg", imageTytes);
        File.WriteAllBytes(Application.persistentDataPath + "/myp/" + Time.time + ".jpg", imageTytes);
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Application.persistentDataPath);
        Destroy(t2d);
        t2d = null;
    }
}