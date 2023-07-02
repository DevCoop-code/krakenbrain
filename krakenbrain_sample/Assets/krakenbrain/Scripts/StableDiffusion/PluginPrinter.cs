using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PluginPrinter : MonoBehaviour
{
    [DllImport("StableDiffusionOSX")]
    private static extern IntPtr PrintHello();

    [DllImport("StableDiffusionOSX")]
    private static extern int PrintNumber();

    [DllImport("StableDiffusionOSX")]
    private static extern int AddTwoIntegers(int a, int b);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Marshal.PtrToStringAnsi(PrintHello()));
        Debug.Log(PrintNumber());
        Debug.Log(AddTwoIntegers(1, 2));
    }
}
