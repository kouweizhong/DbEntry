
using System;
using System.Collections.Generic;
using System.Text;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;

namespace org.hanzify.llf.Data.Definition
{
    public abstract class LazyLoadOneBase<T> : ILazyLoading
    {
        protected object owner;
        protected string RelationName;
        protected DbContext context;
        protected bool m_IsLoaded;
        protected T m_Value;

        public LazyLoadOneBase(object owner)
        {
            this.owner = owner;
            DoSetOwner();
        }

        bool ILazyLoading.IsLoaded
        {
            get { return m_IsLoaded; }
            set { m_IsLoaded = value; }
        }

        object ILazyLoading.Read()
        {
            if (!m_IsLoaded)
            {
                ((ILazyLoading)this).Load();
                m_IsLoaded = true;
                context = null;
            }
            return m_Value;
        }

        void ILazyLoading.Write(object item, bool IsLoad)
        {
            m_Value = (T)item;
            DoWrite(IsLoad);
            m_IsLoaded = true;
            context = null;
        }

        protected virtual void DoWrite(bool IsLoad) { }

        public T Value
        {
            get
            {
                return (T)((ILazyLoading)this).Read();
            }
            set
            {
                ((ILazyLoading)this).Write(value, false);
            }
        }

        protected virtual void DoSetOwner() {}

        void ILazyLoading.Init(DbContext context, string RelationName)
        {
            this.context = context;
            this.RelationName = RelationName;
        }

        void ILazyLoading.Load()
        {
            DoLoad();
        }

        protected abstract void DoLoad();
    }
}