using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using KrakenBrain.StableDiffusion;

namespace KrakenBrain.Core
{
    public class KrakenBrainiOS: KrakenBrainFoundation
    {
        private static string TAG = "KrakenBrainiOS";
        /*
         * Callbacks
         */
        protected delegate void nativeResultPointerCallbackListener(System.IntPtr intptr);

#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
#endif
        private static extern void LoadWeight(string weightURL);

#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
#endif
        private static extern void generateTextToImg(string prompt, string nprompt, float guidanceScale, float seed, int stepCount, int imageCount, bool disableSafety, nativeResultPointerCallbackListener funcPtr);

        public KrakenBrainiOS() { Console.WriteLine("Generate KrakenBrainiOS"); }

        public KrakenBrainiOS(string modelWeightURL)
        {
            Console.WriteLine("[{0}] LoadWeight {1}", TAG, modelWeightURL);
            LoadWeight(modelWeightURL);
        }

        public override void loadWeight(string modelWeightURL)
        {
            Console.WriteLine("[{0}] LoadWeight {1}", TAG, modelWeightURL);
            LoadWeight(modelWeightURL);
        }

        public override void GenerateTextToImage(ImageGenerateParams sdparams)
        {
            Console.WriteLine("[{0}] GenerateTextToImage", TAG);

            generateTextToImg(sdparams.prompt, sdparams.negativePrompt, sdparams.guidanceScale, sdparams.seed, sdparams.stepCount, sdparams.imageCount, sdparams.disableSafety, new nativeResultPointerCallbackListener(textToImgListener));
        }

        [MonoPInvokeCallback(typeof(nativeResultPointerCallbackListener))]
        static void textToImgListener(System.IntPtr generatedImg)
        {
            onIntPointerResultEvent(generatedImg);
        }
    }
}
