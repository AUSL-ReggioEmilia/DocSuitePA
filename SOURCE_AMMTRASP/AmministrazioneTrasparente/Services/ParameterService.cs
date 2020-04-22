using AmministrazioneTrasparente.SQLite.Entities;
using AmministrazioneTrasparente.SQLite.Repositories;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.ExtensionMethods;

namespace AmministrazioneTrasparente.Services
{
    public class ParameterService
    {
        private readonly IRepository<Parameter> _repository;

        public ParameterService()
        {
            this._repository = new Repository<Parameter>();
        }

        public IList<Parameter> GetParameters()
        {
            IEnumerable<Parameter> parameters = this._repository.SelectQuery("");
            return parameters.ToList();
        }

        public Parameter GetParameterById(int id)
        {
            Parameter parameter = this._repository.Find(id);
            return parameter;
        }

        public void UpdateParameter(int id, string keyValue)
        {
            Parameter parameter = this.GetParameterById(id);
            parameter.KeyValue = keyValue;
            this._repository.Update(parameter);
        }

        public void AddParameter(Parameter parameter)
        {
            this._repository.Insert(parameter);
        }

        public Parameter GetParameterByName(string keyName)
        {
            IEnumerable<Parameter> parameters = this._repository.SelectQuery("WHERE KeyName =@0", keyName);
            if (parameters.IsNullOrEmpty()) return null;
            return parameters.First();
        }

        public string GetString(string keyName)
        {
            var result = this.GetParameterByName(keyName);
            if (result == null || string.IsNullOrEmpty(result.KeyValue))
                return string.Empty;

            return result.KeyValue;
        }

        public int GetInteger(string keyName)
        {
            var result = this.GetParameterByName(keyName);
            if (result == null || string.IsNullOrEmpty(result.KeyValue))
                return 0;

            int val;
            int.TryParse(result.KeyValue, out val);
            return val;
        }

        public bool GetBoolean(string keyName)
        {
            var result = this.GetParameterByName(keyName);
            if (result == null || string.IsNullOrEmpty(result.KeyValue))
                return false;

            bool val;
            bool.TryParse(result.KeyValue, out val);
            return val;
        }
    }
}