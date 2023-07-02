using System;
using System.Collections;
using System.Collections.Generic;

using KrakenBrain.DeepLearning;
using KrakenBrain.Core;
using UnityEngine;

namespace KrakenBrain.StableDiffusion
{
    public class KrakenBrainStableDiffusion: KrakenBrainDeepLearning
    {
        private static string TAG = "KrakenBrainStableDiffusion";

        private static int GeneratedTexWidth    = 512;
        private static int GeneratedTexHeight   = 512;

        public Texture2D generatedSDTex;

        public KrakenBrainStableDiffusion(string modelWeightURL): base(modelWeightURL)
        {
            Console.WriteLine(string.Format("KrakenBrainSD Init : {0}", modelWeightURL));

            // Register the Callback
            KrakenBrainFoundation.onIntPointerResultEvent += GenerateImageEventNotify;
        }

        public void generateTextToImage(string prompt, string negativePrompt, float guidanceScale, float seed, int stepCount, int imageCount, bool disableSafety)
        {
            string paramsData = "Prompt : " + prompt + ", Negative Prompt : " + negativePrompt + ", GuidanceScale : " + guidanceScale + ", Seed : " + seed + ", StepCount : " + stepCount + ", ImageCount : " + imageCount + ", DisableSafety : " + disableSafety;
            Console.WriteLine(string.Format("KrakenBrainSD Generate Image(Text To Image) : {0}", paramsData));

            ImageGenerateParams SDparams = new ImageGenerateParams(prompt, negativePrompt, guidanceScale, seed, stepCount, imageCount, disableSafety);
            krakenBrainInstance.GenerateImage(SDparams);
        }

        public void GenerateImageEventNotify(System.IntPtr generatedImgPtr)
        {
            Console.WriteLine(string.Format("KrakenBrainSD GenerateImageEventNotify"));

            if (System.IntPtr.Zero != generatedImgPtr)
            {
                //if (null == generatedSDTex)
                //{
                    generatedSDTex = Texture2D.CreateExternalTexture(GeneratedTexWidth, GeneratedTexHeight, TextureFormat.BGRA32, false, false, generatedImgPtr);
                    generatedSDTex.filterMode = FilterMode.Bilinear;
                    generatedSDTex.wrapMode = TextureWrapMode.Repeat;
                //}

                generatedSDTex.UpdateExternalTexture(generatedImgPtr);

                // Toast Generated Texture to High-Level Layer
                onGeneratedTextureEvent(generatedSDTex);
            }
        }
    }
}