using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public TerrainManager tm;

    public Slider frequencySlider;
    public Slider smoothnessSlider;
    public Text smoothText;
    public Slider amplitudeSlider;
    public Text ampText;
    public Slider octaveSlider;
    public Text octText;
    public Slider powerSlider;
    public Text powText;

    public static float amp = 1f;

    public static float frequency = 1f;
    public static Vector2 offset = new Vector2( 0f, 0f );
    public  Vector2 off = new Vector2( 0f, 0f );

    enum FunctionType
    {
        Noise,
        fBm,
        Ridged
    }
    FunctionType lastFunction = FunctionType.Noise;
    private void Start() {
        off = new Vector2( Random.Range( 0f, 128f ), Random.Range( 0f, 128f ) );

        SetSmoothText( smoothnessSlider.value );
        SetAmpText( amplitudeSlider.value );
        SetOctText( octaveSlider.value );
        SetPowText(powerSlider.value);
    }
    private void Update() {
        offset = off;
        if(Input.GetKeyDown(KeyCode.R))
            off = new Vector2( Random.Range( 0f, 4000000f ), Random.Range( 0f, 4000000f ) );
    }
    public void ResetTerrain() {
        tm.ResetTerrain();
    }
    public void DoFault() {
        tm.DoFault();
    }
    public void DoNoise() {
        lastFunction = FunctionType.Noise;
        tm.DoNoise(1.0f / (amplitudeSlider.value * (smoothnessSlider.value)), amplitudeSlider.value * frequencySlider.value);
    }
    public void DoFBM()
    {
        lastFunction = FunctionType.fBm;
        tm.DoFBM(1.0f / (amplitudeSlider.value * (smoothnessSlider.value)), amplitudeSlider.value * frequencySlider.value, Mathf.FloorToInt(octaveSlider.value), powerSlider.value);
    }

    public void DoRidged()
    {
        lastFunction = FunctionType.Ridged;
        tm.DoRidge(1.0f / (amplitudeSlider.value * (smoothnessSlider.value)), amplitudeSlider.value * frequencySlider.value, Mathf.FloorToInt( octaveSlider.value ), powerSlider.value );
    }

    public void DoRandomise() {
        tm.Randomise( amplitudeSlider.value );
    }
    public void DoSmooth()
    {
        tm.DoSmooth();
    }
    public void DoMidpointDisplacement() {
        tm.DoMidPoint( amplitudeSlider.value );
    }

    //Text setters
    public void SetSmoothText( float val ) {
        frequency = val;
        smoothText.text = val.ToString();
    }
    public void SetAmpText( float val ) {
        amp = val;
        ampText.text = val.ToString();
    }
    public void SetOctText( float val ) {
        octText.text = val.ToString();
    }
    public void SetPowText( float val ) {
        powText.text = val.ToString();
    }
    public void DoLast()
    {
        switch (lastFunction)
        {
            case FunctionType.Noise:
                DoNoise();
                break;
            case FunctionType.fBm:
                DoFBM();
                break;
            case FunctionType.Ridged:
                DoRidged();
                break;
        }
    }
}
