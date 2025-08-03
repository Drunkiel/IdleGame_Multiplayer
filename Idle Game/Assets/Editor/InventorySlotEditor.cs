#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventorySlot))]
public class InventorySlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventorySlot script = (InventorySlot)target;

        //Saving changes
        Undo.RecordObject(script, "Modified InventorySlot");

        DrawDefaultInspector();

        //Draw the EnumFlagsField for itemRestriction
        script.itemRestriction = (ItemType)EditorGUILayout.EnumFlagsField("Item Restriction", script.itemRestriction);

        //Show the WeaponType array if the Weapon flag is set
        if (script.itemRestriction.HasFlag(ItemType.Weapon))
        {
            EditorGUILayout.LabelField("Weapon Holding Types");

            //Draw the array for WeaponType
            if (script.holdingTypes != null)
            {
                for (int i = 0; i < script.holdingTypes.Length; i++)
                    script.holdingTypes[i] = (HoldingType)EditorGUILayout.EnumPopup($"Weapon Holding Type {i}", script.holdingTypes[i]);
            }

            //Add button to allow adding new WeaponType elements
            if (GUILayout.Button("Add Weapon Holding Type"))
            {
                //Resize the array and add a new element
                int newSize = (script.holdingTypes != null) ? script.holdingTypes.Length + 1 : 1;
                System.Array.Resize(ref script.holdingTypes, newSize);
                script.holdingTypes[newSize - 1] = HoldingType.Weapon; // Default new entry to a valid enum value
            }

            //Add button to allow removing the last WeaponType element
            if (script.holdingTypes != null && script.holdingTypes.Length > 0 && GUILayout.Button("Remove Weapon Holding Type"))
            {
                //Resize the array to remove the last element
                int newSize = script.holdingTypes.Length - 1;
                System.Array.Resize(ref script.holdingTypes, newSize);
            }
        }

        //Show the ArmorType field if the Armor flag is set
        if (script.itemRestriction.HasFlag(ItemType.Armor))
            script.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type", script.armorType);

        if (GUI.changed)
            EditorUtility.SetDirty(script);
    }
}
#endif
