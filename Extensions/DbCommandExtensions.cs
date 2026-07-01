using System;
using System.Data.Common;

namespace Cinema_Management_App.Extensions
{
    public static class DbCommandExtensions
    {
        // The 'this DbCommand cmd' tells C# to attach this method to all DbCommand objects
        public static DbParameter AddParameterWithValue(this DbCommand cmd, string parameterName, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;

            // Automatically handle null values for database compatibility
            parameter.Value = value ?? DBNull.Value;

            cmd.Parameters.Add(parameter);
            return parameter;
        }
    }
}