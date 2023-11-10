using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameRequestHandler : DownloadHandlerScript
{
    protected override byte[] GetData()
    {
        return base.GetData();
    }

    protected override void CompleteContent()
    {
        base.CompleteContent();
    }

    protected override float GetProgress()
    {
        return base.GetProgress();
    }

    protected override string GetText()
    {
        return base.GetText();
    }

    protected override void ReceiveContentLength(int contentLength)
    {
        base.ReceiveContentLength(contentLength);
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        return base.ReceiveData(data, dataLength);
    }
}
