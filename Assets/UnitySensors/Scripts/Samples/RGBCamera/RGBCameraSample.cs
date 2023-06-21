using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(FRJ.Sensor.RGBCamera))]
public class RGBCameraSample : MonoBehaviour
{

  private float _timeElapsed = 0f;
  private float _timeStamp   = 0f;

  private FRJ.Sensor.RGBCamera _camera;
    
  void Start()
  {
    // Get Rotate Lidar
    this._camera = GetComponent<FRJ.Sensor.RGBCamera>();
    this._camera.Init();
  }

    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._camera.scanRate))
        {
            // You can access compressed (jpeg) data as following.
            byte[] camera_data = this._camera.data;
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;
        }
    }
}
