using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KrakenBrain.StableDiffusion;

namespace KrakenBrain.Core
{
    public class KrakenBrainOSX : KrakenBrainFoundation
    {
        public KrakenBrainOSX() { Console.WriteLine("Generate KrakenBrainOSX"); }

        public KrakenBrainOSX(string weightURL)
        {

        }

        public override void loadWeight(string modelWeightURL)
        {
            Console.WriteLine("[KrakenBrainOSX] LoadWeight {0}", modelWeightURL);
        }

        public override void GenerateTextToImage(ImageGenerateParams sdparams)
        {
            Console.WriteLine("[KrakenBrainOSX] GenerateTextToImage");

            //for (int i = 0; i < 1; i++)
            //{
            //    Thread.Sleep(10 * 1000);
            //}

            // For Test
            TestCode testCodeObject = GameObject.Find("KrakenBrainManager").GetComponent<TestCode>();
            onIntPointerResultEvent(testCodeObject.GetTestTexturePointer());
        }
    }
}
