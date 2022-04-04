using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Base.DataAccess.GenericRepository.Helper
{
    public static class Helper
    {
        public static bool HasProperty(this Type obj, string propertyName)
        {

            return obj.GetProperty(propertyName.Trim(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static void SetProperty(this object obj, string propertyName, int value)
        {
            obj.GetType().GetProperty(propertyName.Trim(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(obj, value, null);
        }
        public static void SetProperty(this object obj, string propertyName, DateTime value)
        {
            obj.GetType().GetProperty(propertyName.Trim(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(obj, value, null);
        }

        public static IQueryable<T> WhereEquals<T>(this IQueryable<T> source, string member, object value)
        {
            var item = Expression.Parameter(typeof(T), "item");
            var memberValue = member.Split('.').Aggregate((Expression)item, Expression.PropertyOrField);
            var memberType = memberValue.Type;
            if (value != null && value.GetType() != memberType)
                value = Convert.ChangeType(value, memberType);
            var condition = Expression.Equal(memberValue, Expression.Constant(value, memberType));
            var predicate = Expression.Lambda<Func<T, bool>>(condition, item);
            return source.Where(predicate);
        }
        public static IQueryable<TEntity> SetBasePropertiesOnGet<TEntity>(IContextHelper _contextHelper, IQueryable<TEntity> query)
        {

            var entity = typeof(TEntity);

            //check if entity is skipping base properties
            if (_contextHelper.IsSkippingBaseProperties(entity.ToString()))
                return query;

            //check for base properties
            var user = _contextHelper.GetUser();
            string propName;
            object value;

            return query;
        }
        public static void SetBasePropertiesOnInsert<TEntity>(TEntity entity, IContextHelper _contextHelper)
        {
            //check if entity is skipping base properties
            if (_contextHelper.IsSkippingBaseProperties(entity.ToString()))
                return;

            entity = SetBaseProperties(entity, _contextHelper);
            var user = _contextHelper.GetUser();

            #region User Properties

            if (HasProperty(typeof(TEntity), "CreatedBy"))
            {
                SetProperty(entity, "CreatedBy", _contextHelper.GetUserId());
            }

            if (HasProperty(typeof(TEntity), "CreatedOn"))
            {
                SetProperty(entity, "CreatedOn", _contextHelper.GetUTCDateTime());
            }

            #endregion
        }
        public static void SetBasePropertiesOnUpdate<TEntity>(TEntity entity, IContextHelper _contextHelper)
        {
            //check if entity is skipping base properties
            if (_contextHelper.IsSkippingBaseProperties(entity.ToString()))
                return;

            entity = SetBaseProperties(entity, _contextHelper);
            var user = _contextHelper.GetUser();

            #region User Properties

            if (HasProperty(typeof(TEntity), "ModifiedBy"))
            {
                SetProperty(entity, "ModifiedBy", _contextHelper.GetUserId());
            }

            if (HasProperty(typeof(TEntity), "ModifiedOn"))
            {
                SetProperty(entity, "ModifiedOn", _contextHelper.GetUTCDateTime());
            }

            #endregion
        }
        public static TEntity SetBaseProperties<TEntity>(TEntity entity, IContextHelper _contextHelper)
        {
            var user = _contextHelper.GetUser();

            #region Tenant Properties

            #endregion                        

            return entity;
        }

    }
}
