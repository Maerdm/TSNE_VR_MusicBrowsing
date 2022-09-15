using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class MIRSelection : MonoBehaviour
{
    public GameObject TsneItems;
    public Dropdown Danceable, Voice_Instru, Tonal_Atonal;

    // all features in UI are set to both
    private List<int> feature_selection = new[] { -1, -1, -1, -1, -1, -1, -1, -1 }.ToList();

    // Calculate scaling/offset
    private double ScaleValue(double x, double in_min, double in_max, double out_min, double out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    // set Feature Category of selection Array to -1
    void SetSelection(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            feature_selection.RemoveAt(i);
            feature_selection.Insert(i, -1);
        }
    }

    // get UI Input selection
    public void DanceableNotDanceable()
    {
        SetSelection(0, 2);

        if (Danceable.value == 0)
        {
            StartCoroutine(Alpha(-1)); // both
        }

        if (Danceable.value == 1)
        {
            StartCoroutine(Alpha(0)); // danceable 
        }

        if (Danceable.value == 2)
        {
            StartCoroutine(Alpha(1)); // not_danceable
        }
    }

    public void GenderVoice()
    {
        SetSelection(2, 6);

        if (Voice_Instru.value == 0)
        {
            StartCoroutine(Alpha(-2)); // both
        }

        if (Voice_Instru.value == 1)
        {
            StartCoroutine(Alpha(2)); // female
        }

        if (Voice_Instru.value == 2)
        {
            StartCoroutine(Alpha(3));// male
        }

        if (Voice_Instru.value == 3)
        {
            StartCoroutine(Alpha(4)); // voice
        }

        if (Voice_Instru.value == 4)
        {
            StartCoroutine(Alpha(5)); // intrumental
        }
    }

    public void TonalAtonal()
    {
        SetSelection(6, 8);

        if (Tonal_Atonal.value == 0)
        {
            StartCoroutine(Alpha(-3)); // both
        }

        if (Tonal_Atonal.value == 1)
        {
            StartCoroutine(Alpha(6)); // tonal
        }

        if (Tonal_Atonal.value == 2)
        {
            StartCoroutine(Alpha(7)); // atonal
        }
    }

    IEnumerator Alpha(int feature)
    {
        // get selected Features from UI
        if (feature >= 0)
        {
            feature_selection.RemoveAt(feature);
            feature_selection.Insert(feature, feature);
        }

        // getting Features that are switched on in UI
        List<int> current_selected = new List<int>();

        foreach (int item in feature_selection)
        {
            if (item != -1)
            {
                current_selected.Add(item);
            }
        }

        // if no Features selected, set all items bright
        if (current_selected.Count() == 0)
        {
            foreach (Transform child in TsneItems.transform)
            {
                child.GetComponent<Renderer>().material.SetColor("set_color", new Color(1, 1, 1, 1));
            }
        }

        // only one feature selected, other features in UI set to "both"
        if (current_selected.Count() == 1)
        {
            int index = 0;

            foreach (Transform child in TsneItems.transform)
            {
                float[] mir_features;
                mir_features = LoadFeatures.songItemInfo.ElementAt(index).Value.ToArray();

                child.GetComponent<Renderer>().material.SetColor("set_color", new Color(mir_features[current_selected[0]],
                                    mir_features[current_selected[0]], mir_features[current_selected[0]], 1));
                index++;
            }
        }

        // more tha one feature selected
        if (current_selected.Count() > 1)
        {
            int index = 0;

            // getting selected features and calculate mean
            List<float> merged_features = new List<float>();
            foreach (Transform child in TsneItems.transform)
            {
                float[] mir_features;
                mir_features = LoadFeatures.songItemInfo.ElementAt(index).Value.ToArray();

                // getting selected Features
                List<float> merged = new List<float>();
                foreach (int a in current_selected)
                {
                    merged.Add(mir_features[a]);
                }

                // calculating mean
                merged_features.Add(merged.Sum() / feature_selection.Count());
                index++;
            }

            float min1 = merged_features.Min();
            float max1 = merged_features.Max();
            var scaledVal = merged_features.Select(x => Math.Exp((float)ScaleValue(x, min1, max1, 0, 6)));

            // Scaling merged Features
            List<float> merged_features_scaled = new List<float>();
            var min2 = scaledVal.Min();
            var max2 = scaledVal.Max();
            index = 0;
            foreach (float item in scaledVal)
            {
                merged_features_scaled.Add((float)ScaleValue(item, min2, max2, 0, 1));
                index++;
            }

            // setting brightness based on merged features
            index = 0;
            foreach (Transform child in TsneItems.transform)
            {
                child.GetComponent<Renderer>().material.SetColor("set_color", new Color(merged_features_scaled[index],
                                    merged_features_scaled[index], merged_features_scaled[index], 1));

                index++;
            }
        }

        yield return null;
    }
}
