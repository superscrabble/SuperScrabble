namespace SuperScrabble.Common
{
    using System;

    public static class Guard
    {
        const string ParameterName = "Parameter";

        public static class Against
        {
            public static void Null(object parameter, string parameterName = ParameterName)
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
            }
        }
    }
}
