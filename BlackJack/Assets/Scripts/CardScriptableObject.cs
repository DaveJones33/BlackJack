using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Cards", menuName = "Display Cards")]
public class CardScriptableObject : ScriptableObject
{
    public List<CardProperties> _cards = new List<CardProperties>(); 

    [System.Serializable]
    public class CardProperties
    {
        public Sprite _card;
        public int _value;
    }
}