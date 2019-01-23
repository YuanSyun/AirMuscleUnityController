using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UniOSC;
using OSCsharp.Data;

public class WeightSensorReceiver : UniOSCEventTarget {

    [Header("Debug")]
    public bool showReceive = false;
    public float receivedWeight;
    public Text weightText;

    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
        OscMessage msg = (OscMessage)args.Packet;
        if (msg.Data.Count < 1) return;

        receivedWeight = (float)msg.Data[0];
        if (weightText != null) weightText.text = receivedWeight.ToString("F2") + "kg";
        if(showReceive) Debug.LogFormat("[Debug] weight: {0}", receivedWeight);
    }
}
