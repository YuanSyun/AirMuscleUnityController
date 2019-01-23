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

    [Header("Left Sine Wave")]
    public bool leftSinWaveFlag = true;
    public float leftSendSinSpeed = 0.05f;
    public float leftSinWaveTime = 1.0f;
    public int leftMinSinPercentage = 0;
    public int leftMaxSinPercentage = 70;

    [Header("Left Air Controller")]
    public Text leftPressureText;
    public Text leftPercentageText;
    public Slider leftSinSlider;
    private bool m_LSV1 = false;
    private bool m_LSV2 = false;
    private bool m_LSV3 = false;
    private bool m_LSV4 = false;
    private float m_leftValvePercentage = 0;

    [Header("Right Sine Wave")]
    public bool rightSinWaveFlag = true;
    public float rightSendSinSpeed = 0.05f;
    public float rightSinWaveTime = 1.0f;
    public float rightMinSinPercentage = 0;
    public int rightMaxSinPercentage = 70;

    [Header("Right Air Controller")]
    public Text rightPressureText;
    public Text rightPercentageText;
    public Slider rightSinSlider;
    private bool m_RSV1 = false;
    private bool m_RSV2 = false;
    private bool m_RSV3 = false;
    private bool m_RSV4 = false;
    private float m_rightValvePercentage = 0;



    #region api
    public void SetLeftValve(string _value)
    {
        if (_value != "") m_leftValvePercentage = float.Parse(_value);
        SendSingal();
    }

    public void SetLeftLayer1(bool _flag)
    {
        m_LSV1 = _flag;
        SendSingal();
    }

    public void SetLeftLayer2(bool _flag)
    {
        m_LSV2 = _flag;
        SendSingal();
    }

    public void SetLeftLayer3(bool _flag)
    {
        m_LSV3 = _flag;
        SendSingal();
    }

    public void SetLeftLayer4(bool _flag)
    {
        m_LSV4 = _flag;
        SendSingal();
    }

    public void StartLeftSinWave()
    {
        leftSinWaveFlag = true;
        StartCoroutine(RunLeftSinWave());
    }

    public void PauseLeftSinWave()
    {
        leftSinWaveFlag = false;
    }

    public void StopLeftSinWave()
    {
        leftSinWaveFlag = false;
        SendSingal();
    }

    public void SetRightValue(string _value)
    {
        if (_value != "") m_rightValvePercentage = float.Parse(_value);
        SendSingal();
    }

    public void SetRightLayer1(bool _flag)
    {
        m_RSV1 = _flag;
        SendSingal();
    }

    public void SetRightLayer2(bool _flag)
    {
        m_RSV2 = _flag;
        SendSingal();
    }

    public void SetRightLayer3(bool _flag)
    {
        m_RSV3 = _flag;
        SendSingal();
    }

    public void SetRightLayer4(bool _flag)
    {
        m_RSV4 = _flag;
        SendSingal();
    }

    public void StartRightSinWave()
    {
        rightSinWaveFlag = true;
        StartCoroutine(RunRightSinWave());
    }

    public void PauseRightSinWave()
    {
        rightSinWaveFlag = false;
    }

    public void StopRightSinWave()
    {
        rightSinWaveFlag = false;
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

        if (leftSinWaveFlag) StartLeftSinWave();
        if (rightSinWaveFlag) StartRightSinWave();
    }




    // Update is called once per frame
    void Update() {

        /* Update the pressure value form the CKD pressure valvue */
        string pressureValue = sender.GetSerialReadData;
        if (pressureValue != "")
        {
            string[] values = pressureValue.Split(' ');
            if(leftPressureText != null)
            {
                leftPressureText.text = AnalogValueToKPa(values[0]).ToString();
            }
            if(rightPressureText != null)
            {
                rightPressureText.text = AnalogValueToKPa(values[1]).ToString();
            }
        }
    }



    
    int AnalogValueToKPa(string _value)
    {
        float kpa = 0;
        try
        {
            int analog = int.Parse(_value);

            /* Transform the analog signal(0~1023) to voltages (0~5) */
            float voltage = (5 * analog) / 1023.0f;

            /* Transform the voltage to KPa */
            if(voltage >= 1.0f)
            {
                kpa = (225 * voltage) - 225;
            }

        }catch(System.FormatException e)
        {
            //Debug.LogFormat("[Debug] {0}", e.Message);
        }

        return (int)kpa;
    }

    void SendSingal()
    {
        /* Limit the value value */
        m_leftValvePercentage = Mathf.Clamp(m_leftValvePercentage, MIN_VALVE, MAX_VALVE);
        m_rightValvePercentage = Mathf.Clamp(m_rightValvePercentage, MIN_VALVE, MAX_VALVE);

        /* Sending to the arduino*/
        string command = "";
        command += m_leftValvePercentage.ToString() + " ";
        command += (m_LSV1 ? 1 : 0) + " ";
        command += (m_LSV2 ? 1 : 0) + " ";
        command += (m_LSV3 ? 1 : 0) + " ";
        command += (m_LSV4 ? 1 : 0) + " ";
        command += m_rightValvePercentage + " ";
        command += (m_RSV1 ? 1 : 0) + " ";
        command += (m_RSV2 ? 1 : 0) + " ";
        command += (m_RSV3 ? 1 : 0) + " ";
        command += (m_RSV4 ? 1 : 0);
        sender.WriteToArduino(command);

        if (DEBUG_LOG)
        {
            Debug.LogFormat("[Command] {0}", command);
        }

        /* refreshing the UI */
        if (leftPercentageText != null) leftPercentageText.text = ((int)m_leftValvePercentage).ToString() + "%";
        if (leftSinSlider != null) leftSinSlider.value = m_leftValvePercentage;
        if (rightPercentageText != null) rightPercentageText.text = ((int)m_rightValvePercentage).ToString() + "%";
        if (rightSinSlider != null) rightSinSlider.value = m_rightValvePercentage;
    }



    IEnumerator RunLeftSinWave()
    {
        while (leftSinWaveFlag)
        {
            
            float t = Mathf.PingPong(Time.time * leftSinWaveTime, Mathf.PI);
            //Debug.LogFormat("Testing sin wave, the valve: {0}", t);

            //半個正弦波，一個週期3秒，SinWaveTime可控制週期。
            this.m_leftValvePercentage = leftMaxSinPercentage * Mathf.Sin(t) + leftMinSinPercentage;

            SendSingal();

            yield return new WaitForSeconds(leftSendSinSpeed);
        }
        m_leftValvePercentage = 0;
    }

    IEnumerator RunRightSinWave()
    {
        while (rightSinWaveFlag)
        {
            float t = Mathf.PingPong(Time.time * rightSinWaveTime, Mathf.PI);

            m_rightValvePercentage = rightMaxSinPercentage * Mathf.Sin(t) + rightMinSinPercentage;

            SendSingal();

            yield return new WaitForSeconds(rightSendSinSpeed);
        }
        m_rightValvePercentage = 0;
    }

    void Reset()
    {
        m_leftValvePercentage = 0;
        m_LSV1 = false;
        m_LSV2 = false;
        m_LSV3 = false;
        m_LSV4 = false;

        m_rightValvePercentage = 0;
        m_RSV1 = false;
        m_RSV2 = false;
        m_RSV3 = false;
        m_RSV4 = false;
    }

}
