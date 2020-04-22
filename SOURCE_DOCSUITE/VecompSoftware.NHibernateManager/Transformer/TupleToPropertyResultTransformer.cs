using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Transform;
using VecompSoftware.Helpers;


namespace VecompSoftware.NHibernateManager.Transformer
{
    public class TupleToPropertyResultTransformer : IResultTransformer
    {
        #region "Fields"

        private Type _result;
        private PropertyInfo[] _properties;
        private ArrayList _subProperties;
        private IDictionary<string, object> _collectionList;
        private bool _enableMerge;

        #endregion

        #region "Constructor"
        public TupleToPropertyResultTransformer(Type result, IDictionary<string, string> projections, bool enableMerge)
        {
            _result = result;
            _enableMerge = enableMerge;
            List<PropertyInfo> props = new List<PropertyInfo>();
            _subProperties = new ArrayList();
            //popola la lista delle proprietà da mappare
            foreach (string key in projections.Keys)
            {
                props.Add(result.GetProperty(projections[key]));
                _subProperties.Add(key);
            }
            _properties = props.ToArray();
        }
        #endregion

        #region "IResultTransformer Implementation"
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            object instance = Activator.CreateInstance(_result);
            object objInstance = null;
            IList listInstance = null;

            _collectionList = new Dictionary<string, object>();

            for (int i = 0; i <= tuple.Length - 1; i++)
            {
                //verifica che la proprietà sia una lista. In quel caso istanzio un oggetto di quel tipo e lo popolo con i
                //campi della tupla
                if (_properties[i].PropertyType.Name.StartsWith("IList"))
                {
                    objInstance = GetTypeCollectionInstance(aliases[i], ref _properties[i]);
                    SetCollectionObject(ref objInstance, (string)_subProperties[i], tuple[i]);
                    //properties(i).SetValue(instance, obj, Nothing)
                }
                else
                {
                    _properties[i].SetValue(instance, tuple[i], null);
                }
            }

            for (int i = 0; i <= tuple.Length - 1; i++)
            {
                if (_collectionList.ContainsKey(aliases[i]))
                {
                    objInstance = _collectionList[aliases[i]];
                    Type type = typeof(List<>).MakeGenericType(objInstance.GetType());
                    listInstance = (IList)Activator.CreateInstance(type);
                    listInstance.Add(objInstance);
                    ReflectionHelper.SetProperty(instance, aliases[i], listInstance);
                    _collectionList.Remove(aliases[i]);
                }
            }

            return instance;
        }

        public IList TransformList(IList collection)
        {
            if (_enableMerge)
            {
                int i = 0;
                int k = 0;
                while (i < collection.Count)
                {
                    k = i + 1;
                    while (k < collection.Count)
                    {
                        if (collection[i].Equals(collection[k]))
                        {
                            MergeCollectionItem(collection[i], collection[k]);
                            collection.RemoveAt(k);
                        }
                        else
                        {
                            k = k + 1;
                        }
                    }
                    i = i + 1;
                }
            }

            return collection;
        }
        #endregion

        private object GetTypeCollectionInstance(string collectionName, ref PropertyInfo prop)
        {
            if (_collectionList.ContainsKey(collectionName))
            {
                return _collectionList[collectionName];
            }
            else
            {
                //istanzio un oggetto del tipo del contenuto della lista
                Type typeInstance = prop.PropertyType.GetGenericArguments()[0];
                object instance = Activator.CreateInstance(typeInstance);
                _collectionList.Add(collectionName, instance);
                return instance;
            }
        }

        #region "Collection Functions"
        private void MergeCollectionItem(Object entity, Object entity2)
        {
            foreach (PropertyInfo propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.Name.StartsWith("IList"))
                {
                    IList prop = (IList)ReflectionHelper.GetProperty(entity, propertyInfo.Name);
                    IList prop2 = (IList)ReflectionHelper.GetProperty(entity2, propertyInfo.Name);
                    if ((prop != null) & (prop2 != null))
                    {
                        foreach(object item in prop2)
                        {
                            prop.Add(item);
                        }
                    }
                }
            }
        }

        private void SetCollectionObject(ref object objInstance, string subProperty, object value)
        {
            CreateObject(ref objInstance, ref subProperty, value);
        }

        private void CreateObject(ref object obj, ref string propertyName, object value)
        {
            object instance = null;
            //Splitto il nome della proprietà per capire se è un oggetto composto
            string[] propHierarchy = propertyName.Split('.');
            //Proprietà composta
            if (propHierarchy.Length > 1)
            {
                instance = ReflectionHelper.GetProperty(obj, propHierarchy[0]);
                if (instance == null)
                {
                    instance = _result.Assembly.CreateInstance(_result.Namespace + "." + propHierarchy[0]);
                }
                //oggetto composto....imposto la parte precedente
                propertyName = propertyName.Substring(propertyName.IndexOf(".") + 1);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    CreateObject(ref instance, ref propertyName, value);
                    ReflectionHelper.SetProperty(obj, propHierarchy[0], instance);
                }
            }
            else
            {
                //proprietà foglia
                ReflectionHelper.SetProperty(obj, propertyName, value);
            }
        }
        #endregion
    }
}
