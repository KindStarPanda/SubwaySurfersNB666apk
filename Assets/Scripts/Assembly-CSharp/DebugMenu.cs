using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
	public class Menu
	{
		private string _name;

		private string _label;

		public List<Menu> subMenus;

		public List<MenuItem> menuItems;

		public Menu parent;

		public string name
		{
			get
			{
				return _name;
			}
		}

		public string label
		{
			get
			{
				return _label ?? _name;
			}
			set
			{
				_label = value;
			}
		}

		public Menu(string name)
		{
			_name = name;
			subMenus = new List<Menu>();
			menuItems = new List<MenuItem>();
		}

		public void AddSubMenu(Menu subMenu)
		{
			subMenus.Add(subMenu);
			subMenu.parent = this;
		}

		public void AddItem(MenuItem item)
		{
			menuItems.Add(item);
			item.parent = this;
		}

		public void Remove(Menu subMenu)
		{
			subMenus.Remove(subMenu);
			subMenu.parent = null;
		}

		public void Remove(MenuItem item)
		{
			menuItems.Remove(item);
			item.parent = null;
		}
	}

	public class MenuItem
	{
		private string _name;

		private string _label;

		private Action _handler;

		private Action<MenuItem> _handlerWithName;

		public Menu parent;

		public string name
		{
			get
			{
				return _name;
			}
		}

		public string label
		{
			get
			{
				return _label ?? _name;
			}
			set
			{
				_label = value;
			}
		}

		public MenuItem(string name, Action handler)
		{
			_name = name;
			_handler = handler;
		}

		public MenuItem(string name, Action<MenuItem> handlerWithName)
		{
			_name = name;
			_handlerWithName = handlerWithName;
		}

		public void Invoke()
		{
			if (_handlerWithName != null)
			{
				_handlerWithName(this);
			}
			else if (_handler != null)
			{
				_handler();
			}
		}
	}

	private const float BUTTON_HEIGHT = 0.12f;

	private const float MIN_BUTTON_WIDTH = 0.2f;

	private Menu _root;

	private Menu _currentMenu;

	private static DebugMenu _instance;

	public static DebugMenu instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("DebugMenu");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.AddComponent<DebugMenu>();
			}
			return _instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	private void Awake()
	{
		if (_instance != null)
		{
			UnityEngine.Object.Destroy(_instance);
		}
		_instance = this;
		_root = new Menu("DebugMenu");
		AddMenuItem("Unity/Quality Settings/Anti-Aliasing (" + QualitySettings.antiAliasing + "x)", ToggleAntiAliasing);
		AddMenuItem("Unity/Reload scene", ReloadScene);
		AddMenuItem("Unity/Delete PlayerPrefs/Yes I'm sure", DeletePlayerPrefs);
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void OnGUI()
	{
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		GUILayoutOption[] options = new GUILayoutOption[2]
		{
			GUILayout.MinHeight((float)Screen.height * 0.12f),
			GUILayout.MinWidth((float)Screen.width * 0.2f)
		};
		if (_currentMenu != null)
		{
			GUILayout.Label(_currentMenu.label);
			Menu[] array = _currentMenu.subMenus.ToArray();
			Menu[] array2 = array;
			foreach (Menu menu in array2)
			{
				if (GUILayout.Button("> " + menu.label, options))
				{
					_currentMenu = menu;
				}
			}
			MenuItem[] array3 = _currentMenu.menuItems.ToArray();
			MenuItem[] array4 = array3;
			foreach (MenuItem menuItem in array4)
			{
				if (GUILayout.Button(menuItem.label, options))
				{
					menuItem.Invoke();
				}
			}
			if (_currentMenu.parent != null)
			{
				if (GUILayout.Button("< Back", options))
				{
					_currentMenu = _currentMenu.parent;
				}
			}
			else if (GUILayout.Button("< Close", options))
			{
				Close();
			}
		}
		else if (GUILayout.Button(_root.name, options))
		{
			_currentMenu = _root;
		}
	}

	public MenuItem AddMenuItem(string itemName, Action handler)
	{
		return AddMenuItem(itemName, handler, null);
	}

	public MenuItem AddMenuItem(string itemName, Action<MenuItem> handlerWithName)
	{
		return AddMenuItem(itemName, null, handlerWithName);
	}

	public void RemoveMenuItem(MenuItem item)
	{
		Menu menu = item.parent;
		menu.Remove(item);
		while (menu.parent != null && menu.menuItems.Count == 0)
		{
			Menu parent = menu.parent;
			if (menu == _currentMenu)
			{
				_currentMenu = menu.parent;
			}
			parent.Remove(menu);
			menu = parent;
		}
	}

	public void RemoveMenuItem(string itemName)
	{
		string[] array = itemName.Split('/');
		Menu menu = _root;
		for (int i = 0; i < array.Length; i++)
		{
			if (i == array.Length - 1)
			{
				foreach (MenuItem menuItem in menu.menuItems)
				{
					if (menuItem.name == array[i])
					{
						menu.Remove(menuItem);
						while (menu.parent != null && menu.menuItems.Count == 0 && menu.subMenus.Count == 0)
						{
							Menu parent = menu.parent;
							if (menu == _currentMenu)
							{
								_currentMenu = menu.parent;
							}
							parent.Remove(menu);
							menu = parent;
						}
						return;
					}
					Debug.LogError("DebugMenu.RemoveMenuItem: Invalid Path. Item not found: " + array[i]);
				}
				continue;
			}
			Menu menu2 = null;
			foreach (Menu subMenu in menu.subMenus)
			{
				if (subMenu.name == array[i])
				{
					menu2 = subMenu;
					break;
				}
			}
			if (menu2 == null)
			{
				Debug.LogError("DebugMenu.RemoveMenuItem: Invalid Path. Submenu not found: " + array[i]);
				break;
			}
			menu = menu2;
		}
	}

	public void Close()
	{
		_currentMenu = null;
	}

	private MenuItem AddMenuItem(string itemName, Action handler, Action<MenuItem> handlerWithName)
	{
		string[] array = itemName.Split('/');
		Menu menu = _root;
		for (int i = 0; i < array.Length; i++)
		{
			if (i == array.Length - 1)
			{
				foreach (MenuItem menuItem2 in menu.menuItems)
				{
					if (menuItem2.name == array[i])
					{
						Debug.LogError("DebugMenu cannot add '" + itemName + "' as it already is added", this);
						return null;
					}
				}
				MenuItem menuItem = ((handlerWithName == null) ? new MenuItem(array[i], handler) : new MenuItem(array[i], handlerWithName));
				menu.AddItem(menuItem);
				return menuItem;
			}
			Menu menu2 = null;
			foreach (Menu subMenu in menu.subMenus)
			{
				if (subMenu.name == array[i])
				{
					menu2 = subMenu;
					break;
				}
			}
			if (menu2 == null)
			{
				menu2 = new Menu(array[i]);
				menu.AddSubMenu(menu2);
			}
			menu = menu2;
		}
		return null;
	}

	private void ToggleAntiAliasing(MenuItem item)
	{
		if (QualitySettings.antiAliasing == 0)
		{
			QualitySettings.antiAliasing = 2;
		}
		else if (QualitySettings.antiAliasing == 2)
		{
			QualitySettings.antiAliasing = 4;
		}
		else if (QualitySettings.antiAliasing == 4)
		{
			QualitySettings.antiAliasing = 8;
		}
		else
		{
			QualitySettings.antiAliasing = 0;
		}
		item.label = "Anti-Aliasing (" + QualitySettings.antiAliasing + "x)";
	}

	private void ReloadScene()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	private void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}
