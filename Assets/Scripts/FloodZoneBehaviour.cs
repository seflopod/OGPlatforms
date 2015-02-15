using UnityEngine;
using System.Collections;
using OgreToast.Attributes;
using OgreToast.Utility;

[RequireComponent(typeof(Collider2D))]
public class FloodZoneBehaviour : MonoBehaviour
{
	public bool DisplaySpawnZonesInEditor = true;
	public float SpawnZoneWidth = 56f;

	[TwoVarRange("Y Screen Pct Range","For determing where things are spawned on y-axis", 0.1f, 0.9f)]
	public Vector2 ScreenPctRange = new Vector2(0.3f, 0.7f);

	public int WavesToSpawn = 25;

	[TwoVarRange("Time Between Waves", 0.1f, 5f)]
	public Vector2 WaveTimingRange = new Vector2(0.25f, 2f);

	[TwoVarIntRange("Enemies per Wave", 1, 10)]
	public Vector2 WaveSizeRange = new Vector2(1, 3);

	public GameObject[] EnemiesToSpawn = new GameObject[0];

	private SimpleTimer _waveTimer = new SimpleTimer();
	private int _wavesSpawned = 0;
	private bool _isSpawning = false;
	private bool _doSpawn = false;

	private void Update()
	{
		if(_doSpawn && _waveTimer.IsExpired && !_isSpawning)
		{
			spawnWave();
		}
	}

	//NEEDS TWEAKING
	private void OnDrawGizmosSelected()
	{
		if(DisplaySpawnZonesInEditor)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			float midPct = ScreenPctRange.x + (ScreenPctRange.y - ScreenPctRange.x) / 2f;
			Vector3 size = new Vector3(SpawnZoneWidth, (ScreenPctRange.x - ScreenPctRange.y) * Camera.main.pixelHeight, 1f);
			Vector3 center = new Vector3(collider2D.bounds.min.x + SpawnZoneWidth / 2f, collider2D.bounds.center.y, transform.position.z);
			Gizmos.DrawCube(center, size);
			center = new Vector3(collider2D.bounds.max.x - SpawnZoneWidth / 2f, collider2D.bounds.center.y, transform.position.z);
			Gizmos.DrawCube(center, size);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(!_doSpawn)
		{
			_doSpawn = (EnemiesToSpawn.Length > 0 && _wavesSpawned < WavesToSpawn && other.gameObject.tag == "Player");
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{	//ONLY SETTING IF THE TAG IS PLAYER.  DON'T CARE ABOUT ANYTHING ELSE
			_doSpawn = false;
		}
	}

	private void spawnWave()
	{
		_isSpawning = true;
		_waveTimer.Stop();

		float spawnY = Camera.main.ViewportToWorldPoint(new Vector3(0f, Random.Range(ScreenPctRange.x, ScreenPctRange.y), transform.position.z)).y;

		Vector3 node1 = new Vector3(collider2D.bounds.min.x + Random.Range(0f, SpawnZoneWidth), spawnY, transform.position.z);
		Vector3 node2 = new Vector3(collider2D.bounds.max.x - Random.Range(0f, SpawnZoneWidth), spawnY, transform.position.z);

		float spawnX;
		Quaternion rot;
		if(Random.value > 0.5f)
		{
			spawnX = node1.x;
			rot = Quaternion.identity;
		}
		else
		{
			spawnX = node2.x;
			rot = Quaternion.AngleAxis(180f, Vector3.up);
		}
		GameObject spawned = (GameObject)Instantiate(EnemiesToSpawn[Random.Range(0, EnemiesToSpawn.Length)], new Vector3(spawnX, spawnY, transform.position.z), rot);
		MovementComponent mc = spawned.GetComponent<MovementComponent>();
		BasicAI ba = spawned.GetComponent<BasicAI>();
		if((mc as FlyingComponent) == null && ba != null)
		{
			ba.PatrolNode1 = node1;
			ba.PatrolNode2 = node2;
		}

		if(++_wavesSpawned >= WavesToSpawn)
		{
			_doSpawn = false;
		}
		else
		{
			_waveTimer.TargetTime = Random.Range(WaveTimingRange.x, WaveTimingRange.y);
			_waveTimer.Start();
		}
		_isSpawning = false;
	}

}
