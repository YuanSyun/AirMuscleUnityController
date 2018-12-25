using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AirMuscleController : MonoBehaviour {

    [Header("Arduino")]
    public ArduinoSender sender;
    public const int MAX_VALVE = 70;
    public const int MIN_VALVE = 0;

    [Header("Debug")]
    public bool FlagEnabeSinWave = true;
    public float SendSinWaveSpeed = 0.05f;
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




    public void SetValve(string _value)
    {
        if(_value != "") SetValve(float.Parse(_value));
    }



    void SetValve(float _value)
    {
        /* Limit the value value */
        _value = Mathf.Clamp(_value, MIN_VALVE, MAX_VALVE);

        /* Sending to the arduino*/
        sender.WriteToArduino(_value.ToString());

        /* refreshing the UI */
        if (DebugText != null) DebugText.text = ((int)_value).ToString()+"%";
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
            
            float t = Mathf.PingPong(Time.time * SinWaveTime, Mathf.PI);
            //Debug.LogFormat("Testing sin wave, the valve: {0}", t);

            //半個正弦波，一個週期3秒，SinWaveTime可控制週期。
            float valve = MaxSinValve * Mathf.Sin(t) + MinSinValve;
            
            SetValve(valve);

            yield return new WaitForSeconds(SendSinWaveSpeed);
        }
    }
    #endregion

}
