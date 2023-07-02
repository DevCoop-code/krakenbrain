using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Only use for test
 */
public class TestCode: MonoBehaviour
{
    [SerializeField] private Texture2D testTexture;

    public System.IntPtr GetTestTexturePointer()
    {
        return testTexture.GetNativeTexturePtr();
    }
}
