using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.IO.Ports;

public class ArduinoSender : MonoBehaviour {

    [Header("Serial Setting")]
    public string Port = "COM15";

    [Header("Data")]
    public string data="";

    private SerialPort stream;



	// Use this for initialization
	void Start () {

        try
        {
            stream = new SerialPort("\\\\.\\COM15", 9600);
            stream.ReadTimeout = 50;
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
	


	// Update is called once per frame
	void Update () {

        WriteToArduino(data);
        data = "";

	}


    public void WriteToArduino(string message)
    {
        if (message != "")
        {
            stream.WriteLine(message);
            stream.BaseStream.Flush();
        }
    }



    
    void OnApplicationQuit()
    {
        if (stream.IsOpen) stream.Close();
    }
}
