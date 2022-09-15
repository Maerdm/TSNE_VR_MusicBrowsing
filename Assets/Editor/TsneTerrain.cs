using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;


public class TsneTerrain : ScriptableWizard
{
    // Song Prefab and Path to TSNE
    public GameObject songItem;
    public string TSNE_path;

    // path to Album ArtWork
    string img_path = "/Users/matthias/Music/Music/TSNE_VR_Browsing/Original/TSNE_VR_CoverArt/";

    [MenuItem("GameObject/Create TSNE Terrain")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<TsneTerrain>("Create TSNE", "Create");
    }

    void OnWizardCreate()
    {
        using (TextFieldParser parser = new TextFieldParser("./" + TSNE_path))
        {
            GameObject TSNE = new GameObject("TSNEItems");

            List<GameObject> TSNEItems = new List<GameObject>();

            // csv TextFieldParser stuff
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            bool firstLine = true;

            // iterate through csv
            while (!parser.EndOfData)
            {
                string[] data_values = parser.ReadFields();
                // ignore header of csv file
                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }

                // coordinates of TSNE
                float x = float.Parse(data_values[2]);
                float y = float.Parse(data_values[3]);
                float z = float.Parse(data_values[4]);

                Vector3 position = new Vector3(x, y, z);
                //Debug.Log("Position: " + x + " " + y + " " + z);

                //songItem.transform.localScale = new Vector3(15f, 15f, 15f);

                TSNEItems.Add(Instantiate(songItem, position, Quaternion.Euler(90f, 0f, 180f)));
                TSNEItems.Last().name = data_values[1].ToString();

                loadCover(TSNEItems.Last(), data_values[1].ToString());

                //TSNEItems.Last().transform.Rotate(0f, 180f, 0f, Space.Self);
            }

            //add to parent object and load cover image
            foreach (GameObject item in TSNEItems)
            {
                item.transform.parent = TSNE.transform;
            }
        }

    }

    void loadCover(GameObject instantiatedObject, string full_name)
    {
        Renderer rend;
        byte[] fileName;

        fileName = File.ReadAllBytes(img_path + full_name + ".jpg");
        Texture2D img_texture = new Texture2D(2, 2);
        img_texture.LoadImage(fileName);

        rend = instantiatedObject.GetComponent<Renderer>();
        rend.material.mainTexture = img_texture;
        
        rend.material.SetTexture("cover", img_texture);
    }
}

