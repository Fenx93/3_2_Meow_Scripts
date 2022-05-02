using EasyMobile;
using UnityEngine;

public class ShareController : MonoBehaviour
{
    public void Share()
    {
        Sharing.ShareText("Try 32Meow! Available from: https://play.google.com/store/apps/details?id=com.RaroxStudios.three_two_Meow");
    }
}
