using UnityEngine;

public class IKArm : MonoBehaviour
{
	public GameObject thisArm;
	public GameObject otherArm;
	public bool isClosedHand;

	private void Start()
	{
		Cursor.visible = false;
	}

	void Update(){ 
	
		Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = cursorPos;

		if(isClosedHand)
		{ 
			if(Input.GetMouseButtonUp(0)){ 
				thisArm.gameObject.SetActive(false);
				otherArm.SetActive(true);
			}
		} else{
			if (Input.GetMouseButtonDown(0))
			{
				thisArm.gameObject.SetActive(false);
				otherArm.SetActive(true);
			}
		}
	}
}
