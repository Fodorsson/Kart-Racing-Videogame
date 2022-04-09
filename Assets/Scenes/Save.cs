using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Save : MonoBehaviour
{

    public static bool p1ControllerIsKeyboard = true;
    public static KeyCode p1BrakeKey = KeyCode.Space;
    public static KeyCode p1ActivateKey = KeyCode.E;
    public static KeyCode p1LookBehindKey = KeyCode.Q;
    public static KeyCode p1PauseKey = KeyCode.Escape;

    public static bool p2ControllerIsKeyboard = true;
    public static KeyCode p2BrakeKey = KeyCode.Keypad2;
    public static KeyCode p2ActivateKey = KeyCode.Keypad1;
    public static KeyCode p2LookBehindKey = KeyCode.Keypad3;
    public static KeyCode p2PauseKey = KeyCode.Escape;

    public static int brightness = 5;
    public static int trackSegs = 20;
    public static int bgSegs = 8;
    public static bool doodads = true;

    public static float musicVol = 5f;
    public static float ambienceVol = 5f;
    public static float sfxVol = 5f;

    public static void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(p1ControllerIsKeyboard, p1BrakeKey, p1ActivateKey, p1LookBehindKey, p1PauseKey, p2ControllerIsKeyboard, p2BrakeKey, p2ActivateKey, p2LookBehindKey, p2PauseKey, brightness, trackSegs, bgSegs, doodads, musicVol, ambienceVol, sfxVol);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public static void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
            return;

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        p1ControllerIsKeyboard = data.p1ControllerIsKeyboard;
        p1BrakeKey = data.p1BrakeKey;
        p1ActivateKey = data.p1ActivateKey;
        p1LookBehindKey = data.p1LookBehindKey;
        p1PauseKey = data.p1PauseKey;

        p2ControllerIsKeyboard = data.p2ControllerIsKeyboard;
        p2BrakeKey = data.p2BrakeKey;
        p2ActivateKey = data.p2ActivateKey;
        p2LookBehindKey = data.p2LookBehindKey;
        p2PauseKey = data.p2PauseKey;

        brightness = data.brightness;
        trackSegs = data.trackSegs;
        bgSegs = data.bgSegs;
        doodads = data.doodads;

        musicVol = data.musicVol;
        ambienceVol = data.ambienceVol;
        sfxVol = data.sfxVol;
    }

}
