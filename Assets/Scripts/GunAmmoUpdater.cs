using UnityEngine;
using UnityEngine.UI;

public class GunAmmoUpdater : MonoBehaviour
{
	private AbstractAttacker _playerWeapon;
	private string _curWeaponName;
	private Image _ammoFG;
	private Image _ammoBG;

	private void Start()
	{
		PlayerManager player = FindObjectOfType<PlayerManager>();
		if(player != null)
		{
			_playerWeapon = player.GetComponent<AbstractAttacker>();
			_curWeaponName = _playerWeapon.CurrentWeapon.Name;
		}
		_ammoFG = transform.FindChild("Gun FG").GetComponent<Image>();
		_ammoBG = transform.FindChild("Gun BG").GetComponent<Image>();
		_ammoFG.sprite = _playerWeapon.CurrentWeapon.UserInterfaceSprite;
		_ammoBG.sprite = _playerWeapon.CurrentWeapon.UserInterfaceBG;
	}

	private void Update()
	{
		if(_playerWeapon.CurrentWeapon.Name != _curWeaponName)
		{
			_curWeaponName = _playerWeapon.CurrentWeapon.Name;
			_ammoFG.sprite = _playerWeapon.CurrentWeapon.UserInterfaceSprite;
			_ammoBG.sprite = _playerWeapon.CurrentWeapon.UserInterfaceBG;
		}

		float pct = (_playerWeapon.CurrentWeapon.HasAmmo) ? (float)_playerWeapon.AmmoRemaining / _playerWeapon.CurrentWeapon.StartAmmo : 1f;
		_ammoFG.fillAmount = pct;

	}
}
