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



	// Use this for initialization
	void Awake () {

        try
        {
            stream = new SerialPort("\\\\.\\COM15", 14400);
            stream.ReadTimeout = 2;
            stream.WriteTimeout = 32;
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


    public void WriteToArduino(string message)
    {
        if (stream.IsOpen && (message != ""))
        {
            try
            {
                stream.WriteLine(message);
            }
            catch(IOException e)
            {
                Debug.Log(e.Message);
            }
        }
    }



    
    void OnDisable()
    {
        /* When exit the app will reset the valve value */
        Debug.LogFormat("[Debug] Reset the valve value");
        WriteToArduino("0 0 0 0 0");
        if (stream.IsOpen) stream.Close();
    }

    IEnumerator ReadTheSerialPort()
    {
        yield return new WaitForSeconds(1.0f);

        while (stream != null && stream.IsOpen)
        {
            try
            {
                string serial_info = stream.ReadLine();
                Debug.LogFormat("[SerialPort]\n{0}", serial_info);

            }
            catch(System.TimeoutException e)
            {
                //Debug.LogFormat("[Debug] {0}", e.Message);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
