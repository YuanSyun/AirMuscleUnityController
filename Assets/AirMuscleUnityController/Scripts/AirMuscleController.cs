using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AirMuscleController : MonoBehaviour {

    [Header("Arduino")]
    public ArduinoSender sender;

    [Header("Debug")]
    public bool FlagTestingSinWave = true;
    public float SendSinWaveSpeed = 0.1f;
    public float SinWaveTime = 1.0f;
    public int MinSinValve = 0;
    public int MaxSinValve = 70;
    public Text DebugText;
    public Slider DebugSinSlider;
    private float lastSinTime;



	// Use this for initialization
	void Start () {
		
        if(sender==null)
        {
            Debug.Log("Not found the arduino sender");
            this.gameObject.SetActive(false);
        }

        lastSinTime = Time.time;
        if (SinWaveTime < 0.1f) SinWaveTime = 0.1f;
    }




    // Update is called once per frame
    void Update() {

        if (FlagTestingSinWave && (Time.time - lastSinTime > SendSinWaveSpeed))
        {
            //int valve = (int)(MaxSinValve * Mathf.Sin(Mathf.PingPong(Time.time * 2.0f, Mathf.PI)) + MinSinValve);
            float valve = MaxSinValve * Mathf.Sin(Mathf.PingPong(Time.time * SinWaveTime, Mathf.PI)) + MinSinValve;
            //Debug.LogFormat("Testing sin wave, the valve: {0}", valve);
            sender.WriteToArduino(valve.ToString());
            if (DebugText != null) DebugText.text = valve.ToString();
            if (DebugSinSlider != null) DebugSinSlider.value = valve;
            lastSinTime = Time.time;
        }
    }




    public void StartSinWave()
    {
        FlagTestingSinWave = true;
    }

    public void EndSinWave()
    {
        FlagTestingSinWave = false;
    }
}
