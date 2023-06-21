using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FRJ.Sensor.GPS))]
public class GPSSample : MonoBehaviour
{
    private float _timeElapsed = 0f;
    private float _timeStamp   = 0f;

    private FRJ.Sensor.GPS _gps;
    
    void Start()
    {
        // Setup GPS
        this._gps = GetComponent<FRJ.Sensor.GPS>();
        this._gps.Init();
    }

    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._gps.updateRate))
        {
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update GPS 
            this._gps.updateGPS();

            // You can access GPGGA message
            string gpgga = this._gps.gpgga;
        }
    }
}
