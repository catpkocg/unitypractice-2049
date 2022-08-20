using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
	GameManager GM;

	void OnEnable()
	{
		GM = target as GameManager;
		GM.EditorRepaint += Repaint;
	}

	void OnDisable()
	{
		GM.EditorRepaint -= Repaint;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.Space(20);

		EditorGUILayout.LabelField("Array");
		for (int i = 7; i >= 0; i--)
		{
			GUILayout.BeginHorizontal();
			for (int j = 0; j < 8; j++)
				GM.Array[j, i] = EditorGUILayout.IntField(GM.Array[j, i], GUILayout.Width(30));
			GUILayout.EndHorizontal();
		}
	}
}