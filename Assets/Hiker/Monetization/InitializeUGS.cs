using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitializeUGS : MonoBehaviour
{
    public string environment = "production";

    public static InitializeUGS instance;

    public bool IsInited { get; set; } = false;
    private void Awake()
    {
        IsInited = false;
        if (instance == null)
        {
            instance = this;
        }
    }

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (System.Exception exception)
        {
            // An error occurred during initialization.
            Debug.LogError(exception.Message);
        }

        IsInited = true;
    }
}
