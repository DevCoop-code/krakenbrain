using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using KrakenBrain.DeepLearning;
using KrakenBrain.StableDiffusion;

public class KrakenBrainDemo : MonoBehaviour
{
    /*
     * [StableDiffusion Parameters]
     * - guidanceScale : Parameter that controls how much the image generation process follows the text prompt. The higher the value, the more image sticks to a given text input
     * - seed : Number used to initialize the generation. Controlling the seed can help you generate reproducible images, experiment with other parameters, or prompt variations
     */
    private string prompt = "a photo of an astronaut riding a horse on mars";
    private string negativePrompt = "lowres, bad anatomy, bad hands, text, error, missing fingers, extra digit, fewer digits, cropped, worst quality, low quality, normal quality, jpeg artifacts, blurry, multiple legs, malformation";
    private float  guidanceScale = 8;
    private float  seed = 1000000;
    private int    stepCount = 20;
    private int    imageCount = 1;
    private bool   disableSafety = false;

    // Input Model Weight Name: Ex]CoreMLModels
    [SerializeField] private string modelWeightURI;

    // UIs
    [SerializeField] private InputField promptInputField;
    [SerializeField] private InputField npromptInputField;
    [SerializeField] private Slider     guidanceScaleSlider;
    [SerializeField] private Text       guidanceScaleValueText;
    [SerializeField] private InputField seedValueInputField;
    [SerializeField] private Slider     stepCountSlider;
    [SerializeField] private Text       stepCountValueText;
    [SerializeField] private Slider     imageCountSlider;
    [SerializeField] private Text       imageCountValueText;
    [SerializeField] private Text       statusText;

    [SerializeField] private RawImage[] rawImges;

    private KrakenBrainStableDiffusion stableDiffusionInstance;

    private int rawImageIndex = 0;
    private bool isGenerating = false;

    private Coroutine loadingRoutine;
    void Awake()
    {
        guidanceScaleSlider.onValueChanged.AddListener(delegate { ValueChangeGuidanceScale(); });
        stepCountSlider.onValueChanged.AddListener(delegate { ValueChangeStepCount(); });
        imageCountSlider.onValueChanged.AddListener(delegate { ValueChangeImageCount(); });
    }
    void ValueChangeGuidanceScale()
    {
        guidanceScaleValueText.text = ((int)guidanceScaleSlider.value).ToString();
    }
    void ValueChangeStepCount()
    {
        stepCountValueText.text = ((int)stepCountSlider.value).ToString();
    }
    void ValueChangeImageCount()
    {
        imageCountValueText.text = ((int)imageCountSlider.value).ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        stableDiffusionInstance = new KrakenBrainStableDiffusion(modelWeightURI);
        KrakenBrainStableDiffusion.onGeneratedTextureEvent += SetGenerateTexture;

        statusText.text = "STATUS : IDLE";
    }

    void SetGenerateTexture(Texture2D tex)
    {
        Debug.Log("KrakenBrainDemo SetGenerateTexture : " + rawImageIndex);
        isGenerating = false;
        if (null != loadingRoutine)
            StopCoroutine(loadingRoutine);
        statusText.text = "STATUS : DONE";

        if (rawImageIndex < rawImges.Length)
        {
            if (null != rawImges[rawImageIndex] && null != tex)
            {
                rawImges[rawImageIndex].texture = tex;
            }
        }

        rawImageIndex++;
    }

    public void GenerateImageFromPrompt()
    {
        rawImageIndex = 0;
        Debug.Log("KrakenBrainDemo GenerateImageFromPrompt : " + rawImageIndex);

        SetPromptSettings();

        isGenerating = true;
        loadingRoutine = StartCoroutine(ChangeStatus());

        // Release the Texture
        for(int i = 0; i < rawImges.Length; i++)
        {
            if (null != rawImges[i].texture)
            {
                DestroyImmediate(rawImges[i].texture);
            }
        }

        stableDiffusionInstance.generateTextToImage(prompt, negativePrompt, guidanceScale, seed, stepCount, imageCount, disableSafety);
    }

    void SetPromptSettings()
    {
        if (!string.IsNullOrEmpty(promptInputField.text))
            prompt = promptInputField.text;
        if (!string.IsNullOrEmpty(npromptInputField.text))
            negativePrompt = npromptInputField.text;
        guidanceScale = guidanceScaleSlider.value;
        if (!string.IsNullOrEmpty(seedValueInputField.text))
            seed = float.Parse(seedValueInputField.text);
        stepCount = (int)stepCountSlider.value;
        imageCount = (int)imageCountSlider.value;
    }

    IEnumerator ChangeStatus()
    {
        while (isGenerating)
        {
            Debug.Log("ChangeStatus: " + isGenerating);
            statusText.text = "STATUS : Generating...";
            yield return new WaitForSeconds(1);

            statusText.text = "STATUS : Generating..";
            yield return new WaitForSeconds(1);

            statusText.text = "STATUS : Generating.";
            yield return new WaitForSeconds(1);
        }
    }
}
