using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AnswerUIBehaviour : MonoBehaviour
{
    public Button ButtonRef => GetComponent<Button>();
    public TextMeshProUGUI LabelRef;
    public RawImage ImageRef;

    private AnswerObject _data;

    public static UnityEvent<AnswerObject> QuestionAnswered = new UnityEvent<AnswerObject>();    

    public void InitializeAnswer(AnswerObject data)
    {
        if (data == null) { throw new System.Exception("No data for answer found"); }
        _data = data;
        LabelRef.text = data.AnswerLabel;
        ImageRef.texture = data.AnswerImage;
        ImageRef.gameObject.SetActive(data.AnswerImage != null);
    }

    public void Awake()
    {
        ButtonRef.onClick.AddListener(() => QuestionAnswered.Invoke(_data));
    }
}
