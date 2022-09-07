using UnityEngine;
using System.Collections;
//using UnitySampleAssets.ImageEffects;

public class cam : MonoBehaviour {
	public float speedCam = 1f;
	public float speedMouse = 1f;
	public float distFocus = 0f;
	public float distFocusTarget = 0f;
	//private DepthOfField depth;


	void Start(){
		//depth = GetComponent<DepthOfField> ();
		Cursor.visible = false;
	}

	void Update () {
		Vector3 temp = transform.eulerAngles;
		temp.x -= Input.GetAxis ("Mouse Y") * speedMouse;
		temp.y += Input.GetAxis ("Mouse X") * speedMouse;
		transform.eulerAngles = new Vector3(temp.x,temp.y,temp.z);


		if (Input.GetKey(KeyCode.UpArrow))
			transform.Translate(Vector3.forward * speedCam * Time.deltaTime);
		if (Input.GetKey(KeyCode.DownArrow))
			transform.Translate(Vector3.back * speedCam * Time.deltaTime);
		if (Input.GetKey(KeyCode.LeftArrow))
			transform.Translate(Vector3.left * speedCam * Time.deltaTime);
		if (Input.GetKey(KeyCode.RightArrow))
			transform.Translate(Vector3.right * speedCam * Time.deltaTime);

		// focus
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
		if (Physics.Raycast (ray, out hit, 1000f)) {
			distFocusTarget = Vector3.Distance(hit.point,transform.position);
		}

		distFocus = Mathf.Lerp (distFocus, distFocusTarget, Time.deltaTime*5f);
		//depth.focalLength = distFocus;
		//depth.focalSize = distFocus/2f;


	}
}
