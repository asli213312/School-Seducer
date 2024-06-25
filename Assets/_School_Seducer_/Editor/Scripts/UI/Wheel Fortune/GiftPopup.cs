using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using UnityEngine.UI;

public class GiftPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image icon;

    public Vector3 Offset => _offset;

    private Vector3 _offset;

    public void Render(int score, Sprite iconInfo) 
    {
    	scoreText.text = "+" + score.ToString();
    	icon.sprite = iconInfo;
    }

    public void SetOffset(Vector3 offset) => _offset = offset;
}
