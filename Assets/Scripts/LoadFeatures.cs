using UnityEngine;
using System.Collections.Generic;

// loads MIR Features from Resource Folder
public class LoadFeatures : MonoBehaviour
{
    const int numFeatures = 9;

    // danceable, not_danceable, female, male, voice, instrumental, tonal, atonal, spectral_complexity
    public static Dictionary<int, List<float>> songItemInfo = new Dictionary<int, List<float>>();
    public static Dictionary<int, List<float>> tsne3D_Coordinates = new Dictionary<int, List<float>>();  

    void Start()
    {
        MIRFeatures();
        TSNE3D_Data();
    }

    void MIRFeatures()
    {
        int index = 0;
        TextAsset txtAsset = (TextAsset)Resources.Load("3DVR_TSNE_MIR_2500", typeof(TextAsset));

        string[] linesFromfile = txtAsset.text.Split("\n"[0]);

        for (int i = 1; i < linesFromfile.Length - 1; i++)
        {
            List<float> song = new List<float>();

            string[] data_values;
            data_values = linesFromfile[i].Split(","[0]);

            for (int a = 1; a <= numFeatures; a++)
            {
                float features = float.Parse(data_values[a]);
                song.Add(features);
            }

            songItemInfo.Add(index, song);

            index++;
        }
    }

    // load z coordinate for 2D/3D TSNE Scaling
    void TSNE3D_Data()
    {
        int index = 0;
        TextAsset txtAsset = (TextAsset)Resources.Load("3DVR_Z_TSNE_1500", typeof(TextAsset));

        string[] linesFromfile = txtAsset.text.Split("\n"[0]);

        for (int i = 1; i < linesFromfile.Length - 1; i++)
        {
            List<float> data = new List<float>();

            string[] data_values;
            data_values = linesFromfile[i].Split(","[0]);

            float z = float.Parse(data_values[1]);
            data.Add(z);

            // add z coordinate with slight offset --> no glitches
            data.Add(Random.Range(-0.001f, 0.001f));

            tsne3D_Coordinates.Add(index, data);

            index++;
        }
    }
}