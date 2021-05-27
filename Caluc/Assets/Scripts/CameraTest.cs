using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class CameraTest : MonoBehaviour
{

    public string DeviceName;
    public Vector2 CameraSize;
    public float CameraFPS;  //在计算机图像领域中，“FPS”是词组“Frames Per Second”的缩写。“Frames Per Second”在计算机图像范畴内被翻译为：“每秒传输帧数”。更确切的解释，就是“每秒中填充图像的帧数(帧/秒)“。

    public RawImage rawImage;//相机渲染的UI
    WebCamTexture _webCameraTexture; //接收返回的图片数据 
    public GameObject Plane;//作为显示摄像头的面板
    public Button btnTakePhoto;
    public Button btnClose;

    /// <summary> 
    /// 初始化摄像头
    /// </summary> 
    public IEnumerator InitCameraCor()   //定义一个协程
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Log.i("==================================初始化成功");

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
                _webCameraTexture = new WebCamTexture(DeviceName, (int)rawImage.rectTransform.sizeDelta.x, (int)rawImage.rectTransform.sizeDelta.y, (int)60);
                Log.i("==================================有设备" + DeviceName + _webCameraTexture.videoVerticallyMirrored + "y" + _webCameraTexture.videoRotationAngle);
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
        StartCoroutine("InitCameraCor");//开始协程，意思就是启动一个辅助的线程
    }

    void addEvent()
    {
        btnTakePhoto?.onClick.AddListener(() =>
        {
            Log.i("点击拍照");
            savePhoto(_webCameraTexture);
        });

        btnClose?.onClick.AddListener(() =>
        {
            Log.i("关闭");
            SceneManager.LoadScene("calc");
        });
    }

    void savePhoto(WebCamTexture webCamTexture)
    {
        Texture2D t2d = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.ARGB32, true);
        t2d.SetPixels(webCamTexture.GetPixels());
        t2d.Apply();
        byte[] imageTytes = t2d.EncodeToJPG(100);
        Log.i(Application.streamingAssetsPath);
        Log.i(Application.persistentDataPath);
        string platformPath = Application.persistentDataPath + "/MyPhoto";
#if UNITY_ANDROID 
        platformPath = "/sdcard/DCIM/";
#endif 
        if (!Directory.Exists(platformPath))
        {
            Directory.CreateDirectory(platformPath);
        }

        File.WriteAllBytes(platformPath + "/" + Time.time + ".jpg", imageTytes);
        // File.WriteAllBytes(Application.streamingAssetsPath +"/"+ Time.time + ".jpg", imageTytes);

        Destroy(t2d);
        t2d = null;
    }
}