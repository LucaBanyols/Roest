using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionId;
    public ulong playerSteamId;
    private bool avatarReceived = false;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerReadyText;
    public bool playerReady = false;
    public RawImage playerAvatarImage;

    protected Callback<AvatarImageLoaded_t> avatarLoaded;

    private void Start()
    {
        avatarLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarLoaded);
    }

    public void SetPlayerValues()
    {
        playerNameText.text = playerName;

        ChangeReadyStatus();
        if (!avatarReceived)
        {
            GetPlayerIcon();
        }
    }

    void GetPlayerIcon()
    {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamId);
        if (imageId == -1)
        {
            Debug.Log("No Avatar");
            return;
        }
        else
        {
            playerAvatarImage.texture = GetSteamImageAsTexture(imageId);
        }
    }

    private void OnAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerSteamId)
        {
            playerAvatarImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            Debug.Log("Another Player");
            return;
        }
    }

    public void ChangeReadyStatus()
    {
        if (playerReady)
        {
            playerReady = true;
            playerReadyText.text = "Ready";
            playerReadyText.color = Color.green;
            
        }
        else
        {
            playerReady = false;
            playerReadyText.text = "Not Ready";
            playerReadyText.color = Color.red;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarReceived = true;
        return texture;
    }

    private void OnDestroy()
    {
        avatarLoaded.Dispose();
    }
}
