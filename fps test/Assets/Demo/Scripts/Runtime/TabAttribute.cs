// Designed by Kinemation, 2023

using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public class TabAttribute : PropertyAttribute
    {
        public readonly string tabName;

        public TabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}