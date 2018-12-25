using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AirMuscleController : MonoBehaviour {

    [Header("Arduino")]
    public ArduinoSender sender;

    [Header("Debug")]
    public bool FlagEnabeSinWave = true;
    public float SendSinWaveSpeed = 0.1f;
    public float SinWaveTime = 1.0f;
    public int MinSinValve = 0;
    public int MaxSinValve = 70;
    public Text DebugText;
    public Slider DebugSinSlider;



	// Use this for initialization
	void Start () {
		
        if(sender==null)
        {
            Debug.Log("Not found the arduino sender");
            this.gameObject.SetActive(false);
        }

        if (FlagEnabeSinWave) StartSinWave();
    }




    // Update is called once per frame
    void Update() {

    }



    void SetValve(float _value)
    {
        sender.WriteToArduino(_value.ToString());
        if (DebugText != null) DebugText.text = _value.ToString();
        if (DebugSinSlider != null) DebugSinSlider.value = _value;
    }



    #region Sin Wave
    public void StartSinWave()
    {
        FlagEnabeSinWave = true;
        StartCoroutine(TestingSinWave());
    }

    public void PauseSinWave()
    {
        FlagEnabeSinWave = false;
    }

    public void StopSinWave()
    {
        FlagEnabeSinWave = false;
        SetValve(0);
    }

    IEnumerator TestingSinWave()
    {
        while (FlagEnabeSinWave)
        {
            float valve = MaxSinValve * Mathf.Sin(Mathf.PingPong(Time.time * SinWaveTime, Mathf.PI)) + MinSinValve;
            //Debug.LogFormat("Testing sin wave, the valve: {0}", valve);
            SetValve(valve);
            yield return new WaitForSeconds(SendSinWaveSpeed);
        }
    }
    #endregion

}
