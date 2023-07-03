# KrakenBrain - DeepLearning OnDevice Unity SDK
Run DeepLearning on Device at runtime in Unity

## Support DeepLearning Model
- [StableDiffusion - CoreML](#stablediffusion)

|   DeepLearning Model  |    Android    |   iOS     |   Mac OSX     |   Window  |
| :-------------------: | :-----------: | :-------: | :-----------: | :-------: |
|    StableDiffusion    |***Not*** Support|**Support**|***Not*** Support|***Not*** Support|


## StableDiffusion
<p align="center">
    <img src="Assets/sampleImage01.png" height="300dp" >
</p>
<br>

### System Requirements
<details>
    <summary> Details (Click to expand) </summary>

Target Device Runtime:
| iPadOS, iOS |
| :---------: |
|    16.2     |

Target Device Runtime
| iPadOS, iOS |
| :---------: |
|    17.0     |

Target Device Hardware Generation
|   iPad   |   iPhone   |
| :------: | :---------:|
|    M1    |     A14    |
</details>

### Using Ready-made Core ML Models from Hugging Face Hub
<details>
    <summary> Details (Click to expand) </summary>

- 6-bit quantized models (suitable for iOS 17)
    - [CompVis/stable-diffusion-v1-4](https://huggingface.co/apple/coreml-stable-diffusion-1-4-palettized)
    - [runwayml/stable-diffusion-v1-5](https://huggingface.co/apple/coreml-stable-diffusion-v1-5-palettized)
    - [stabilityai/stable-diffusion-2-base](https://huggingface.co/apple/coreml-stable-diffusion-2-base-palettized)
    - [stabilityai/stable-diffusion-2-1-base](https://huggingface.co/apple/coreml-stable-diffusion-2-1-base-palettized)

- [Converting Models to Core ML](https://github.com/apple/ml-stable-diffusion/blob/main/README.md#-converting-models-to-core-ml)

</details>

### Using Stable Diffusion onDevice in Unity
<details>
    <summary> Details (Click to expand) </summary>

**Step 1** : Put the weight file on "Assets/krakenbrain/Plugins/iOS"

**Step 2** : Input the weight file name on KrakenBrain Settings

<p align="center">
    <img src="Assets/stablediffusion/unity_stablediffusion_ready.png" width="500dp">
</p>

**Step 3** : Build iOS & export xcode project

**Step 4** : Go to Frameworks directory and Change StableDiffusionFramework Target Membership Unity-iPhone to UnityFramework

**Step 5** : Set StableDiffusionFramework to Embed & Sign

<p align="center">
    <img src="Assets/stablediffusion/stablediffusion_xcodesettings.png" width="500dp">
</p>

</details>

### Issues
- Low Memory Issue frequentily occured

Reference : [Apple ml-stable-diffusion](https://github.com/apple/ml-stable-diffusion/blob/main/README.md)

