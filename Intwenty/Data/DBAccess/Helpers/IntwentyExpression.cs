using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intwenty.Data.DBAccess.Helpers
{
    public class IntwentyExpression
    {
        private string _expression = string.Empty;

        private string _computeexpression = string.Empty;

        private List<IntwentyParameter> _parameters = new List<IntwentyParameter>();

        private static DataTable _evaldt;

        private bool _hasbuiltexpression = false;

        public IntwentyExpression(string expression, List<IntwentyParameter> parameters)
        {
            _expression = expression;
            _computeexpression = expression;
            if (parameters != null)
                _parameters = parameters;

            BuildExpression();
        }

        public IntwentyExpression(string expression)
        {
            _expression = expression;
            _computeexpression = expression;
        }

        public void AddParameter(IntwentyParameter p)
        {
            _parameters.Add(p);
        }

        public List<IntwentyParameter> GetParameters()
        {
            return _parameters;
        }


        public string GetSqlWhere()
        {
            return " WHERE " + _expression;
        }

        public MongoDB.Bson.BsonDocument GetMongoDbFilterDefinition() 
        {
            if (!IsAndExpression)
                throw new InvalidOperationException("Cand build filters based on expressions including OR at the moment");

            var filter = new MongoDB.Bson.BsonDocument();
            foreach(var p in _parameters)
                filter.Add(new MongoDB.Bson.BsonElement(p.ParameterName.Substring(1), Convert.ToString(p.Value)));

            return filter;
        }

        public MongoDB.Driver.FilterDefinition<T> GetMongoDbFilterDefinition<T>()
        {
            if (!IsAndExpression)
                throw new InvalidOperationException("MongoDb filters based on expressions including OR is not supported yet.");

            var res = new List<MongoDB.Driver.FilterDefinition<T>>();
            foreach (var p in _parameters)
            {
                if (p.DataType == DbType.String)
                    res.Add(MongoDB.Driver.Builders<T>.Filter.Eq(p.ParameterName.Substring(1), Convert.ToString(p.Value)));
                else
                    res.Add(MongoDB.Driver.Builders<T>.Filter.Eq(p.ParameterName.Substring(1), p.Value));


            }
            return MongoDB.Driver.Builders<T>.Filter.And(res);
        }

        public LiteDB.BsonExpression GetLiteDbFilterDefinition()
        {
            var localexpression = _computeexpression;


            foreach (var p in _parameters)
            {
                localexpression = localexpression.Replace("'[" + p.ParameterName.Substring(1) + "]'", "$." + p.ParameterName.Substring(1));
                if (p.DataType == DbType.String)
                    localexpression = localexpression.Replace("'" + p.ParameterName + "'", "'" + Convert.ToString(p.Value) + "'");
                else
                    localexpression = localexpression.Replace("'" + p.ParameterName + "'",  Convert.ToString(p.Value));

            }


            return localexpression;
        }

        public bool ComputeExpression(LiteDB.BsonDocument document)
        {
            if (!_hasbuiltexpression)
                BuildExpression();

            var localexpression = _computeexpression;

            foreach (var t in document)
            {
                if (localexpression.Contains(string.Format("[{0}]", t.Key)))
                {
                    localexpression = localexpression.Replace(string.Format("[{0}]", t.Key), Convert.ToString(t.Value.RawValue));
                }
            }

            if (_evaldt == null)
                _evaldt = new DataTable();

            var result = (bool)_evaldt.Compute(localexpression, "");

            return result;
        }

        public bool ComputeExpression(MongoDB.Bson.BsonDocument document)
        {
            if (!_hasbuiltexpression)
                BuildExpression();

            var localexpression = _computeexpression;

            foreach (var t in document)
            {
                if (localexpression.Contains(string.Format("[{0}]", t.Name)))
                {
                    localexpression = localexpression.Replace(string.Format("[{0}]", t.Name), t.Value.ToString());
                }
            }

            if (_evaldt == null)
                _evaldt = new DataTable();

            var result = (bool)_evaldt.Compute(localexpression, "");

            return result;

        }



        private void BuildExpression()
        {
            _hasbuiltexpression = true;

            if (string.IsNullOrEmpty(_computeexpression))
                throw new InvalidOperationException("expression must not be empty");
            if (_parameters.Count == 0)
                throw new InvalidOperationException("parameters must not be empty");
            if (_computeexpression.IndexOf("[") > -1)
                throw new InvalidOperationException("parameters must not contain the character [");
            if (_computeexpression.IndexOf("]") > -1)
                throw new InvalidOperationException("parameters must not contain the character ]");

            foreach (var p in _parameters)
            {
                var indexparamstart = _computeexpression.IndexOf(p.ParameterName);
                if (indexparamstart < 0)
                    throw new InvalidOperationException(string.Format("The parameter {0} must exist in the expression", p.ParameterName));
                if (!p.ParameterName.Contains("@"))
                    throw new InvalidOperationException(string.Format("The parametername {0} must begin with @", p.ParameterName));
                if (p.Value == null)
                    throw new InvalidOperationException(string.Format("The parameter {0} must have a value", p.ParameterName));
                if (p.Value == DBNull.Value)
                    throw new InvalidOperationException(string.Format("The parameter {0} must have a value", p.ParameterName));

                _computeexpression = _computeexpression.Replace(p.ParameterName, string.Format("'{0}'", p.ParameterName));
                _computeexpression = _computeexpression.Replace(string.Format("''{0}''", p.ParameterName), string.Format("'{0}'", p.ParameterName));
                _computeexpression = _computeexpression.Replace(string.Format("'%'{0}'%'", p.ParameterName), string.Format("'%{0}%'", p.ParameterName));
                _computeexpression = _computeexpression.Replace(p.ParameterName, Convert.ToString(p.Value));

                var counter = 0;
                var check = false;
                var found = false;
                var startval = 0;
                while (!check)
                {
                    counter += 1;
                    if (counter > 10)
                        check = true;

                    var indexfldstart = _computeexpression.IndexOf(p.ParameterName.Substring(1), startval);
                    if (indexfldstart < 0)
                        continue;

                    if (indexfldstart > 0)
                    {
                        if (_computeexpression.Substring(indexfldstart - 1, 1) == "@")
                            continue;
                    }

                    var lastindex = -1;
                    var test = _computeexpression.IndexOf('=', indexfldstart + 1);
                    if (test > -1)
                        lastindex = test;

                    if (lastindex == -1)
                    {
                        test = _computeexpression.IndexOf(">=", indexfldstart + 1);
                        if (test > -1)
                            lastindex = test;
                    }

                    if (lastindex == -1)
                    {
                        test = _computeexpression.IndexOf("<=", indexfldstart + 1);
                        if (test > -1)
                            lastindex = test;
                    }
                    if (lastindex == -1)
                    {
                        test = _computeexpression.ToLower().IndexOf("like", indexfldstart + 1);
                        if (test > -1)
                            lastindex = test;
                    }

                    if (lastindex == -1)
                    {
                        test = _computeexpression.IndexOf('<', indexfldstart + 1);
                        if (test > -1)
                            lastindex = test;
                    }
                    if (lastindex == -1)
                    {
                        test = _computeexpression.IndexOf('>', indexfldstart + 1);
                        if (test > -1)
                            lastindex = test;
                    }

                    if (lastindex < 1)
                        continue;

                    while (char.IsWhiteSpace(_computeexpression[lastindex - 1]))
                        lastindex -= 1;

                    _computeexpression = _computeexpression.Insert(indexfldstart, "'[");
                    lastindex += 2;
                    _computeexpression = _computeexpression.Insert(lastindex, "]'");

                    found = true;

                    startval = indexfldstart + p.ParameterName.Substring(1).Length - 1;
                    if (startval >= p.ParameterName.Substring(1).Length)
                        check = true;
                }

                if (!found)
                    throw new InvalidOperationException(string.Format("The field {0} must exist in the expression", p.ParameterName.Substring(1)));
            }
        }

        public bool IsAndExpression
        {
            get
            {
                return !_expression.ToLower().Contains(" or ");
            }
        }


    }
}
