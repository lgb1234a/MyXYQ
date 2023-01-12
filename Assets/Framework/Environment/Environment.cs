using System;

namespace Framework
{
    public static class Environment
    {
        private static IEnvironmentHandler m_defaultEnvironment;
        private static IEnvironmentHandler m_customEnvironment;
        public static IEnvironmentHandler GetDefaultEnvironment()
        {
            if (m_defaultEnvironment == null)
            {
                m_defaultEnvironment = new DefaultEnvironment();
            }
            return m_defaultEnvironment;
        }

        public static IEnvironmentHandler GetCustomEnvironment()
        {
            return m_customEnvironment;
        }

        public static void SetCustomEnvironment(IEnvironmentHandler customEnvironment)
        {
            m_customEnvironment = customEnvironment;
        }

        private static IEnvironmentHandler GetCurrentEnvironment()
        {
            if (m_customEnvironment != null)
            {
                return m_customEnvironment;
            }
            return GetDefaultEnvironment();
        }

        public static string GetMapJsonDataPath(string mapName)
        {
            return GetCurrentEnvironment().GetMapJsonDataPath(mapName);
        }
    }
}