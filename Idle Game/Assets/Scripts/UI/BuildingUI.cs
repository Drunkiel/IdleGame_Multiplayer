using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public int displayedBuildingID;
    [SerializeField] private Image buildingImage;
    [SerializeField] private TMP_Text buildingNameText;
    [SerializeField] private Transform recipeParent;
    public List<RecipeItemUI> currentRecipe = new();

    public RecipeItemUI recipeItemPrefab;

    public void EnterBuildingMode()
    {
        BuildingSystem.instance.BuildingManager(displayedBuildingID);
    }

    public void ChangeDisplay(int value)
    {
        if (displayedBuildingID + value >= BuildingSystem.instance._buildings.Count)
            displayedBuildingID = 0;
        else if (displayedBuildingID + value < 0)
            displayedBuildingID = BuildingSystem.instance._buildings.Count - 1;
        else
            displayedBuildingID += value;

        ChangeDisplayedBuilding(BuildingSystem.instance._buildings[displayedBuildingID]);
    }

    private void ChangeDisplayedBuilding(Building _building)
    {
        buildingImage.sprite = _building._buildingID.GetSprite();
        buildingNameText.text = _building._buildingID.buildingName;

        GenerateRecipe(_building);
    }

    private void GenerateRecipe(Building _building)
    {
        //Clear current recipe item list
        if (currentRecipe.Count > 0)
        {
            foreach (RecipeItemUI item in currentRecipe)
                Destroy(item.gameObject);

            currentRecipe.Clear();
        }

        //Generate new recipe items
        for (int i = 0; i < _building._buildingRecipes.Count; i++)
        {
            RecipeItemUI _buildingRecipeUI = Instantiate(recipeItemPrefab, recipeParent);
            ItemID _recipeItem = ItemContainer.instance.GetItemByID(_building._buildingRecipes[i].itemID);
            _buildingRecipeUI.image.sprite = _recipeItem._itemData.spriteIcon;
            _buildingRecipeUI.nameText.text = $"{_recipeItem._itemData.displayedName} | {_building._buildingRecipes[i].quantity}";

            currentRecipe.Add(_buildingRecipeUI);
        }
    }
}
