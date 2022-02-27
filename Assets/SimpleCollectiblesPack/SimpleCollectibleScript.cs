using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleCollectibleScript : MonoBehaviour {

	public enum CollectibleTypes {SwimRing_Red, SwimRing_Blue}
	public CollectibleTypes collectibleType;
	public bool rotate;

	public float rotationSpeed;

	public AudioClip collectSound;

	public GameObject collectEffect;

	public CollectibleUI collectibleUI;

	private void Start()
	{
		collectibleUI = FindObjectOfType<CollectibleUI>();
	}

	void Update () {

		if (rotate)
		{
			transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
		}
			
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Player")) {
			Collect ();
		}
	}

	public void Collect()
	{
		if (collectSound)
		{
			AudioSource.PlayClipAtPoint(collectSound, transform.position);
		}

		if (collectEffect)
		{
			Instantiate(collectEffect, transform.position, Quaternion.identity);
		}

		if (collectibleType == CollectibleTypes.SwimRing_Red) {

			//Add in code here;
		}

		if (collectibleType == CollectibleTypes.SwimRing_Blue) {

			//Add in code here;
		}
		
		collectibleUI.CollectItem();
		
		Destroy (gameObject);
	}
}
