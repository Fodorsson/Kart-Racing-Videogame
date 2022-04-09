using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool p1ControllerIsKeyboard;
    public KeyCode p1BrakeKey;
    public KeyCode p1ActivateKey;
    public KeyCode p1LookBehindKey;
    public KeyCode p1PauseKey;

    public bool p2ControllerIsKeyboard;
    public KeyCode p2BrakeKey;
    public KeyCode p2ActivateKey;
    public KeyCode p2LookBehindKey;
    public KeyCode p2PauseKey;

    public int brightness;
    public int trackSegs;
    public int bgSegs;
    public bool doodads;

    public float musicVol;
    public float ambienceVol;
    public float sfxVol;

    public GameData(bool _p1ControllerIsKeyboard, KeyCode _p1BrakeKey, KeyCode _p1ActivateKey, KeyCode _p1LookBehindKey, KeyCode _p1PauseKey, bool _p2ControllerIsKeyboard, KeyCode _p2BrakeKey, KeyCode _p2ActivateKey, KeyCode _p2LookBehindKey, KeyCode _p2PauseKey, int _brightness, int _trackSegs, int _bgSegs, bool _doodads, float _musicVol, float _ambienceVol, float _sfxVol)
    {
        p1ControllerIsKeyboard = _p1ControllerIsKeyboard;
        p1BrakeKey = _p1BrakeKey;
        p1ActivateKey = _p1ActivateKey;
        p1LookBehindKey = _p1LookBehindKey;
        p1PauseKey = _p1PauseKey;

        p2ControllerIsKeyboard = _p2ControllerIsKeyboard;
        p2BrakeKey = _p2BrakeKey;
        p2ActivateKey = _p2ActivateKey;
        p2LookBehindKey = _p2LookBehindKey;
        p2PauseKey = _p2PauseKey;

        brightness = _brightness;
        trackSegs = _trackSegs;
        bgSegs = _bgSegs;
        doodads = _doodads;

        musicVol = _musicVol;
        ambienceVol = _ambienceVol;
        sfxVol = _sfxVol;
    }
}