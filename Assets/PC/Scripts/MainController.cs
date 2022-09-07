using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour
{

//	private CsoundUnity csound;
	public GameObject[] cubes;

	private int currentBeat=0;
	public int tempo = 2;
	// Use this for initialization
	void Start () 
	{
		 //csound = GetComponent<CsoundUnity>();
		 //csound.setChannel("tempo", tempo);  
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown("up"))
		{
			tempo++;
		//	csound.setChannel("tempo", tempo);
		}
		else if(Input.GetKeyDown("down"))
		{
			tempo--;
		//	csound.setChannel("tempo", tempo);
		}

		//if(currentBeat == csound.getChannel("beat"))
		//{
		//	ResizeCube(currentBeat);
		//	currentBeat = (currentBeat!=7 ? currentBeat+1 : 0);				
		//}
	}

	void ResizeCube(int index)
	{
		for( int i = 0 ; i < 8 ; i++)
		{
			if(i == index)
				cubes[i].gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			else
				cubes[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void EnableCubeToPlaySound(int index)
	{
		//Debug.Log("udate");
		//csound.setChannel("updateTable", index-1);
	}
}
