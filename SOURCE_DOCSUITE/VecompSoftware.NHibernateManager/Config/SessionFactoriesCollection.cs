using System.Configuration;

namespace VecompSoftware.NHibernateManager.Config
{
    [ConfigurationCollection(typeof(SessionFactoryElement))]
    public sealed class SessionFactoriesCollection : ConfigurationElementCollection
    {
        public SessionFactoriesCollection()
        {
            var sessionFactory = (SessionFactoryElement)CreateNewElement();
            Add(sessionFactory);
        }

        public string DefaultSessionFactory { get; private set; }

        public string Interceptor { get; private set; }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SessionFactoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SessionFactoryElement)element).Name;
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            bool result;
            switch (name)
            {
                case "default":
                    {
                        DefaultSessionFactory = value;
                        result = true;
                        break;
                    }
                case "interceptor":
                    {
                        Interceptor = value;
                        result = true;
                        break;
                    }
                default:
                    {
                        result = base.OnDeserializeUnrecognizedAttribute(name, value);
                        break;
                    }
            }
            return result;
        }

        public SessionFactoryElement this[int index]
        {
            get { return (SessionFactoryElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public new SessionFactoryElement this[string name]
        {
            get { return (SessionFactoryElement)BaseGet(name); }
        }

        public int IndexOf(SessionFactoryElement sessionFactory)
        {
            return BaseIndexOf(sessionFactory);
        }

        public void Add(SessionFactoryElement sessionFactory)
        {
            BaseAdd(sessionFactory);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(SessionFactoryElement sessionFactory)
        {
            if (BaseIndexOf(sessionFactory) >= 0)
            {
                BaseRemove(sessionFactory.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

    }
}
