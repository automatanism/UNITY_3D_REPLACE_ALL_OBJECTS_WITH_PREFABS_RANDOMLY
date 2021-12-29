//Author: God Bennett
//takes folder name(assumed to be in "Resources") and generates prefab list, to facilitate large scale replacement randomly.


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using static System.IO.Path;
using static System.IO.FileNotFoundException;

 
public class ReplaceWithMultiplePrefabsRandomly : EditorWindow
{
	private List<GameObject> prefabList = new List<GameObject>();
	private List<string> prefabNamesList = new List<string>();
	private int PREFAB_RANDOM_ITERATOR; 
	string prefabResourceFolderName = "Eg: 'Extortionist_prefabs' no quotes, no slashes";

	int indexAI;
	int indexSpawn;	
		
	[MenuItem("Tools/V2_Replace With Multiple Prefabs Randomly (Scammah)")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithMultiplePrefabsRandomly>();
	}
	
	private void OnGUI()
	{
		prefabResourceFolderName = EditorGUILayout.TextField("Prefab '/Resource' Folder Name", prefabResourceFolderName);
		
		if (GUILayout.Button("Replace"))
		{
			var res = Resources.LoadAll<GameObject>(""+prefabResourceFolderName+"/");

			foreach (GameObject obj in res)
				prefabList.Add(obj);
			
			/*for (int x=0; x<prefabList.Count; x++)
			{
				//indexAI = UnityEngine.Random.Range(0,ai.Count);
				//indexSpawn = UnityEngine.Random.Range(0, spawn.Length);

				string name = prefabList[x].name;
				Debug.Log(name);

				//I am currently using this kind of format since this is what I know for now.
				//Instantiate(name,spawn[indexSpawn].transform.position,spawn[indexSpawn].transform.rotation);
			}		
			//Instantiate(prefabList[0],new Vector3(0f,0f,0f),Quaternion.identity);
			*/
			
			

			var selection = Selection.gameObjects;
			
			for (var i = selection.Length - 1; i >= 0; --i)
			{
			   var selected = selection[i];
			   PREFAB_RANDOM_ITERATOR = UnityEngine.Random.Range(0,prefabList.Count);
			   GameObject replacementPrefab = prefabList[PREFAB_RANDOM_ITERATOR];
			   replacementPrefab.transform.parent = selected.transform;
			
				var prefabType = PrefabUtility.GetPrefabType(replacementPrefab);
				GameObject newObject;
				
				if (prefabType == PrefabType.Prefab)
				{
					newObject = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);
				}
				else
				{
					newObject = Instantiate(replacementPrefab);
					newObject.name = replacementPrefab.name + "__" + selected.name;
				}

				if (newObject == null)
				{
					Debug.LogError("Error instantiating prefab");
					break;
				}
				float parentPositionX = selected.transform.position.x;
				float parentPositionY = selected.transform.position.y-6f;
				float parentPositionZ = selected.transform.position.z;
				float parentScaleXFactor = 10/selected.transform.localScale.x;
				float parentScaleYFactor = 10/selected.transform.localScale.y;
				float parentScaleZFactor = 10/selected.transform.localScale.z;
				Vector3 newScale = Vector3.Scale ( new Vector3 ( parentScaleXFactor, parentScaleYFactor, parentScaleZFactor ), new Vector3 ( 2.5f, 2.5f, 2.5f ) );
				
				Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
				newObject.transform.parent = selected.transform.parent;
				newObject.transform.position = new Vector3 ( parentPositionX, parentPositionY, parentPositionZ );
				newObject.transform.localRotation = selected.transform.localRotation;
				newObject.transform.localScale = newScale;
				
				newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
				Undo.DestroyObjectImmediate(selected);
			}
		}

		GUI.enabled = false;
		EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
	}

}