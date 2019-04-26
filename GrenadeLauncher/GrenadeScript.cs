using RemoteAdmin;
using UnityEngine;

namespace GrenadeLauncher
{
	public class GrenadeScript : MonoBehaviour
	{
		public Grenade grenade;
		public GameObject thrower;
		public void Start()
		{
			grenade = GetComponent<Grenade>();
		}
		public void OnCollisionEnter(Collision other)
		{
			ContactPoint point = other.contacts[0];
			Vector3 position = point.point;
			if (thrower == null) return;
			GrenadeManager manager = thrower.GetComponent<GrenadeManager>();
			manager.CallRpcExplode(grenade.id, thrower.GetComponent<QueryProcessor>().PlayerId);
		}
	}
}