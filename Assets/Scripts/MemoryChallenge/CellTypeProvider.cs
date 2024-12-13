using System.Collections.Generic;
using UnityEngine;

namespace MemoryChallenge
{
    public class CellTypeProvider
    {
        private readonly CellTypes[] _allTypes = (CellTypes[])System.Enum.GetValues(typeof(CellTypes));
    
        public List<CellTypes> GetPair(int pairsCount)
        {
            List<CellTypes> uniqueTypes = new List<CellTypes>();
            List<CellTypes> pairs = new List<CellTypes>(pairsCount * 2); 
        
            foreach (CellTypes cellType in _allTypes)
            {
                if (cellType != CellTypes.None)
                {
                    uniqueTypes.Add(cellType);
                }
            }
        
            while (pairs.Count < pairsCount * 2)
            {
                CellTypes randomCell = uniqueTypes[Random.Range(0, uniqueTypes.Count)];
                pairs.Add(randomCell);
                pairs.Add(randomCell);
            }
        
        
            ShuffleList(pairs);

            return pairs;
        }

        private void ShuffleList(List<CellTypes> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                CellTypes temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }

    public enum CellTypes
    {
        Pineapple,
        Orange,
        Grape,
        Avocado,
        Peach,
        Melon,
        Pear,
        Pomegranate,
        Lemon,
        None
    }
}