using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Seacher.Models;
using static Seacher.Models.DBTable;

namespace Seacher.ViewModel
{
    public class DBTableViewModel : BaseViewModel, IDBQerry
    {
        private DBTable model;

        public string Title
        {
            get => model.Title;
            set => SetProperty(ref model.Title, value);
        }

        public string Description
        {
            get => model.Description;
            set => SetProperty(ref model.Description, value);
        }

        public string Name
        {
            get => model.Name;
            set => SetProperty(ref model.Name, value);
        }
        public string MainTable
        {
            get => model.MainTable;
            set => SetProperty(ref model.MainTable, value);
        }

        private ObservableCollection<DBFieldViewModel> fields;
        public ObservableCollection<DBFieldViewModel> Fields
        {
            get => fields;
            set
            {
                model.Fields = value.Select(f => (DBField)f).ToList();
                SetProperty(ref fields, value);
            }
        }

        public string RelationToMainTable
        {
            get => model.RelationToMainTable;
            set => SetProperty(ref model.RelationToMainTable, value);
        }

        public DBTableViewModel(DBTable dBTable)
        {
            model = dBTable ?? new DBTable();
            Fields = new ObservableCollection<DBFieldViewModel>(model.Fields.Select(f => new DBFieldViewModel(f)));             
        }

        public string GetFields(int tablePosition)
        {
            return model.GetFields(tablePosition);
        }

        public string GetTableName(int tablePosition)
        {
            return model.GetTableName(tablePosition);
        }

        public string GetTableRelation(int tablePosition)
        {
            return model.GetTableRelation(tablePosition);
        }

        public string GetAlias(int tablePosition)
        {
            return GetAlias(tablePosition);
        }


    }
}
