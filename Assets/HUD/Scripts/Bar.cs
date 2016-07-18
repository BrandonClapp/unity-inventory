using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bar : MonoBehaviour {

    float _fillAmount;
    float _maxHealth;
    float _currentHealth;

    [SerializeField]
    private Image content;
    [SerializeField]
    private Text healthText;

	// Use this for initialization
	void Start ()
    {
        _currentHealth = 1000f;
        _maxHealth = 1200f;
        //content.fillAmount = 0.20f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        _currentHealth -= 1f;
        HandleBar();
	}

    void HandleBar()
    {
        content.fillAmount = CalculateHealthPercent(_currentHealth, _maxHealth);
        healthText.text = _currentHealth + " / " + _maxHealth;
    }

    float CalculateHealthPercent(float current, float max)
    {
        var percent = current / max;
        if (percent < 0f) percent = 0f;
        if (percent > 1f) percent = 1f;
        return percent;
    }
}
