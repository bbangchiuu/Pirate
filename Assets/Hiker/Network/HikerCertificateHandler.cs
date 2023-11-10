using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

class HikerCertificateHandler : CertificateHandler
{
    // Encoded RSAPublicKey
    private static string PUB_KEY = "3082010A0282010100CEEFDFF5EE9D9764899A11FD1FB6F450A9D6D534C44F9BA6DCFAB3D8796A1D2A8ACBF971AAD308F6601173B6384663F0B4B4877249727504615A3FEE9E9E7674D03242C522757624BA8507107028A6AF564C28E7CE627AA4FB008607E33B5DCEFBCD90C9A49D3B13C25E69212CFBA792DF559AA8A8CEEB3C1AF9ED9A56602993B59B17A0407E83761DFBDF8C38AF17F0D27B51C8E55AB600FDFE1CD2E520AF3EA9EA27AB7D6264483352AA10B0BA6ECFFC6BF2D224C9125C114A17C45440EFAD1B1C72CFA140232A3202769C2211E9552C1D92E48FB7708B6BD3F53AD1599DC801DD34CE9A65720C62526DDEFCFDF51DEB7BB2012C3F7BC507CD7822E2B4191D0203010001";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        
        if (pk.Equals(PUB_KEY))
            return true;

        // Bad dog
        Debug.Log("Bad dog");
        Debug.Log(pk);
        return false;
        //return true;
    }
}
