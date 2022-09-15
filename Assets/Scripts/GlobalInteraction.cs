using System.Collections;
using UnityEngine;
using System.Linq;
using System;

public class GlobalInteraction : MonoBehaviour
{
    public GameObject player;
    public GameObject Controllers;
    public GameObject TsneItems, UI, Navigation, Navigation3dimension;

    bool secondaryHandTrigger;
    bool scale, scale2D, scale3D = false;
    bool UI_active = false;
    bool Navigation_active = false;
    bool countdown = true;

    float[] z_hover;
    float movingSpeed = 35f;
    float offset = 0f;

    Vector3 positionAtPress, currentPosition, movingPosition, globalTSNE;
    Vector3 tsne_Start, tsne_End;

    void Start()
    {
        UI.SetActive(false);
        Navigation.SetActive(false);
    }

    void Update()
    {
        // Show UI
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {           
            if (UI_active == true)
            {
                UI.SetActive(false);
                UI_active = false;
                Controllers.SetActive(true);
            }
            else
            {
                UI.SetActive(true);
                UI_active = true;
                Controllers.SetActive(false);
            }
        }

        // Show Orientation
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            if (Navigation_active == true)
            {
                Navigation.SetActive(false);
                Navigation_active = false;

            }
            else
            {
                Navigation.SetActive(true);
                Navigation_active = true;
                if (scale == true)
                {
                    Navigation3dimension.SetActive(false);
                }
                else
                {
                    Navigation3dimension.SetActive(true);
                }
            }
        }

        // TSNE Scaling
        if (OVRInput.GetDown(OVRInput.Button.One) && TsneSongItem.hover == true && countdown == true)
        {
            if (!scale)
            {
                scale2D = true;
                scale = true;
                StartCoroutine(CountDown());
                countdown = false;
                if (Navigation_active == true)
                {
                    Navigation3dimension.SetActive(false);
                }

            }
            else
            {
                scale3D = true;
                scale = false;
                StartCoroutine(CountDown());
                countdown = false;
                if (Navigation_active == true)
                {
                    Navigation3dimension.SetActive(true);
                }
            }

            z_hover = LoadFeatures.tsne3D_Coordinates.ElementAt(TsneSongItem.id - 1).Value.ToArray();
            offset = Offset(player.transform.eulerAngles.y);
        }

        // Start 2D/3D Scaling
        if (scale3D)
        {
            TSNEScaling(0, z_hover[0] * -1f);
        }

        if (scale2D)
        {
            TSNEScaling(1, 0);
        }

        // TSNE Moving
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            secondaryHandTrigger = true;
            positionAtPress = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
            positionAtPress = transform.TransformPoint(positionAtPress);
            globalTSNE = TsneItems.transform.position;
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            secondaryHandTrigger = false;
        }

        if (secondaryHandTrigger)
        {
            currentPosition = transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand));
            movingPosition = positionAtPress - currentPosition;
            movingPosition = new Vector3(movingPosition.x, movingPosition.y, movingPosition.z);
            TsneItems.transform.position = globalTSNE - movingPosition * movingSpeed;
        }
    }

    void TSNEScaling(int z_array, float z)
    {
        Vector3 tsne3D;
        Vector3 tsne2D;
        int index = 0;
        float speed;

        // move individual TSNE Items
        foreach (Transform child in TsneItems.transform)
        {
            float[] z_coordinate = LoadFeatures.tsne3D_Coordinates.ElementAt(index).Value.ToArray();

            speed = Math.Abs(z_coordinate[0]);

            tsne2D = child.transform.localPosition;
            tsne3D = new Vector3(tsne2D.x, tsne2D.y, z_coordinate[z_array]);
            child.transform.localPosition = Vector3.MoveTowards(tsne2D, tsne3D, speed * Time.deltaTime);

            index++;
        }

        // move global TSNE position based on Offset from Controller Input
        speed = Math.Abs(z_hover[0]);

        tsne_Start = TsneItems.transform.localPosition;
        //offset = Offset(player.transform.eulerAngles.y);
        tsne_End = new Vector3(tsne_Start.x, tsne_Start.y, z + offset);

        if (TsneItems.transform.localPosition != tsne_End)
        {
            TsneItems.transform.localPosition = Vector3.MoveTowards(tsne_Start, tsne_End, speed * Time.deltaTime);
        }
    }

    // Calculate scaling/offset
    private double ScaleValue(double x, double in_min, double in_max, double out_min, double out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    private float Offset(float y)
    {
        float offset = 0f;
        if (y <= 90)
        {
            offset = (float)ScaleValue(y, 0, 90, 3, 0);
        }
        if (y > 90 && y <= 180)
        {
            offset = (float)ScaleValue(y, 90, 180, 0, -3);
        }
        if (y > 180 && y <= 270)
        {
            offset = (float)ScaleValue(y, 180, 270, -3, 0);
        }
        if (y > 270)
        {
            offset = (float)ScaleValue(y, 270, 360, 0, 3);
        }

        return offset;
    }


    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1.5f);

        if (scale)
        {
            scale2D = false;
        }
        else
        {
            scale3D = false;
        }

        countdown = true;
    }
}
