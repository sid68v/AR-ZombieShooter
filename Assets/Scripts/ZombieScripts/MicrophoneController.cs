using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioListener))]

public class MicrophoneController : MonoBehaviour
{
    public static MicrophoneController Instance;


    public float dBOffsetInitial = 500f;
    public float maxAmplitude;
    public float minimumThreshold = 50f;
    public int recordingDurationLooped = 999;

    [HideInInspector]
    public AudioSource audioSource;
    AudioClip recordedClip;
    int samples = 128;
    string device;

    bool isRecording;
    bool isInitialOffsetSet;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isInitialOffsetSet = false;
        StartCoroutine(SetPeak());
        InitMicrophone();
    }

    private void OnEnable()
    {
        StartCoroutine(SetPeak());
        InitMicrophone();
    }

    // set microphone device and start recording from it.
    public void InitMicrophone()
    {
        //get the first mic device connected.
        if (device == null)
            device = Microphone.devices[0];

        recordedClip = Microphone.Start(device, true, recordingDurationLooped, 44100);
        isRecording = true;
    }

    public void StopMicrophone()
    {
        Microphone.End(device);
        isRecording = false;
    }

    public float GetCurrentMaxAmplitude()
    {
        float[] samplingWindow = new float[samples];

        int micPosition = Microphone.GetPosition(device) - (samples + 1);
        if (micPosition < 0)
            return 0;

        recordedClip.GetData(samplingWindow, micPosition);

        float maxPeak = 0;
        for (int windowSampleCount = 0; windowSampleCount < samplingWindow.Length; windowSampleCount++)
        {
            float currentPeak = samplingWindow[windowSampleCount] * samplingWindow[windowSampleCount];
            if (maxPeak < currentPeak)
                maxPeak = currentPeak;
        }

        if (!isInitialOffsetSet)
        {
            dBOffsetInitial = -20 * Mathf.Log(Mathf.Abs(maxPeak));
            isInitialOffsetSet = true;
        }
        float maxdB = Mathf.RoundToInt(dBOffsetInitial + 20 * Mathf.Log(Mathf.Abs(maxPeak)) + GameController.Instance.sensitivitySlider.value);

        return maxdB;

    }

    private void OnDestroy()
    {
        StopMicrophone();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            if (!isRecording)
                InitMicrophone();
        if (!focus)
            StopMicrophone();

    }


    public IEnumerator SetPeak()
    {
        while (true)
        {
            if (isRecording)
            {
                maxAmplitude = GetCurrentMaxAmplitude();
                GameController.Instance.soundText.text = "Intensity : " + maxAmplitude.ToString();
                if (maxAmplitude > minimumThreshold)
                {
                    GameController.Instance.isFire = true;
                    audioSource.Play();
                    yield return new WaitForSeconds(.2f);
                    //yield return new WaitWhile( ()=> audioSource.isPlaying);
                }
              SetIntensityColor();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetIntensityColor()
    {
        if (maxAmplitude < minimumThreshold)
            GameController.Instance.soundText.color = Color.gray;
        else if (maxAmplitude > minimumThreshold+100)
            GameController.Instance.soundText.color = Color.red;
        else if (maxAmplitude > minimumThreshold+70)
            GameController.Instance.soundText.color = Color.yellow;
        else if (maxAmplitude > minimumThreshold+30)
            GameController.Instance.soundText.color = Color.green;
        else if (maxAmplitude > minimumThreshold)
            GameController.Instance.soundText.color = Color.cyan;

    }


    // Update is called once per frame
    void Update()
    {

        //Debug.Log("Sound Amplitude in dB : " + maxAmplitude);

        //float[] spectrum = new float[256];

        //AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        //for (int i = 1; i < spectrum.Length - 1; i++)
        //{
        //    Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
        //    Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
        //    Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
        //    Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        //}
    }

}
