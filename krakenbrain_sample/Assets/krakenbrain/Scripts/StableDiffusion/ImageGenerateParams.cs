using System.Collections;
using System.Collections.Generic;

namespace KrakenBrain.StableDiffusion
{
    /*
     * For StableDiffusion Parameters
     */
    public struct ImageGenerateParams
    {
        public string prompt;
        public string negativePrompt;
        public float guidanceScale;
        public float seed;
        public int stepCount;
        public int imageCount;
        public bool disableSafety;

        public ImageGenerateParams(string prompt,
            string negativePrompt,
            float guidanceScale,
            float seed,
            int stepCount,
            int imageCount,
            bool disableSafety)
        {
            this.prompt = prompt;
            this.negativePrompt = negativePrompt;
            this.guidanceScale = guidanceScale;
            this.seed = seed;
            this.stepCount = stepCount;
            this.imageCount = imageCount;
            this.disableSafety = disableSafety;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", prompt, negativePrompt, guidanceScale, seed, stepCount, imageCount, disableSafety);
        }
    }
}
