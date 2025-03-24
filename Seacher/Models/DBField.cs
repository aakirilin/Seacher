using Seacher.Common;
using Seacher.Enttiy;
using Seacher.ViewModel;

namespace Seacher.Models
{

    public class DBField
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title;
        /// <summary>
        /// Описание
        /// </summary>
        public string Description;
        /// <summary>
        /// Название
        /// </summary>
        public string Name;
        /// <summary>
        /// В запросе
        /// </summary>
        public bool InQerry;
        /// <summary>
        /// В условии
        /// </summary>
        public bool InCodition;
        /// <summary>
        /// Тип поля ввода
        /// </summary>
        public ConditionTypes ConditionType;

        public static explicit operator DBField(DBFieldViewModel v)
        {
            return new DBField()
            {
                Title = v.Title,
                Description = v.Description,
                Name = v.Name
            };
        }

        public static explicit operator DBField(FieldsEnttiy v)
        {
            return new DBField()
            {
                Title = v?.Title,
                Name = v?.Name,
                Description = v?.Description,
                InQerry = v?.InQerry == 1,
                InCodition = v?.InCodition == 1,
                ConditionType = Enum.Parse<ConditionTypes>(v.ConditionType)
            };
        }
    }
}
