using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VampireVillage.Network
{
    public class ChatBubble : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text messageText;

        public void SetChat(Chat chat)
        {
            nameText.text = chat.sender;
            messageText.text = chat.message;
        }
    }
}
