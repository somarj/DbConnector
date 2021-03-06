﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DBConnector.Tools
{
    public sealed class SqlDbParameter
    {
        private static readonly SqlDbParameter _instance = new SqlDbParameter();
        private SqlDbParameter() { }
        static SqlDbParameter() { }
        public static SqlDbParameter Instance { get { return _instance; } }

        public SqlParameter BuildParam(string parameter_name, object parameter_value, SqlDbType parameter_type, int parameter_size = 0, ParameterDirection param_direction = ParameterDirection.Input)
        {
            SqlParameter sqlp = new SqlParameter();
            sqlp.ParameterName = parameter_name;
            sqlp.SqlDbType = parameter_type;
            sqlp.Direction = param_direction;
            if (parameter_size > 0) { sqlp.Size = parameter_size; }

            //check the value for nulls or empty string and pass in a dbnull.value
            if (parameter_type == SqlDbType.Date || parameter_type == SqlDbType.DateTime)
            {
                DateTime dt = new DateTime();
                if (parameter_value == null) { sqlp.Value = DBNull.Value; return sqlp; } //if null value set it to DBNULL and return
                DateTime.TryParse(parameter_value.ToString(), out dt);
                if (dt.Year < 1900) { sqlp.Value = System.Data.SqlTypes.SqlDateTime.MinValue; } else { sqlp.Value = parameter_value; }
                return sqlp;
            }

            if (parameter_value == null) { sqlp.Value = DBNull.Value; }
            else { sqlp.Value = parameter_value; }

            return sqlp;
        }
    }

    public static class SqlConvert
    {
        public static SqlDbType ConvertToSqlDbType(this Type obj_type)
        {
            SqlParameter parm = new SqlParameter();
            System.ComponentModel.TypeConverter tcon = System.ComponentModel.TypeDescriptor.GetConverter(parm.DbType);
            if (tcon.CanConvertFrom(obj_type))
            {
                parm.DbType = ((DbType)tcon.ConvertFrom(obj_type.Name));
                return parm.SqlDbType;
            }
            switch (obj_type.Name)
            {
                case "Char":
                    return SqlDbType.Char;
                case "SByte":
                case "UInt16":
                    return SqlDbType.SmallInt;
                case "UInt32":
                    return SqlDbType.Int;
                case "UInt64":
                    return SqlDbType.Decimal;
                case "Byte[]":
                    return SqlDbType.Binary;
            }
            return SqlDbType.VarChar; //all else fails return varchar
        }
    }
}