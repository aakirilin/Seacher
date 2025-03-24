using Seacher.Models;
using System.ComponentModel.DataAnnotations;

namespace Seacher.ViewModel
{
    public class DBFieldViewModel : BaseViewModel
    { 
        private DBField model;

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
        public bool InQerry
        {
            get => model.InQerry;
            set => SetProperty(ref model.InQerry, value);
        }
        public bool InCodition
        {
            get => model.InCodition;
            set => SetProperty(ref model.InCodition, value);
        }

        public DBFieldViewModel() 
        {
            this.model = new DBField();
        }
        public DBFieldViewModel(DBField model)
        {
            this.model = model;
        }
    }
}
