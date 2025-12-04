using System.Collections.Generic;
using UnityEngine;

public struct IngredientStats
{
    public float aggroPerUnit;
    public float energyPerUnit;
    public float clarityPerUnit;

    public IngredientStats(float aggro, float energy, float clarity)
    {
        aggroPerUnit = aggro;
        energyPerUnit = energy;
        clarityPerUnit = clarity;
    }

}
public static class IngredientDatabase
{
    public static readonly Dictionary<IngredientId, IngredientStats> Stats =
    new Dictionary<IngredientId, IngredientStats>
    {
        { IngredientId.Alko1, new IngredientStats(+3f,  0f,  -3f) }, // klasyk
        { IngredientId.Alko2, new IngredientStats(+5f, +1f,  -5f) }, // hardcore
        { IngredientId.Alko3, new IngredientStats(+2f,  +3f,  -2f) }, //whisky
        { IngredientId.Alko4, new IngredientStats(+6f, +2f,  -6f) }, // „metal fuel”
        { IngredientId.Alko5, new IngredientStats(+2f,  -3f,  +2f) }, //piwko

        { IngredientId.Energy1, new IngredientStats( 0f, +4f,  -2f) }, //Energolek
        { IngredientId.Energy2, new IngredientStats(+1f, +8f,  -2f) }, // zajebiście mocny ener-golek
        { IngredientId.Energy3, new IngredientStats( 0f, +3f,  +2f) }, // yerba/mate 
        { IngredientId.Energy4, new IngredientStats(-2f, -2f,  +4f) }, // izotonik –
        { IngredientId.Energy5, new IngredientStats(-0.5f, -0.5f,  +0.5f) }, // cienias dla balansu
    };
}
