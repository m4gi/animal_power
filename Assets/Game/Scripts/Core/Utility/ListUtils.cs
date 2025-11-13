using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Utils
{
    public static class ListUtils
    {

        public static List<T> GetRandomElements<T>(T[] sourceList, int count)
        {
           return GetRandomElements<T>(sourceList.ToList(), count);
        }
        
        /// <summary>
        /// Gets a specified number of unique random elements from a source list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="sourceList">The list to draw elements from.</param>
        /// <param name="count">The number of unique elements to retrieve.</param>
        /// <returns>A new list containing the randomly selected elements.</returns>
        public static List<T> GetRandomElements<T>(List<T> sourceList, int count)
        {
            // --- Edge Case Handling ---

            // If the source list is null, empty, or the requested count is zero or less, return an empty list.
            if (sourceList == null || sourceList.Count == 0 || count <= 0)
            {
                return new List<T>();
            }

            // If the requested count is greater than or equal to the list's size,
            // return a copy of the entire list.
            if (count >= sourceList.Count)
            {
                return new List<T>(sourceList);
            }

            // --- Main Logic ---

            // Create a copy of the source list to avoid modifying the original.
            List<T> tempList = new List<T>(sourceList);
            List<T> resultList = new List<T>();

            for (int i = 0; i < count; i++)
            {
                // Pick a random index from the current state of the temporary list.
                int randomIndex = UnityEngine.Random.Range(0, tempList.Count);

                // Add the item at that index to our results.
                resultList.Add(tempList[randomIndex]);

                // Remove the item from the temporary list to ensure it's not picked again.
                tempList.RemoveAt(randomIndex);
            }

            return resultList;
        }
    }
}