using UnityEngine;
using UnityEngine.UI;

public class HealthUpdater : MonoBehaviour
{
	private HealthComponent _playerHealth;
	private Image _healthFG;

	private void Start()
	{
		PlayerManager player = FindObjectOfType<PlayerManager>();
		if(player != null)
		{
			_playerHealth = player.GetComponent<HealthComponent>();
		}
		_healthFG = transform.FindChild("HP FG").GetComponent<Image>();
	}

	private void LateUpdate()
	{
		float pct = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
		_healthFG.fillAmount = pct;
	}


}
