using System.Collections;
using UnityEngine;

public class LoadLevelCtrl : MonoBehaviour
{
	[SerializeField]
	private GameObject backrender;

	private AsyncOperation asyncMerge;

	private GameObject notebook;

	[SerializeField]
	private UILabel titleTipLabel;

	[SerializeField]
	private UILabel tipLabel;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite foreground;

	[SerializeField]
	private UITexture splash;

	[SerializeField]
	private Transform backgroundAnchor;

	private float sliderSteps;

	[SerializeField]
	private UIFont[] usedFonts;

	[SerializeField]
	private UIFont[] lowResFonts;

	[SerializeField]
	private UIFont[] highResFonts;

	[SerializeField]
	private float inspectorProgress;

	private string[] tipArray = new string[33]
	{
		"Stay on top of trains\nto get a better view\nof what's coming!", "Swipe down in air\nto cancel jumps and\nmaster the Super Sneakers!", "Pick up the 2X Multiplier\nto double the score you earn!", "Jump barriers, dodge trains\n- and don't let\nthe Inspector catch you!", "Surf the trains\nas far as possible\nto set a new highscore!", "Get the Double Coins Booster\nto increase every coin pickup!", "Find your favorite\ncharacters in the Shop!", "Complete Daily Challenges\neach day and\nincrease your reward!", "Upgrade your preferred\npowerups in the Shop\nto make them last longer!", "Brag to your friends\nwhen you beat their high score!",
		"Join your friends on Facebook\nand show off your skills!", "Complete missions\nto increase your Multiplier\nup to x30!", "Reached Multiplier 30?\nComplete extra missions\nfor Super Mystery Boxes!", "Earn coins for your friends\nby making multiple runs!", "Go to the Friends screen\nto get coin bonuses\nfrom your friends!", "Switch lanes in mid-air\nto dodge trains\nat the last minute!", "Double tap the screen\nto grab a Hoverboard!", "Use Hoverboards\nto avoid crashing!", "Discover secret Trophies\nin Mystery Boxes!", "Reach the top of\nthe leaderboard by\nincreasing your Multiplier!",
		"Open Mystery Boxes\n- go for the Jackpot!", "Catch those letters\nto complete the Daily Challenge!", "Surf the trains\nfor a better view!", "Put on your\npaint powered Jetpack\nto glide with style!", "The 2X Multiplier\ndoubles your Multiplier\ntemporarily!", "Skip Missions in the Shop!", "Use the bouncy\nSuper Sneakers\nto jump over trains!", "Swipe down in mid-air\nto master the Super Sneakers!", "Use Headstarts\nfor a helping hand!", "Unlock Tricky\nby collecting specific\nCharacter Tokens!",
		"Unlock Fresh\nby collecting specific\nCharacter Tokens!", "Unlock Spike\nby collecting specific\nCharacter Tokens!", "Unlock Yutani\nby collecting specific\nCharacter Tokens!"
	};

	private float progress;

	private bool sendDebug;

	private float freezetime;

	public static float continueTime = 99999f;

	private float fakeProgress
	{
		get
		{
			float num = 0f;
			num += progress * 0.5f;
			if (progress > 0.74f)
			{
				num += (progress - 0.74f) * 3.7f;
			}
			return num;
		}
	}

	private void Awake()
	{
		Application.targetFrameRate = 160;
		Object.DontDestroyOnLoad(base.transform.gameObject);
		sliderSteps = (int)slider.fullSize.x * 2;
		splash.mainTexture = Resources.Load("Textures/splashScreen") as Texture;
		if (DeviceInfo.isHighres)
		{
			for (int i = 0; i < usedFonts.Length; i++)
			{
				usedFonts[i].replacement = highResFonts[i];
			}
		}
		else
		{
			for (int j = 0; j < usedFonts.Length; j++)
			{
				usedFonts[j].replacement = lowResFonts[j];
			}
		}
	}

	private void Update()
	{
		LoadBar();
	}

	private string GetTip()
	{
		return tipArray[Random.Range(0, tipArray.Length)];
	}

	private IEnumerator Start()
	{
		GameObject prefab = Resources.Load("Prefabs/Screens/NotebookPanel2", typeof(GameObject)) as GameObject;
		notebook = Object.Instantiate(prefab) as GameObject;
		Utility.SetLayerRecursively(notebook.transform, tipLabel.gameObject.layer);
		notebook.transform.parent = backgroundAnchor;
		notebook.transform.localScale = Vector3.one;
		notebook.transform.localPosition = tipLabel.cachedTransform.localPosition - new Vector3(0f, 400f, 0f);
		NotebookScreen notebookScreen = notebook.GetComponent<NotebookScreen>();
		notebookScreen.Init();
		notebookScreen.Show();
		notebookScreen.HideBackground();
		titleTipLabel.text = "Here's a tip!";
		tipLabel.text = GetTip();
		tipLabel.panel.Refresh();
		asyncMerge = Application.LoadLevelAdditiveAsync("Merge");
		StartCoroutine(Progress());
		yield return asyncMerge;
		Debug.Log("Merge Level Loaded " + Time.frameCount);
	}

	private IEnumerator Progress()
	{
		while (!asyncMerge.isDone)
		{
			progress = 0f;
			if (asyncMerge != null)
			{
				progress = asyncMerge.progress;
			}
			if (progress > 0.89f && !sendDebug)
			{
				freezetime = Time.realtimeSinceStartup;
				sendDebug = true;
			}
			yield return null;
		}
		Debug.Log("FROZEN TIMER:" + freezetime + ", FROZEN DELTATIME:" + (Time.realtimeSinceStartup - freezetime));
		Transform[] componentsInChildren = GameObject.Find("cameraRig_startGame").GetComponentsInChildren<Transform>(true);
		foreach (Transform child in componentsInChildren)
		{
			if (child.name != "Main Camera")
			{
				child.parent = Camera.main.transform;
			}
		}
		Object.Destroy(GameObject.Find("cameraRig_startGame"));
		continueTime = 0f;
		yield return null;
		Object.Destroy(base.gameObject);
		UnloadAssets();
	}

	public float LoadBar()
	{
		slider.sliderValue = (float)Mathf.RoundToInt(fakeProgress * sliderSteps) / sliderSteps;
		return slider.sliderValue;
	}

	private void UnloadAssets()
	{
		Object.Destroy(notebook);
		Resources.UnloadAsset(splash.mainTexture);
		Object.Destroy(splash);
	}
}
