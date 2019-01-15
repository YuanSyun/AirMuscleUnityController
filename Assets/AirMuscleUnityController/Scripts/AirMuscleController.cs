using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AirMuscleController : MonoBehaviour {

    [Header("Debug")]
    public bool DEBUG_LOG = false;

    [Header("Arduino")]
    public ArduinoSender sender;
    public const int MAX_VALVE = 70;
    public const int MIN_VALVE = 0;

    [Header("Sine Wave")]
    public bool FlagEnabeSinWave = true;
    public float SendSinWaveSpeed = 0.05f;
    public float SinWaveTime = 1.0f;
    public int MinSinValve = 0;
    public int MaxSinValve = 70;
    public Text DebugText;
    public Slider DebugSinSlider;

    [Header("Left Air Controller")]
    private bool m_LSV1 = false;
    private bool m_LSV2 = false;
    private bool m_LSV3 = false;
    private bool m_LSV4 = false;
    private float leftValveValue = 0;

    private float lastSendingTime;



    #region api
    public void SetLeftValve(string _value)
    {
        if (_value != "") this.leftValveValue = float.Parse(_value);
        SendSingal();
    }

    public void SetLeftLayer1(bool _flag)
    {
        this.m_LSV1 = _flag;
        SendSingal();
    }

    public void SetLeftLayer2(bool _flag)
    {
        this.m_LSV2 = _flag;
        SendSingal();
    }

    public void SetLeftLayer3(bool _flag)
    {
        this.m_LSV3 = _flag;
        SendSingal();
    }

    public void SetLeftLayer4(bool _flag)
    {
        this.m_LSV4 = _flag;
        SendSingal();
    }

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
        this.leftValveValue = 0;
        SendSingal();
    }
    #endregion



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



    void OnDisable()
    {

    }



    void SendSingal()
    {
        /* Limit the value value */
        this.leftValveValue = Mathf.Clamp(this.leftValveValue, MIN_VALVE, MAX_VALVE);

        /* Sending to the arduino*/
        string command = "";
        command += this.leftValveValue.ToString() + " ";
        command += (m_LSV1 ? 1 : 0) + " ";
        command += (m_LSV2 ? 1 : 0) + " ";
        command += (m_LSV3 ? 1 : 0) + " ";
        command += (m_LSV4 ? 1 : 0);
        sender.WriteToArduino(command);
        if (DEBUG_LOG)
        {
            Debug.LogFormat("[Command] {0}", command);
        }

        /* refreshing the UI */
        if (DebugText != null) DebugText.text = ((int)this.leftValveValue).ToString() + "%";
        if (DebugSinSlider != null) DebugSinSlider.value = this.leftValveValue;
    }



    IEnumerator TestingSinWave()
    {
        while (FlagEnabeSinWave)
        {
            
            float t = Mathf.PingPong(Time.time * SinWaveTime, Mathf.PI);
            //Debug.LogFormat("Testing sin wave, the valve: {0}", t);

            //半個正弦波，一個週期3秒，SinWaveTime可控制週期。
            this.leftValveValue = MaxSinValve * Mathf.Sin(t) + MinSinValve;

            SendSingal();

            yield return new WaitForSeconds(SendSinWaveSpeed);
        }
    }

    void Reset()
    {
        this.leftValveValue = 0;
        m_LSV1 = false;
        m_LSV2 = false;
        m_LSV3 = false;
        m_LSV4 = false;
    }

}
