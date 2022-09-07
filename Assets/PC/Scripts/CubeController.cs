using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour 
{

	private Renderer rend;
	public bool state = false;
	private int cubeIndex;
	Color offColour, onColour;
	public GameObject camera;

	// Use this for initialization
	void Start () 
	{
		
		offColour = new Color();
		ColorUtility.TryParseHtmlString ("#050045FF", out offColour);
		onColour = new Color();
		ColorUtility.TryParseHtmlString ("#004511FF", out onColour);

		string cubeName = gameObject.name;
		int.TryParse(cubeName.Substring(cubeName.IndexOf("(")+1, 1), out cubeIndex);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnMouseDown()
	{

		if(state == false)
		{
			gameObject.GetComponent<Renderer>().material.color = onColour;
		}
        else
			gameObject.GetComponent<Renderer>().material.color = offColour;

		state =! state;

		camera.GetComponent<MainController>().EnableCubeToPlaySound(cubeIndex);
	}
}
