using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.IO.Ports;

public class ArduinoSender : MonoBehaviour {

    [Header("Serial Setting")]
    public string Port = "COM15";
    public bool readSerialPort = false;

    [Header("Data")]
    public string data="";

    private SerialPort stream;
    private string m_serialReadData = "";



    #region api
    public string GetSerialReadData
    {
        get { return m_serialReadData; }
    }

    public void WriteToArduino(string message)
    {
        if (stream.IsOpen && (message != ""))
        {
            try
            {
                stream.WriteLine(message);
                stream.BaseStream.Flush();
            }
            catch (IOException e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    #endregion



    // Use this for initialization
    void Awake () {

        try
        {
            stream = new SerialPort("\\\\.\\COM15", 19200);
            stream.ReadTimeout = 8;
            stream.WriteTimeout = 32;
            stream.NewLine = "\n";
            stream.Open();
        }
        catch (IOException e)
        {
            Debug.Log(e);

            string[] ports_name = SerialPort.GetPortNames();
            string current_port = "";
            for(int i=0;i<ports_name.Length;i++)
            {
                current_port += ports_name[i] + ", ";
            }
            Debug.Log(current_port);

            this.gameObject.SetActive(false);
        }

	}

    void Start()
    {
        if (readSerialPort)
        {
            StartCoroutine(ReadTheSerialPort());
            Debug.LogFormat("[Debug] start reading the serial port");
        }
    }

	// Update is called once per frame
	void Update () {

        if(data != "")
        {
            WriteToArduino(data);
            data = "";
        }
	}

    void OnDisable()
    {
        /* When exit the app will reset the valve value */
        Debug.LogFormat("[Debug] Reset the valve value");
        WriteToArduino("0 0 0 0 0 0 0 0 0 0");
        if (stream.IsOpen) stream.Close();
    }

    IEnumerator ReadTheSerialPort()
    {
        yield return new WaitForSeconds(1.0f);

        Debug.LogFormat("stream: {0}, is open: {1}", stream, stream.IsOpen);

        while (stream != null && stream.IsOpen)
        {
            try
            {
                m_serialReadData = stream.ReadLine();

                //stream.BaseStream.Flush();
                //stream.DiscardInBuffer();
                //Debug.LogFormat("[SerialPort]\n{0}", m_serialReadData);
            }
            catch (System.TimeoutException e)
            {
                //Debug.LogFormat("[Debug] {0}", e.Message);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
