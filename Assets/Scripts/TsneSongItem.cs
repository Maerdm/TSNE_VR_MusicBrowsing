using UnityEngine;

public class TsneSongItem : AudioManager
{
    //string adress = "http://ipadress:8080/TSNE_VR_Browsing/TSNE_VR_Music_Convert/";
    string adress = "http://ipadress:8080/TSNE_VR_Browsing/TSNE_VR_Music_Convert/";
    Color color;
     
    public static bool hover;
    public static int id;
    public static Vector3 posHover;

    public void Hover()
    {
        color = gameObject.GetComponent<Renderer>().material.GetColor("set_color");
        gameObject.transform.localScale = new Vector3(30f, 30f, 30f);

        // light up object if if its not bright
        gameObject.GetComponent<Renderer>().material.SetColor("set_color", new Color(1, 1, 1, 1));

        // position and id of hovered Object, used in GlobalInteraction.cs
        hover = true;
        posHover = gameObject.transform.localPosition;
        string[] split = gameObject.name.ToString().Split('_');
        id = int.Parse(split[0]);
    }

    public void UnHover()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("set_color", color);
        gameObject.transform.localScale = new Vector3(15f, 15f, 15f);
        hover = false;
    }

    public void AudioStart()
    {
        string a = gameObject.name.ToString();
        string[] split = a.Split('_');
        playAudio(adress + split[0] + ".mp3");
    }
}
