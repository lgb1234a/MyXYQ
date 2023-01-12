using System;

namespace Framework
{
    public class DefaultEnvironment : IEnvironmentHandler
    {
        public string GetMapJsonDataPath(string mapName)
        {
            return $"Assets/Resources/Text/Map/{mapName}.txt";
        }
    }
}