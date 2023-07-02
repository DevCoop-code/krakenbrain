using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KrakenBrain.StableDiffusion;

namespace KrakenBrain.Core
{
    public class KrakenBrain
    {
        public static string SDKVERSION = "0.0.1";

        KrakenBrainFoundation instance = null;

        public KrakenBrain()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                instance = new KrakenBrainiOS();
            else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                instance = new KrakenBrainOSX();
            else
                instance = null;
        }

        public void LoadWeight(string modelWeightURL)
        {
            Debug.Log("[KrakenBrain] LoadWeight : " + modelWeightURL);
            if (null != instance)
                instance.loadWeight(modelWeightURL);
            else
                Debug.Log("Not Support these Platform(ONLY Support iOS, OSX)");
        }

        // For StableDiffusion
        public void GenerateImage(ImageGenerateParams sdparams)
        {
            Debug.Log("[KrakenBrain] GenerateImage Params : " + sdparams.ToString());
            

            if (null != instance)
            {
                // TODO: Register the Callback, Callback 등록을 이동 (현재는 KrakenBrainStableDiffusion에서 하는 중)
                

                instance.GenerateTextToImage(sdparams);
            }
        }
    }
}
