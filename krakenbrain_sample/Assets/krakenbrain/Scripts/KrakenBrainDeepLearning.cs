using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TextureEventNotify(Texture2D tex);

namespace KrakenBrain.DeepLearning
{
    // TODO: Maybe it is not necessary
    public interface SDCallback
    {
        void GeneratedTextureCallback(Texture2D generatedImg);
    }

    public interface DLCommonCallback
    {
        void ErrorCallback(string errorDescription);
    }

    public class KrakenBrainDeepLearning
    {
        protected static KrakenBrain.Core.KrakenBrain krakenBrainInstance;

        public static TextureEventNotify onGeneratedTextureEvent;

        public KrakenBrainDeepLearning()
        {

        }

        public KrakenBrainDeepLearning(string modelWeightURL)
        {
            if (null == krakenBrainInstance)
                krakenBrainInstance = new Core.KrakenBrain();
            krakenBrainInstance.LoadWeight(modelWeightURL);
        }
    }
}
