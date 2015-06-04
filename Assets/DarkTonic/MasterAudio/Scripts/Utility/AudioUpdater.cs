using UnityEngine;

class AudioUpdater: MonoBehaviour
{
	#region Members

	private Transform m_transform = null;
	private Transform m_follow = null;
	
	public Transform FollowTransform
	{
		get
		{
			return m_follow;
		}
		set
		{
			m_follow = value;
		}
	}
	
	private Transform ParentTransform
	{
		get { 
			if ((m_transform == null) && (gameObject != null)) {
				m_transform = gameObject.transform;
			}
			return m_transform;
		}
	}

	#endregion

	#region Messages

	public void Update()
	{
		if ((FollowTransform != null) && (ParentTransform != null)) {
			ParentTransform.position = FollowTransform.position;
		}
	}

	#endregion
}