using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    DownloadHandlerAudioClip dHA;
    UnityWebRequest server;

    private string songName;

    void Start()
    {
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
    }

    public void playAudio(string name)
    {
        if (!audioSource.isPlaying)
        {
            StartCoroutine(SongAudioClip(name));
            songName = name;
            return;
        }

        if (audioSource.isPlaying && songName == name)
        {
            stopAudio();
            return;
        }

        if (audioSource.isPlaying && songName != name)
        {
            StartCoroutine(SongAudioClip(name));
            songName = name;
            return;
        }
    }

    public void stopAudio()
    {
        audioSource.Stop();
    }

    IEnumerator SongAudioClip(string song)
    {
        using (server = UnityWebRequestMultimedia.GetAudioClip(song, AudioType.MPEG))
        {
            dHA = new DownloadHandlerAudioClip(song, AudioType.MPEG);
            dHA.streamAudio = true;
            server.downloadHandler = dHA;
            server.SendWebRequest();

            while (server.result != UnityWebRequest.Result.ConnectionError && server.downloadProgress <= 0.1)
            {
                yield return new WaitForSeconds(.1f);
            }

            audioSource.clip = dHA.audioClip;
            audioSource.Play();

            while (server.result != UnityWebRequest.Result.ConnectionError && server.downloadProgress < 1)
            {
                yield return new WaitForSeconds(.1f);
            }

            audioSource.clip = dHA.audioClip;
        }
    }
}