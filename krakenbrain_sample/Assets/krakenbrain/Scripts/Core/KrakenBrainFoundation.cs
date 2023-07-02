using System;
using System.Collections;
using System.Collections.Generic;
using KrakenBrain.StableDiffusion;

namespace KrakenBrain.Core
{
    public delegate void nativeResultPointerNotify(System.IntPtr generatedImage);

    public abstract class KrakenBrainFoundation
    {
        public static nativeResultPointerNotify onIntPointerResultEvent;

        public virtual void loadWeight(string modelWeightURL) { Console.WriteLine(String.Format("[KrakenBrainFoundation] loadWeight {0}", modelWeightURL)); }

        /*
         * For Stable Diffusion Model
         */
        public virtual void GenerateTextToImage(ImageGenerateParams sdparams)
        {
#if !UNITY_IPHONE
            Console.WriteLine("Your platform does not support Stable Diffusion on Device");
            return;
#endif
        }
    }
}