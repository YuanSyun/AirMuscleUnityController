using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniOSC;
using OSCsharp.Data;

public class WeightSensorReset :  UniOSCEventDispatcher{

    public bool resetWeightSenser;
    public bool changeCalivrationFactor = false;
    public int calibrationFactor = 39070; // for 9kg, 5v

    public override void OnEnable()
    {
        base.OnEnable();

        ClearData();

        AppendData(0);
        AppendData(0);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (resetWeightSenser || changeCalivrationFactor)
        {
            SendResetWeight();
        }
	}

    public void SendResetWeight()
    {
        /* Send the reset weight signal to adafruit*/
        if(_OSCeArg.Packet is OscMessage)
        {
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            msg.UpdateDataAt(0, resetWeightSenser ? 1 : 0);
            msg.UpdateDataAt(1, calibrationFactor);
        }
        _SendOSCMessage(_OSCeArg);
        resetWeightSenser = false;
        changeCalivrationFactor = false;
        Debug.LogFormat("[OSC] Send the reset weight signal");
    }
}
